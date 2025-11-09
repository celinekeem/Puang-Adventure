using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Basic Item")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemID;           // Unique identifier for saving/loading
    public string itemName;
    public Sprite sprite;

    [Header("Stacking")]
    public bool isStackable = true;
    public int maxStackSize = 99;

    [HideInInspector]
    public int stackCount = 1;      // Runtime stack count (not serialized in ScriptableObject)

    /// <summary>
    /// Create a runtime copy of this ItemData
    /// </summary>
    public ItemData CreateRuntimeCopy()
    {
        ItemData copy = CreateInstance<ItemData>();
        copy.itemID = this.itemID;
        copy.itemName = this.itemName;
        copy.sprite = this.sprite;
        copy.isStackable = this.isStackable;
        copy.maxStackSize = this.maxStackSize;
        copy.stackCount = 1;
        return copy;
    }

    /// <summary>
    /// Copy properties from another ItemData
    /// </summary>
    public void CopyFrom(ItemData other)
    {
        this.itemID = other.itemID;
        this.itemName = other.itemName;
        this.sprite = other.sprite;
        this.isStackable = other.isStackable;
        this.maxStackSize = other.maxStackSize;
    }
}
