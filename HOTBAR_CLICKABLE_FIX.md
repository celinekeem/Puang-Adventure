# 🖱 인벤토리 열린 상태에서 핫바 클릭 가능하게 만들기

## 🎯 문제

- Inventory가 열린 상태(Tab 키)에서 Hotbar 아이템을 클릭할 수 없음
- UI_MasterPanel의 배경이 Hotbar를 가리고 있음

## ✅ 해결 방법 1: Background Raycast Target 비활성화 (권장)

### Step 1: UI_MasterPanel Background 찾기

1. **Hierarchy**에서 `UI_MasterPanel` 확장
2. **Background** GameObject 찾기 (보통 가장 위에 있음)

### Step 2: Raycast Target 비활성화

1. **Background** GameObject 선택
2. **Inspector**에서 **Image** 컴포넌트 확인
3. **Raycast Target**: ☐ **체크 해제**

### 설명

Background의 Raycast Target을 끄면:
- ✅ 배경을 클릭해도 아래의 Hotbar가 반응함
- ✅ Inventory 슬롯들은 여전히 클릭 가능 (각 슬롯이 자체 Raycast Target을 가짐)
- ✅ Top Buttons도 여전히 클릭 가능

---

## ✅ 해결 방법 2: Background를 투명하게 유지 (대안)

만약 Background를 완전히 투명하게 사용한다면:

1. **Background** 선택
2. **Inspector**에서 **Image** 컴포넌트
3. **Color**의 **Alpha** 값을 `0`으로 설정
4. **Raycast Target**: ☐ **체크 해제**

---

## ✅ 해결 방법 3: Background에 구멍 뚫기 (고급)

UI_MasterPanel의 디자인을 유지하면서 Hotbar 영역만 투명하게 만들고 싶다면:

### Option A: Mask 사용

1. Background에 **Mask** 컴포넌트 추가
2. Hotbar 영역을 제외한 영역만 렌더링

### Option B: 여러 개의 Panel로 분할

```
UI_MasterPanel
├── Background_Top (상단 배경)
├── Background_Left (왼쪽 배경)
├── Background_Right (오른쪽 배경)
└── Background_Bottom (하단 배경, Hotbar 제외)
```

각 Panel의 Raycast Target을 개별적으로 제어

---

## 🧪 테스트

### 테스트 1: Background Raycast 확인

1. Play 모드 진입
2. **Tab** 키로 Inventory 열기
3. Hotbar 아이템 클릭
4. ✅ 클릭되면 성공!

### 테스트 2: Inventory 여전히 작동하는지 확인

1. Play 모드에서 Inventory 열기
2. Inventory 슬롯 클릭
3. ✅ 여전히 클릭되면 성공!

### 테스트 3: 드래그 테스트

1. Hotbar → Inventory 드래그
2. Inventory → Hotbar 드래그
3. ✅ 둘 다 작동하면 성공!

---

## 🎨 UI 디자인 고려사항

### 배경이 중요한 경우

만약 UI_MasterPanel의 배경이 반투명 어두운 오버레이라면:

1. **Background**의 **Raycast Target**을 끔
2. **ESC 키**로 닫을 수 있도록 UI_MasterController에 설정됨
3. 사용자는 배경 바깥을 클릭하면 Hotbar와 상호작용 가능

### 배경 클릭으로 UI 닫기를 원하는 경우

Background를 클릭하면 UI가 닫히게 하고 싶다면:

1. Background에 **Button** 컴포넌트 추가
2. **OnClick()** 이벤트에 `UI_MasterController.CloseMasterUI()` 연결
3. **Raycast Target**: ✅ 활성화 유지

하지만 이 경우 Hotbar가 가려집니다. 따라서:
- **권장하지 않음**: Hotbar를 클릭할 수 없게 됨
- **대안**: ESC 키로 닫기 (이미 구현됨)

---

## 📊 정리

| 방법 | Hotbar 클릭 | Inventory 클릭 | 배경 클릭으로 닫기 |
|------|-------------|---------------|-------------------|
| **Background Raycast Target 끄기** | ✅ 가능 | ✅ 가능 | ❌ 불가 (ESC 키 사용) |
| **Background에 Button + CloseMasterUI** | ❌ 불가 | ✅ 가능 | ✅ 가능 |
| **Background를 여러 Panel로 분할** | ✅ 가능 | ✅ 가능 | ⚠ 일부 영역만 |

**권장**: **방법 1 (Raycast Target 끄기)**
- Hotbar와 Inventory 모두 자유롭게 사용 가능
- ESC 키로 닫기 (이미 구현됨)

---

## 🛠 자동 수정 스크립트

Unity에서 실행할 수 있는 간단한 스크립트:

```csharp
using UnityEngine;
using UnityEngine.UI;

public class FixMasterPanelBackground : MonoBehaviour
{
    [ContextMenu("Fix UI_MasterPanel Background")]
    void FixBackground()
    {
        // UI_MasterPanel 찾기
        GameObject masterPanel = GameObject.Find("UI_MasterPanel");
        if (masterPanel == null)
        {
            Debug.LogError("UI_MasterPanel not found!");
            return;
        }

        // Background 찾기
        Transform background = masterPanel.transform.Find("Background");
        if (background == null)
        {
            Debug.LogError("Background not found in UI_MasterPanel!");
            return;
        }

        // Raycast Target 비활성화
        Image bgImage = background.GetComponent<Image>();
        if (bgImage != null)
        {
            bgImage.raycastTarget = false;
            Debug.Log("✅ Background Raycast Target disabled!");
        }
        else
        {
            Debug.LogError("Background has no Image component!");
        }
    }
}
```

---

**작성일**: 2025-11-11
**관련 파일**: UI_MasterPanel Prefab
