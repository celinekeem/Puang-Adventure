# 🎮 UI 리팩터링 마이그레이션 가이드

## 📋 개요

기존 `Canvas_UI` 구조를 **완전히 폐기**하고, 새로운 `HUD_Canvas` + `UI_MasterPanel` 구조로 전환했습니다.

이 가이드는 Unity Inspector에서 필요한 모든 연결 작업과 기존 참조 제거 방법을 단계별로 설명합니다.

---

## 🏗 새로운 UI 구조

### 1. HUD_Canvas (항상 표시되는 HUD)
- **Sort Order**: 0
- **항상 활성화**: `SetActive(true)`
- **DontDestroyOnLoad 설정 필요**: ❌ 각 씬마다 존재

**구성 요소**:
```
HUD_Canvas
├── HPBar (Slider)
├── STBar (Slider)
├── DialoguePanel (GameObject)
└── Hotbar (Transform with Slot children)
```

### 2. UI_MasterPanel (Tab 기반 메뉴 UI)
- **Sort Order**: 1
- **시작 시 비활성화**: `SetActive(false)`
- **Tab/ESC 키로 토글**

**구성 요소**:
```
UI_MasterPanel
├── Background (Image)
├── TopButtonsPanel (Panel)
│   ├── Button_Map (Button)
│   ├── Button_Inventory (Button)
│   ├── Button_Settings (Button)
│   └── Button_Save (Button)
├── MapPanel (GameObject)
├── InventoryPanel (GameObject)
│   └── SlotGrid (Transform with Slot children)
├── SettingsPanel (GameObject)
│   ├── Slider_Brightness (Slider)
│   ├── Slider_BGM (Slider)
│   └── Slider_SFX (Slider)
└── SavePanel (GameObject)
    ├── Button_Save (Button)
    └── Button_Load (Button)
```

---

## ✅ Inspector 연결 체크리스트

### Step 1: HUD_Canvas 씬에 배치
1. **HUD_Canvas Prefab**을 각 씬(TutorialScene, CaveScene 등)에 드래그
2. Canvas 설정 확인:
   - Render Mode: `Screen Space - Overlay`
   - Sort Order: `0`
3. **EventSystem**이 씬에 1개만 존재하는지 확인 (중복 시 삭제)

### Step 2: UI_MasterPanel 씬에 배치
1. **UI_MasterPanel Prefab**을 각 씬에 드래그
2. Canvas 설정 확인:
   - Render Mode: `Screen Space - Overlay`
   - Sort Order: `1`
3. Inspector에서 `SetActive(false)` 체크 (시작 시 꺼져있어야 함)

### Step 3: UIReferenceManager 씬에 추가
1. 빈 GameObject 생성 → 이름: `UIReferenceManager`
2. `UIReferenceManager.cs` 스크립트 추가
3. **Inspector에서 연결**:

#### HUD Canvas References
- `hudCanvas` → **HUD_Canvas** 드래그
- `hpSlider` → **HUD_Canvas/HPBar** 드래그
- `staminaSlider` → **HUD_Canvas/STBar** 드래그
- `hotbarParent` → **HUD_Canvas/Hotbar** 드래그
- `dialoguePanel` → **HUD_Canvas/DialoguePanel** 드래그 (선택)

#### Master Panel References
- `masterPanel` → **UI_MasterPanel** 드래그
- `inventoryPanel` → **UI_MasterPanel/InventoryPanel** 드래그
- `inventorySlotGrid` → **UI_MasterPanel/InventoryPanel/SlotGrid** 드래그
- `mapPanel` → **UI_MasterPanel/MapPanel** 드래그
- `settingsPanel` → **UI_MasterPanel/SettingsPanel** 드래그
- `savePanel` → **UI_MasterPanel/SavePanel** 드래그

#### Top Buttons
- `buttonMap` → **UI_MasterPanel/TopButtonsPanel/Button_Map** 드래그
- `buttonInventory` → **UI_MasterPanel/TopButtonsPanel/Button_Inventory** 드래그
- `buttonSettings` → **UI_MasterPanel/TopButtonsPanel/Button_Settings** 드래그
- `buttonSave` → **UI_MasterPanel/TopButtonsPanel/Button_Save** 드래그

#### Settings Panel UI
- `sliderBrightness` → **UI_MasterPanel/SettingsPanel/Slider_Brightness** 드래그
- `sliderBGM` → **UI_MasterPanel/SettingsPanel/Slider_BGM** 드래그
- `sliderSFX` → **UI_MasterPanel/SettingsPanel/Slider_SFX** 드래그

#### Save Panel UI
- `buttonSaveGame` → **UI_MasterPanel/SavePanel/Button_Save** 드래그
- `buttonLoadGame` → **UI_MasterPanel/SavePanel/Button_Load** 드래그

### Step 4: UI_MasterController 씬에 추가
1. 빈 GameObject 생성 → 이름: `UI_MasterController`
2. `UI_MasterController.cs` 스크립트 추가
3. **Inspector에서 연결**:
   - `masterPanel` → **UI_MasterPanel** 드래그
   - `mapPanel` → **UI_MasterPanel/MapPanel** 드래그
   - `inventoryPanel` → **UI_MasterPanel/InventoryPanel** 드래그
   - `settingsPanel` → **UI_MasterPanel/SettingsPanel** 드래그
   - `savePanel` → **UI_MasterPanel/SavePanel** 드래그
   - `hudCanvas` → **HUD_Canvas** 드래그
   - `buttonMap` → **UI_MasterPanel/TopButtonsPanel/Button_Map** 드래그
   - `buttonInventory` → **UI_MasterPanel/TopButtonsPanel/Button_Inventory** 드래그
   - `buttonSettings` → **UI_MasterPanel/TopButtonsPanel/Button_Settings** 드래그
   - `buttonSave` → **UI_MasterPanel/TopButtonsPanel/Button_Save** 드래그

4. **Settings 설정**:
   - `useUIReferenceManager`: ✅ 체크
   - `defaultPanel`: `Inventory` 선택
   - `showDebugLogs`: ✅ 체크 (테스트 시)

### Step 5: SavePanelController 추가
1. **UI_MasterPanel/SavePanel**에 `SavePanelController.cs` 추가
2. **Inspector에서 연결**:
   - `buttonSave` → **SavePanel/Button_Save** 드래그
   - `buttonLoad` → **SavePanel/Button_Load** 드래그
   - `statusText` → **SavePanel/StatusText** 드래그 (선택)
   - `saveFileName`: `"save_slot_1"` (기본값)

### Step 6: SettingsPanelController 추가
1. **UI_MasterPanel/SettingsPanel**에 `SettingsPanelController.cs` 추가
2. **Inspector에서 연결**:
   - `sliderBrightness` → **SettingsPanel/Slider_Brightness** 드래그
   - `sliderBGM` → **SettingsPanel/Slider_BGM** 드래그
   - `sliderSFX` → **SettingsPanel/Slider_SFX** 드래그
   - `audioMixer` → **AudioMixer Asset** 드래그 (선택)
   - `lightReference` → **Main Light** 드래그 (선택)

### Step 7: Player GameObject 확인
1. **Player GameObject** 선택
2. `PlayerHealth.cs` 확인:
   - `hpSlider` 필드가 비어있어도 OK (자동으로 UIReferenceManager에서 찾음)
3. `PlayerStamina.cs` 확인:
   - `staminaSlider` 필드가 비어있어도 OK (자동으로 찾음)

### Step 8: Inventory 관련 설정
1. **InventoryUI.cs**:
   - `slotsParent` → **UI_MasterPanel/InventoryPanel/SlotGrid** 드래그
2. **Hotbar.cs**:
   - `slotsParent` → **HUD_Canvas/Hotbar** 드래그

---

## 🗑 기존 Canvas_UI 참조 제거

### 제거해야 할 GameObject들
❌ **씬에서 삭제**:
- `Canvas_UI` (모든 씬에서)
- `InventoryToggle` GameObject (더 이상 사용하지 않음)
- `CloseInventory` 스크립트가 붙은 X 버튼

### 제거/비활성화해야 할 스크립트들
❌ **더 이상 사용하지 않음**:
- `CloseInventory.cs` (완전 삭제 가능)
- `InventoryToggle.cs` (더 이상 사용하지 않음, UI_MasterController가 대체)

### 수정된 스크립트 요약
✅ **업데이트 완료**:
1. `PlayerHealth.cs` → `HUD_Canvas/HPBar` 사용
2. `PlayerStamina.cs` → `HUD_Canvas/STBar` 사용
3. `Inventory.cs` → UIReferenceManager 사용 가능
4. `InventoryUI.cs` → `UI_MasterPanel/InventoryPanel/SlotGrid` 사용
5. `Hotbar.cs` → `HUD_Canvas/Hotbar` 사용

---

## 🎹 Input System 설정

### UIControls.inputactions 파일 확인
다음 액션들이 필요합니다:

1. **ToggleMasterUI**:
   - Key: `Tab`
   - Action Type: `Button`

2. **CloseMasterUI**:
   - Key: `Escape`
   - Action Type: `Button`

3. **InventoryToggle**:
   - Key: `I`
   - Action Type: `Button`

### UIControls.inputactions 수정 방법
1. `UIControls.inputactions` 파일을 Unity에서 더블클릭
2. `UI` Action Map에 다음 액션 추가:
   - `ToggleMasterUI` → Tab 키
   - `CloseMasterUI` → Escape 키
   - `InventoryToggle` → I 키 (이미 존재하면 그대로 사용)
3. **저장** 후 **Generate C# Class** 클릭

---

## 🧪 테스트 체크리스트

### 필수 테스트 항목
- [ ] **Tab 키**: Master UI가 열리고 닫히는가?
- [ ] **ESC 키**: Master UI가 열려있을 때만 닫히는가?
- [ ] **I 키**: Inventory 탭으로 바로 이동하는가?
- [ ] **Time.timeScale**: UI 열릴 때 0, 닫힐 때 1인가?
- [ ] **HPBar**: 데미지 받을 때 UI 업데이트되는가?
- [ ] **STBar**: 달리기/대시 시 UI 업데이트되는가?
- [ ] **Top Buttons**: Map, Inventory, Settings, Save 버튼이 작동하는가?
- [ ] **Hotbar ↔ Inventory**: 드래그가 정상 작동하는가?
- [ ] **Save/Load**: 저장 및 불러오기가 작동하는가?
- [ ] **Settings**: Brightness, BGM, SFX 슬라이더가 작동하는가?

### 씬 전환 테스트
- [ ] **InitialScene → TutorialScene**: UI가 정상적으로 표시되는가?
- [ ] **TutorialScene → CaveScene**: UI가 유지되는가?
- [ ] **포탈 이동**: Master UI가 자동으로 닫히는가?

---

## 🚨 자주 발생하는 문제 해결

### 문제 1: "NullReferenceException: UIReferenceManager.Instance is null"
**원인**: UIReferenceManager가 씬에 없거나 Awake() 실행 전에 접근함
**해결**:
1. UIReferenceManager GameObject가 씬에 존재하는지 확인
2. Script Execution Order에서 UIReferenceManager를 가장 먼저 실행하도록 설정

### 문제 2: "HPBar/STBar UI가 업데이트되지 않음"
**원인**: UIReferenceManager의 참조가 제대로 연결되지 않음
**해결**:
1. UIReferenceManager Inspector에서 `hpSlider`와 `staminaSlider`가 연결되었는지 확인
2. 또는 `autoFindReferences`를 ✅ 체크

### 문제 3: "Tab 키가 작동하지 않음"
**원인**: UIControls.inputactions에 액션이 없거나 C# 클래스가 생성되지 않음
**해결**:
1. UIControls.inputactions 열기
2. `ToggleMasterUI` 액션이 Tab 키에 바인딩되었는지 확인
3. **Generate C# Class** 클릭
4. Unity 재시작

### 문제 4: "Hotbar와 Inventory 간 드래그가 작동하지 않음"
**원인**: 두 Canvas의 Sort Order나 Event System 문제
**해결**:
1. HUD_Canvas Sort Order: `0`, UI_MasterPanel Sort Order: `1` 확인
2. 씬에 EventSystem이 **1개만** 존재하는지 확인
3. 두 Canvas 모두 `GraphicRaycaster` 컴포넌트가 활성화되어 있는지 확인

### 문제 5: "TimeScale이 1로 돌아오지 않음"
**원인**: Master UI가 닫히지 않았거나 다른 스크립트에서 TimeScale 조작
**해결**:
1. Master UI가 완전히 `SetActive(false)`로 닫혔는지 확인
2. 다른 스크립트에서 `Time.timeScale`을 변경하는 코드가 있는지 검색

---

## 📊 변경 사항 요약

| 항목 | 기존 (Canvas_UI) | 신규 (HUD + Master) |
|------|------------------|---------------------|
| HP/Stamina | Canvas_UI/HPBar, STBar | HUD_Canvas/HPBar, STBar |
| Inventory UI | Canvas_UI/InventoryPanel | UI_MasterPanel/InventoryPanel |
| Hotbar | Canvas_UI/Hotbar | HUD_Canvas/Hotbar |
| 인벤토리 열기 | I 키 → InventoryToggle.cs | I 키 → UI_MasterController |
| 인벤토리 닫기 | X 버튼 | Tab/ESC 키 |
| 설정/저장 | 없음 | Settings/Save 패널 추가 |
| TimeScale 제어 | InventoryToggle.cs | UI_MasterController.cs |

---

## 🎯 최종 확인 사항

완료 후 다음을 확인하세요:

1. ✅ **Canvas_UI가 씬에서 완전히 제거**되었는가?
2. ✅ **CloseInventory.cs가 프로젝트에서 삭제**되었는가?
3. ✅ **InventoryToggle GameObject가 씬에서 제거**되었는가?
4. ✅ **UIReferenceManager가 모든 씬에 존재**하는가?
5. ✅ **UI_MasterController가 모든 씬에 존재**하는가?
6. ✅ **SavePanelController와 SettingsPanelController가 제대로 연결**되었는가?
7. ✅ **Input Actions (Tab, ESC, I)가 모두 작동**하는가?

---

## 📞 추가 도움말

문제가 계속 발생하면:
1. Unity Console에서 **⚠ 경고 로그** 확인
2. UIReferenceManager에서 **Context Menu → Debug: Validate All References** 실행
3. UI_MasterController에서 **Context Menu → Debug: Toggle Master UI** 실행

---

**작성일**: 2025-11-11
**버전**: 1.0
**작성자**: Claude Code
