# 🎯 드래그 앤 드롭 인벤토리 시스템 - 구현 가이드

## 📋 개요

이 가이드는 완전한 드래그 앤 드롭 인벤토리 시스템의 구현 방법을 단계별로 설명합니다.

### 주요 기능
- ✅ 핫바 ↔ 인벤토리 양방향 드래그 앤 드롭
- ✅ 핫바 내부 슬롯 순서 변경
- ✅ 인벤토리 내부 슬롯 이동
- ✅ UI 외부로 드래그 시 월드에 아이템 생성 (플레이어 y-3 위치)
- ✅ TimeScale = 0 상태에서도 작동
- ✅ 디버그 로그 시스템

---

## 🏗 새로운 파일 구조

### 생성된 파일
1. **SlotType.cs** - Inventory / Hotbar 구분 Enum
2. **ItemSlot.cs** - 개선된 슬롯 시스템 (기존 Slot.cs 대체)
3. **ItemWorldSpawner.cs** - 월드 아이템 생성 관리자
4. **CANVAS_SETUP_GUIDE.md** - Canvas 설정 가이드
5. **DRAG_DROP_IMPLEMENTATION_GUIDE.md** - 이 파일

### 수정된 파일
1. **ItemData.cs** - `worldPrefab` 필드 추가
2. **InventoryUI.cs** - ItemSlot 사용으로 변경
3. **Hotbar.cs** - ItemSlot 사용으로 변경

### 폐기된 파일
- **Slot.cs** - ItemSlot.cs로 대체됨 (삭제 가능)

---

## 🔧 Unity Inspector 설정

### Step 1: ItemWorldSpawner GameObject 추가

1. **InitialScene** 열기
2. Hierarchy에서 빈 GameObject 생성
3. 이름: `ItemWorldSpawner`
4. `ItemWorldSpawner.cs` 스크립트 추가
5. Inspector 설정:
   - `spawnOffsetY`: `-3`
   - `pickupIgnoreDuration`: `0.5`
   - `defaultWorldItemPrefab`: 기본 아이템 프리팹 드래그 (예: ItemPrefab)
   - `showDebugLogs`: ✅ 체크 (테스트 시)

**중요**: ItemWorldSpawner는 DontDestroyOnLoad이므로 InitialScene에만 배치하면 됩니다.

---

### Step 2: 기존 Slot 컴포넌트를 ItemSlot으로 변경

#### 2-1. Hotbar 슬롯 변경

**경로**: `HUD_Canvas/Hotbar/Slot_0, Slot_1, Slot_2, ...`

각 슬롯에 대해:
1. 기존 `Slot` 스크립트 제거 (Remove Component)
2. `ItemSlot` 스크립트 추가 (Add Component → ItemSlot)
3. Inspector 설정:
   - `icon`: `Icon` 자식 GameObject의 Image 컴포넌트 드래그
   - `slotType`: `Hotbar` 선택
   - `index`: 0 (자동으로 설정되므로 신경 쓰지 않아도 됨)
   - `showDebugLogs`: ✅ 체크 (테스트 시)

#### 2-2. Inventory 슬롯 변경

**경로**: `UI_MasterPanel/InventoryPanel/SlotGrid/Slot_0, Slot_1, ...`

각 슬롯에 대해:
1. 기존 `Slot` 스크립트 제거 (Remove Component)
2. `ItemSlot` 스크립트 추가 (Add Component → ItemSlot)
3. Inspector 설정:
   - `icon`: `Icon` 자식 GameObject의 Image 컴포넌트 드래그
   - `slotType`: `Inventory` 선택
   - `index`: 0 (자동으로 설정되므로 신경 쓰지 않아도 됨)
   - `showDebugLogs`: ✅ 체크 (테스트 시)

**팁**: Prefab을 사용한다면 Prefab에서 한 번만 수정하면 모든 슬롯에 적용됩니다.

---

### Step 3: Canvas 설정 확인

[CANVAS_SETUP_GUIDE.md](CANVAS_SETUP_GUIDE.md) 참고

필수 확인 사항:
- [ ] **HUD_Canvas**: Graphic Raycaster 활성화, Sort Order = 0
- [ ] **UI_MasterPanel**: Graphic Raycaster 활성화, Sort Order = 1
- [ ] 슬롯 배경 Image: **Raycast Target ✅ 활성화**
- [ ] Icon Image: **Raycast Target ☐ 비활성화**
- [ ] EventSystem이 씬에 **1개만** 존재

---

### Step 4: ItemData ScriptableObject 설정

각 아이템의 ItemData ScriptableObject에서:

1. **Project 창**에서 ItemData Asset 선택 (예: `Items/Apple.asset`)
2. Inspector에서 **World Object** 섹션 확인
3. `worldPrefab` 필드에 월드 프리팹 할당:
   - 옵션 1: 각 아이템마다 고유 프리팹 할당
   - 옵션 2: 비워두고 ItemWorldSpawner의 defaultWorldItemPrefab 사용

**예시**:
```
Items/
├── Apple.asset (worldPrefab: ApplePrefab)
├── Sword.asset (worldPrefab: SwordPrefab)
└── Potion.asset (worldPrefab: PotionPrefab)
```

---

### Step 5: Slot.cs 스크립트 삭제 (선택)

기존 `Slot.cs` 파일은 더 이상 사용되지 않으므로 삭제 가능:

1. `Assets/Scripts/Slot.cs` 파일 선택
2. Delete 키 눌러 삭제
3. Unity Console에서 에러가 없는지 확인

**주의**: 다른 스크립트에서 `Slot` 클래스를 참조하는 경우 먼저 수정 필요

---

## 🧪 테스트 가이드

### 테스트 체크리스트

#### 기본 드래그 앤 드롭
- [ ] **핫바 → 인벤토리**: 핫바 아이템을 인벤토리로 드래그 가능
- [ ] **인벤토리 → 핫바**: 인벤토리 아이템을 핫바로 드래그 가능
- [ ] **핫바 내부**: 핫바 슬롯 간 드래그로 순서 변경 가능
- [ ] **인벤토리 내부**: 인벤토리 슬롯 간 드래그로 이동 가능

#### 슬롯 교환
- [ ] **빈 슬롯 → 빈 슬롯**: 아무 일도 일어나지 않음
- [ ] **아이템 → 빈 슬롯**: 아이템이 이동됨
- [ ] **아이템 → 아이템**: 두 아이템의 위치가 교환됨

#### 월드 드롭
- [ ] **UI 외부로 드래그**: 마우스가 UI 밖으로 나감
- [ ] **월드 아이템 생성**: 플레이어 y-3 위치에 아이템 생성됨
- [ ] **인벤토리에서 제거**: 드롭한 아이템이 인벤토리에서 사라짐
- [ ] **아이템 줍기**: 0.5초 후 다시 주울 수 있음

#### TimeScale = 0 테스트
- [ ] **Tab 키로 UI 열기**: Master UI가 열리고 TimeScale = 0
- [ ] **드래그 가능**: UI가 열린 상태에서도 드래그 작동
- [ ] **아이콘 따라감**: 마우스 커서를 따라 아이템 아이콘 이동
- [ ] **드롭 작동**: TimeScale = 0 상태에서도 드롭 정상 작동

#### 디버그 로그
- [ ] **OnBeginDrag**: `[ItemSlot] OnBeginDrag: Started dragging...` 출력
- [ ] **OnDrag**: 드래그 중 로그 없음 (정상)
- [ ] **OnDrop**: `[ItemSlot] OnDrop: Dropped '...' on ...` 출력
- [ ] **OnEndDrag**: `[ItemSlot] OnEndDrag: Ending drag...` 출력
- [ ] **World Spawn**: `[ItemWorldSpawner] Spawning '...' at ...` 출력

---

## 🐛 문제 해결

### 문제 1: 컴파일 에러 "Slot does not exist"

**에러 메시지**:
```
Assets\Scripts\InventoryUI.cs(6,12): error CS0246: The type or namespace name 'Slot' could not be found
Assets\Scripts\Hotbar.cs(10,12): error CS0246: The type or namespace name 'Slot' could not be found
```

**원인**: InventoryUI.cs와 Hotbar.cs가 아직 ItemSlot을 사용하지 않음

**해결**:
1. 이 가이드의 Step 2에 따라 스크립트가 수정되었는지 확인
2. Unity 재컴파일 (Ctrl+R)

---

### 문제 2: "ItemWorldSpawner.Instance is null"

**증상**: UI 외부로 드래그 시 아이템이 생성되지 않고 에러 발생

**원인**: ItemWorldSpawner GameObject가 씬에 없음

**해결**:
1. InitialScene에 ItemWorldSpawner GameObject 추가
2. ItemWorldSpawner 스크립트 추가 및 설정
3. Play 모드 진입 시 Console에서 `✅ ItemWorldSpawner: Initialized` 확인

---

### 문제 3: 드래그 시 "NullReferenceException: icon"

**증상**: 드래그 시작 시 NullReferenceException 발생

**원인**: ItemSlot의 `icon` 필드가 연결되지 않음

**해결**:
1. Hierarchy에서 문제의 슬롯 선택
2. Inspector에서 ItemSlot의 `icon` 필드 확인
3. 자식 GameObject "Icon"의 Image 컴포넌트를 드래그하여 연결

---

### 문제 4: 월드에 아이템이 생성되지만 스프라이트가 안보임

**증상**: 아이템이 생성되지만 보이지 않음

**원인**:
1. ItemData의 worldPrefab에 SpriteRenderer가 없음
2. SpriteRenderer의 Sorting Layer 문제

**해결**:
1. worldPrefab에 SpriteRenderer 컴포넌트 추가
2. Sorting Layer를 "Player"로 설정
3. Sorting Order를 5 이상으로 설정 (플레이어 위에 표시)

---

### 문제 5: 드래그 중 아이콘이 다른 UI 뒤에 가려짐

**증상**: 드래그 아이콘이 UI 패널 뒤에 숨겨짐

**원인**: DragIcon의 Sort Order가 너무 낮음

**해결**:
ItemSlot.cs의 `CreateDragIcon()` 메서드에서 이미 처리됨:
```csharp
Canvas dragCanvas = dragIcon.AddComponent<Canvas>();
dragCanvas.overrideSorting = true;
dragCanvas.sortingOrder = 1000; // 매우 높은 값
```

만약 여전히 문제가 있다면:
1. ItemSlot.cs 확인
2. `dragCanvas.sortingOrder`를 더 높은 값으로 변경 (예: 10000)

---

### 문제 6: 같은 슬롯에 드롭 시 아이템이 사라짐

**증상**: 아이템을 원래 슬롯에 드롭하면 아이템이 사라짐

**원인**: ItemSlot의 OnEndDrag에서 같은 슬롯 체크 누락

**해결**:
ItemSlot.cs의 `HandleSlotDrop()` 메서드에 이미 처리됨:
```csharp
if (source == null || target == null || source == target)
{
    if (showDebugLogs && source == target)
        Debug.Log("[ItemSlot] Dropped on same slot, no action needed");
    return;
}
```

---

## 📊 디버그 로그 해석

### 정상 드래그 로그 예시

#### 핫바 → 인벤토리 드래그
```
[ItemSlot] OnBeginDrag: Started dragging 'Apple' from Hotbar slot 0
[ItemSlot] OnDrop: Dropped 'Apple' on Inventory slot 10
[ItemSlot] Swapping: Hotbar[0] <-> Inventory[10]
[ItemSlot] OnEndDrag: Ending drag for 'Apple'
```

#### UI 외부 드롭 (월드 생성)
```
[ItemSlot] OnBeginDrag: Started dragging 'Sword' from Inventory slot 15
[ItemSlot] OnEndDrag: Dropped outside UI, spawning in world
[ItemSlot] Dropping 'Sword' to world from slot 15
[ItemWorldSpawner] Spawning 'Sword' at (10.5, -1.2, 0.0)
[ItemWorldSpawner] Using ItemData.worldPrefab for 'Sword'
[ItemWorldSpawner] Configured Rigidbody2D
[ItemWorldSpawner] Configured SpriteRenderer
[ItemWorldSpawner] Configured Item component with pickup delay 0.5s
[ItemWorldSpawner] Successfully spawned 'Sword'
```

#### 같은 슬롯에 드롭
```
[ItemSlot] OnBeginDrag: Started dragging 'Potion' from Hotbar slot 2
[ItemSlot] OnDrop: Dropped 'Potion' on Hotbar slot 2
[ItemSlot] Dropped on same slot, no action needed
[ItemSlot] OnEndDrag: Ending drag for 'Potion'
```

---

## 🔍 고급 기능

### 슬롯별 디버그 로그 제어

개별 슬롯의 디버그 로그를 제어하려면:

1. Hierarchy에서 특정 슬롯 선택
2. Inspector에서 ItemSlot의 `showDebugLogs` 체크박스 토글
3. ✅ 체크: 해당 슬롯의 모든 로그 출력
4. ☐ 체크 해제: 해당 슬롯의 로그 숨김

---

### 커스텀 스폰 위치

ItemWorldSpawner.cs의 `SpawnItemAtPosition()` 메서드 사용:

```csharp
// 예시: 특정 위치에 아이템 생성
Vector3 customPosition = new Vector3(10f, 5f, 0f);
ItemWorldSpawner.Instance.SpawnItemAtPosition(itemData, customPosition);
```

---

### 드래그 제한 조건 추가

ItemSlot.cs의 `OnBeginDrag()`에 조건 추가:

```csharp
public void OnBeginDrag(PointerEventData eventData)
{
    if (currentItem == null) return;

    // 예시: 특정 아이템은 드래그 불가
    if (currentItem.itemName == "Quest Item")
    {
        Debug.Log("[ItemSlot] Quest items cannot be moved!");
        return;
    }

    // 기존 코드...
}
```

---

## 🎯 최종 확인 사항

구현 완료 후 다음을 확인하세요:

1. ✅ **ItemWorldSpawner가 InitialScene에 존재**하는가?
2. ✅ **모든 슬롯이 ItemSlot 스크립트를 사용**하는가?
3. ✅ **Slot.cs 파일이 삭제**되었는가?
4. ✅ **Canvas들의 Graphic Raycaster가 활성화**되었는가?
5. ✅ **ItemData들의 worldPrefab이 설정**되었는가?
6. ✅ **핫바 ↔ 인벤토리 드래그가 정상 작동**하는가?
7. ✅ **UI 외부 드롭 시 월드에 아이템 생성**되는가?
8. ✅ **TimeScale = 0 상태에서도 드래그 작동**하는가?

---

## 📝 추가 개선 아이디어

### 1. 드래그 아이콘 크기 조정
ItemSlot.cs의 `CreateDragIcon()` 메서드에서:
```csharp
rt.sizeDelta = new Vector2(50, 50); // 기본값
// → rt.sizeDelta = new Vector2(64, 64); // 더 크게
```

### 2. 드래그 중 반투명 효과
```csharp
Image img = dragIcon.AddComponent<Image>();
img.sprite = draggingItem.sprite;
img.raycastTarget = false;
Color color = img.color;
color.a = 0.7f; // 70% 불투명도
img.color = color;
```

### 3. 슬롯 하이라이트 효과
드롭 가능한 슬롯에 하이라이트 추가:
```csharp
public void OnDrop(PointerEventData eventData)
{
    // 슬롯 배경색 변경
    GetComponent<Image>().color = Color.yellow;

    // 기존 코드...
}
```

### 4. 사운드 효과 추가
```csharp
public void OnBeginDrag(PointerEventData eventData)
{
    if (currentItem == null) return;

    // 사운드 재생
    AudioManager.Instance.PlaySFX("PickupSound");

    // 기존 코드...
}
```

---

**작성일**: 2025-11-11
**버전**: 1.0
**관련 파일**:
- ItemSlot.cs
- ItemWorldSpawner.cs
- SlotType.cs
- ItemData.cs
- InventoryUI.cs
- Hotbar.cs
- CANVAS_SETUP_GUIDE.md
