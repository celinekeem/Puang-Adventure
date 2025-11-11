using UnityEngine;

public class Hotbar : MonoBehaviour
{
    public static Hotbar instance;

    [Header("Slots Parent (Grid Layout Object)")]
    public Transform slotsParent;

    private ItemSlot[] slots;

    void Awake()
    {
        instance = this;

        // 슬롯 초기화 (Awake 단계에서 미리 잡기)
        if (slotsParent != null)
            slots = slotsParent.GetComponentsInChildren<ItemSlot>();

        // 각 슬롯에 올바른 index 부여 및 SlotType 설정
        if (slots != null)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].index = i; // Hotbar는 항상 0부터 시작
                slots[i].slotType = SlotType.Hotbar;
            }
        }
    }

    void Start()
    {
        // Inventory가 아직 초기화 안 됐으면 동기화 시도
        if (Inventory.instance != null && (Inventory.instance.items == null || Inventory.instance.items.Length < SlotCount))
        {
            Inventory.instance.Initialize(Inventory.instance.capacity);
        }

        UpdateUI();
    }

    public int SlotCount
    {
        get
        {
            if (slots != null) return slots.Length;
            if (slotsParent != null) return slotsParent.childCount;
            return 0;
        }
    }

    /// <summary>
    /// Hotbar UI 업데이트 — Inventory 데이터 기준으로 슬롯 갱신
    /// </summary>
    public void UpdateUI()
    {
        Debug.Log($"[Hotbar] 🔄 UpdateUI called");

        if (slots == null)
        {
            if (slotsParent != null)
            {
                slots = slotsParent.GetComponentsInChildren<ItemSlot>();
                // Set SlotType when lazy-initializing
                for (int i = 0; i < slots.Length; i++)
                {
                    slots[i].slotType = SlotType.Hotbar;
                }
            }
            else
            {
                Debug.LogWarning("[Hotbar] ⚠ UpdateUI: slotsParent not assigned.");
                return;
            }
        }

        if (Inventory.instance == null)
        {
            Debug.LogWarning("[Hotbar] ⚠ UpdateUI: Inventory.instance is null.");
            return;
        }

        // Inventory 데이터 크기 확인
        if (Inventory.instance.items == null || Inventory.instance.items.Length == 0)
        {
            Inventory.instance.Initialize(Inventory.instance.capacity);
        }

        Debug.Log($"[Hotbar] Updating {slots.Length} slots");

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].index = i; // 인덱스 동기화 (중요!)
            slots[i].slotType = SlotType.Hotbar; // SlotType 동기화
            if (Inventory.instance.items != null &&
                i < Inventory.instance.items.Length &&
                Inventory.instance.items[i] != null)
            {
                slots[i].AddItem(Inventory.instance.items[i]);
                Debug.Log($"[Hotbar]   Slot {i}: {Inventory.instance.items[i].itemName}");
            }
            else
            {
                slots[i].ClearSlot();
                Debug.Log($"[Hotbar]   Slot {i}: Empty");
            }
        }
        Debug.Log($"[Hotbar] ✅ UpdateUI complete");
    }

    /// <summary>
    /// 비어 있는 첫 슬롯 인덱스 반환 (없으면 -1)
    /// </summary>
    public int GetFirstEmptyIndex()
    {
        if (slots == null)
        {
            if (slotsParent != null)
                slots = slotsParent.GetComponentsInChildren<ItemSlot>();
            else
                return -1;
        }

        if (Inventory.instance == null || Inventory.instance.items == null)
            return -1;

        int limit = Mathf.Min(slots.Length, Inventory.instance.items.Length);

        for (int i = 0; i < limit; i++)
        {
            if (Inventory.instance.items[i] == null)
                return i;
        }
        return -1;
    }
}
