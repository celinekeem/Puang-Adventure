using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform slotsParent;
    private ItemSlot[] slots;

    void Start()
    {
        // Initialize slots (also lazy-initialized in UpdateUI if needed).
        EnsureInitialized();

        // Ensure inventory capacity includes hotbar slots + inventory UI slots so indices map correctly.
        if (Inventory.instance != null && slots != null)
        {
            int invSlots = slots.Length;
            int hotSlots = Hotbar.instance != null ? Hotbar.instance.SlotCount : 0;
            int total = hotSlots + invSlots;
            if (Inventory.instance.capacity != total)
            {
                Inventory.instance.capacity = total;
                Inventory.instance.Initialize(total);
                Debug.Log($"InventoryUI: synced Inventory.capacity to {total} (hotbar {hotSlots} + inventory {invSlots})");
            }
            else
            {
                // ensure underlying array exists
                Inventory.instance.Initialize(total);
            }
        }

        UpdateUI();
    }

    // Ensure slots are populated from the parent transform. Called on Start and lazily in UpdateUI.
    private void EnsureInitialized()
    {
        if (slots == null)
        {
            if (slotsParent == null)
            {
                Debug.LogWarning("InventoryUI: slotsParent is not assigned. Cannot initialize slots.");
                return;
            }

            slots = slotsParent.GetComponentsInChildren<ItemSlot>();

            // Set SlotType for all inventory slots
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].slotType = SlotType.Inventory;
            }
        }
    }

    public void UpdateUI()
    {
        Debug.Log($"[InventoryUI] 🔄 UpdateUI called");
        EnsureInitialized();

        if (slots == null)
        {
            Debug.LogWarning("[InventoryUI] ⚠ UpdateUI: slots not initialized. Skipping UI update.");
            return;
        }

        if (Inventory.instance == null)
        {
            Debug.LogWarning("[InventoryUI] ⚠ UpdateUI: Inventory.instance is null. Skipping UI update.");
            return;
        }

        int hotOffset = Hotbar.instance != null ? Hotbar.instance.SlotCount : 0;
        Debug.Log($"[InventoryUI] Updating {slots.Length} slots (hotOffset={hotOffset})");

        for (int i = 0; i < slots.Length; i++)
        {
            // assign index for drag/drop operations (inventory indices start after hotbar slots)
            int idx = hotOffset + i;
            slots[i].index = idx;
            slots[i].slotType = SlotType.Inventory;

            if (Inventory.instance.items != null && idx < Inventory.instance.items.Length && Inventory.instance.items[idx] != null)
            {
                slots[i].AddItem(Inventory.instance.items[idx]);
                Debug.Log($"[InventoryUI]   Slot {i} (idx={idx}): {Inventory.instance.items[idx].itemName}");
            }
            else
            {
                slots[i].ClearSlot();
                Debug.Log($"[InventoryUI]   Slot {i} (idx={idx}): Empty");
            }
        }
        Debug.Log($"[InventoryUI] ✅ UpdateUI complete");
    }
}
