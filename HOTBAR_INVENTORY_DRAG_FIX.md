# 🔧 핫바 ↔ 인벤토리 드래그 문제 해결 가이드

## 🎯 증상

- InventoryPanel이 열려있을 때 Hotbar 아이템을 드래그할 수 없음
- 또는 Hotbar에서 Inventory로 드래그가 안됨

## 🔍 원인

두 개의 Canvas (HUD_Canvas와 UI_MasterPanel)가 서로 다른 Raycast 설정을 가지고 있기 때문입니다.

---

## ✅ 해결 방법

### Step 1: HUD_Canvas 확인

1. **Hierarchy**에서 `HUD_Canvas` 선택
2. **Inspector** 확인:
   - **Graphic Raycaster** 컴포넌트가 있는가?
   - **Blocking Objects**: `None`
   - **Blocking Mask**: `Everything`

만약 Graphic Raycaster가 없다면:
1. **Add Component** 클릭
2. **Graphic Raycaster** 검색 후 추가

---

### Step 2: UI_MasterPanel 확인

1. **Hierarchy**에서 `UI_MasterPanel` 선택
2. **Inspector** 확인:
   - **Graphic Raycaster** 컴포넌트가 있는가?
   - **Blocking Objects**: `None`
   - **Blocking Mask**: `Everything`

---

### Step 3: 슬롯 Raycast Target 확인

#### Hotbar 슬롯 (HUD_Canvas/Hotbar/Slot_*)

각 슬롯에 대해:
1. 슬롯 GameObject 선택
2. **Inspector**에서 **Image** 컴포넌트 확인
3. **Raycast Target**: ✅ **반드시 체크**

#### Inventory 슬롯 (UI_MasterPanel/InventoryPanel/SlotGrid/Slot_*)

각 슬롯에 대해:
1. 슬롯 GameObject 선택
2. **Inspector**에서 **Image** 컴포넌트 확인
3. **Raycast Target**: ✅ **반드시 체크**

---

### Step 4: Icon Image Raycast Target 비활성화

**중요**: Icon(자식 GameObject)의 Raycast Target은 **비활성화**해야 합니다!

#### Hotbar Icon

1. `HUD_Canvas/Hotbar/Slot_0/Icon` 선택
2. **Inspector**에서 **Image** 컴포넌트 확인
3. **Raycast Target**: ☐ **반드시 체크 해제**

모든 Hotbar 슬롯의 Icon에 대해 반복

#### Inventory Icon

1. `UI_MasterPanel/InventoryPanel/SlotGrid/Slot_0/Icon` 선택
2. **Inspector**에서 **Image** 컴포넌트 확인
3. **Raycast Target**: ☐ **반드시 체크 해제**

모든 Inventory 슬롯의 Icon에 대해 반복

---

### Step 5: Canvas Sort Order 확인

드래그 시 올바른 렌더링 순서를 위해:

1. **HUD_Canvas**:
   - Canvas → **Sort Order**: `0`

2. **UI_MasterPanel**:
   - Canvas → **Sort Order**: `1`

---

## 🧪 테스트

### 테스트 1: Hotbar → Inventory
1. Play 모드 진입
2. Tab 키로 Inventory 열기
3. Hotbar의 아이템을 Inventory로 드래그
4. ✅ 성공하면 아이템이 Inventory로 이동

### 테스트 2: Inventory → Hotbar
1. Play 모드 진입
2. Tab 키로 Inventory 열기
3. Inventory의 아이템을 Hotbar로 드래그
4. ✅ 성공하면 아이템이 Hotbar로 이동

### 테스트 3: 디버그 로그 확인
1. Slot Prefab에서 ItemSlot의 `showDebugLogs` ✅ 체크
2. Play 모드에서 드래그 시도
3. Console에서 다음 로그 확인:
   ```
   [ItemSlot] OnBeginDrag: Started dragging '...' from Hotbar slot 0
   [ItemSlot] OnDrop: Dropped '...' on Inventory slot 10
   [ItemSlot] Swapping: Hotbar[0] <-> Inventory[10]
   ```

---

## 🐛 여전히 안되는 경우

### 문제 1: 드래그 시작은 되지만 드롭이 안됨

**원인**: 대상 슬롯의 Raycast Target이 꺼져있음

**해결**:
- 모든 슬롯의 배경 Image: Raycast Target ✅
- 모든 Icon Image: Raycast Target ☐

### 문제 2: Hotbar에서만 드래그가 안됨

**원인**: HUD_Canvas에 Graphic Raycaster가 없음

**해결**:
1. HUD_Canvas 선택
2. Add Component → Graphic Raycaster

### 문제 3: Inventory에서만 드래그가 안됨

**원인**: UI_MasterPanel에 Graphic Raycaster가 없음

**해결**:
1. UI_MasterPanel 선택
2. Add Component → Graphic Raycaster

### 문제 4: 드래그 아이콘이 보이지 않음

**원인**: Canvas Sort Order 문제

**해결**:
1. HUD_Canvas: Sort Order = 0
2. UI_MasterPanel: Sort Order = 1
3. DragIcon은 자동으로 Sort Order 1000으로 생성됨

---

## 📊 최종 체크리스트

드래그가 작동하려면 다음 모든 조건이 충족되어야 합니다:

- [ ] **HUD_Canvas**: Graphic Raycaster 활성화
- [ ] **UI_MasterPanel**: Graphic Raycaster 활성화
- [ ] **모든 Hotbar 슬롯 배경**: Raycast Target ✅
- [ ] **모든 Inventory 슬롯 배경**: Raycast Target ✅
- [ ] **모든 Icon Image**: Raycast Target ☐
- [ ] **HUD_Canvas**: Sort Order = 0
- [ ] **UI_MasterPanel**: Sort Order = 1
- [ ] **EventSystem**: 씬에 1개만 존재
- [ ] **Slot Prefab**: ItemSlot 스크립트 사용 중

---

## 💡 빠른 수정 스크립트

모든 슬롯을 자동으로 수정하고 싶다면, Unity Console에서 다음 스크립트를 실행할 수 있습니다:

```csharp
// Window → General → Console에서 실행
// (임시 스크립트를 만들어서 실행)

using UnityEngine;
using UnityEngine.UI;

public class FixSlotRaycastTargets : MonoBehaviour
{
    [ContextMenu("Fix All Slot Raycast Targets")]
    void FixAllSlots()
    {
        // 모든 ItemSlot 찾기
        ItemSlot[] allSlots = FindObjectsOfType<ItemSlot>();

        int fixed = 0;
        foreach (ItemSlot slot in allSlots)
        {
            // 슬롯 배경 Image의 Raycast Target 활성화
            Image slotImage = slot.GetComponent<Image>();
            if (slotImage != null)
            {
                slotImage.raycastTarget = true;
                fixed++;
            }

            // Icon Image의 Raycast Target 비활성화
            if (slot.icon != null)
            {
                slot.icon.raycastTarget = false;
            }
        }

        Debug.Log($"✅ Fixed {fixed} slots!");
    }
}
```

---

**작성일**: 2025-11-11
**관련 파일**: ItemSlot.cs, HUD_Canvas, UI_MasterPanel
