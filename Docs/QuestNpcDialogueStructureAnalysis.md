# Quest / NPC / Dialogue 구조 분석 문서

## 1. 목적

본 문서는 Quest 리팩터링 전에 현재 프로젝트에서 퀘스트 역할을 수행하는 코드 흐름을 분석하고, 기능별로 분류하기 위한 문서다.

현재 프로젝트에는 독립된 Quest 시스템이 거의 없고, 퀘스트 역할이 NPC, Dialogue, Item, Entrance, GameManager 상태에 분산되어 있다. 따라서 바로 QuestManager를 구현하기보다, 먼저 기존 역할을 분류하고 리팩터링 경계를 정한다.

## 2. 현재 구조 요약

현재 퀘스트 역할은 다음 시스템에 흩어져 있다.

```text
NPCDialogue
NPCStateManager
DialogueManager
ItemRequirementChecker
ItemGiver
DonationBox
TutorialNPC
MovementNPC
LoadCheckNPC
NPCVisibilityByState
DialogueRouter
GuardedEnterance
BossEnterance
GameManager
PlayerManager
```

핵심 흐름:

```text
NPC 상호작용
-> NPCDialogue.Interactive()
-> NPCStateManager.GetState(NPCId)
-> NPCState에 맞는 DialogueNodeData 선택
-> DialogueManager.StartDialogue()
-> NodeEvent / UnityEvent 실행
-> Item 검사, 보상 지급, NPC 상태 변경, 입구 해금 등이 개별 스크립트에서 처리됨
```

현재 구조의 특징:

- NPC 상태가 퀘스트 상태를 일부 대체한다.
- DialogueNodeData와 UnityEvent가 퀘스트 액션 실행 지점으로 사용된다.
- 아이템 조건, 보상, 누적 수치, 입구 해금이 독립된 QuestData 없이 개별 MonoBehaviour에 들어 있다.
- 저장해야 할 진행 상태가 `NPCState`, `GameManager` 필드, 오브젝트 active 상태 등에 흩어져 있다.

## 3. 현재 구현 위치별 역할 분류

| 현재 구현 위치 | 실제 역할 | 퀘스트 분류 | 현재 문제 | 리팩터링 방향 |
| --- | --- | --- | --- | --- |
| `NPCDialogue` | NPC 상호작용, 상태별 대화 선택, 노드 이벤트 실행 | 대화 진행형 | NPC 상태, 대화 선택, 이벤트 연결이 한 클래스에 집중됨 | `NpcDialogueResolver`, `DialogueRunner`, `QuestAction`으로 역할 분리 |
| `NPCStateManager` | NPC별 상태 저장 | 상태 저장 / 진행도 | NPC 상태가 퀘스트 상태 역할까지 수행 | 단기 유지, 장기적으로 QuestState와 NPCState 분리 |
| `DialogueManager` | 대화 진행, 선택지 처리, 노드 이동 | 대화 실행 | Quest 조건/보상 자체는 모르지만 UnityEvent 실행 통로가 됨 | Dialogue 진행만 담당하고 Action 실행은 별도 Executor로 분리 |
| `ItemRequirementChecker` | 현재 장착 무기가 요구 아이템인지 검사 | 아이템 요구형 | 무기 비교가 `w.name == currentWeapon.name`에 의존 | `QuestCondition: HasItem/EquippedWeapon`으로 이동 |
| `ItemGiver` | 특정 무기 지급 | 보상 지급형 | 보상이 MonoBehaviour와 Prefab name에 의존 | `QuestReward: GiveWeapon`으로 이동 |
| `DonationBox` | 아이템 기부, 누적 점수, 완료 처리 | 수집/기부형 | 진행도가 `GameManager.donationScore`에 저장됨 | `QuestProgress` 또는 `CounterObjective`로 이동 |
| `TutorialNPC` | 적 피격 이벤트에 따라 튜토리얼 문구 진행, 완료 처리 | 튜토리얼형 | Tutorial 진행도와 UI 표시가 NPC에 직접 결합 | `TutorialQuestStep` 또는 별도 Tutorial 시스템으로 분리 |
| `MovementNPC` | 대화 이벤트 후 NPC 이동 및 비활성화 | NPC 이동/상태 변화형 | 이동 완료 이벤트와 대화 재시작이 직접 연결 | `QuestAction: MoveNpc`, `SetNpcVisible`로 분리 |
| `LoadCheckNPC` | 상태별 자동 대화/이벤트 실행 | 상태 기반 자동 이벤트 | 이름은 Load지만 실제로는 상태 조건 이벤트 트리거 | `ConditionTrigger` 또는 `QuestEventTrigger`로 정리 |
| `NPCVisibilityByState` | NPC 상태에 따른 표시 여부 결정 | NPC 표시 조건 | 상태 조건이 컴포넌트별로 분산됨 | `NpcVisibilityCondition` 또는 공통 Condition으로 이동 |
| `DialogueRouter` | NPC 이동 완료 후 특정 노드 대화 시작 | 대화 분기 연결 | Debug.Log와 이벤트 연결이 임시적이고 강결합 | `DialogueAction: StartDialogue`로 이동 |
| `GuardedEnterance` | NPC 상태에 따라 입구 통과 제한 | 입구 해금형 | Entrance가 NPCState를 직접 조회 | `EntranceCondition: NpcState/QuestState`로 이동 |
| `BossEnterance` | 보스 비활성화 여부로 입구 사용 가능 | 보스 클리어형 | 보스 activeSelf에 의존 | `BossState` 또는 `flag_boss_defeated`로 이동 |
| `GameManager` | 전역 플래그, 기부 점수, 씬 전환 | 전역 진행 상태 | `donationScore`, `Stage2CutScene`가 직접 필드로 존재 | Flag/QuestProgress 저장 구조로 이동 |
| `PlayerManager` | 무기 보유/장착 상태 | 인벤토리 조건/보상 | 무기 종류가 이름 기반 | Weapon ID 기반으로 변경 |

## 4. 퀘스트 유형 분류

### 4.1 대화 진행형 Quest

현재 위치:

```text
NPCDialogue
NPCStateManager
DialogueManager
DialogueNodeData
NodeEvent
```

현재 방식:

- NPC 상호작용 시 `NPCState`를 읽는다.
- `(int)NPCState`를 배열 인덱스로 사용해 시작 대화 노드를 고른다.
- 대화 노드 진입/종료/선택 시 UnityEvent가 실행된다.

문제:

- NPC 상태와 Quest 상태가 구분되지 않는다.
- 상태 enum 값이 배열 인덱스와 강하게 결합되어 있다.
- 대화 데이터가 ScriptableObject 참조 중심이라 JSON 전환 시 매핑 구조가 필요하다.

리팩터링 방향:

```text
NpcProfileData
NpcStateStore
DialogueRepository
DialogueRunner
DialogueConditionResolver
DialogueActionExecutor
```

우선 결정할 것:

- NPCState를 유지할지, QuestState로 대체할지
- Dialogue 분기 기준을 NPCState, QuestState, Flag 중 무엇으로 둘지

### 4.2 아이템 요구형 Quest

현재 위치:

```text
ItemRequirementChecker
PlayerManager
WeaponBase
WeaponData
```

현재 방식:

- 현재 장착 무기를 가져온다.
- 요구 무기 배열과 이름 비교를 한다.
- 성공 시 NPC 상태를 Completed로 바꾸고, 실패 시 Failed로 바꾼다.
- 현재 장착 무기를 제거한다.

문제:

- 무기 비교가 이름 기반이다.
- 성공/실패 결과가 NPCState에 직접 기록된다.
- 아이템 소비 여부가 조건 검사 스크립트 안에 고정되어 있다.

리팩터링 방향:

```text
QuestCondition: HasWeapon / EquippedWeapon
QuestAction: ConsumeWeapon
QuestAction: SetNpcState
QuestAction: SetQuestState
```

권장 데이터 예:

```json
{
  "type": "equipped_weapon",
  "weaponId": "weapon_honey_small",
  "consumeOnSuccess": true
}
```

### 4.3 아이템 지급형 Quest

현재 위치:

```text
ItemGiver
PlayerManager
WeaponBase
```

현재 방식:

- 지정된 `WeaponBase targetItem`의 이름과 내구도를 사용해 인벤토리에 추가한다.
- 필요하면 다음 노드를 다시 실행한다.

문제:

- 보상이 Prefab/Scene 오브젝트 참조에 의존한다.
- 지급하는 무기가 `targetItem.name` 기반이다.
- 대화 진행 제어가 보상 지급 스크립트 안에 들어 있다.

리팩터링 방향:

```text
QuestReward: GiveWeapon
DialogueAction: ContinueDialogueNextFrame
```

권장 데이터 예:

```json
{
  "type": "give_weapon",
  "weaponId": "weapon_pipe_small",
  "durability": 3
}
```

### 4.4 수집/기부형 Quest

현재 위치:

```text
DonationBox
GameManager.donationScore
NPCDialogue
DialogueNodeData.CloneRuntime()
```

현재 방식:

- 현재 장착 무기가 기부 가능한 무기인지 검사한다.
- 아이템별 점수를 `GameManager.donationScore`에 누적한다.
- 진행 중 대화를 런타임 클론으로 만들어 누적 점수를 표시한다.
- 목표 점수 도달 시 NPCState를 Completed로 바꾼다.

문제:

- 진행도가 GameManager에 직접 저장된다.
- 특정 기부 퀘스트 전용 점수가 전역 필드로 존재한다.
- 대화 텍스트를 런타임에 직접 수정한다.

리팩터링 방향:

```text
QuestProgress.Counter
QuestObjective: DonateWeapon
QuestAction: SetNpcState
Dialogue variable formatting
```

권장 데이터 예:

```json
{
  "objectiveId": "donate_value",
  "type": "counter",
  "targetValue": 20
}
```

### 4.5 튜토리얼형 Quest

현재 위치:

```text
TutorialNPC
EnemyBase.OnDamagedAction
NPCStateManager
Tutorial UI Text
```

현재 방식:

- 특정 적이 피격될 때마다 튜토리얼 문구가 다음 단계로 진행된다.
- 마지막 단계에 도달하면 NPCState가 Completed가 된다.
- 상호작용 시 별도 guardText를 표시한다.

문제:

- 튜토리얼 단계, UI 표시, NPC 상태 변경이 한 클래스에 묶여 있다.
- 진행 조건이 특정 EnemyBase 이벤트에 직접 결합되어 있다.

리팩터링 방향:

```text
TutorialStepData
QuestObjective: HitEnemy / PerformAction
TutorialUI
QuestAction: SetNpcState
```

단기 판단:

- 튜토리얼이 작다면 완전 Quest 시스템에 포함하지 않고 별도 Tutorial 시스템으로 유지해도 된다.
- 단, 저장할 필요가 있으면 step index 또는 flag가 필요하다.

### 4.6 NPC 이동/상태 변화형 Quest

현재 위치:

```text
MovementNPC
DialogueRouter
NPCVisibilityByState
LoadCheckNPC
```

현재 방식:

- UnityEvent로 NPC 이동을 시작한다.
- 이동 완료 후 `OnDialogueSignal`을 발행한다.
- DialogueRouter가 특정 노드 대화를 시작한다.
- NPCVisibilityByState가 상태 조건에 따라 NPC를 숨긴다.

문제:

- 이동 완료 후 대화 연결이 이벤트 구독과 Debug.Log 중심으로 구성되어 있다.
- NPC 표시 조건이 각 컴포넌트에 분산되어 있다.
- `LoadCheckNPC`는 이름과 달리 저장 로드보다 상태 기반 자동 이벤트에 가깝다.

리팩터링 방향:

```text
QuestAction: MoveNpc
QuestAction: SetNpcVisible
QuestAction: StartDialogue
NpcVisibilityCondition
ConditionTrigger
```

### 4.7 입구 해금형 Quest

현재 위치:

```text
GuardedEnterance
BossEnterance
Enterance
NPCStateManager
```

현재 방식:

- GuardedEntrance는 특정 NPC 상태가 Completed 이상이면 통과시킨다.
- 조건 미달이면 guardNPC 대화를 시작한다.
- BossEntrance는 보스 오브젝트가 비활성화되면 통과 가능하다.

문제:

- Entrance가 NPCState와 Boss activeSelf를 직접 검사한다.
- 조건식이 Entrance 하위 클래스에 하드코딩되어 있다.
- 저장 가능한 `entranceId`, `conditionId`, `flagId`가 없다.

리팩터링 방향:

```text
EntranceData
EntranceConditionResolver
Condition: NpcState
Condition: QuestState
Condition: Flag
Condition: BossDefeated
BlockedDialogueId
```

## 5. 현재 구조의 주요 문제

### 5.1 상태 의미가 섞여 있음

`NPCState`가 다음 의미를 동시에 가진다.

```text
NPC 대화 상태
퀘스트 진행 상태
입구 해금 조건
NPC 표시 조건
튜토리얼 완료 여부
```

개선 방향:

- NPCState는 NPC의 표시/관계 상태로 제한한다.
- QuestState는 별도 Quest 시스템에서 관리한다.
- 조건 판단은 공통 ConditionResolver로 이동한다.

### 5.2 데이터와 로직이 섞여 있음

현재 UnityEvent와 MonoBehaviour가 퀘스트 조건/보상/분기를 직접 수행한다.

개선 방향:

- 조건은 `ConditionData`
- 결과는 `ActionData`
- 실행은 `ConditionResolver`, `ActionExecutor`
- 콘텐츠는 JSON 또는 ScriptableObject로 분리한다.

### 5.3 이름 기반 식별이 많음

대표 사례:

```text
w.name == currentWeapon.name
targetItem.name
gameObject.name
DialogueNodeData.name
```

개선 방향:

- Weapon은 `weaponId`
- NPC는 `npcId`
- Dialogue는 `dialogueId`, `nodeId`
- Entrance는 `entranceId`
- Flag는 `flagId`

### 5.4 배열 인덱스 의존

대표 사례:

```csharp
int idx = (int)state;
dialogueNodeData[idx]
```

개선 방향:

- 상태별 대화 매핑을 배열이 아니라 명시적 매핑 데이터로 변경한다.
- JSON 전환 시 `state -> dialogueId` 형태가 적합하다.

## 6. 추천 리팩터링 방향

### 6.1 단기 목표

Quest 시스템을 바로 크게 구현하지 않고, 현재 분산된 역할을 다음 형태로 정리한다.

```text
QuestData
QuestProgress
QuestManager
QuestConditionResolver
QuestActionExecutor
```

단, NPC/Dialogue와 동시에 묶어서 진행한다.

### 6.2 최소 핵심 구조

```csharp
public enum QuestState
{
    NotStarted,
    InProgress,
    Completed,
    Rewarded,
    Failed
}
```

```csharp
[Serializable]
public class QuestProgress
{
    public string questId;
    public QuestState state;
    public List<string> completedObjectiveIds;
}
```

```csharp
public interface IQuestCondition
{
    bool IsMet();
}
```

```csharp
public interface IQuestAction
{
    void Execute();
}
```

실제 구현에서는 JSON 데이터와 Resolver 기반으로 전환한다.

### 6.3 공통 Condition 후보

```text
quest_state
npc_state
has_weapon
equipped_weapon
flag_set
boss_defeated
scene_entered
counter_reached
```

### 6.4 공통 Action 후보

```text
set_quest_state
set_npc_state
give_weapon
consume_weapon
set_flag
start_dialogue
move_npc
set_npc_visible
unlock_entrance
play_vfx
play_cutscene
```

## 7. 리팩터링 우선순위

권장 순서:

1. 현재 퀘스트 역할 목록 확정
2. QuestState와 QuestProgress 정의
3. 공통 Condition/Action 타입 목록 정의
4. NPCState와 QuestState의 경계 결정
5. Dialogue 분기 기준 정의
6. ItemRequirementChecker / ItemGiver / DonationBox를 QuestCondition/Action 후보로 치환
7. GuardedEntrance / BossEntrance를 EntranceCondition 후보로 치환
8. Save/Load용 `questId`, `npcId`, `flagId` 연결

## 8. 유지하거나 후순위로 둘 부분

아래는 즉시 갈아엎기보다 후순위로 둔다.

```text
TutorialNPC 전체 재작성
MovementNPC 전체 재작성
BossEntrance 전체 재작성
DialogueNodeData JSON 전면 전환
NPCDialogue 전체 분해
```

우선은 기존 동작을 유지하면서 Quest/Condition/Action 개념으로 이동 가능한 지점부터 정리한다.

## 9. 다음 결정 사항

리팩터링 시작 전 아래를 결정해야 한다.

1. NPCState를 유지할 것인가?
2. QuestState를 새로 만들 것인가?
3. Dialogue 분기는 NPCState, QuestState, Flag 중 무엇을 우선할 것인가?
4. QuestData는 JSON으로 둘 것인가, ScriptableObject로 둘 것인가?
5. UnityEvent는 어디까지 유지할 것인가?
6. ItemRequirementChecker, ItemGiver, DonationBox를 Quest 시스템으로 먼저 옮길 것인가?

## 10. 결론

현재 프로젝트의 퀘스트는 독립 시스템이 아니라 NPC 상태와 Dialogue 이벤트에 분산되어 있다.

따라서 다음 리팩터링은 단순히 `QuestManager`를 추가하는 방식이 아니라, 기존 NPC/Dialogue/Item/Entrance 흐름에서 퀘스트 역할을 분리하는 방식으로 진행해야 한다.

가장 먼저 정리할 대상은 다음 세 가지다.

```text
ItemRequirementChecker -> QuestCondition
ItemGiver -> QuestReward/Action
DonationBox -> QuestProgress + CounterObjective
```

이 세 가지를 정리하면 Quest 시스템의 기본 형태를 포트폴리오에서 설명하기 쉬워진다.

## 11. Quest 개념 분리 기준

이 섹션은 클래스 작성 전에 먼저 확정할 개념 경계다. 현재 단계에서는 문서 기준으로 책임을 분리하고, 이후 구현 시 이 경계를 기준으로 클래스를 작성한다.

### 11.1 핵심 분리 원칙

Quest 리팩터링의 목적은 기존 기능을 모두 QuestManager로 몰아넣는 것이 아니다. 각 시스템이 맡아야 할 책임을 분명히 나누는 것이다.

```text
Quest
- 어떤 목표가 진행 중인지
- 완료 조건이 무엇인지
- 완료되면 어떤 결과가 발생하는지

Condition
- 현재 조건이 만족됐는지 판단

Action
- 조건 만족 또는 선택 결과로 실제 변경을 실행

NPC
- NPC 자신의 ID, 현재 상태, 상호작용 진입점

Dialogue
- 대화 노드 진행, 선택지 표시, 선택 결과 요청

Entrance
- 입구 사용 요청, 이동 대상, 조건 실패 시 차단 처리
```

### 11.2 현재 코드에서 분리할 책임

| 현재 책임 | 현재 위치 | 분리 후 책임 | 비고 |
| --- | --- | --- | --- |
| 현재 장착 무기가 요구 무기인지 검사 | `ItemRequirementChecker` | `Condition: equipped_weapon` | 무기 이름 비교는 `weaponId` 비교로 변경 |
| 요구 아이템 소비 | `ItemRequirementChecker` | `Action: consume_weapon` | 성공/실패 시 소비 여부는 데이터로 결정 |
| NPC 상태를 완료/실패로 변경 | `ItemRequirementChecker` | `Action: set_npc_state` 또는 `set_quest_state` | NPCState와 QuestState 경계 결정 필요 |
| 특정 무기 지급 | `ItemGiver` | `Action: give_weapon` | `targetItem.name` 대신 `weaponId` 사용 |
| 다음 대화 노드 재실행 | `ItemGiver`, `ItemRequirementChecker`, `DonationBox` | `Action: continue_dialogue` 또는 DialogueRunner 책임 | 코루틴 WaitFrame 방식은 후순위 정리 |
| 기부 점수 누적 | `DonationBox`, `GameManager.donationScore` | `QuestProgress: counter` | 전역 필드 제거 후보 |
| 기부 완료 처리 | `DonationBox` | `Condition: counter_reached`, `Action: set_quest_state` | 완료 이후 NPCState 변경은 Action으로 처리 |
| NPC 상태별 대화 선택 | `NPCDialogue` | `NpcDialogueResolver` | 배열 인덱스 매핑 제거 후보 |
| 노드 진입/종료 이벤트 실행 | `NPCDialogue`, `DialogueManager` | `DialogueActionExecutor` | UnityEvent 유지 범위 결정 필요 |
| 경비 NPC 상태로 입구 통과 판단 | `GuardedEnterance` | `EntranceCondition: npc_state` 또는 `quest_state` | Entrance 리팩터링 때 처리 |
| 보스 비활성 여부로 입구 통과 판단 | `BossEnterance` | `EntranceCondition: boss_defeated` 또는 `flag_set` | Boss 리팩터링 때 처리 |
| NPC 표시 여부 판단 | `NPCVisibilityByState` | `NpcVisibilityCondition` | 상태 조건 공통화 후보 |

### 11.3 새 개념별 역할 정의

#### QuestData

정적 퀘스트 정의 데이터다.

담당:

```text
questId
title
description
startConditions
objectives
completeConditions
rewards/actions
```

담당하지 않음:

```text
현재 진행도 저장
Unity 오브젝트 직접 조작
대화 UI 표시
NPC 애니메이션 제어
```

#### QuestProgress

런타임 퀘스트 진행 상태다.

담당:

```text
questId
state
counter values
completed objective ids
reward claimed 여부
```

담당하지 않음:

```text
조건 판정 로직
보상 실행 로직
대화 노드 선택
```

#### QuestManager

퀘스트 상태 변경과 진행도 저장소 역할을 맡는다.

담당:

```text
QuestProgress 조회
QuestProgress 변경
Quest 상태 변경 이벤트 발행
조건 평가 요청
액션 실행 요청
```

담당하지 않음:

```text
UI 표시
NPC 직접 이동
무기 Prefab 직접 Instantiate
Dialogue 노드 직접 진행
```

#### Condition

현재 게임 상태가 특정 조건을 만족하는지 확인한다.

후보:

```text
quest_state
npc_state
has_weapon
equipped_weapon
flag_set
boss_defeated
counter_reached
scene_entered
```

예:

```json
{
  "type": "equipped_weapon",
  "weaponId": "weapon_honey_small"
}
```

#### Action

조건 만족, 대화 선택, 퀘스트 완료 시 실제 변경을 실행한다.

후보:

```text
set_quest_state
set_npc_state
give_weapon
consume_weapon
set_flag
start_dialogue
continue_dialogue
move_npc
set_npc_visible
unlock_entrance
play_vfx
play_cutscene
```

예:

```json
{
  "type": "give_weapon",
  "weaponId": "weapon_pipe_small",
  "durability": 3
}
```

### 11.4 NPCState와 QuestState 경계

현재 `NPCState`는 여러 의미를 동시에 가진다.

```text
FirstMeet
Repeat
InProgress
Failed
Completed
```

리팩터링 후 권장 경계:

```text
NPCState
- NPC의 대화/표시/관계 상태
- 예: default, unavailable, moved, hidden, friendly

QuestState
- 퀘스트 진행 상태
- 예: NotStarted, InProgress, Completed, Rewarded, Failed
```

단기 방안:

- 기존 `NPCState`는 바로 제거하지 않는다.
- Quest 시스템 초안에서는 `QuestState`를 새로 만들고, 기존 NPCState는 호환 레이어로 둔다.
- 기존 NPC 대화 분기는 당분간 NPCState를 유지하되, 새 Quest 데이터는 QuestState를 기준으로 설계한다.

장기 방안:

- Dialogue 분기는 `Condition` 기반으로 변경한다.
- 특정 NPC 상태가 아니라 `quest_state`, `flag_set`, `has_weapon` 등을 조합해 선택지를 결정한다.

### 11.5 Dialogue와 Quest의 경계

Dialogue는 퀘스트를 직접 완료시키지 않는다.

권장 흐름:

```text
Dialogue 선택
-> DialogueAction 요청
-> QuestActionExecutor 실행
-> QuestManager 상태 변경
-> 상태 변경 이벤트 발행
-> NPC/Entrance/UI가 필요 시 반응
```

Dialogue가 담당할 것:

```text
대사 표시
선택지 표시
선택된 optionId 전달
다음 node 이동
```

Quest가 담당할 것:

```text
선택 결과로 상태 변경
조건 만족 여부 판단
보상 지급
진행도 저장
```

### 11.6 구현 순서 제안

클래스 작성은 아래 순서로 진행하는 것이 안전하다.

1. `QuestState`, `QuestProgress` 정의
2. `ConditionData`, `ActionData` 초안 정의
3. `QuestManager`는 상태 저장과 조회만 먼저 구현
4. `ItemRequirementChecker` 역할을 `equipped_weapon` 조건으로 치환
5. `ItemGiver` 역할을 `give_weapon` 액션으로 치환
6. `DonationBox` 역할을 `counter` 진행도로 치환
7. 이후 Dialogue/NPC와 연결

### 11.7 지금 당장 구현하지 않을 것

아래는 개념만 정의하고, 바로 구현하지 않는다.

```text
전체 Dialogue JSON 전환
NPCDialogue 전체 제거
UnityEvent 전면 제거
모든 Entrance 조건 데이터화
TutorialNPC 전체 Quest화
Boss 상태 저장 전체 구현
```

이 항목들은 Quest 핵심 구조가 안정된 뒤 진행한다.
