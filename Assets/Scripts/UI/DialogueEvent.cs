using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DialogueEvent
{
    [Tooltip("이 텍스트 인덱스 바로 다음에 이벤트를 실행합니다 (0-based)")]
    [field : SerializeField ]public int TriggerIndex { get; private set; }
    [field: SerializeField] public UnityEvent UnityEvent { get; private set; }
     public bool HasFired { get; set; } // 이미 실행됐는지 체크용
}
