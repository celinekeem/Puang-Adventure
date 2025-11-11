# 🎮 드래그 앤 드롭 인벤토리 시스템 - 완료 요약

## ✅ 구현 완료

드래그 앤 드롭 인벤토리 시스템이 완성되었습니다!

---

## 📦 생성된 파일

### 1. 핵심 스크립트
| 파일명 | 설명 | 경로 |
|--------|------|------|
| **SlotType.cs** | Inventory/Hotbar 구분 Enum | `Assets/Scripts/SlotType.cs` |
| **ItemSlot.cs** | 개선된 슬롯 시스템 (Slot.cs 대체) | `Assets/Scripts/ItemSlot.cs` |
| **ItemWorldSpawner.cs** | 월드 아이템 생성 관리자 | `Assets/Scripts/ItemWorldSpawner.cs` |

### 2. 문서
| 파일명 | 설명 | 경로 |
|--------|------|------|
| **CANVAS_SETUP_GUIDE.md** | Canvas 설정 가이드 | 프로젝트 루트 |
| **DRAG_DROP_IMPLEMENTATION_GUIDE.md** | 구현 가이드 | 프로젝트 루트 |
| **DRAG_DROP_SYSTEM_SUMMARY.md** | 이 파일 | 프로젝트 루트 |

---

## 🔧 수정된 파일

### 1. ItemData.cs
**변경 사항**: `worldPrefab` 필드 추가

```csharp
[Header("World Object")]
public GameObject worldPrefab;  // Prefab to spawn when dropped into world
```

**위치**: [ItemData.cs:15-16](Assets/Scripts/ItemData.cs#L15-L16)

---

### 2. InventoryUI.cs
**변경 사항**: `Slot` → `ItemSlot` 변경, SlotType 설정 추가

```csharp
// BEFORE:
private Slot[] slots;
slots = slotsParent.GetComponentsInChildren<Slot>();

// AFTER:
private ItemSlot[] slots;
slots = slotsParent.GetComponentsInChildren<ItemSlot>();

// SlotType 설정 추가
for (int i = 0; i < slots.Length; i++)
{
    slots[i].slotType = SlotType.Inventory;
}
```

**위치**: [InventoryUI.cs:6](Assets/Scripts/InventoryUI.cs#L6), [InventoryUI.cs:46-52](Assets/Scripts/InventoryUI.cs#L46-L52)

---

### 3. Hotbar.cs
**변경 사항**: `Slot` → `ItemSlot` 변경, SlotType 설정 추가

```csharp
// BEFORE:
private Slot[] slots;
slots = slotsParent.GetComponentsInChildren<Slot>();

// AFTER:
private ItemSlot[] slots;
slots = slotsParent.GetComponentsInChildren<ItemSlot>();

// SlotType 설정 추가
for (int i = 0; i < slots.Length; i++)
{
    slots[i].slotType = SlotType.Hotbar;
}
```

**위치**: [Hotbar.cs:10](Assets/Scripts/Hotbar.cs#L10), [Hotbar.cs:18-28](Assets/Scripts/Hotbar.cs#L18-L28)

---

## 🗑 폐기된 파일

| 파일명 | 상태 | 대체 파일 |
|--------|------|----------|
| **Slot.cs** | 삭제 권장 | ItemSlot.cs |

**주의**: Slot.cs를 삭제하기 전에 다른 스크립트에서 참조하는지 확인하세요.

---

## 🎯 주요 기능

### 1. 핫바 ↔ 인벤토리 양방향 드래그
- ✅ 핫바 아이템을 인벤토리로 드래그 가능
- ✅ 인벤토리 아이템을 핫바로 드래그 가능
- ✅ 빈 슬롯 및 아이템 교환 지원

### 2. 내부 슬롯 이동
- ✅ 핫바 내부 슬롯 순서 변경 가능
- ✅ 인벤토리 내부 슬롯 이동 가능

### 3. 월드 아이템 드롭
- ✅ UI 외부로 드래그 시 월드에 아이템 생성
- ✅ 생성 위치: 플레이어 y-3 (설정 가능)
- ✅ 0.5초 동안 다시 줍기 방지

### 4. TimeScale = 0 지원
- ✅ UI 열릴 때 (Time.timeScale = 0) 드래그 작동
- ✅ Unscaled Time 기반 이벤트 처리

### 5. 디버그 시스템
- ✅ 각 슬롯별 디버그 로그 on/off 가능
- ✅ 상세한 드래그 이벤트 로깅
- ✅ ItemWorldSpawner 생성 로그

---

## 🛠 Unity Inspector 작업 필요

구현을 완료하려면 Unity Inspector에서 다음 작업이 필요합니다:

### ⚠ 필수 작업

#### 1. ItemWorldSpawner GameObject 추가
- **씬**: InitialScene
- **설정**:
  - `spawnOffsetY`: `-3`
  - `pickupIgnoreDuration`: `0.5`
  - `defaultWorldItemPrefab`: 기본 아이템 프리팹 할당
  - `showDebugLogs`: ✅ (테스트 시)

#### 2. 모든 슬롯의 스크립트 교체
- **경로**: `HUD_Canvas/Hotbar/Slot_*`와 `UI_MasterPanel/InventoryPanel/SlotGrid/Slot_*`
- **작업**:
  1. 기존 `Slot` 스크립트 제거
  2. `ItemSlot` 스크립트 추가
  3. `icon` 필드에 자식 Icon Image 연결
  4. Hotbar: `slotType` = `Hotbar`
  5. Inventory: `slotType` = `Inventory`

#### 3. Canvas 설정 확인
- **HUD_Canvas**:
  - Graphic Raycaster 활성화
  - Sort Order: `0`
- **UI_MasterPanel**:
  - Graphic Raycaster 활성화
  - Sort Order: `1`

#### 4. ItemData worldPrefab 설정
- **경로**: Project 창 → Items 폴더
- **작업**: 각 ItemData Asset의 `worldPrefab` 필드에 프리팹 할당

---

## 📚 상세 가이드 링크

더 자세한 정보는 다음 가이드를 참고하세요:

1. **[CANVAS_SETUP_GUIDE.md](CANVAS_SETUP_GUIDE.md)**
   - Canvas 설정 방법
   - 슬롯 설정 방법
   - 문제 해결

2. **[DRAG_DROP_IMPLEMENTATION_GUIDE.md](DRAG_DROP_IMPLEMENTATION_GUIDE.md)**
   - Unity Inspector 설정 단계
   - 테스트 가이드
   - 디버그 로그 해석
   - 고급 기능

---

## 🧪 테스트 체크리스트

구현 후 다음 항목들을 테스트하세요:

### 기본 기능
- [ ] 핫바 → 인벤토리 드래그
- [ ] 인벤토리 → 핫바 드래그
- [ ] 핫바 내부 슬롯 이동
- [ ] 인벤토리 내부 슬롯 이동
- [ ] 아이템 교환 (Swap)

### 월드 드롭
- [ ] UI 외부로 드래그 시 아이템 생성
- [ ] 플레이어 y-3 위치에 생성
- [ ] 생성된 아이템 줍기
- [ ] 0.5초 픽업 딜레이 작동

### TimeScale = 0
- [ ] Tab 키로 UI 열기 (TimeScale = 0)
- [ ] 드래그 앤 드롭 작동
- [ ] 아이콘이 마우스 따라감
- [ ] 드롭 정상 작동

### 디버그
- [ ] Console에 드래그 로그 출력
- [ ] 월드 생성 로그 출력
- [ ] showDebugLogs로 개별 제어 가능

---

## 🎓 코드 구조 설명

### SlotType.cs
```csharp
public enum SlotType
{
    Inventory,  // 인벤토리 슬롯
    Hotbar      // 핫바 슬롯
}
```

**용도**: 슬롯이 Inventory에 속하는지 Hotbar에 속하는지 구분

---

### ItemSlot.cs (핵심 기능)

#### 주요 메서드

| 메서드 | 설명 |
|--------|------|
| `OnBeginDrag()` | 드래그 시작, DragIcon 생성 |
| `OnDrag()` | 드래그 중, 아이콘을 마우스 따라 이동 |
| `OnEndDrag()` | 드래그 종료, 월드 드롭 또는 슬롯 복원 |
| `OnDrop()` | 다른 슬롯에 드롭됨 |
| `HandleSlotDrop()` | 슬롯 간 아이템 교환 처리 |
| `DropItemToWorld()` | UI 외부 드롭 시 월드에 생성 |

#### Static 변수 (드래그 상태 공유)
```csharp
private static GameObject dragIcon;       // 드래그 아이콘
private static ItemSlot dragSourceSlot;   // 드래그 시작 슬롯
private static ItemData draggingItem;     // 드래그 중인 아이템
private static SlotType dragSourceType;   // 드래그 시작 슬롯 타입
private static bool isDraggingOutsideUI;  // UI 외부 드래그 여부
```

---

### ItemWorldSpawner.cs

#### 주요 기능
- **Singleton 패턴**: DontDestroyOnLoad로 씬 전환 시에도 유지
- **SpawnItemAtPlayer()**: 플레이어 y-3 위치에 아이템 생성
- **SpawnItemAtPosition()**: 특정 위치에 아이템 생성
- **Prefab 우선순위**:
  1. ItemData.worldPrefab (개별 설정)
  2. ItemWorldSpawner.defaultWorldItemPrefab (기본값)
  3. Inventory.itemWorldPrefab (폴백)

#### 생성된 아이템 자동 설정
- Rigidbody2D: gravityScale = 0, velocity = 0
- SpriteRenderer: sortingLayerName = "Player", sortingOrder = 5
- Item 컴포넌트: data 설정, 픽업 딜레이 설정

---

## 🔍 기술적 하이라이트

### 1. TimeScale = 0 지원
Unity의 EventSystem과 GraphicRaycaster는 **Unscaled Time**을 사용하므로, `Time.timeScale = 0` 상태에서도 다음이 작동합니다:
- IBeginDragHandler, IDragHandler, IEndDragHandler
- RectTransformUtility.ScreenPointToLocalPointInRectangle
- PointerEventData

### 2. Static 변수로 드래그 상태 공유
모든 ItemSlot 인스턴스가 같은 static 변수를 공유하여 드래그 상태를 추적합니다. 이를 통해:
- 드래그 시작 슬롯 추적
- 드래그 중인 아이템 추적
- UI 외부 드래그 감지

### 3. 드래그 아이콘 최상위 렌더링
```csharp
Canvas dragCanvas = dragIcon.AddComponent<Canvas>();
dragCanvas.overrideSorting = true;
dragCanvas.sortingOrder = 1000; // 최상위
```

### 4. Raycast Target 전략
- **슬롯 배경 Image**: Raycast Target ✅ (드롭 감지용)
- **Icon Image**: Raycast Target ☐ (드래그 방해 방지)

---

## 📊 성능 고려사항

### 최적화된 부분
- ✅ 드래그 중에는 1프레임마다 위치만 업데이트 (OnDrag)
- ✅ 불필요한 UI 갱신 최소화 (RefreshAllUI는 드래그 완료 시에만)
- ✅ Static 변수로 메모리 절약

### 주의사항
- ⚠ 슬롯이 매우 많을 경우 (100개 이상) 초기화 시간 고려
- ⚠ showDebugLogs가 켜져있으면 Console 부하 증가

---

## 🚀 다음 단계 (선택)

구현이 완료되면 다음 기능들을 추가 고려할 수 있습니다:

### 1. 스택 분할 (Shift + 드래그)
```csharp
if (Input.GetKey(KeyCode.LeftShift))
{
    // 스택 절반 분할 로직
}
```

### 2. 우클릭 빠른 이동
```csharp
public void OnPointerClick(PointerEventData eventData)
{
    if (eventData.button == PointerEventData.InputButton.Right)
    {
        // 핫바 → 인벤토리 또는 인벤토리 → 핫바로 빠르게 이동
    }
}
```

### 3. 슬롯 하이라이트
드롭 가능한 슬롯에 시각적 피드백 추가

### 4. 드래그 사운드
드래그 시작, 드롭, 월드 생성 시 효과음 재생

### 5. 아이템 툴팁
슬롯에 마우스 오버 시 아이템 정보 표시

---

## ⚠ 알려진 제한사항

1. **동시 드래그 불가**: 한 번에 1개의 아이템만 드래그 가능
2. **멀티터치 미지원**: 마우스/단일 터치만 지원
3. **Prefab 필수**: 월드 드롭 시 worldPrefab이 없으면 생성 불가

---

## 📞 도움이 필요하면

문제가 발생하면:

1. **Console 로그 확인**: `[ItemSlot]`, `[ItemWorldSpawner]` 로그 확인
2. **가이드 참고**: DRAG_DROP_IMPLEMENTATION_GUIDE.md의 문제 해결 섹션
3. **Unity Profiler**: 성능 문제 발생 시 프로파일링

---

## ✨ 구현 완료!

모든 코드가 작성되었습니다. 이제 Unity에서 Inspector 설정만 하면 드래그 앤 드롭 시스템을 사용할 수 있습니다!

**Happy Coding! 🎮**

---

**작성일**: 2025-11-11
**버전**: 1.0
**작성자**: Claude Code
