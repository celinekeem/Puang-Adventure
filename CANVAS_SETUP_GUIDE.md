# 🎨 Canvas 설정 가이드 - 드래그 앤 드롭 시스템

## 개요

드래그 앤 드롭 인벤토리 시스템이 정상 작동하려면 **HUD_Canvas**와 **UI_MasterPanel** Canvas가 올바르게 설정되어야 합니다.

이 가이드는 `TimeScale = 0` 상태에서도 작동하는 드래그 시스템을 위한 Canvas 설정 방법을 설명합니다.

---

## 필수 Canvas 설정

### 1. HUD_Canvas 설정

**경로**: `TutorialScene/HUD_Canvas`

#### Canvas 컴포넌트 설정
- **Render Mode**: `Screen Space - Overlay`
- **Pixel Perfect**: ☐ (선택)
- **Sort Order**: `0`
- **Target Display**: `Display 1`
- **Additional Shader Channels**:
  - ✅ Nothing (기본값)

#### Canvas Scaler 설정
- **UI Scale Mode**: `Scale With Screen Size`
- **Reference Resolution**: `1920 x 1080` (프로젝트 해상도에 맞게 조정)
- **Screen Match Mode**: `Match Width Or Height`
- **Match**: `0.5` (중간값)

#### Graphic Raycaster 설정 ⚠ 중요!
- **Ignore Reversed Graphics**: ✅ 체크
- **Blocking Objects**: `None`
- **Blocking Mask**: `Everything`

**중요**: Graphic Raycaster가 활성화되어 있어야 드래그 앤 드롭이 작동합니다!

---

### 2. UI_MasterPanel 설정

**경로**: `TutorialScene/UI_MasterPanel`

#### Canvas 컴포넌트 설정
- **Render Mode**: `Screen Space - Overlay`
- **Pixel Perfect**: ☐ (선택)
- **Sort Order**: `1` ⚠ HUD_Canvas보다 높아야 함!
- **Target Display**: `Display 1`
- **Additional Shader Channels**:
  - ✅ Nothing (기본값)

#### Canvas Scaler 설정
- **UI Scale Mode**: `Scale With Screen Size`
- **Reference Resolution**: `1920 x 1080` (HUD_Canvas와 동일하게)
- **Screen Match Mode**: `Match Width Or Height`
- **Match**: `0.5` (중간값)

#### Graphic Raycaster 설정 ⚠ 중요!
- **Ignore Reversed Graphics**: ✅ 체크
- **Blocking Objects**: `None`
- **Blocking Mask**: `Everything`

**중요**: Graphic Raycaster가 활성화되어 있어야 드래그 앤 드롭이 작동합니다!

---

## TimeScale = 0 에서 작동하는 설정

### ⚠ 핵심: Canvas Update Mode

`Time.timeScale = 0`일 때도 UI가 작동하려면 **별도 설정이 필요하지 않습니다**.

Unity의 EventSystem과 GraphicRaycaster는 **Unscaled Time**을 사용하므로, TimeScale이 0이어도 다음이 정상 작동합니다:
- 마우스 클릭
- 드래그 앤 드롭
- UI 버튼
- 슬라이더

### ✅ 이미 지원되는 기능들

- `IBeginDragHandler`, `IDragHandler`, `IEndDragHandler` → **Unscaled Time 사용**
- `RectTransformUtility.ScreenPointToLocalPointInRectangle` → **TimeScale 무관**
- `PointerEventData` → **Unscaled Time 사용**

따라서 **추가 설정 없이** `Time.timeScale = 0`에서도 드래그가 정상 작동합니다.

---

## EventSystem 설정

### EventSystem 확인

각 씬에는 **단 1개의 EventSystem**만 존재해야 합니다.

#### EventSystem 설정
- **First Selected**: `None` (기본값)
- **Send Navigation Events**: ✅ 체크 (또는 ☐ 체크 해제 - Tab 키와 충돌 방지용)
- **Drag Threshold**: `10` (기본값)

#### Standalone Input Module 설정
- **Horizontal Axis**: `Horizontal`
- **Vertical Axis**: `Vertical`
- **Submit Button**: `Submit`
- **Cancel Button**: `Cancel`
- **Input Actions Per Second**: `10`
- **Repeat Delay**: `0.5`

---

## 슬롯(Slot) GameObject 설정

### Hotbar 슬롯 (HUD_Canvas 하위)

**경로**: `HUD_Canvas/Hotbar/Slot_0, Slot_1, ...`

#### 필수 컴포넌트
1. **RectTransform** (자동 추가됨)
2. **Image** (슬롯 배경)
   - **Raycast Target**: ✅ 체크 (중요!)
3. **ItemSlot (Script)**
   - `icon`: `Icon` 자식 GameObject의 Image 컴포넌트
   - `slotType`: `Hotbar`
   - `index`: 자동 설정됨 (0, 1, 2, ...)
   - `showDebugLogs`: ✅ 체크 (테스트 시)

#### 자식 GameObject: Icon
- **Image 컴포넌트**:
  - `Sprite`: None (비어있음)
  - **Raycast Target**: ☐ 체크 해제 (중요!)
  - `Color`: `#FFFFFF` (흰색)
  - **Enabled**: ☐ 체크 해제 (비어있을 때)

**중요**:
- **슬롯 배경 Image**는 Raycast Target **활성화** (드롭 감지용)
- **Icon Image**는 Raycast Target **비활성화** (드래그 방해 방지)

---

### Inventory 슬롯 (UI_MasterPanel 하위)

**경로**: `UI_MasterPanel/InventoryPanel/SlotGrid/Slot_0, Slot_1, ...`

#### 필수 컴포넌트
1. **RectTransform** (자동 추가됨)
2. **Image** (슬롯 배경)
   - **Raycast Target**: ✅ 체크 (중요!)
3. **ItemSlot (Script)**
   - `icon`: `Icon` 자식 GameObject의 Image 컴포넌트
   - `slotType`: `Inventory`
   - `index`: 자동 설정됨 (Hotbar 슬롯 개수 + i)
   - `showDebugLogs`: ✅ 체크 (테스트 시)

#### 자식 GameObject: Icon
- **Image 컴포넌트**:
  - `Sprite`: None (비어있음)
  - **Raycast Target**: ☐ 체크 해제 (중요!)
  - `Color`: `#FFFFFF` (흰색)
  - **Enabled**: ☐ 체크 해제 (비어있을 때)

---

## 문제 해결

### 문제 1: 드래그가 시작되지 않음

**증상**: 아이템을 클릭해도 드래그가 시작되지 않음

**원인**:
1. Graphic Raycaster가 비활성화됨
2. 슬롯의 Image Raycast Target이 꺼져있음
3. ItemSlot 스크립트가 제대로 연결되지 않음

**해결**:
1. Canvas에 Graphic Raycaster 컴포넌트 확인
2. 슬롯 배경 Image의 **Raycast Target** ✅ 체크
3. ItemSlot 스크립트의 `icon` 필드가 제대로 연결되었는지 확인

---

### 문제 2: 드래그 중 아이콘이 보이지 않음

**증상**: 드래그를 시작하면 아이콘이 사라짐

**원인**:
1. DragIcon이 Canvas 외부에 생성됨
2. Sort Order가 너무 낮음

**해결**:
1. ItemSlot.cs의 `CreateDragIcon()` 메서드 확인
2. DragIcon이 최상위 Canvas의 자식으로 생성되는지 확인
3. Console에서 `[ItemSlot] Created drag icon on canvas...` 로그 확인

---

### 문제 3: 드롭이 감지되지 않음

**증상**: 드래그는 되지만 다른 슬롯에 드롭이 되지 않음

**원인**:
1. 대상 슬롯의 Raycast Target이 꺼져있음
2. EventSystem이 씬에 없거나 여러 개 존재
3. Icon Image의 Raycast Target이 켜져있어 방해함

**해결**:
1. **슬롯 배경 Image**: Raycast Target ✅ 활성화
2. **Icon Image**: Raycast Target ☐ 비활성화
3. EventSystem이 씬에 **1개만** 존재하는지 확인

---

### 문제 4: TimeScale = 0에서 드래그가 안됨

**증상**: UI가 열릴 때 드래그가 작동하지 않음

**원인**:
1. Canvas의 설정이 잘못됨 (거의 발생하지 않음)

**해결**:
1. Unity는 기본적으로 UI EventSystem을 Unscaled Time으로 처리하므로 정상적으로 작동해야 함
2. Console에서 `[ItemSlot]` 디버그 로그 확인
3. ItemSlot의 `showDebugLogs`를 ✅ 체크하여 드래그 이벤트 확인

---

### 문제 5: 핫바 ↔ 인벤토리 교차 드래그가 안됨

**증상**: 같은 종류의 슬롯끼리만 드래그 가능

**원인**:
1. 두 Canvas의 Sort Order가 같음
2. Graphic Raycaster가 한쪽 Canvas에만 있음

**해결**:
1. **HUD_Canvas**: Sort Order = `0`
2. **UI_MasterPanel**: Sort Order = `1`
3. 두 Canvas 모두 Graphic Raycaster **활성화** 확인

---

### 문제 6: UI 외부로 드래그 시 아이템이 생성되지 않음

**증상**: 슬롯을 UI 밖으로 드래그해도 월드에 아이템이 생성되지 않음

**원인**:
1. ItemWorldSpawner가 씬에 없음
2. ItemData에 worldPrefab이 설정되지 않음
3. Player 태그가 제대로 설정되지 않음

**해결**:
1. InitialScene에 **ItemWorldSpawner** GameObject 추가
2. ItemWorldSpawner 스크립트 추가:
   - `spawnOffsetY`: `-3`
   - `pickupIgnoreDuration`: `0.5`
   - `defaultWorldItemPrefab`: 기본 아이템 프리팹 할당
3. ItemData ScriptableObject에서 `worldPrefab` 필드 할당
4. Player GameObject의 Tag가 `"Player"`인지 확인

---

## 체크리스트

### HUD_Canvas 설정 확인
- [ ] Canvas 컴포넌트 존재
- [ ] Render Mode: `Screen Space - Overlay`
- [ ] Sort Order: `0`
- [ ] Graphic Raycaster 활성화
- [ ] Hotbar 슬롯들이 ItemSlot 스크립트 사용
- [ ] 각 슬롯의 `slotType`이 `Hotbar`로 설정됨

### UI_MasterPanel 설정 확인
- [ ] Canvas 컴포넌트 존재
- [ ] Render Mode: `Screen Space - Overlay`
- [ ] Sort Order: `1` (HUD_Canvas보다 높음)
- [ ] Graphic Raycaster 활성화
- [ ] Inventory 슬롯들이 ItemSlot 스크립트 사용
- [ ] 각 슬롯의 `slotType`이 `Inventory`로 설정됨

### EventSystem 확인
- [ ] 씬에 EventSystem이 **1개만** 존재
- [ ] Standalone Input Module 활성화

### ItemSlot 설정 확인
- [ ] 각 슬롯에 ItemSlot 스크립트 존재
- [ ] `icon` 필드가 자식 Icon Image에 연결됨
- [ ] 슬롯 배경 Image의 Raycast Target ✅ 활성화
- [ ] Icon Image의 Raycast Target ☐ 비활성화

### ItemWorldSpawner 확인
- [ ] InitialScene에 ItemWorldSpawner GameObject 존재
- [ ] ItemWorldSpawner 스크립트 추가됨
- [ ] `defaultWorldItemPrefab` 할당됨
- [ ] ItemData들의 `worldPrefab` 필드 설정됨

---

## Unity Inspector 빠른 확인 방법

### 1. Canvas 확인
```
1. Hierarchy에서 HUD_Canvas 선택
2. Inspector에서 다음 확인:
   - Canvas: Sort Order = 0
   - Graphic Raycaster: ✅ 체크
3. Hierarchy에서 UI_MasterPanel 선택
4. Inspector에서 다음 확인:
   - Canvas: Sort Order = 1
   - Graphic Raycaster: ✅ 체크
```

### 2. 슬롯 확인
```
1. Hierarchy에서 HUD_Canvas/Hotbar/Slot_0 선택
2. Inspector 확인:
   - Image: Raycast Target ✅
   - ItemSlot: slotType = Hotbar, icon 연결됨
   - Icon 자식: Image: Raycast Target ☐
3. Inventory 슬롯도 동일하게 확인 (slotType = Inventory)
```

### 3. EventSystem 확인
```
1. Hierarchy에서 "EventSystem" 검색
2. 1개만 존재하는지 확인
3. 2개 이상이면 중복 삭제
```

---

**작성일**: 2025-11-11
**버전**: 1.0
**관련 파일**:
- ItemSlot.cs
- ItemWorldSpawner.cs
- Hotbar.cs
- InventoryUI.cs
