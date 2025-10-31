// DialogueResolver.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueResolver : MonoBehaviour
{
    public static DialogueResolver Instance { get; private set; }

    [Header("기본 대사 세트 레지스트리")]
    [SerializeField] private NPCDialogueDefaultSet[] defaultSets;

    [Header("퀘스트 오버레이 레지스트리")]
    [SerializeField] private QuestDialogueOverlay[] questOverlays;

    // npcId -> defaultSet
    private readonly Dictionary<NPCId, NPCDialogueDefaultSet> _defaultMap = new();

    private void Awake()
    {
        if (Instance) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        RebuildRegistry();
    }

    public void RebuildRegistry()
    {
        _defaultMap.Clear();
        if (defaultSets != null)
        {
            foreach (var set in defaultSets)
            {
                if (!set) continue;
                var id = set.NpcId;
                if (_defaultMap.ContainsKey(id))
                    Debug.LogWarning($"[DialogueResolver] 중복 NPC 기본세트: {id}");
                _defaultMap[id] = set;
            }
        }
    }

    // ─────────────────────────────────────────────────────────────
    // 공개 API
    // ─────────────────────────────────────────────────────────────

    /// <summary>기본 시작 노드에, 활성 오버레이가 있으면 덮어씌운 최종 노드</summary>
    public DialogueNodeData ResolveStartNode(NPCId npcId, NPCState state)
    {
        var defaults = _defaultMap.TryGetValue(npcId, out var set) ? set : null;
        var baseNode = defaults ? defaults.GetNode(state) : null;

        if (!TryPickWinnerOverlay(npcId, state, out var winner, out var patch))
            return baseNode;

        // 오버레이가 nodeOverride 제공 시 교체
        return patch.nodeOverride ? patch.nodeOverride : baseNode;
    }

    /// <summary>기본 NodeEvent 배열 + 오버레이 병합/교체 결과</summary>
    public NodeEvent[] ResolveNodeEvents(NPCId npcId, DialogueNodeData node, NPCState state)
    {
        NodeEvent[] baseEvents = null;

        // 기본 세트의 NodeEvent를 기본값으로 사용(있을 경우)
        if (_defaultMap.TryGetValue(npcId, out var set) && set)
        {
            baseEvents = GetDefaultNodeEvents(set, node);
        }

        if (!TryPickWinnerOverlay(npcId, state, out var winner, out var patch) || patch == null)
            return baseEvents;

        var overrideEvents = patch.nodeEventsOverride;

        if (patch.replaceAllNodeEvents)
            return overrideEvents; // 전면 교체

        // 부분 덮어쓰기: 같은 노드 기준으로 override가 기본을 치환
        return MergeNodeEvents(baseEvents, overrideEvents);
    }

    // ─────────────────────────────────────────────────────────────
    // 내부: 우승 오버레이 선정
    // 상황점수(QuestJournal.GetOverlaySituationScore)*1000 + priority 가 높은 것
    // ─────────────────────────────────────────────────────────────
    private bool TryPickWinnerOverlay(NPCId npcId, NPCState state,
                                      out QuestDialogueOverlay winner, out StatePatch winnerPatch)
    {
        winner = null;
        winnerPatch = null;

        if (questOverlays == null || questOverlays.Length == 0)
            return false;

        int bestComposite = int.MinValue;
        var journal = QuestJournal.Instance;

        foreach (var ov in questOverlays)
        {
            if (!ov || !ov.IsActive()) continue;
            if (!ov.TryGetPatch(npcId, state, out var patch) || patch == null) continue;

            int situation = journal ? journal.GetOverlaySituationScore(ov.QuestId, npcId) : 0;
            int composite = situation * 1000 + ov.Priority;

            if (composite > bestComposite)
            {
                bestComposite = composite;
                winner = ov;
                winnerPatch = patch;
            }
        }
        return winner != null && winnerPatch != null;
    }

    // ─────────────────────────────────────────────────────────────
    // 내부: 기본 NodeEvent 취득(있으면)
    // NPCDialogueDefaultSet은 기본 NodeEvent를 직접 Invoke하는 헬퍼만 있으므로
    // 기본 이벤트 배열을 외부로 내보내고 싶다면 세터를 확장하거나, 여기선 null 처리.
    // 필요 시 프로젝트에 맞춰 NPCDialogueDefaultSet에 "ExportDefaultNodeEvents" 추가 권장.
    // ─────────────────────────────────────────────────────────────
    private NodeEvent[] GetDefaultNodeEvents(NPCDialogueDefaultSet set, DialogueNodeData node)
    {
        // 기본적으로 SO에 저장된 NodeEvent 매핑이 있을 수 있으나
        // 공개 Getter가 없다면 null 반환(기본 유지).
        // 필요 시: NPCDialogueDefaultSet에 public NodeEvent[] Export() 구현하여 사용.
        return null;
    }

    // ─────────────────────────────────────────────────────────────
    // 내부: NodeEvent 병합(부분 덮어쓰기)
    // 같은 DialogueNodeData 기준으로 override가 기본을 덮음.
    // ─────────────────────────────────────────────────────────────
    private NodeEvent[] MergeNodeEvents(NodeEvent[] baseArr, NodeEvent[] overrideArr)
    {
        if ((overrideArr == null || overrideArr.Length == 0) && (baseArr == null || baseArr.Length == 0))
            return null;
        if (baseArr == null || baseArr.Length == 0)
            return overrideArr;
        if (overrideArr == null || overrideArr.Length == 0)
            return baseArr;

        var map = new Dictionary<DialogueNodeData, NodeEvent>();

        // 기본 등록
        for (int i = 0; i < baseArr.Length; i++)
        {
            var e = baseArr[i];
            if (e.node == null) continue;
            if (!map.ContainsKey(e.node)) map[e.node] = e;
        }
        // 오버라이드로 덮어쓰기
        for (int i = 0; i < overrideArr.Length; i++)
        {
            var e = overrideArr[i];
            if (e.node == null) continue;
            map[e.node] = e;
        }

        var list = new List<NodeEvent>(map.Values);
        return list.ToArray();
    }
}
