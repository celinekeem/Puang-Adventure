using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages global inventory data across scenes.
/// Stores items, stacks, and slot states for the entire game.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Inventory Settings")]
    [SerializeField] private int maxSlots = 20;

    [Header("Current Inventory Data")]
    private List<ItemData> items = new List<ItemData>();

    [Header("Hotbar Settings")]
    [Tooltip("Number of hotbar slots (usually 5-10)")]
    [SerializeField] private int hotbarSlotCount = 5;
    private int selectedHotbarIndex = 0;

    private void Awake()
    {
        // Singleton pattern with DontDestroyOnLoad
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeInventory();
            Debug.Log("✅ InventoryManager: Initialized with " + maxSlots + " slots");
        }
        else
        {
            Debug.LogWarning("⚠ InventoryManager: Duplicate instance detected - destroying");
            Destroy(gameObject);
        }
    }

    private void InitializeInventory()
    {
        // Initialize empty inventory
        items = new List<ItemData>(maxSlots);
        for (int i = 0; i < maxSlots; i++)
        {
            items.Add(null); // Empty slots
        }
    }

    #region Item Management

    /// <summary>
    /// Add an item to the inventory. Returns true if successful.
    /// </summary>
    public bool AddItem(ItemData item, int quantity = 1)
    {
        if (item == null)
        {
            Debug.LogWarning("⚠ InventoryManager: Tried to add null item");
            return false;
        }

        // Try to stack with existing items first
        if (item.isStackable)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null && items[i].itemID == item.itemID)
                {
                    items[i].stackCount += quantity;
                    Debug.Log($"📦 InventoryManager: Stacked {quantity}x {item.itemName} (now {items[i].stackCount})");
                    OnInventoryChanged();
                    return true;
                }
            }
        }

        // Find first empty slot
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                ItemData newItem = item.CreateRuntimeCopy();
                newItem.stackCount = quantity;
                items[i] = newItem;

                Debug.Log($"📦 InventoryManager: Added {quantity}x {item.itemName} to slot {i}");
                OnInventoryChanged();
                return true;
            }
        }

        Debug.LogWarning("⚠ InventoryManager: Inventory full - cannot add item");
        return false;
    }

    /// <summary>
    /// Remove an item from a specific slot
    /// </summary>
    public bool RemoveItem(int slotIndex, int quantity = 1)
    {
        if (slotIndex < 0 || slotIndex >= items.Count)
        {
            Debug.LogWarning($"⚠ InventoryManager: Invalid slot index {slotIndex}");
            return false;
        }

        if (items[slotIndex] == null)
        {
            Debug.LogWarning($"⚠ InventoryManager: Slot {slotIndex} is empty");
            return false;
        }

        items[slotIndex].stackCount -= quantity;

        if (items[slotIndex].stackCount <= 0)
        {
            Debug.Log($"🗑 InventoryManager: Removed {items[slotIndex].itemName} from slot {slotIndex}");
            items[slotIndex] = null;
        }
        else
        {
            Debug.Log($"📦 InventoryManager: Reduced stack to {items[slotIndex].stackCount}");
        }

        OnInventoryChanged();
        return true;
    }

    /// <summary>
    /// Get item at a specific slot
    /// </summary>
    public ItemData GetItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Count) return null;
        return items[slotIndex];
    }

    /// <summary>
    /// Check if inventory contains a specific item
    /// </summary>
    public bool HasItem(string itemID, int requiredQuantity = 1)
    {
        int totalCount = 0;
        foreach (var item in items)
        {
            if (item != null && item.itemID == itemID)
            {
                totalCount += item.stackCount;
            }
        }
        return totalCount >= requiredQuantity;
    }

    /// <summary>
    /// Get total count of a specific item
    /// </summary>
    public int GetItemCount(string itemID)
    {
        int count = 0;
        foreach (var item in items)
        {
            if (item != null && item.itemID == itemID)
            {
                count += item.stackCount;
            }
        }
        return count;
    }

    /// <summary>
    /// Clear all items from inventory
    /// </summary>
    public void ClearInventory()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i] = null;
        }
        Debug.Log("🗑 InventoryManager: Inventory cleared");
        OnInventoryChanged();
    }

    #endregion

    #region Hotbar Management

    /// <summary>
    /// Get the currently selected hotbar item
    /// </summary>
    public ItemData GetSelectedHotbarItem()
    {
        if (selectedHotbarIndex < 0 || selectedHotbarIndex >= hotbarSlotCount)
            return null;

        return GetItem(selectedHotbarIndex);
    }

    /// <summary>
    /// Select a hotbar slot (0-based index)
    /// </summary>
    public void SelectHotbarSlot(int index)
    {
        if (index < 0 || index >= hotbarSlotCount)
        {
            Debug.LogWarning($"⚠ InventoryManager: Invalid hotbar index {index}");
            return;
        }

        selectedHotbarIndex = index;
        Debug.Log($"🎯 InventoryManager: Selected hotbar slot {index}");
        OnHotbarSelectionChanged();
    }

    /// <summary>
    /// Get the currently selected hotbar index
    /// </summary>
    public int GetSelectedHotbarIndex()
    {
        return selectedHotbarIndex;
    }

    #endregion

    #region Events & Callbacks

    /// <summary>
    /// Called when inventory changes - can be used to refresh UI
    /// </summary>
    private void OnInventoryChanged()
    {
        // Broadcast to UI or other systems
        // Example: InventoryUI.Refresh();
    }

    /// <summary>
    /// Called when hotbar selection changes
    /// </summary>
    private void OnHotbarSelectionChanged()
    {
        // Update hotbar UI highlight
    }

    #endregion

    #region Save/Load Support

    /// <summary>
    /// Get serializable inventory data for saving
    /// </summary>
    public InventorySaveData GetSaveData()
    {
        InventorySaveData data = new InventorySaveData();
        data.itemIDs = new List<string>();
        data.stackCounts = new List<int>();

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null)
            {
                data.itemIDs.Add(items[i].itemID);
                data.stackCounts.Add(items[i].stackCount);
            }
            else
            {
                data.itemIDs.Add("");
                data.stackCounts.Add(0);
            }
        }

        data.selectedHotbarIndex = selectedHotbarIndex;
        return data;
    }

    /// <summary>
    /// Load inventory from save data
    /// </summary>
    public void LoadFromSaveData(InventorySaveData data)
    {
        if (data == null || data.itemIDs == null) return;

        ClearInventory();

        for (int i = 0; i < data.itemIDs.Count && i < maxSlots; i++)
        {
            if (string.IsNullOrEmpty(data.itemIDs[i])) continue;

            // TODO: Load actual ItemData from Resources or AssetDatabase
            // For now, create placeholder
            // ItemData loadedItem = Resources.Load<ItemData>("Items/" + data.itemIDs[i]);
            // if (loadedItem != null) items[i] = loadedItem;
        }

        selectedHotbarIndex = data.selectedHotbarIndex;
        OnInventoryChanged();
        Debug.Log("📂 InventoryManager: Loaded inventory from save data");
    }

    #endregion

#if UNITY_EDITOR
    [ContextMenu("Debug: Log Inventory")]
    private void DebugLogInventory()
    {
        Debug.Log("=== Inventory Contents ===");
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null)
            {
                Debug.Log($"Slot {i}: {items[i].itemName} x{items[i].stackCount}");
            }
        }
        Debug.Log($"Selected Hotbar: {selectedHotbarIndex}");
        Debug.Log("=========================");
    }

    [ContextMenu("Debug: Add Test Item")]
    private void DebugAddTestItem()
    {
        ItemData testItem = ScriptableObject.CreateInstance<ItemData>();
        testItem.itemID = "test_item";
        testItem.itemName = "Test Item";
        testItem.isStackable = true;
        testItem.maxStackSize = 99;
        AddItem(testItem, 5);
    }
#endif
}

/// <summary>
/// Serializable data structure for saving inventory
/// </summary>
[System.Serializable]
public class InventorySaveData
{
    public List<string> itemIDs;
    public List<int> stackCounts;
    public int selectedHotbarIndex;
}
