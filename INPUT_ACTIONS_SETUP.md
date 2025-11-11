# ⌨ Input Actions 설정 가이드

## 개요
새로운 UI 시스템은 Unity Input System을 사용하여 Tab, ESC, I 키를 처리합니다.
`UIControls.inputactions` 파일에 다음 액션들을 추가해야 합니다.

---

## 필요한 액션 목록

| 액션 이름 | 키 | 기능 |
|----------|-----|------|
| `ToggleMasterUI` | **Tab** | Master UI 열기/닫기 |
| `CloseMasterUI` | **Escape** | Master UI 닫기 (열려있을 때만) |
| `InventoryToggle` | **I** | Inventory 탭 직접 열기 |

---

## Unity Input Actions 설정 방법

### Step 1: UIControls.inputactions 파일 열기
1. Project 창에서 `Assets/UIControls.inputactions` 파일 찾기
2. 더블클릭하여 Input Actions 에디터 열기

### Step 2: UI Action Map 확인/생성
1. 왼쪽 패널에서 **"UI"** Action Map이 있는지 확인
2. 없다면 `+` 버튼 클릭 → **Add Action Map** → 이름: `UI`

### Step 3: 액션 추가

#### 3-1. ToggleMasterUI 액션 추가
1. `UI` Action Map 선택
2. **Actions** 섹션에서 `+` 클릭 → **Add Action**
3. 이름: `ToggleMasterUI`
4. Action Type: `Button`
5. **Binding 추가**:
   - `+` 클릭 → **Add Binding**
   - Path: **Keyboard → Tab**

#### 3-2. CloseMasterUI 액션 추가
1. `UI` Action Map 선택
2. **Actions** 섹션에서 `+` 클릭 → **Add Action**
3. 이름: `CloseMasterUI`
4. Action Type: `Button`
5. **Binding 추가**:
   - `+` 클릭 → **Add Binding**
   - Path: **Keyboard → Escape**

#### 3-3. InventoryToggle 액션 확인/추가
1. `UI` Action Map에서 `InventoryToggle` 액션이 이미 있는지 확인
2. **있으면**: 그대로 사용 (키 바인딩이 **I**인지만 확인)
3. **없으면**:
   - **Add Action** → 이름: `InventoryToggle`
   - Action Type: `Button`
   - Binding: **Keyboard → I**

### Step 4: C# 클래스 생성
1. Input Actions 에디터 상단의 **"Generate C# Class"** 버튼 클릭
2. Class Name: `UIControls` (기본값 그대로)
3. Class Namespace: (비워두거나 프로젝트 네임스페이스 입력)
4. Output File Path: 자동으로 설정됨 (`UIControls.cs`)
5. **Apply** 클릭
6. **저장** (Ctrl+S)

### Step 5: Unity 재컴파일 대기
1. Unity Console에서 스크립트 컴파일이 완료될 때까지 대기
2. 에러가 없는지 확인

---

## JSON 형식 참고 (수동 편집 시)

`UIControls.inputactions` 파일을 텍스트 에디터로 직접 편집하려면 다음과 같이 추가하세요:

```json
{
    "name": "UIControls",
    "maps": [
        {
            "name": "UI",
            "id": "unique-guid-here",
            "actions": [
                {
                    "name": "ToggleMasterUI",
                    "type": "Button",
                    "id": "unique-guid-here",
                    "expectedControlType": "Button",
                    "processors": "",
                    "interactions": "",
                    "initialStateCheck": false
                },
                {
                    "name": "CloseMasterUI",
                    "type": "Button",
                    "id": "unique-guid-here",
                    "expectedControlType": "Button",
                    "processors": "",
                    "interactions": "",
                    "initialStateCheck": false
                },
                {
                    "name": "InventoryToggle",
                    "type": "Button",
                    "id": "unique-guid-here",
                    "expectedControlType": "Button",
                    "processors": "",
                    "interactions": "",
                    "initialStateCheck": false
                }
            ],
            "bindings": [
                {
                    "name": "",
                    "id": "unique-guid-here",
                    "path": "<Keyboard>/tab",
                    "interactions": "",
                    "processors": "",
                    "groups": "",
                    "action": "ToggleMasterUI",
                    "isComposite": false,
                    "isPartOfComposite": false
                },
                {
                    "name": "",
                    "id": "unique-guid-here",
                    "path": "<Keyboard>/escape",
                    "interactions": "",
                    "processors": "",
                    "groups": "",
                    "action": "CloseMasterUI",
                    "isComposite": false,
                    "isPartOfComposite": false
                },
                {
                    "name": "",
                    "id": "unique-guid-here",
                    "path": "<Keyboard>/i",
                    "interactions": "",
                    "processors": "",
                    "groups": "",
                    "action": "InventoryToggle",
                    "isComposite": false,
                    "isPartOfComposite": false
                }
            ]
        }
    ],
    "controlSchemes": []
}
```

**주의**: GUID는 Unity가 자동 생성하므로, 위 예시의 `"unique-guid-here"`는 실제 GUID로 대체되어야 합니다.

---

## 코드에서 사용 방법

### UI_MasterController.cs에서의 사용 예시:

```csharp
private UIControls controls;

private void Awake()
{
    controls = new UIControls();

    // Tab 키 바인딩
    controls.UI.ToggleMasterUI.performed += _ => ToggleMasterUI();

    // ESC 키 바인딩
    controls.UI.CloseMasterUI.performed += _ => CloseMasterUI();

    // I 키 바인딩
    controls.UI.InventoryToggle.performed += _ => OpenInventoryDirect();
}

private void OnEnable()
{
    controls?.UI.Enable();
}

private void OnDisable()
{
    controls?.UI.Disable();
}
```

---

## 테스트 방법

### Unity 에디터에서 테스트
1. **Play 모드** 진입
2. **Tab 키** 눌러서 Master UI가 열리는지 확인
3. **Tab 키** 다시 눌러서 닫히는지 확인
4. **I 키** 눌러서 Inventory가 열리는지 확인
5. **ESC 키** 눌러서 닫히는지 확인

### Input System Debugger로 확인
1. **Window → Analysis → Input Debugger** 열기
2. Play 모드 진입
3. **Users → Player** 확인
4. Tab/ESC/I 키를 눌렀을 때 해당 액션이 감지되는지 확인

---

## 문제 해결

### 문제 1: "UIControls does not contain a definition for 'UI'"
**원인**: C# 클래스가 생성되지 않았거나 컴파일 오류
**해결**:
1. UIControls.inputactions 열기
2. **Generate C# Class** 클릭
3. Unity 재시작

### 문제 2: "키 입력이 반응하지 않음"
**원인**: Input System이 제대로 활성화되지 않음
**해결**:
1. `OnEnable()`에서 `controls.UI.Enable()` 호출 확인
2. `OnDisable()`에서 `controls.UI.Disable()` 호출 확인
3. GameObject가 활성화되어 있는지 확인

### 문제 3: "Tab 키가 다른 UI 요소를 선택함"
**원인**: EventSystem의 기본 탭 내비게이션과 충돌
**해결**:
1. EventSystem의 **Send Navigation Events** 비활성화
2. 또는 UI_MasterController에서 `context.performed`로 변경:
   ```csharp
   controls.UI.ToggleMasterUI.performed += context => {
       if (context.performed) ToggleMasterUI();
   };
   ```

### 문제 4: "Input Actions Asset이 없음"
**원인**: UIControls.inputactions 파일이 프로젝트에 없음
**해결**:
1. **Assets → Create → Input Actions** 선택
2. 이름: `UIControls`
3. 위 가이드에 따라 액션 추가

---

## 추가 키 바인딩 (선택 사항)

게임패드 지원을 추가하려면:

| 액션 | 게임패드 버튼 |
|------|--------------|
| ToggleMasterUI | **Start** 버튼 |
| CloseMasterUI | **B** 버튼 (Xbox) / **Circle** (PS) |
| InventoryToggle | **Y** 버튼 (Xbox) / **Triangle** (PS) |

### 게임패드 바인딩 추가 방법:
1. 각 액션 선택
2. **Binding 추가** (+) 클릭
3. Path 선택:
   - ToggleMasterUI: `<Gamepad>/start`
   - CloseMasterUI: `<Gamepad>/buttonEast`
   - InventoryToggle: `<Gamepad>/buttonNorth`

---

**작성일**: 2025-11-11
**버전**: 1.0
**작성자**: Claude Code
