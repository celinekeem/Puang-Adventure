# InitialScene 설정 가이드

## 개요
InitialScene은 DontDestroyOnLoad 오브젝트들을 초기화하고 첫 게임 씬으로 자동 전환하는 시작 씬입니다.

---

## 1단계: InitialScene 생성

### Unity Editor에서:
1. **File → New Scene** (또는 Ctrl+N)
2. Scene 이름을 `InitialScene`으로 저장 (`Assets/Scenes/InitialScene.unity`)

---

## 2단계: InitialScene 구성

### InitialScene에 포함할 오브젝트:
InitialScene은 **오직 DontDestroyOnLoad로 유지될 오브젝트만** 포함합니다:

```
InitialScene
├─ Player (PlayerPersistent 포함)
├─ GameManager
├─ Inventory
├─ InventoryManager
├─ Canvas_UI (InventoryToggle 포함)
└─ SceneInitializer (새로 생성)
```

### 설정 방법:
1. **TutorialScene을 연다**
2. 다음 오브젝트들을 복사:
   - Player
   - GameManager
   - Inventory
   - InventoryManager
   - Canvas_UI (전체)

3. **InitialScene을 연다**
4. 복사한 오브젝트들을 붙여넣기

5. **빈 GameObject 생성**:
   - Hierarchy → 우클릭 → Create Empty
   - 이름: `SceneInitializer`
   - Add Component → `SceneInitializer` 스크립트 추가
   - Inspector에서 설정:
     - **First Scene Name**: `TutorialScene`
     - **Auto Load First Scene**: ✓ (체크)
     - **Show Debug Logs**: ✓ (체크)

---

## 3단계: 게임 씬 정리 (TutorialScene, CaveScene)

### TutorialScene에서 삭제:
다음 오브젝트들을 **삭제**:
- ❌ Player
- ❌ GameManager
- ❌ Inventory
- ❌ InventoryManager

### TutorialScene에 남길 것:
- ✅ Map (Tilemap, Ground 등)
- ✅ Canvas_UI (DialoguePanel, InventoryPanel 등)
- ✅ Portal_ToCave
- ✅ PlayerSpawn (Tag: "PlayerSpawn")
- ✅ Main Camera
- ✅ CM vcam1 (Cinemachine Virtual Camera)

### CaveScene에서 삭제:
TutorialScene과 동일하게 정리:
- ❌ Player, GameManager, Inventory, InventoryManager 삭제
- ✅ Map, Canvas_UI, Portal, PlayerSpawn, Camera 유지

---

## 4단계: Build Settings 설정

### Build Settings 순서 (중요!):
1. **File → Build Settings** 열기
2. Scene 순서를 다음과 같이 설정:

```
✅ 0: InitialScene        (첫 번째 - 게임 시작 씬)
✅ 1: TutorialScene       (두 번째)
✅ 2: CaveScene           (세 번째)
```

### 순서 변경 방법:
- Scene을 드래그하여 순서 변경
- 또는 **Tools → Build Settings Helper** 사용

---

## 5단계: 테스트

### 게임 실행:
1. **InitialScene을 연다**
2. ▶ Play 버튼 클릭

### 예상 동작:
```
🚀 SceneInitializer: Initialization started in 'InitialScene'
✅ PlayerPersistent: Player 'Player' persistence enabled - moved to DontDestroyOnLoad
✅ GameManager: Initialized and persisting across scenes
✅ Inventory: Initialized and persisting across scenes
✅ InventoryManager: Initialized and persisting across scenes
🎬 SceneInitializer: Loading first scene 'TutorialScene'

🔄 PlayerPersistent: Scene 'TutorialScene' loaded
📍 PlayerPersistent: Moved to spawn point at (...)
📷 PlayerPersistent: Successfully connected 1 Cinemachine camera(s)
```

### Portal 테스트:
1. TutorialScene에서 Portal_ToCave로 이동
2. **중복 경고가 없어야 함!** ✅
3. 아이템을 주워서 Inventory 확인
4. CaveScene → TutorialScene 왔다갔다 해도 아이템 유지 ✅

---

## 결과

### ✅ 장점:
- 중복 오브젝트 경고 없음
- 씬 관리가 깔끔함 (각 씬은 맵과 UI만)
- 새 씬 추가 시 Player 등을 복사할 필요 없음
- DontDestroyOnLoad 오브젝트가 한 곳에서 관리됨

### ⚠ 주의사항:
- **반드시 InitialScene에서 게임을 시작**해야 함
- Build Settings에서 InitialScene이 index 0이어야 함
- 다른 씬에서 시작하면 Player 등이 없어서 에러 발생 가능

---

## 트러블슈팅

### Q: TutorialScene에서 직접 시작하면 Player가 없어요!
**A:** InitialScene에서 시작해야 합니다. 또는 디버그용으로 TutorialScene에 Player를 하나 더 두고, PlayerPersistent가 자동으로 중복 제거하도록 할 수 있습니다.

### Q: Portal로 이동했는데 Camera가 Player를 안 따라가요!
**A:** PlayerPersistent.cs가 Cinemachine을 자동으로 연결합니다. 콘솔 로그를 확인하세요:
- `🔍 Searching for Cinemachine cameras...`
- `✅ Connected 'CM vcam1' to follow Player`

로그가 안 나오면 Cinemachine Virtual Camera가 씬에 있는지 확인하세요.

### Q: Build Settings Helper가 안 보여요!
**A:** Unity Editor 상단 메뉴에서 **Tools → Build Settings Helper** 클릭

---

## 완료!

이제 깔끔한 Scene 구조로 게임을 개발할 수 있습니다! 🎉
