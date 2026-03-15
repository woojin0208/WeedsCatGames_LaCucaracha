using System;
using UnityEngine;
using UnityEngine.Events;

// 대화 노드에서 실행할 이벤트 정보를 보관한다.
public class DialogueEvent
{
    [Tooltip("이 텍스트 인덱스 바로 다음에 이벤트를 실행한다. (0-based)")]
    [field: SerializeField] public int TriggerIndex { get; private set; }
    [field: SerializeField] public UnityEvent UnityEvent { get; private set; }

    // 이벤트가 이미 실행됐는지 기록한다.
    public bool HasFired { get; set; }
}