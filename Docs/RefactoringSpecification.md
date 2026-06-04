# La Cucaracha 리팩터링 명세서

## 1. 문서 목적

본 문서는 입사지원 전 최종 리팩터링의 범위, 우선순위, 설계 원칙, 코드 작성 기준을 정리하기 위한 개발 명세서다.

리팩터링의 목표는 단순한 코드 정리가 아니라, 현재 구현된 기능을 유지하면서 포트폴리오에서 설명 가능한 구조로 개선하는 것이다. 특히 입력, 퀘스트, 대화, NPC, 저장/불러오기 연계 구조를 중심으로 유지보수성과 확장성을 높인다.

## 2. 리팩터링 목표

- 기존 게임 기능의 동작을 유지한다.
- 하드코딩된 콘텐츠 흐름을 데이터 기반 구조로 전환한다.
- Quest, Dialogue, NPC, Entrance, Save/Load가 공통 ID와 조건 구조를 공유하도록 설계한다.
- SOLID 원칙을 가능한 범위에서 적용하되, 과도한 추상화는 피한다.
- 응집도는 높이고 결합도는 낮춘다.
- 포트폴리오에서 설명 가능한 대표 시스템을 만든다.
- 제출 전 코드, 주석, 인코딩, 폴더명, 디버그 로그를 정리한다.

## 3. 리팩터링 방향

전체 시스템을 같은 깊이로 재작성하지 않는다. 포트폴리오 설명 가치와 유지보수 효과가 큰 핵심 시스템에 리팩터링 비용을 집중한다.

대규모 리팩터링 대상:

- Input
- Quest
- Dialogue
- NPC
- Save/Load 대비 ID 구조
- Entrance
- Weapon
- Boss

안정화 중심 정리 대상:

- Effect
- Animation
- Sound
- 일반 Enemy FSM
- UI 세부 컴포넌트

## 4. 공통 설계 원칙

### 4.1 SOLID 적용 기준

- 단일 책임 원칙: 하나의 클래스가 입력, 상태, UI, 데이터, 이벤트 실행을 동시에 처리하면 분리한다.
- 개방 폐쇄 원칙: 새 퀘스트, 대화, 입구, NPC 조건을 추가할 때 기존 C# 코드를 계속 수정해야 한다면 데이터화 또는 전략화한다.
- 의존 역전 원칙: Manager 간 직접 참조가 과도하면 이벤트, 인터페이스, Resolver, Repository를 활용한다.
- 인터페이스 분리 원칙: 실제 사용하지 않는 메서드를 강제하는 큰 인터페이스는 만들지 않는다.
- 리스코프 치환 원칙: 상속 구조가 실제 대체 가능성을 갖지 않으면 조합 또는 데이터 기반 구조로 변경한다.

### 4.2 응집도와 결합도

- 같은 이유로 변경되는 코드는 한 곳에 둔다.
- 다른 이유로 변경되는 코드는 분리한다.
- 런타임 로직과 콘텐츠 데이터는 분리한다.
- UI는 게임 진행 로직을 직접 판단하지 않는다.
- Manager는 전역 흐름 조정만 담당하고 세부 판정은 전용 클래스에 위임한다.

### 4.3 디자인 패턴 적용 기준

디자인 패턴은 구조 문제를 해결할 때만 사용한다. 패턴 적용 자체를 목표로 하지 않는다.

사용 후보:

- State: Player, Enemy, Input처럼 상태 전이가 핵심인 시스템
- Strategy: Quest 조건, Dialogue 조건, Entrance 조건, Boss 패턴
- Command: Dialogue 선택 결과, Quest 보상, Cutscene 액션
- Repository: JSON 또는 ScriptableObject 데이터 로딩
- Observer/Event: UI 갱신, 퀘스트 진행 알림, 대화 시작/종료
- Factory: 데이터 기반 런타임 객체 생성
- Resolver: 조건 판정과 액션 실행 공통 처리

## 5. 데이터 관리 기준

### 5.1 JSON으로 분리할 데이터

JSON은 콘텐츠성 데이터에 사용한다.

- Dialogue 텍스트와 노드 연결
- Dialogue 선택지와 조건
- Quest 정의, 목표, 보상
- NPC 프로필과 상태별 대화 연결
- Entrance 조건과 이동 대상
- Tutorial Step
- Cutscene Trigger

### 5.2 ScriptableObject로 유지할 데이터

Unity Object 참조가 필요한 데이터는 ScriptableObject에 둔다.

- WeaponDefinition
- StatusEffectData
- EnemyStatusEffectData
- Sprite, AudioClip, Prefab 참조
- Animator 또는 VFX 참조가 필요한 설정
- 인스펙터 기반 튜닝이 중요한 수치

### 5.3 C# 코드에 남길 로직

- 상태 전이
- 조건 판정 구현
- 액션 실행 구현
- Unity API 호출
- 물리, 애니메이션, 사운드 재생
- 저장/불러오기 실행 로직

## 6. ID 네이밍 규칙

Save/Load와 JSON 데이터 연계를 위해 안정적인 ID를 사용한다. 런타임 중 변경될 수 있는 오브젝트 이름을 저장 키로 사용하지 않는다.

권장 형식:

- Quest: `quest_repair_pipe`
- NPC: `npc_mechanic`
- Dialogue: `dialogue_mechanic_intro`
- Dialogue Node: `node_start`, `node_after_accept`
- Entrance: `entrance_stage01_pipe_02`
- Scene: `scene_stage01`
- Spawn Point: `spawn_stage01_00`
- Flag: `flag_boss_intro_seen`
- Item 또는 Weapon: `weapon_honey_small`

ID 규칙:

- 소문자 snake_case 사용
- 기능 접두어를 붙인다.
- 씬, NPC, 퀘스트 등 소유 맥락을 포함한다.
- 저장 데이터에 들어가는 ID는 출시 후 변경하지 않는다는 기준으로 관리한다.

## 7. 이벤트 사용 규칙

- C# event 또는 Action: 코드 내부 시스템 간 알림에 사용한다.
- UnityEvent: 인스펙터에서 디자이너가 연결해야 하는 이벤트에만 사용한다.
- JSON Action: 데이터 기반 보상, 상태 변경, 플래그 변경에 사용한다.

예시:

- Dialogue 시작/종료 알림: C# event
- 특정 노드 진입 시 인스펙터 오브젝트 활성화: UnityEvent
- 퀘스트 완료 시 NPC 상태 변경: JSON Action

## 8. 코드와 주석 기준

### 8.1 기준 스크립트

역할별 기준 파일:

- 데이터 구조 기준: `Assets/Scripts/ScriptableObjects/KeyBindingData.cs`
- 입력 처리 기준: `Assets/Scripts/Input/GamePlayInputState.cs`
- 이벤트 핸들러 네이밍 기준: `Assets/Scripts/Manager/DialogueManager.cs`
- 작은 UI 컴포넌트 기준: `Assets/Scripts/UI/WeaponDescriptionUI.cs`

기준으로 삼지 않을 파일:

- `PlayerController.cs`: 핵심 기능은 많지만 책임이 많고 임시 로그가 남아 있다.
- `EnemyController.cs`: FSM 구조는 참고 가능하지만 탐지, 상태 전환, 이펙트가 섞여 있다.
- `WeaponBase.cs`: 리팩터링 대상이며 현재 책임이 많다.
- `GameManager.cs`: 전역 관리 구조는 참고 가능하지만 임시 필드와 하드코딩 흔적이 있다.

### 8.2 주석 규칙

- 클래스 주석은 작성한다.
- public 메서드는 외부 호출 의도가 불명확하면 작성한다.
- private 메서드는 이름으로 충분하면 주석을 생략한다.
- 복잡한 조건이나 흐름은 무엇을 하는지가 아니라 왜 필요한지를 설명한다.
- 모든 C# 파일은 UTF-8 인코딩으로 통일한다.
- 깨진 한글 주석은 정리한다.

### 8.3 코드 스타일

- private serialized field는 `[SerializeField] private` 형식을 사용한다.
- 외부 공개는 public field보다 property를 우선한다.
- 이벤트 핸들러는 `Handle...` 또는 `On...` 규칙을 일관되게 사용한다.
- bool 메서드는 `Is`, `Can`, `Has`, `Try` 접두어를 사용한다.
- 실패 가능성이 있는 실행 메서드는 `Try` 접두어를 사용한다.
- 하드코딩 문자열은 상수, ID 데이터, JSON, ScriptableObject 중 적절한 위치로 이동한다.

## 9. 시스템별 리팩터링 계획

### 9.1 Input

현재 상태:

- Gameplay, Pause, Dialogue, CutScene 입력 상태 분리가 거의 완료됐다.
- Dialogue 선택지 입력 이벤트도 추가된 상태다.

목표:

- 입력 상태별 책임을 명확히 유지한다.
- 입력 이벤트와 UI/게임 로직의 결합을 줄인다.
- 키 바인딩 데이터와 입력 설정 UI의 연결을 안정화한다.

작업:

- 불필요한 diff 정리
- KeyBindingData 파일 끝 공백 제거
- 입력 상태 전환 테스트
- Dialogue, Pause, CutScene 상태에서 Gameplay 입력 누수 확인

완료 기준:

- 상태별 입력이 의도한 상황에서만 동작한다.
- Dialogue 선택지는 키보드로 이동 및 확정 가능하다.
- Pause와 Dialogue 입력이 서로 충돌하지 않는다.

### 9.2 Save/Load 대비 ID 설계

목표:

- Easy Save Asset 적용 전 저장 대상과 ID 체계를 확정한다.

저장 후보:

- 현재 씬 ID
- 현재 스폰 포인트 ID
- 플레이어 위치
- 보유 무기 목록
- 현재 장착 무기
- 무기 내구도
- NPC 상태
- Quest 상태
- 완료된 이벤트 플래그
- 보스 클리어 상태
- 키 바인딩 설정

완료 기준:

- 저장 가능한 런타임 상태와 정적 데이터가 분리된다.
- 저장 키가 오브젝트 이름이나 씬 내 배치에 의존하지 않는다.

### 9.3 Quest

현재 상태:

- Quest 1차 리팩터링이 완료됐다.
- `QuestData`, `QuestProgress`, `QuestManager`가 추가되어 정적 데이터와 런타임 진행 상태가 분리되기 시작했다.
- `DonationBox`, `ItemRequirementChecker`, `ItemGiver`가 QuestData와 QuestManager 기반 흐름으로 연결됐다.
- 기존 NPC 대화 분기는 아직 NPCState와 UnityEvent를 유지한다.

목표:

- 퀘스트 정의, 진행 상태, 조건, 보상을 분리한다.
- Dialogue와 NPC 상태 변경을 Quest와 연동한다.

권장 구조:

- `QuestData`
- `QuestProgress`
- `QuestManager`
- `QuestConditionResolver`
- `QuestActionExecutor`

1차 완료 내용:

- `QuestData` ScriptableObject로 퀘스트별 `questId`, 설명, objective 정보를 관리한다.
- `QuestObjectiveData`로 `Counter`, `RequiredWeapon`, `Reward` 목적을 표현한다.
- `QuestProgress`가 퀘스트 상태, 누적 카운터, 완료 objective를 보관한다.
- `QuestManager`가 `questId` 기반 상태 변경과 objective 진행도 변경 API를 제공한다.
- `GameManager.donationScore` 의존을 제거하고 기부 진행도를 QuestProgress counter로 이동했다.
- 요구 무기와 보상 무기 정보를 QuestData로 이동했다.

후속 과제:

- `WeaponBase.name` 비교를 안정적인 `weaponId` 비교로 변경한다.
- `QuestProgress` 저장용 DTO를 분리해 Easy Save 적용에 대비한다.
- `QuestConditionResolver`, `QuestActionExecutor` 도입 여부를 Dialogue/NPC 리팩터링과 함께 결정한다.
- NPCState와 QuestState의 경계를 더 명확히 분리한다.

완료 기준:

- 새 퀘스트 추가 시 C# 수정 없이 ScriptableObject 데이터 추가 중심으로 처리 가능하다.
- 퀘스트 상태가 Dialogue, NPC, Entrance 조건에 사용할 수 있는 형태로 관리된다.
- JSON 전환은 Dialogue/NPC 데이터 구조가 안정된 뒤 검토한다.

### 9.4 Dialogue

현재 상태:

- DialogueManager가 대화 진행, 선택지, 입력 이벤트, UnityEvent 실행을 함께 처리한다.

목표:

- 대화 데이터와 실행 로직을 분리한다.
- 노드 진행, 선택지 조건, 선택 결과 실행을 명확히 나눈다.

권장 구조:

- `DialogueRepository`
- `DialogueRunner`
- `DialogueConditionResolver`
- `DialogueActionExecutor`
- `DialogueUI`

완료 기준:

- Dialogue 텍스트와 선택지는 JSON으로 관리 가능하다.
- 선택지 조건과 결과 액션이 공통 Condition/Action 구조를 사용한다.
- Dialogue UI는 표시와 하이라이트만 담당한다.

### 9.5 NPC

현재 상태:

- NPC별 목적 클래스가 늘어나면서 하드코딩이 증가했다.
- Quest, Dialogue, 상태 변화가 NPC 클래스에 섞여 있다.

목표:

- NPC를 대화 제공자, 상태 보유자, 조건 반응자로 분리한다.
- NPC별 특수 행동은 데이터 액션 또는 작은 컴포넌트로 분리한다.

권장 구조:

- `NpcProfileData`
- `NpcStateStore`
- `NpcDialogueResolver`
- `NpcConditionBinder`

완료 기준:

- NPC 상태별 대화가 데이터로 연결된다.
- NPC 표시 조건, 보상 지급, 대화 분기가 Quest와 연동된다.

### 9.6 Entrance

현재 상태:

- Auto, Interactable, Guarded, Pipe, NPC 입구가 상속으로 섞여 있다.
- 조건 검사와 씬 이동, 특수 연출이 한 구조에 모여 있다.

목표:

- 입구의 조건, 이동 대상, 이동 방식, 차단 대화를 분리한다.

권장 구조:

- `EntranceData`
- `EntranceController`
- `IEntranceCondition`
- `IEntranceTransition`

완료 기준:

- 입구 잠금 조건을 JSON 또는 데이터로 정의할 수 있다.
- 씬 이동과 스폰 포인트가 Save/Load ID 체계와 연결된다.

### 9.7 Weapon

현재 상태:

- 실사용 무기 타입은 적지만 상속과 스크립트가 늘어났다.
- WeaponBase가 획득, 장착, 투척, 충돌, 내구도, 파괴 효과를 많이 담당한다.

목표:

- 무기 데이터, 런타임 인스턴스, 동작 로직을 분리한다.
- 사용하지 않는 상속 구조와 스크립트를 줄인다.

권장 방향:

- `WeaponDefinition` 유지
- `WeaponInstance` 저장 데이터 강화
- `WeaponBase` 책임 축소
- `SmallWeapon`, `LargeWeapon` 차이가 없으면 제거 또는 데이터화
- `EffectableWeapon`은 실제 사용 여부에 따라 유지 또는 통합

완료 기준:

- 무기 추가 시 불필요한 하위 클래스를 만들지 않는다.
- 투척, 내구도, 효과 발동 흐름이 읽기 쉬운 단위로 분리된다.

추가 개선 예정:

- 무기 획득 시 `WeaponDescriptionUI`에 Damage와 Durability도 함께 표시한다.
- 표시 데이터는 `WeaponDefinition`과 `WeaponBase`의 역할을 분리해 UI가 런타임 구조에 과하게 의존하지 않도록 정리한다.

### 9.8 Enemy / Boss

현재 상태:

- 일반 Enemy FSM은 구조가 있다.
- Boss는 하나뿐인데 전용 하드코딩과 상속 구조가 섞여 있다.

목표:

- 일반 Enemy FSM은 유지한다.
- Boss는 하나뿐이라는 현실을 반영해 단순하고 명확한 전용 구조로 정리한다.

완료 기준:

- Boss 패턴, UI, BGM, 시작 연출의 책임이 분리된다.
- 불필요한 상속을 줄인다.
- 일반 Enemy와 Boss를 억지로 같은 추상화에 묶지 않는다.

### 9.9 Effect

현재 상태:

- Player, Enemy, Environment, WeaponEffect가 효과를 각자 처리한다.

목표:

- EffectData와 Apply/Remove 규칙을 통일한다.
- 독립 대공사보다 Weapon, Enemy 정리 중 필요한 부분을 함께 정리한다.

완료 기준:

- 효과 적용 대상과 효과 종류가 명확하다.
- 효과 해제 흐름이 예측 가능하다.

### 9.10 Animation

현재 상태:

- 정상 동작 중이지만 Animation Event와 파라미터 하드코딩 여부를 점검할 필요가 있다.

목표:

- 대규모 구조 변경 없이 안정성을 유지한다.
- Animator 파라미터명, Animation Event 메서드를 정리한다.

완료 기준:

- 파라미터명 문자열이 흩어져 있지 않다.
- Animation Event가 명확한 public 메서드에만 연결된다.
- 불필요한 중복 API가 줄어든다.

### 9.11 Sound

현재 상태:

- Animation Event 중심으로 SFX가 재생된다.
- AudioSource 사용 방식이 일관적인지 점검 필요하다.

목표:

- Animation 타이밍에 맞는 사운드는 Animation Event를 유지한다.
- BGM, SFX, UI 사운드 책임을 분리한다.
- AudioSource 사용 규칙을 정한다.

완료 기준:

- BGM과 SFX 재생 책임이 분리된다.
- 같은 사운드 재생 방식이 여러 스크립트에 중복되지 않는다.
- 필요 시 AudioSource 풀링 또는 중앙 SFX Player를 도입한다.

## 10. 작업 우선순위

1. Input 마무리
2. Save/Load 대비 ID 설계
3. Quest 정의 및 데이터 구조 설계
4. Dialogue와 NPC 통합 리팩터링
5. Entrance 구조 정리
6. Weapon 구조 축소
7. Boss 하드코딩 정리
8. Effect 규칙 정리
9. Sound 사용 방식 정리
10. Animation 안정화 정리
11. 제출 품질 정리

## 11. 제출 품질 체크리스트

- `Debug.Log` 제거 또는 `#if UNITY_EDITOR` 처리
- 깨진 한글 주석 수정
- C# 파일 UTF-8 인코딩 통일
- 불필요한 using 제거
- 사용하지 않는 스크립트 제거
- 하드코딩 문자열 정리
- `Enteracnes` 폴더명 수정 여부 결정
- README 또는 포트폴리오 설명 문서 작성
- 대표 리팩터링 전/후 비교 정리
- 핵심 시스템 다이어그램 작성

## 12. 포트폴리오 어필 포인트

대표 설명 문장:

> 기존 하드코딩 중심의 NPC, Dialogue, Quest 흐름을 데이터 기반 구조로 전환하고, 공통 Condition/Action Resolver를 통해 대화 분기, 퀘스트 진행, 입구 잠금, 저장/불러오기 상태를 일관된 방식으로 관리하도록 개선했다.

강조할 수 있는 항목:

- 상태 패턴 기반 플레이어/적 FSM
- 입력 상태 라우팅 시스템
- JSON 기반 Dialogue/Quest/NPC 데이터 구조
- 공통 Condition/Action Resolver
- Save/Load 대비 안정적인 ID 설계
- 무기 획득, 장착, 투척, 내구도, 효과 연동 구조
- 싱글턴 Manager 의존도 완화
- 코드/주석/인코딩/폴더 구조 정리

## 13. 최종 완료 기준

리팩터링은 다음 조건을 만족하면 완료로 본다.

- 기존 주요 플레이 흐름이 깨지지 않는다.
- Input, Quest, Dialogue, NPC, Entrance, Weapon, Boss의 책임이 명확해진다.
- 새 대화, 새 퀘스트, 새 NPC 상태 추가가 기존 C# 수정 없이 데이터 중심으로 가능하다.
- Easy Save 기반 Save/Load 추가를 위한 ID와 저장 대상이 정리되어 있다.
- 포트폴리오 문서에서 리팩터링 목표, 문제점, 개선 방식, 결과를 설명할 수 있다.

## 14. AI 디버깅

사용자의 요구, 허락 없이 스크립트를 업데이트 하지 않고, 코드 조각과 예시를 먼저 보여주고 허락을 받는다.

## 15. NPC / Dialogue 리팩터링 적용 결과

### 15.1 완료된 항목

- `NPCDialogueData`를 추가하여 NPCState별 DialogueNodeData 매핑을 ScriptableObject로 분리했다.
- `NpcDialogueResolver`를 추가하여 entry override와 상태별 대화 선택 책임을 분리했다.
- `NPCDialogue`는 대화 시작, 상태 변경 래퍼, 씬 이벤트 연결 책임만 유지하도록 정리했다.
- `DialogueManager`는 라인 진행, 옵션 선택, 노드 이벤트 호출 흐름을 안정화했다.
- `DialogueUI`는 본문 표시와 옵션 UI 생성 책임으로 정리했다.
- `MovementNPC`, `LoadCheckNPC`, `NPCVisibilityByState`, `TutorialNPC`의 null 방어와 책임 경계를 보강했다.
- `DialogueNodeData`는 property 기반 직렬화 구조로 변경했고 기존 SO 데이터 40개를 이관했다.
- `DialogueRouter`, `NPCDialoguePreset`, `NPCDialogueDefaultSet`은 미사용 잔재로 정리했다.

### 15.2 현재 유지한 설계 결정

- 전체 JSON 전환은 보류하고 ScriptableObject 기반 구조를 유지한다.
- Scene 오브젝트 참조가 필요한 이벤트는 `NodeEvent`의 UnityEvent로 유지한다.
- `entry`는 기본 대사가 아니라 1회성 시작 노드 override로 사용한다.
- `TutorialNPC`는 표준 NPCDialogueData 흐름의 예외로 두되, `ValidateDialogueData()` override로 처리한다.

### 15.3 추후 과제

- `DialogueUI.OnEnable/OnDisable`의 `Time.timeScale` 제어를 GameFlow 또는 PauseController 계층으로 이동한다.
- Scene 전환 직후 자동 대사는 LoadPanel 종료 또는 SceneTransition 상태를 기준으로 실행한다.
- Player / Entrance 리팩터링 시 스폰 위치 Ground 보정과 물리 초기화 타이밍을 정리한다.
- NodeEvent UnityEvent 구조를 장기적으로 Condition/Action Executor 구조로 대체할지 결정한다.
- DialogueNodeData의 ID 체계와 JSON 전환 여부는 Save/Load 설계와 함께 재검토한다.
## 16. NPC / Dialogue / Scene Transition 1차 안정화 완료 기록

### 16.1 적용 범위

- `NPCDialogueData` 기반 상태별 대화 매핑을 유지한다.
- `DialogueNodeData`는 대사 본문과 선택지 데이터를 담당한다.
- `NodeEvent`는 Scene 오브젝트 참조가 필요한 이벤트 연결만 담당한다.
- `DialogueManager`는 대화 진행, 입력 상태 전환, 대화 중 `Time.timeScale` 제어를 담당한다.
- `DialogueUI`는 텍스트/선택지 표시 책임만 담당한다.
- `LoadPanel` 기반 Scene 전환 흐름을 정리하고, 전환 중 입력을 잠근다.
- `Enterance` / `AutoEnterance`의 Scene 이동 성공 여부를 반환값으로 추적한다.

### 16.2 완료된 개선 사항

- `NPCDialogue`의 `entry`를 1회성 시작 노드 override로 명확히 분리했다.
- `NpcDialogueResolver`는 `entry`가 있으면 상태별 대화보다 우선 사용한다.
- `NPCDialogueData.ContainsNode()`를 추가해 `NodeEvent`가 현재 NPC 대화 데이터에 속한 노드인지 검증한다.
- `NodeEvent` 검증 항목을 추가했다.
  - 빈 Node
  - 중복 Node
  - DialogueData에 등록되지 않은 Node
  - Options 개수와 OptionEvents 개수 불일치
- `NPCDialogue`의 Animator `IsTalk` 파라미터는 존재 여부를 캐싱한 뒤 호출하도록 변경했다.
- `DialogueUI`의 `Time.timeScale` 직접 제어를 제거하고 `DialogueManager`로 책임을 이동했다.
- `DialogueManager.EndDialogue()`는 중복 종료를 방지하고, 씬 전환 중 입력 상태를 `Gameplay`으로 덮지 않도록 변경했다.
- `LoadCheckNPC`는 LoadPanel 전환이 끝난 뒤 강제 이벤트를 실행한다.
- `GameManager`는 Scene 전환 중 입력을 Lock/Unlock하고, LoadPanel FadeIn/Load/FadeOut 순서로 전환한다.
- `Enterance.EnterArea()`와 `GameManager.TryLoadScene()`은 성공 여부를 반환한다.
- `AutoEnterance`는 `OnTriggerStay2D`와 cooldown을 사용해 씬 진입 직후 트리거 내부에 있는 상황을 처리한다.
- `AutoEnterance`는 실제 씬 전환 요청에 성공했을 때만 중복 진입 방지 플래그를 설정한다.
- `DonationBox`는 무기 이름 비교 실패가 조용히 무시되지 않도록 방어 로그와 `WeaponDefinition` 기반 비교를 추가했다.

### 16.3 확인된 문제와 해결

- WhiteGuard 기부 이벤트가 실행되지 않던 문제는 코드 문제가 아니라 Stage10 `WhiteGuardNPC`의 `NodeEvent`가 `TurtleB_D_Inprogress`에 잘못 연결된 Inspector 오연결이었다.
- `NPCDialogueData.ContainsNode()` 검증으로 같은 유형의 오연결을 런타임 Warning으로 확인할 수 있게 했다.
- `AudioSource?.Play()` 호출로 일부 Enterance에서 씬 이동이 막히던 문제는 Unity Object null 처리와 AudioClip 누락 방어로 해결했다.
- 씬 이동 직후 AutoEnterance가 재동작하지 않던 문제는 `OnTriggerEnter2D` 단독 의존을 제거하고, 씬 전환 성공 여부를 기준으로 잠금 플래그를 관리해 해결했다.

### 16.4 현재 유지하는 설계 결정

- 전체 JSON 전환은 보류하고 ScriptableObject 기반 구조를 유지한다.
- Scene 오브젝트 참조가 필요한 이벤트는 당분간 `NodeEvent` + `UnityEvent`로 유지한다.
- `OnEnd`는 "노드의 모든 텍스트 출력이 끝난 직후" 호출되는 이벤트로 정의한다.
- `OnEnd`는 선택지가 있는 노드라면 선택지 표시 전에 호출된다.
- 대화 중 게임 정지는 당분간 `Time.timeScale = 0` 정책을 유지하되, 제어 책임은 `DialogueManager`가 가진다.
- `Enteracnes` 폴더명과 `Enterance` 클래스명은 Unity 참조 리스크 때문에 이번 단계에서는 유지한다.

### 16.5 남은 과제

- `NodeEvent` / `UnityEvent` 기반 이벤트 연결을 장기적으로 Condition/Action Executor 구조로 대체할지 검토한다.
- Dialogue 전체 JSON 전환 여부는 Save/Load ID 설계와 함께 다시 결정한다.
- Player / Entrance 리팩터링 시 SpawnPoint와 AutoEnterance Collider 배치 규칙을 문서화한다.
- Animation 리팩터링 시 Animator 파라미터 문자열을 `AnimatorParams` 같은 전용 static class로 상수화/Hash화할지 검토한다.
- 깨진 한글 주석/로그 인코딩은 별도 정리 작업으로 처리한다.

## 17. Weapon 1차 리팩터링 완료 기록

### 17.1 적용 범위

- 무기 정적 데이터는 `WeaponDefinition`으로 관리한다.
- 인벤토리 저장 대상은 `WeaponInstance`로 관리한다.
- `WeaponInstance.Id`는 개별 무기 인스턴스 식별자, `WeaponInstance.WeaponId`는 무기 종류 식별자로 사용한다.
- `WeaponData`는 `WeaponId` 기반 프리팹 조회와 획득 무기 ID 기록을 담당한다.
- `PlayerManager`는 보유 무기 목록, 현재 장착 ID, 무기 획득/장착/제거 흐름을 담당한다.
- `WeaponBase`는 월드 상호작용, 장착 위치 유지, 투척, 충돌, 내구도 감소, 파괴 효과 호출을 담당한다.

### 17.2 완료된 개선 사항

- `WeaponDefinition`에 안정적인 `WeaponId`를 추가했다.
- `WeaponInstance`를 Save/Load 대비 런타임 저장 데이터로 분리했다.
- 보유 무기 목록과 현재 장착 무기는 오브젝트 이름이 아니라 ID 기반으로 추적한다.
- `WeaponData.TryGetWeaponPrefab()`을 통해 `WeaponId`로 무기 프리팹을 조회한다.
- `InventoryUI`와 `InventoryIcon`은 무기 오브젝트 직접 참조 대신 인스턴스 ID 기반 선택 흐름을 사용한다.
- `SmallWeapon`, `LargeWeapon`, 테스트용 `Sword` 잔재를 제거했다.
- 삭제된 `SmallWeapon` / `LargeWeapon` 스크립트를 참조하던 무기 Prefab을 `WeaponBase`로 복구했다.
- `WeaponBase`의 `OnThrow()` 흐름을 분리해 부모 해제, 투척 속도 적용, 충돌 전환, 인벤토리 제거 책임을 읽기 쉬운 단위로 정리했다.
- 인벤토리 4칸이 가득 찬 상태에서는 `WeaponBase.Interactive()`가 무기 상태를 변경하지 않고 즉시 종료한다.
- `Physics2D.IgnoreLayerCollision(6, 8, ...)` 하드코딩을 `GameLayers` 기반 참조로 변경했다.
- `Enemy`, `Ground`, `Wall` Tag 문자열을 `GameTags`로 이동해 공용 상수 기준을 만들었다.
- `Map_Boundary` 계열 오브젝트와 자식 오브젝트에 `Wall` Tag를 적용해 투척 무기가 경계 밖으로 빠지는 문제를 완화했다.

### 17.3 확인된 문제와 해결

- 무기 Prefab이 삭제된 `SmallWeapon` / `LargeWeapon` 스크립트를 참조해 Interactive UI와 줍기가 동작하지 않던 문제는 Prefab 스크립트 참조를 `WeaponBase`로 복구해 해결했다.
- 인벤토리가 꽉 찬 상태에서 무기 획득 시 월드 무기가 원점으로 이동하고 상호작용 불가 상태가 되던 문제는 빈 슬롯 선검사로 해결했다.
- `Map_Boundary`에 `Ground` Layer를 넣으면 플레이어 착지 판정에 영향을 줄 수 있어, Layer 변경 대신 `Wall` Tag만 적용하는 방향으로 정리했다.

### 17.4 현재 유지하는 설계 결정

- 무기별 하위 클래스는 현재 단계에서 만들지 않는다.
- `EffectableWeapon`은 실제 파괴 효과 연동이 있어 유지한다.
- 무기 장착 오브젝트는 사용 빈도가 낮으므로 Object Pooling을 적용하지 않고 `Instantiate` / `Destroy` 흐름을 유지한다.
- `GameTags`, `GameLayers`는 우선 Weapon에서만 적용하고, 전체 시스템 치환은 후속 리팩터링 때 진행한다.
- `Ground` Layer는 플레이어 착지 판정에 사용되므로 Map Boundary에는 적용하지 않는다.

### 17.5 테스트 완료 항목

- 월드 무기 근처에서 Interactive UI가 표시된다.
- 무기 획득 시 인벤토리에 등록되고 장착된다.
- 인벤토리 슬롯 선택과 선택 UI 갱신이 정상 동작한다.
- 장착 무기 투척 후 인벤토리에서 제거된다.
- 던진 무기를 다시 주울 수 있다.
- 인벤토리 4칸이 가득 찬 상태에서 추가 획득이 막힌다.
- Map Boundary에 닿은 투척 무기가 `Wall` 충돌로 처리된다.

### 17.6 남은 과제

- `WeaponDescriptionUI`에 Damage와 Durability 표시를 추가한다.
- `IWeaponable` 인터페이스가 현재 구조에서 실질적인 가치가 있는지 재검토한다.
- `PlayerManager.UpdateWeapon()`의 미사용 파라미터와 호출 여부를 정리한다.
- `GameTags`, `GameLayers`를 Enemy, Entrance, Player 감지 코드에 단계적으로 적용한다.
- Weapon 파괴, 내구도, Effect 연동 책임을 더 분리할지 Effect 리팩터링 때 재검토한다.

## 18. Effect 1차 정리 메모

- `EffectBase`는 범위형 상태 이상 효과의 적용, 유지, 해제를 담당한다.
- `StatusEffectData`는 정적 설정값으로 유지하고, 런타임 값은 복제본에서만 변경한다.
- `EffectableWeapon`은 무기 파괴 시 효과 프리팹 생성만 담당하고, 효과별 초기화는 `IWeaponEffect`로 분리한다.
- Boss는 의도적으로 `IStatusEffectHandler`를 구현하지 않으므로 Honey Slow, Smoke Blind 같은 상태 이상 효과를 받지 않는다.
- `EnemyStatusEffectData`, `EffectableEnvironment`, `EffectableWall`은 현재 사용처가 없어 삭제 후보로 정리했다.
- `PlayerWallClingState` 종료 시 현재는 애니메이션 전환 부자연스러움 때문에 `IdleState`로 이동한다. Animation 리팩터링 시 `WallCling -> Fall` 전환 조건과 애니메이션을 재검토한다.
