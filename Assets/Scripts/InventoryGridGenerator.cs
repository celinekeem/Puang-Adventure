using UnityEngine;

public class InventoryGridGenerator : MonoBehaviour
{
    [Header("슬롯 관련 설정")]
    public GameObject slotPrefab;   // Slot 프리팹
    public int slotCount = 20;      // 생성할 슬롯 개수

    [Header("슬롯 배치 위치")]
    public Transform slotParent;    // SlotContainer (Grid Layout Group이 붙은 오브젝트)

    [Header("옵션")]
    [Tooltip("이미 슬롯이 존재하면 GenerateSlots는 아무 작업도 하지 않습니다. 강제 재생성하려면 ForceGenerateSlots() 호출")]
    public bool generateOnStart = true;

    void Start()
    {
        if (generateOnStart) GenerateSlots();
    }

    // 기본: 이미 슬롯이 존재하면 아무 작업 안함.
    public void GenerateSlots()
    {
        if (slotPrefab == null || slotParent == null)
        {
            Debug.LogWarning("InventoryGridGenerator: slotPrefab or slotParent not assigned.");
            return;
        }

        if (slotParent.childCount > 0)
        {
            // 이미 생성된 슬롯이 있으면 유지
            Debug.Log("InventoryGridGenerator: slots already exist - skip generation");
            return;
        }

        for (int i = 0; i < slotCount; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotParent);
            newSlot.name = $"Slot_{i + 1}";
        }
    }

    // 필요 시 슬롯을 완전히 재생성하고 싶다면 이 메서드 호출
    public void ForceGenerateSlots()
    {
        if (slotPrefab == null || slotParent == null) return;

        // 기존 슬롯 삭제
        for (int i = slotParent.childCount - 1; i >= 0; i--)
        {
            var c = slotParent.GetChild(i);
#if UNITY_EDITOR
            DestroyImmediate(c.gameObject);
#else
            Destroy(c.gameObject);
#endif
        }

        // 새로 생성
        for (int i = 0; i < slotCount; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotParent);
            newSlot.name = $"Slot_{i + 1}";
        }
    }
}