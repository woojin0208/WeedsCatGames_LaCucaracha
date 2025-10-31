// ─────────────────────────────────────────────────────────────
// QuestEvents.cs
// 역할: 게임플레이 → 저널로 퀘스트 진행 이벤트 보고
// ─────────────────────────────────────────────────────────────
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class QuestEvents
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init() { /* 도메인 리로드 대비 자리 */ }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static QuestJournal J()
    {
        var j = QuestJournal.Instance;
        if (j == null)
            Debug.LogWarning("[QuestEvents] QuestJournal.Instance 가 없습니다. 씬에 배치되었는지 확인하세요.");
        return j;
    }

    public static void ReportKill(string enemyId, int add = 1)
    {
        var j = J(); if (j == null) return;
        j.AddProgress(ObjectiveType.Kill, enemyId, add);
    }

    public static void ReportCollect(string itemId, int add = 1)
    {
        var j = J(); if (j == null) return;
        j.AddProgress(ObjectiveType.Collect, itemId, add);
    }

    public static void ReportEnter(string areaId, int add = 1)
    {
        var j = J(); if (j == null) return;
        j.AddProgress(ObjectiveType.GoTo, areaId, add);
    }

    /// <summary>NPC와 대화 시작/완료 시 호출. targetId 규약은 NPCId.ToString()과 맞추세요.</summary>
    public static void ReportTalk(string npcId, int add = 1)
    {
        var j = J(); if (j == null) return;
        j.AddProgress(ObjectiveType.TalkTo, npcId, add);
    }
}
