// QuestJournal.cs (발췌)
using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestJournal : MonoBehaviour
{
    public static QuestJournal Instance { get; private set; }

    [Header("등록된 퀘스트(SO)")]
    [SerializeField] private QuestDefinition[] allQuests;

    private readonly Dictionary<string, QuestDefinition> _defs = new();
    private readonly Dictionary<string, QuestProgress> _progress = new();

    public event Action<QuestDefinition> OnAccepted;
    public event Action<QuestDefinition> OnCompleted;
    public event Action<QuestDefinition> OnFailed;
    public event Action OnJournalChanged;

    public IReadOnlyDictionary<string, QuestProgress> AllProgress => _progress;
    public bool TryGetDefinition(string questId, out QuestDefinition def) => _defs.TryGetValue(questId, out def);

    void Awake()
    {
        if (Instance) { Destroy(gameObject); return; }
        Instance = this; DontDestroyOnLoad(gameObject);

        _defs.Clear();
        if (allQuests != null)
        {
            foreach (var q in allQuests)
            {
                if (!q) continue;
                if (!q.IsValid(out var why)) { Debug.LogError($"[Quest] {q.name} invalid: {why}", q); continue; }
                if (_defs.ContainsKey(q.QuestId)) { Debug.LogError($"[Quest] Dup questId: {q.QuestId}", q); continue; }
                _defs.Add(q.QuestId, q);
            }
        }
        Broadcast();
    }

    // ─────────────────────────────────────────────────────────────
    // Accept / Progress / Complete / Fail
    // ─────────────────────────────────────────────────────────────

    public bool CanAccept(string questId)
    {
        if (!_defs.TryGetValue(questId, out var def)) return false;
        if (_progress.TryGetValue(questId, out _)) return false; // 이미 수락됨(반복퀘는 별도 정책)
        return PrerequisitesCompleted(def);
    }

    public bool Accept(string questId)
    {
        if (!CanAccept(questId)) return false;
        var def = _defs[questId];

        var prg = QuestProgress.Start(def);
        _progress[questId] = prg;

        // ★ UnityEvent 호출 제거 → 액션 실행
        RunActions(def.OnAcceptActions, def, prg);

        OnAccepted?.Invoke(def);
        Broadcast();
        return true;
    }

    public void AddProgress(ObjectiveType type, string targetId, int add = 1)
    {
        foreach (var (qid, prg) in _progress)
        {
            if (prg.Status != QuestStatus.InProgress) continue;
            var def = _defs[qid];

            for (int i = 0; i < def.ObjectiveCount; i++)
            {
                var o = def.Objectives[i];
                if (o.type != type) continue;
                if (!string.Equals(o.targetId, targetId, StringComparison.Ordinal)) continue;

                prg.AddCount(i, add, def);
            }
        }
        Broadcast();
    }

    public bool TryComplete(string questId)
    {
        if (!_defs.TryGetValue(questId, out var def)) return false;
        if (!_progress.TryGetValue(questId, out var prg)) return false;
        if (prg.Status != QuestStatus.InProgress) return false;
        if (!prg.AllObjectivesCompleted(def)) return false;

        prg.MarkCompleted();

        // ★ UnityEvent 호출 제거 → 액션 실행
        RunActions(def.OnCompleteActions, def, prg);

        OnCompleted?.Invoke(def);
        Broadcast();
        return true;
    }

    public bool Fail(string questId)
    {
        if (!_defs.TryGetValue(questId, out var def)) return false;
        if (!_progress.TryGetValue(questId, out var prg)) return false;

        prg.MarkFailed();

        // ★ UnityEvent 호출 제거 → 액션 실행
        RunActions(def.OnFailActions, def, prg);

        OnFailed?.Invoke(def);
        Broadcast();
        return true;
    }

    private void Broadcast() => OnJournalChanged?.Invoke();

    // ─────────────────────────────────────────────────────────────
    // 선행 조건
    // ─────────────────────────────────────────────────────────────
    private bool PrerequisitesCompleted(QuestDefinition def)
    {
        var requirements = def.RequiredQuestCompleted;
        if (requirements == null || requirements.Count == 0) return true;

        foreach (var rq in requirements)
        {
            if (rq == null) return false;
            if (!_progress.TryGetValue(rq.QuestId, out var prg)) return false;
            if (prg.Status != QuestStatus.Completed) return false;
        }
        return true;
    }

    // ─────────────────────────────────────────────────────────────
    // 대사/오버레이 헬퍼 (기존 유지)
    // ─────────────────────────────────────────────────────────────
    public QuestStatus ComputeStatus(string questId)
    {
        if (!_defs.TryGetValue(questId, out var def)) return QuestStatus.Locked;
        if (_progress.TryGetValue(questId, out var prg)) return prg.Status;
        return PrerequisitesCompleted(def) ? QuestStatus.Available : QuestStatus.Locked;
    }

    public NPCState GetNpcDialogueState(NPCId npcId)
    {
        bool anyAvailableFromThisNpc = false;

        foreach (var (qid, prg) in _progress)
        {
            if (prg.Status != QuestStatus.InProgress) continue;
            var def = _defs[qid];
            if (prg.AllObjectivesCompleted(def) && def.GetTurnInNpc().Equals(npcId))
                return NPCState.Completed;

            for (int i = 0; i < def.ObjectiveCount; i++)
            {
                var o = def.Objectives[i];
                if (o.type == ObjectiveType.TalkTo && o.targetId == npcId.ToString())
                    return NPCState.InProgress;
            }
        }

        foreach (var def in _defs.Values)
        {
            if (ComputeStatus(def.QuestId) == QuestStatus.Available &&
                def.GetAcceptNpc().Equals(npcId))
            {
                anyAvailableFromThisNpc = true;
                break;
            }
        }

        if (anyAvailableFromThisNpc) return NPCState.Repeat;
        return NPCState.FirstMeet;
    }

    public bool IsOverlayActive(string questId)
    {
        var st = ComputeStatus(questId);
        return st == QuestStatus.InProgress || st == QuestStatus.Available || st == QuestStatus.Completed;
    }

    public int GetOverlaySituationScore(string questId, NPCId npcId)
    {
        if (!_defs.TryGetValue(questId, out var def)) return 0;
        var st = ComputeStatus(questId);

        if (st == QuestStatus.InProgress && _progress.TryGetValue(questId, out var prg) &&
            prg.AllObjectivesCompleted(def) && def.GetTurnInNpc().Equals(npcId))
            return 100;
        if (st == QuestStatus.InProgress)
        {
            for (int i = 0; i < def.ObjectiveCount; i++)
            {
                var o = def.Objectives[i];
                if (o.type == ObjectiveType.TalkTo && o.targetId == npcId.ToString())
                    return 50;
            }
        }
        if (st == QuestStatus.Available && def.GetAcceptNpc().Equals(npcId))
            return 10;

        return 0;
    }

    // ─────────────────────────────────────────────────────────────
    // 액션 실행부 (신규)
    // ─────────────────────────────────────────────────────────────
    private void RunActions(IReadOnlyList<QuestAction> actions, QuestDefinition def, QuestProgress prg)
    {
        if (actions == null || actions.Count == 0) return;

        var ctx = new QuestContext
        {
            QuestId = def.QuestId,
            Definition = def,
            Progress = prg,
            Journal = this
        };

        for (int i = 0; i < actions.Count; i++)
        {
            try { actions[i]?.Execute(ctx); }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
