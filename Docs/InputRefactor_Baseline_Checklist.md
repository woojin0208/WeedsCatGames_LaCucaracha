# Input 리팩터링 기준선 체크리스트

## 목적
- Input 리팩터링 전 현재 동작을 고정한다.
- 이후 단계에서 동작 변경/회귀 여부를 동일 기준으로 비교한다.

## 실행 환경 기록
- Unity 버전:
- 실행 씬:
- 테스트 일시:
- 테스트 담당:

## 핵심 동작 기준선
- [ ] 플레이어 이동/점프/대시/공격/상호작용/던지기
- [ ] 무기 슬롯 변경(숫자키)
- [ ] Pause 열기/닫기
- [ ] Input 설정 UI 열기/닫기 및 키 변경 반영
- [ ] Dialogue 진행(다음 문장/선택지 이동/선택 확정)
- [ ] CutScene 재생/종료(현재 스킵 동작 포함)
- [ ] 무기 설명 UI 열기/닫기
- [ ] 미니맵 열기/닫기

## 회귀 비교용 테스트 케이스(수동)
1. `Gameplay` 상태에서 기본 입력 동작이 모두 동작한다.
2. Pause 진입 후 Resume 시 Gameplay 입력으로 정상 복귀한다.
3. Dialogue 진입 중에는 대화 입력만 작동하고 종료 시 Gameplay로 복귀한다.
4. CutScene 진입/종료 후 입력 상태가 정상 복귀한다.
5. 키 변경 후 즉시 Gameplay 입력에 반영된다.

## 현재 직접 입력 검사 지점(리팩터링 대상)
- `Assets/Scripts/Manager/UIManager.cs`
- `Assets/Scripts/Manager/DialogueManager.cs`
- `Assets/Scripts/UI/WeaponDescriptionUI.cs`
- `Assets/Scripts/NPC/LoadCheckNPC.cs`
- `Assets/Scripts/Settings/InputSetting.cs`
- `Assets/Scripts/Input/GamePlayInputState.cs` (Mouse API 혼용 포함)

## 직렬화 잔재 점검 대상
- `Assets/Prefabs/Player.prefab`
- `Assets/Prefabs/Managers/GameManager.prefab`
- `Assets/Prefabs/Managers/UIManager.prefab`
- `Assets/Scenes/Tutorial.unity`

## Step 1 완료 조건
- [ ] 본 문서 작성 완료
- [ ] 테스트 담당자가 실행 환경/기준선 결과를 기록 완료
- [ ] Step 2 진행 승인 완료
