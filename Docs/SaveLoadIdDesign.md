# Save/Load ID 설계 문서

## 1. 목적

본 문서는 Easy Save Asset 적용 전에 저장 대상과 ID 규칙을 먼저 고정하기 위한 설계 문서다.

현재 Quest, Dialogue, NPC는 리팩터링 예정이므로 세부 저장 구조는 확정하지 않는다. 대신 Save/Load에 영향을 주는 공통 ID 규칙과 시스템별 저장 경계를 먼저 정한다.

## 2. 기본 원칙

- 저장 데이터는 오브젝트 이름, Hierarchy 위치, 배열 순서에 의존하지 않는다.
- 저장 대상은 안정적인 문자열 ID로 식별한다.
- Static Data는 저장하지 않고, Runtime State만 저장한다.
- Static Data는 ID로 다시 찾는다.
- ID는 소문자 `snake_case`를 사용한다.
- ID는 타입 접두어를 포함한다.
- 저장에 들어가는 ID는 리팩터링 이후에도 유지 가능해야 한다.

예:

```text
저장하지 않음: WeaponDefinition 전체 데이터
저장함: weapon_honey_small, durability, instanceId
```

## 3. ID 네이밍 규칙

공통 형식:

```text
{type}_{owner_or_scene}_{name_or_index}
```

필요하면 소유 맥락을 생략할 수 있다.

```text
weapon_honey_small
npc_mechanic
quest_repair_pipe
scene_stage01
spawn_stage01_00
```

## 4. 시스템별 ID 규칙

### 4.1 Scene ID

씬 저장과 로드 기준으로 사용한다.

형식:

```text
scene_{scene_name}
```

예:

```text
scene_title
scene_tutorial
scene_stage01
scene_stage02
scene_boss
```

주의:

- Unity Scene 이름이 바뀌어도 저장 ID가 흔들리지 않도록 별도 매핑을 둘 수 있다.
- 초기에는 Scene 이름과 ID를 맞춰도 되지만, SaveData에는 `SceneManager.GetActiveScene().name`을 직접 저장하지 않는 방향을 권장한다.

### 4.2 Spawn Point ID

플레이어 로드 위치와 씬 진입 지점을 식별한다.

형식:

```text
spawn_{scene}_{index}
```

예:

```text
spawn_stage01_00
spawn_stage01_01
spawn_stage02_pipe_00
spawn_boss_entry
```

주의:

- 배열 인덱스만 저장하지 않는다.
- 씬 내 SpawnPoint 오브젝트에 고정 ID를 부여한다.
- Entrance 리팩터링 시 `targetSpawnId`로 연결한다.

### 4.3 Entrance ID

입구, 파이프, 자동 이동, 조건부 이동을 식별한다.

형식:

```text
entrance_{from_scene}_{type}_{index_or_target}
```

예:

```text
entrance_stage01_door_stage02
entrance_stage01_pipe_00
entrance_stage02_boss_gate
entrance_tutorial_exit
```

저장 후보:

- 잠금 해제 여부
- 이미 사용한 1회성 입구 여부
- 조건부 입구의 상태

주의:

- Entrance 리팩터링 전에는 세부 저장 구조를 만들지 않는다.
- 현재 단계에서는 ID 규칙만 확정한다.

### 4.4 Weapon ID

무기 종류를 식별한다. 무기 인스턴스와 구분한다.

형식:

```text
weapon_{name}_{variant}
```

예:

```text
weapon_honey_small
weapon_stick_small
weapon_pipe_small
```

저장 후보:

```text
weaponId
instanceId
durability
isEquipped
```

구분:

```text
weaponId: 무기 종류 ID
instanceId: 플레이 중 생성된 개별 무기 ID
```

예:

```json
{
  "instanceId": "weapon_inst_4f8a91",
  "weaponId": "weapon_honey_small",
  "durability": 2
}
```

주의:

- `gameObject.name`을 저장 키로 쓰지 않는다.
- `WeaponDefinition` 또는 별도 데이터에 `weaponId`를 추가하는 방향을 권장한다.

### 4.5 NPC ID

NPC를 식별한다.

형식:

```text
npc_{name_or_role}
```

예:

```text
npc_mechanic
npc_guard
npc_tutorial
npc_minimap
```

저장 후보:

```text
npcId
state
visibility
```

현재 단계:

```csharp
StateSaveData
{
    string id;
    string state;
}
```

주의:

- NPC, Dialogue, Quest는 리팩터링 예정이므로 현재는 `id + state` 수준만 확정한다.
- NPC별 특수 저장 구조는 NPC 리팩터링 이후 결정한다.

### 4.6 Quest ID

퀘스트를 식별한다.

형식:

```text
quest_{goal_or_topic}
```

예:

```text
quest_repair_pipe
quest_find_weapon
quest_open_boss_gate
quest_tutorial_escape
```

저장 후보:

```text
questId
state
completedObjectiveIds
```

현재 단계:

```csharp
StateSaveData
{
    string id;
    string state;
}
```

추후 확정:

- objective 저장 방식
- 보상 수령 여부
- 반복 가능 퀘스트 여부
- Dialogue 분기와 Quest 상태 연결 방식

### 4.7 Dialogue ID

대화 데이터와 노드를 식별한다.

대화 ID 형식:

```text
dialogue_{npc}_{context}
```

노드 ID 형식:

```text
node_{context}
```

예:

```text
dialogue_guard_intro
dialogue_mechanic_waiting_tool
dialogue_boss_before_fight

node_start
node_accept
node_refuse
node_after_quest
```

현재 단계:

- Dialogue 진행 상태는 저장하지 않는다.
- Dialogue는 Quest/NPC 리팩터링 이후 저장 필요 여부를 다시 판단한다.

저장 가능성이 있는 경우:

- 긴 대화 중간 저장이 필요할 때
- 1회성 대화 노드 재진입 방지가 필요할 때
- 선택 결과를 별도 Flag로 남겨야 할 때

권장:

- 대화 진행 자체보다 선택 결과를 `flag` 또는 `quest/npc state`로 저장한다.

### 4.8 Flag ID

1회성 이벤트, 컷신, 문 열림, 특정 선택 결과를 저장한다.

형식:

```text
flag_{owner}_{event}
```

예:

```text
flag_boss_intro_seen
flag_stage02_cutscene_seen
flag_pipe_repaired
flag_guard_allowed_entry
```

저장 후보:

```text
completedFlags: List<string>
```

주의:

- 단순 bool 필드가 늘어나는 것보다 flag ID 목록으로 관리하는 방향을 권장한다.
- 단, 자주 조회하는 핵심 상태는 전용 Runtime State로 분리할 수 있다.

### 4.9 Boss ID

보스 또는 주요 적 처치 상태를 식별한다.

형식:

```text
boss_{name_or_area}
```

예:

```text
boss_anchored_01
boss_stage02_main
```

저장 후보:

```text
defeatedBossIds: List<string>
```

주의:

- 보스가 하나뿐이면 세부 구조보다 `defeatedBossIds` 또는 `flag_boss_defeated`로 충분할 수 있다.
- Boss 리팩터링 이후 최종 저장 방식을 결정한다.

### 4.10 Key Binding ID

키 바인딩은 액션 ID 기준으로 저장한다.

형식:

```text
input_{state}_{action}
```

예:

```text
input_gameplay_attack
input_gameplay_jump
input_gameplay_interaction
input_pause_submit
input_dialogue_confirm
input_cutscene_skip
```

저장 후보:

```text
actionId
primary
secondary
```

예:

```json
{
  "actionId": "input_gameplay_jump",
  "primary": "Space",
  "secondary": "None"
}
```

주의:

- 현재 `KeyType`은 Gameplay 중심이므로, Pause/Dialogue/Cutscene까지 저장하려면 별도 action ID 구조가 더 적합하다.
- 지금 단계에서는 `KeyBindingData`의 primary/secondary 구조를 유지하고, Easy Save 적용 시 저장 포맷을 확정한다.

## 5. 저장 데이터 경계

### 5.1 저장하지 않는 데이터

정적 데이터는 저장하지 않는다.

```text
WeaponDefinition
Dialogue JSON 전체
Quest JSON 전체
NPC Profile 전체
Sprite
AudioClip
Prefab
AnimatorController
StatusEffectData
```

### 5.2 저장하는 데이터

플레이 중 바뀐 상태만 저장한다.

```text
현재 씬 ID
현재 스폰 포인트 ID
플레이어 위치
보유 무기 인스턴스 목록
현재 장착 무기 instanceId
NPC 상태
Quest 상태
완료된 flag 목록
처치한 boss ID 목록
키 바인딩 설정
```

## 6. SaveData 초안

현재 단계에서는 NPC/Quest/Dialogue 세부 구조를 확정하지 않고 placeholder 수준으로 둔다.

```csharp
[Serializable]
public class GameSaveData
{
    public string sceneId;
    public string spawnPointId;
    public Vector3 playerPosition;

    public List<WeaponSaveData> weapons;
    public string equippedWeaponInstanceId;

    public List<StateSaveData> npcStates;
    public List<StateSaveData> questStates;
    public List<string> completedFlags;
    public List<string> defeatedBossIds;

    public List<KeyBindingSaveData> keyBindings;
}
```

```csharp
[Serializable]
public class WeaponSaveData
{
    public string instanceId;
    public string weaponId;
    public int durability;
}
```

```csharp
[Serializable]
public class StateSaveData
{
    public string id;
    public string state;
}
```

```csharp
[Serializable]
public class KeyBindingSaveData
{
    public string actionId;
    public string primary;
    public string secondary;
}
```

## 7. 현재 단계에서 할 일

지금 바로 구현하지 않고, 각 리팩터링 때 아래 기준을 반영한다.

1. 새 저장 대상에는 고정 ID를 먼저 부여한다.
2. `gameObject.name`을 저장 ID로 사용하지 않는다.
3. 배열 인덱스만 저장하지 않는다.
4. Static Data는 저장하지 않고 ID로 참조한다.
5. Runtime State는 SaveData로 분리할 수 있게 유지한다.

## 8. 리팩터링별 적용 기준

### Input 리팩터링

- `KeyBindingData`는 primary/secondary 구조를 유지한다.
- Save/Load 적용 시 action ID 기반으로 저장한다.
- `KeyType` enum 값 자체를 저장 키로 직접 쓰지 않는다.

### Quest 리팩터링

- 모든 QuestData에 `questId`를 둔다.
- Quest 상태는 `NotStarted`, `InProgress`, `Completed`, `Rewarded` 같은 문자열 또는 enum으로 관리한다.
- 세부 objective 저장 구조는 Quest 리팩터링 이후 확정한다.

### Dialogue 리팩터링

- Dialogue 데이터에 `dialogueId`를 둔다.
- Node에는 `nodeId`를 둔다.
- Dialogue 선택 결과는 가능하면 flag, npc state, quest state로 저장한다.

### NPC 리팩터링

- NPC Profile 또는 NPC 컴포넌트에 `npcId`를 둔다.
- NPC 상태 저장은 우선 `id + state` 구조로 처리한다.
- NPC별 특수 저장 데이터는 최소화한다.

### Entrance 리팩터링

- Entrance에는 `entranceId`를 둔다.
- 이동 대상은 `targetSceneId`, `targetSpawnId`로 연결한다.
- 잠금 조건은 Quest/Flag/State 기반으로 표현한다.

### Weapon 리팩터링

- WeaponDefinition에 `weaponId`를 둔다.
- WeaponInstance에는 `instanceId`, `weaponId`, `durability`를 둔다.
- 장착 무기는 `equippedWeaponInstanceId`로 저장한다.

### Boss 리팩터링

- Boss에는 `bossId`를 둔다.
- 보스가 하나뿐이면 `defeatedBossIds` 또는 flag로 저장한다.
- 보스 패턴 진행 중간 저장은 현재 범위에서 제외한다.

## 9. 완료 기준

- 저장 후보마다 고정 ID 규칙이 있다.
- 저장 데이터와 정적 데이터의 경계가 분리되어 있다.
- NPC/Quest/Dialogue는 리팩터링 전이므로 placeholder 수준으로만 정의되어 있다.
- 이후 Easy Save 적용 시 `GameSaveData`로 확장 가능한 구조다.
- 저장 키가 오브젝트 이름, 씬 내 배치, 배열 순서에 의존하지 않는다.

## 10. 현재 프로젝트 ID 후보 점검

이 섹션은 현재 코드 기준의 ID 후보와 위험 지점을 기록한다. 실제 Save/Load 구현 전, 각 시스템 리팩터링 때 아래 항목을 기준으로 정리한다.

### 10.1 바로 활용 가능한 후보

#### NPC

현재 후보:

```csharp
NPCId
NPCState
```

위치:

```text
Assets/Scripts/Manager/NPCStateManager.cs
Assets/Scripts/NPC/NPCDialogue.cs
Assets/Scripts/ScriptableObjects/NPCDialogueDefaultSet.cs
Assets/Scripts/ScriptableObjects/NPCDialoguePreset.cs
```

평가:

- `NPCId` enum은 현재 NPC 상태 저장 키 후보로 사용할 수 있다.
- `NPCStateManager`는 `Dictionary<NPCId, NPCState>` 구조라 SaveData의 `npcId + state` 형태로 매핑하기 쉽다.

주의:

- enum 이름은 저장 문자열로 직접 사용하면 리팩터링 시 변경 위험이 있다.
- 최종 저장에는 `NPCId.FrogBoy` 대신 `npc_frog_boy` 같은 문자열 ID를 권장한다.
- NPC/Quest/Dialogue 리팩터링 이후 `NpcProfileData` 또는 NPC 컴포넌트에 명시적 `npcId` 문자열을 두는 방향이 더 안정적이다.

#### Weapon Instance

현재 후보:

```csharp
WeaponInstance.Id
WeaponBase.InstanceId
PlayerManager.CurrentEquipId
```

위치:

```text
Assets/Scripts/Inventory/WeaponInstance.cs
Assets/Scripts/Weapon/WeaponBase.cs
Assets/Scripts/Manager/PlayerManager.cs
```

평가:

- `WeaponInstance.Id`는 개별 무기 인스턴스 식별자로 활용 가능하다.
- `CurrentEquipId`는 장착 무기 저장 후보로 적절하다.

주의:

- 현재 `WeaponInstance.Id`는 `Guid.NewGuid()`로 생성된다.
- 새로 획득하는 무기에는 적절하지만, 저장/로드 후 같은 인스턴스를 복원하려면 저장된 `instanceId`를 다시 주입할 수 있는 생성자 또는 복원 메서드가 필요하다.

#### Key Binding

현재 후보:

```csharp
KeyBinding.Primary
KeyBinding.Secondary
KeyType
```

위치:

```text
Assets/Scripts/ScriptableObjects/KeyBindingData.cs
Assets/Scripts/Settings/InputSetting.cs
```

평가:

- primary/secondary 구조는 저장하기 쉽다.
- 현재 `InputSetting`은 primary만 수정하는 정책이므로 저장 포맷도 `primary`, `secondary`를 분리하면 된다.

주의:

- `KeyType`은 Gameplay 중심 enum이므로 Pause/Dialogue/Cutscene까지 포함한 최종 저장 키로 직접 쓰기에는 부족하다.
- 최종 SaveData에는 `input_gameplay_jump`, `input_pause_submit`, `input_dialogue_confirm` 같은 action ID를 권장한다.

### 10.2 수정이 필요한 후보

#### Weapon Type

현재 후보:

```csharp
WeaponDefinition.WeaponName
WeaponBase.gameObject.name
WeaponInstance.WeaponName
WeaponData.GetCurrentWeaponData(string weaponName)
```

위치:

```text
Assets/Scripts/ScriptableObjects/WeaponData.cs
Assets/Scripts/Weapon/WeaponDefinition.cs
Assets/Scripts/Weapon/WeaponBase.cs
Assets/Scripts/Manager/PlayerManager.cs
Assets/Scripts/UI/InventoryUI.cs
```

문제:

- 현재 무기 종류 식별이 `WeaponName` 또는 `gameObject.name`에 의존한다.
- `WeaponBase.Start()`에서 `(Clone)` 문자열을 제거한다.
- `PlayerManager.AddWeapon()`에서도 `weaponName.Contains("(Clone)")`을 처리한다.

판단:

- 저장 ID 후보로 부적합하다.
- `WeaponDefinition`에 별도 `weaponId`를 추가하는 방향이 필요하다.

권장:

```csharp
[field: SerializeField] public string WeaponId { get; private set; }
```

저장 예:

```json
{
  "instanceId": "weapon_inst_4f8a91",
  "weaponId": "weapon_honey_small",
  "durability": 2
}
```

#### Scene

현재 후보:

```csharp
SceneManager.GetActiveScene().name
GameManager.TryLoadScene(string sceneName)
PlayerManager.CurrentSceneName
```

위치:

```text
Assets/Scripts/Manager/GameManager.cs
Assets/Scripts/Manager/PlayerManager.cs
Assets/Scripts/Manager/MapManager.cs
Assets/Scripts/UI/Minimap.cs
Assets/Scripts/UI/StartVideo.cs
```

문제:

- 씬 이름 문자열을 직접 사용한다.
- `Minimap`은 `$"stage{num}"` 형식으로 씬명을 만든다.
- `StartVideo`는 `"Tutorial"` 문자열을 직접 넘긴다.

판단:

- 현재 동작에는 문제 없지만 저장 ID로는 불안정하다.

권장:

- `SceneId` 또는 `SceneNames` static class를 도입한다.
- SaveData에는 Unity 씬 이름보다 `scene_stage01` 같은 ID를 저장한다.
- 실제 로드는 `sceneId -> Unity scene name` 매핑으로 처리한다.

#### Spawn Point

현재 후보:

```csharp
Enterance.CurrentSpawnPoint
Enterance.NextSpawnPoint
PlayerManager.CurrentSpawnPoint
```

위치:

```text
Assets/Scripts/Enteracnes/Enterance.cs
Assets/Scripts/Manager/MapManager.cs
Assets/Scripts/Manager/PlayerManager.cs
Assets/Scripts/Player/PlayerBase.cs
```

문제:

- 현재 스폰 지점이 int 값으로만 매칭된다.
- 배열 순서보다는 낫지만, 씬 내 의미가 명시적이지 않다.

판단:

- 임시 저장 후보로는 사용 가능하다.
- 최종 Save/Load에는 명시적 `spawnId`가 더 적합하다.

권장:

```csharp
[SerializeField] private string currentSpawnId;
[SerializeField] private string nextSpawnId;
```

예:

```text
spawn_stage01_00
spawn_stage01_pipe_00
spawn_boss_entry
```

#### Entrance

현재 후보:

```csharp
EnteranceType
currentArea
nextArea
CurrentSpawnPoint
NextSpawnPoint
```

위치:

```text
Assets/Scripts/Enteracnes/Enterance.cs
Assets/Scripts/Enteracnes/AutoEnterance.cs
Assets/Scripts/Enteracnes/GuardedEnterance.cs
Assets/Scripts/Enteracnes/PipeEnterance.cs
Assets/Scripts/Enteracnes/NPCEnterance.cs
```

문제:

- `currentArea`는 현재 코드에서 저장/로드 기준으로 쓰이지 않는다.
- `nextArea`는 Unity 씬명에 가깝다.
- `EnteranceType`은 동작 타입이지 개별 입구 ID가 아니다.

판단:

- 현재 구조에는 안정적인 entrance ID가 없다.

권장:

```csharp
[SerializeField] private string entranceId;
[SerializeField] private string targetSceneId;
[SerializeField] private string targetSpawnId;
```

Entrance 리팩터링 때 확정한다.

### 10.3 리팩터링 이후 확정할 후보

#### Quest

현재 상태:

```text
Quest 시스템 정의가 아직 약함
NPCState, Dialogue, ItemRequirement가 퀘스트 역할을 일부 대체 중
```

판단:

- 지금 구체 SaveData를 만들지 않는다.
- Quest 리팩터링 때 모든 QuestData에 `questId`를 추가한다.

권장 예:

```text
quest_repair_pipe
quest_find_weapon
quest_open_boss_gate
```

#### Dialogue

현재 후보:

```csharp
DialogueNodeData ScriptableObject 참조
DialogueNodeData.name
DialogueOption.nextNode
```

위치:

```text
Assets/Scripts/ScriptableObjects/DialogueNodeData.cs
Assets/Scripts/Manager/DialogueManager.cs
Assets/Scripts/NPC/NPCDialogue.cs
```

문제:

- 현재 노드는 ScriptableObject 참조 중심이다.
- 저장 ID로 사용할 `dialogueId`, `nodeId`가 없다.
- `NPCDialogue`의 NodeEvent 매핑도 노드 참조 기반이다.

판단:

- Dialogue 리팩터링 전에는 저장 구조를 확정하지 않는다.
- 대화 진행 자체보다 대화 결과를 flag/npc state/quest state로 저장하는 편이 우선이다.

권장 예:

```text
dialogue_guard_intro
node_start
node_accept
```

#### Boss

현재 후보:

```text
AnchoredBossBase
AnchoredBossController
GameManager.Stage2CutScene
```

문제:

- 명시적 bossId가 없다.
- 컷신 여부가 `Stage2CutScene` bool로 하드코딩되어 있다.

판단:

- Boss 리팩터링 때 `bossId` 또는 flag로 정리한다.

권장 예:

```text
boss_anchored_01
flag_stage02_cutscene_seen
flag_boss_anchored_01_defeated
```

### 10.4 현재 코드에서 저장 ID로 피해야 할 값

아래 값은 SaveData의 영구 키로 사용하지 않는다.

```text
gameObject.name
WeaponDefinition.WeaponName
DialogueNodeData.name
SceneManager.GetActiveScene().name
Enterance.CurrentSpawnPoint 단독 int
Enterance.NextSpawnPoint 단독 int
배열 인덱스
Hierarchy 순서
프리팹 인스턴스 이름의 "(Clone)" 제거 결과
```

### 10.5 우선 적용 추천

1. Weapon 리팩터링 때 `WeaponDefinition.WeaponId` 추가
2. Entrance 리팩터링 때 `entranceId`, `targetSceneId`, `targetSpawnId` 추가
3. Quest 리팩터링 때 `questId` 추가
4. Dialogue 리팩터링 때 `dialogueId`, `nodeId` 추가
5. NPC 리팩터링 때 enum 기반 `NPCId`를 문자열 저장 ID로 매핑
6. Boss 리팩터링 때 `bossId` 또는 flag ID 추가
