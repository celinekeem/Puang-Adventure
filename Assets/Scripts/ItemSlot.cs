using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Enhanced slot system supporting drag & drop between Inventory and Hotbar
/// Supports TimeScale=0 and world item dropping
/// </summary>
public class ItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("Slot Configuration")]
    public Image icon;
    public SlotType slotType = SlotType.Inventory;

    [HideInInspector]
    public int index;

    private ItemData currentItem;

    // Static drag state (shared across all slots)
    private static GameObject dragIcon;
    private static ItemSlot dragSourceSlot;
    private static ItemData draggingItem;
    private static SlotType dragSourceType;
    private static bool isDraggingOutsideUI;
    private static bool dropHandled; // 🔹 Flag to prevent OnEndDrag from restoring after successful drop

    [Header("Debug")]
    public bool showDebugLogs = true;

    #region Slot Management

    /// <summary>
    /// Add an item to this slot
    /// </summary>
    public void AddItem(ItemData newItem)
    {
        if (newItem == null)
        {
            ClearSlot();
            return;
        }

        currentItem = newItem;
        if (icon != null)
        {
            icon.sprite = newItem.sprite;
            icon.enabled = true;
        }

        if (showDebugLogs)
            Debug.Log($"[ItemSlot] Added '{newItem.itemName}' to {slotType} slot {index}");
    }

    /// <summary>
    /// Clear this slot
    /// </summary>
    public void ClearSlot()
    {
        currentItem = null;
        if (icon != null)
        {
            icon.sprite = null;
            icon.enabled = false;
        }
    }

    /// <summary>
    /// Get the item in this slot
    /// </summary>
    public ItemData GetItem() => currentItem;

    /// <summary>
    /// Check if this slot is empty
    /// </summary>
    public bool IsEmpty() => currentItem == null;

    #endregion

    #region Drag & Drop Implementation

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null)
        {
            if (showDebugLogs)
                Debug.Log($"[ItemSlot] OnBeginDrag: Slot {index} is empty, cannot drag");
            return;
        }

        // Store drag source information
        dragSourceSlot = this;
        draggingItem = currentItem;
        dragSourceType = slotType;
        dropHandled = false; // 🔹 Reset flag at start of drag
        isDraggingOutsideUI = false;

        if (showDebugLogs)
            Debug.Log($"[ItemSlot] OnBeginDrag: Started dragging '{currentItem.itemName}' from {slotType} slot {index}");

        // Create drag icon
        CreateDragIcon();

        // Visually clear the source slot (data remains until drop)
        if (icon != null)
            icon.enabled = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon == null || draggingItem == null) return;

        // Move drag icon with cursor (works with TimeScale=0)
        UpdateDragIconPosition(eventData);

        // Check if dragging outside any UI element
        CheckIfDraggingOutsideUI(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (showDebugLogs)
            Debug.Log($"[ItemSlot] OnEndDrag: Ending drag for '{draggingItem?.itemName}' (dropHandled={dropHandled})");

        // Destroy drag icon
        if (dragIcon != null)
        {
            Destroy(dragIcon);
            dragIcon = null;
        }

        if (draggingItem == null || dragSourceSlot == null)
        {
            if (showDebugLogs)
                Debug.LogWarning("[ItemSlot] OnEndDrag: No dragging item or source slot");
            return;
        }

        // 🔹 If OnDrop already handled this, skip restoration logic
        if (dropHandled)
        {
            if (showDebugLogs)
                Debug.Log($"[ItemSlot] OnEndDrag: Drop already handled by OnDrop, skipping");
        }
        // Check if dropped outside UI
        else if (isDraggingOutsideUI || !IsPointerOverUI(eventData))
        {
            if (showDebugLogs)
                Debug.Log($"[ItemSlot] OnEndDrag: Dropped outside UI, spawning in world");

            DropItemToWorld();
        }
        else
        {
            // If not dropped on any slot, restore source slot
            if (showDebugLogs)
                Debug.Log($"[ItemSlot] OnEndDrag: Not dropped on valid slot, restoring source");

            RestoreSourceSlot();
        }

        // Cleanup
        draggingItem = null;
        dragSourceSlot = null;
        isDraggingOutsideUI = false;
        dropHandled = false;

        // Refresh UI
        RefreshAllUI();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (draggingItem == null || dragSourceSlot == null)
        {
            if (showDebugLogs)
                Debug.LogWarning("[ItemSlot] OnDrop: No dragging item");
            return;
        }

        if (showDebugLogs)
            Debug.Log($"[ItemSlot] 🎯 OnDrop: Dropped '{draggingItem.itemName}' on {slotType} slot {index} (Current item: {(currentItem != null ? currentItem.itemName : "Empty")})");

        // Prevent dropping outside UI flag
        isDraggingOutsideUI = false;

        // Handle the drop based on slot types
        HandleSlotDrop(dragSourceSlot, this);

        // 🔹 Mark drop as handled to prevent OnEndDrag from restoring
        dropHandled = true;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Create the drag icon that follows the cursor
    /// </summary>
    private void CreateDragIcon()
    {
        // Find the highest sort order canvas for the drag icon
        Canvas targetCanvas = GetHighestSortOrderCanvas();
        if (targetCanvas == null)
        {
            Debug.LogError("[ItemSlot] CreateDragIcon: No Canvas found!");
            return;
        }

        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(targetCanvas.transform, false);

        Image img = dragIcon.AddComponent<Image>();
        img.sprite = draggingItem.sprite;
        img.raycastTarget = false; // Don't block raycasts

        RectTransform rt = dragIcon.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50, 50);

        // Set high sort order to be on top
        Canvas dragCanvas = dragIcon.AddComponent<Canvas>();
        dragCanvas.overrideSorting = true;
        dragCanvas.sortingOrder = 1000;

        if (showDebugLogs)
            Debug.Log($"[ItemSlot] Created drag icon on canvas '{targetCanvas.name}'");
    }

    /// <summary>
    /// Update drag icon position to follow cursor
    /// </summary>
    private void UpdateDragIconPosition(PointerEventData eventData)
    {
        if (dragIcon == null) return;

        RectTransform parentRect = dragIcon.transform.parent as RectTransform;
        if (parentRect != null)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint
            );
            dragIcon.transform.localPosition = localPoint;
        }
    }

    /// <summary>
    /// Check if pointer is over any UI element
    /// </summary>
    private bool IsPointerOverUI(PointerEventData eventData)
    {
        return eventData.pointerEnter != null &&
               eventData.pointerEnter.GetComponent<ItemSlot>() != null;
    }

    /// <summary>
    /// Check if currently dragging outside any UI bounds
    /// </summary>
    private void CheckIfDraggingOutsideUI(PointerEventData eventData)
    {
        // Check if pointer is over any UI element
        if (eventData.pointerEnter == null)
        {
            isDraggingOutsideUI = true;
        }
    }

    /// <summary>
    /// Get the canvas with highest sort order for drag icon
    /// </summary>
    private Canvas GetHighestSortOrderCanvas()
    {
        Canvas myCanvas = GetComponentInParent<Canvas>();
        if (myCanvas != null)
            return myCanvas;

        // Fallback: find any canvas
        return FindAnyObjectByType<Canvas>();
    }

    /// <summary>
    /// Handle dropping item from source to target slot
    /// </summary>
    private void HandleSlotDrop(ItemSlot source, ItemSlot target)
    {
        if (source == null || target == null || source == target)
        {
            if (showDebugLogs && source == target)
                Debug.Log("[ItemSlot] Dropped on same slot, no action needed");
            return;
        }

        if (showDebugLogs)
        {
            Debug.Log($"[ItemSlot] 🔄 BEFORE SWAP:");
            Debug.Log($"  Source: {source.slotType}[{source.index}] = {(Inventory.instance?.items[source.index]?.itemName ?? "null")}");
            Debug.Log($"  Target: {target.slotType}[{target.index}] = {(Inventory.instance?.items[target.index]?.itemName ?? "null")}");
        }

        // Use Inventory's swap method (handles both Hotbar and Inventory)
        if (Inventory.instance != null)
        {
            Inventory.instance.SwapItems(source.index, target.index);

            if (showDebugLogs)
            {
                Debug.Log($"[ItemSlot] 🔄 AFTER SWAP:");
                Debug.Log($"  Source: {source.slotType}[{source.index}] = {(Inventory.instance?.items[source.index]?.itemName ?? "null")}");
                Debug.Log($"  Target: {target.slotType}[{target.index}] = {(Inventory.instance?.items[target.index]?.itemName ?? "null")}");
            }
        }
        else
        {
            Debug.LogError("[ItemSlot] Inventory.instance is null!");
        }
    }

    /// <summary>
    /// Restore the source slot when drag is cancelled
    /// </summary>
    private void RestoreSourceSlot()
    {
        if (dragSourceSlot != null && dragSourceSlot.icon != null)
        {
            dragSourceSlot.icon.enabled = true;
        }
    }

    /// <summary>
    /// Drop item to world at player position
    /// </summary>
    private void DropItemToWorld()
    {
        if (dragSourceSlot == null || draggingItem == null)
        {
            Debug.LogError("[ItemSlot] DropItemToWorld: Missing source or item");
            return;
        }

        if (showDebugLogs)
            Debug.Log($"[ItemSlot] Dropping '{draggingItem.itemName}' to world from slot {dragSourceSlot.index}");

        // Use ItemWorldSpawner to spawn the item
        if (ItemWorldSpawner.Instance != null)
        {
            ItemWorldSpawner.Instance.SpawnItemAtPlayer(draggingItem);
        }
        else
        {
            // Fallback to Inventory's drop method
            if (Inventory.instance != null)
            {
                Inventory.instance.DropItemToWorld(dragSourceSlot.index);
                return; // DropItemToWorld already removes item and refreshes UI
            }
            else
            {
                Debug.LogError("[ItemSlot] Neither ItemWorldSpawner nor Inventory available!");
                RestoreSourceSlot();
                return;
            }
        }

        // Remove item from inventory
        if (Inventory.instance != null)
        {
            Inventory.instance.RemoveItemAt(dragSourceSlot.index);
        }
    }

    /// <summary>
    /// Refresh all UI (Inventory + Hotbar)
    /// </summary>
    private void RefreshAllUI()
    {
        if (showDebugLogs)
            Debug.Log($"[ItemSlot] 🔄 RefreshAllUI called");

        if (Inventory.instance != null)
        {
            // 🔹 Auto-find InventoryUI if null
            if (Inventory.instance.inventoryUI == null)
            {
                Inventory.instance.RefreshUIReferences();
                if (showDebugLogs)
                    Debug.Log($"[ItemSlot] 🔍 Auto-found InventoryUI via RefreshUIReferences()");
            }

            if (Inventory.instance.inventoryUI != null)
            {
                Inventory.instance.inventoryUI.UpdateUI();
                if (showDebugLogs)
                    Debug.Log($"[ItemSlot] ✅ InventoryUI.UpdateUI() called");
            }
            else
            {
                if (showDebugLogs)
                    Debug.LogWarning($"[ItemSlot] ⚠ Cannot refresh InventoryUI (inventoryUI is still null after RefreshUIReferences)");
            }
        }

        if (Hotbar.instance != null)
        {
            Hotbar.instance.UpdateUI();
            if (showDebugLogs)
                Debug.Log($"[ItemSlot] ✅ Hotbar.UpdateUI() called");
        }
        else
        {
            if (showDebugLogs)
                Debug.LogWarning($"[ItemSlot] ⚠ Hotbar.instance is null");
        }
    }

    #endregion
}
