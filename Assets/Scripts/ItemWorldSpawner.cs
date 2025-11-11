using UnityEngine;

/// <summary>
/// Singleton manager for spawning items in the world when dropped from UI
/// Spawns items at player position (y-0.5)
/// </summary>
public class ItemWorldSpawner : MonoBehaviour
{
    public static ItemWorldSpawner Instance { get; private set; }

    [Header("Spawn Settings")]
    [Tooltip("Vertical offset from player position (negative = below player)")]
    public float spawnOffsetY = -0.5f;

    [Tooltip("Delay before item can be picked up again")]
    public float pickupIgnoreDuration = 0.5f;

    [Header("Fallback Prefab")]
    [Tooltip("Default prefab if ItemData doesn't have worldPrefab assigned")]
    public GameObject defaultWorldItemPrefab;

    [Header("Debug")]
    public bool showDebugLogs = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("✅ ItemWorldSpawner: Initialized");
        }
        else
        {
            Debug.LogWarning("⚠ ItemWorldSpawner: Duplicate instance destroyed");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Spawn an item in the world at player position + offset
    /// </summary>
    /// <param name="itemData">The item to spawn</param>
    /// <returns>The spawned GameObject, or null if failed</returns>
    public GameObject SpawnItemAtPlayer(ItemData itemData)
    {
        if (itemData == null)
        {
            Debug.LogError("[ItemWorldSpawner] Cannot spawn null ItemData");
            return null;
        }

        // Find player
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[ItemWorldSpawner] Player not found! Cannot spawn item.");
            return null;
        }

        // Calculate spawn position (player y-0.5)
        Vector3 spawnPosition = player.transform.position + new Vector3(0f, spawnOffsetY, 0f);

        if (showDebugLogs)
            Debug.Log($"[ItemWorldSpawner] Spawning '{itemData.itemName}' at {spawnPosition}");

        // Determine which prefab to use
        GameObject prefabToSpawn = GetPrefabForItem(itemData);
        if (prefabToSpawn == null)
        {
            Debug.LogError($"[ItemWorldSpawner] No prefab available for '{itemData.itemName}'");
            return null;
        }

        // Instantiate the item
        GameObject spawnedItem = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        // Configure the spawned item
        ConfigureSpawnedItem(spawnedItem, itemData);

        if (showDebugLogs)
            Debug.Log($"[ItemWorldSpawner] Successfully spawned '{itemData.itemName}'");

        return spawnedItem;
    }

    /// <summary>
    /// Get the appropriate prefab for an item
    /// Priority: ItemData.worldPrefab > defaultWorldItemPrefab > Inventory.itemWorldPrefab
    /// </summary>
    private GameObject GetPrefabForItem(ItemData itemData)
    {
        // First priority: ItemData's specific worldPrefab
        if (itemData.worldPrefab != null)
        {
            if (showDebugLogs)
                Debug.Log($"[ItemWorldSpawner] Using ItemData.worldPrefab for '{itemData.itemName}'");
            return itemData.worldPrefab;
        }

        // Second priority: Default prefab from this spawner
        if (defaultWorldItemPrefab != null)
        {
            if (showDebugLogs)
                Debug.Log($"[ItemWorldSpawner] Using defaultWorldItemPrefab for '{itemData.itemName}'");
            return defaultWorldItemPrefab;
        }

        // Third priority: Fallback to Inventory's prefab
        if (Inventory.instance != null && Inventory.instance.itemWorldPrefab != null)
        {
            if (showDebugLogs)
                Debug.Log($"[ItemWorldSpawner] Using Inventory.itemWorldPrefab for '{itemData.itemName}'");
            return Inventory.instance.itemWorldPrefab;
        }

        return null;
    }

    /// <summary>
    /// Configure the spawned item's components
    /// </summary>
    private void ConfigureSpawnedItem(GameObject spawnedItem, ItemData itemData)
    {
        if (spawnedItem == null) return;

        // Configure Rigidbody2D (disable gravity, zero velocity)
        Rigidbody2D rb = spawnedItem.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            if (showDebugLogs)
                Debug.Log("[ItemWorldSpawner] Configured Rigidbody2D");
        }

        // Configure SpriteRenderer (sorting layer)
        SpriteRenderer sr = spawnedItem.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingLayerName = "Player";
            sr.sortingOrder = 5;

            // Set sprite from ItemData
            if (itemData.sprite != null)
            {
                sr.sprite = itemData.sprite;
            }

            if (showDebugLogs)
                Debug.Log("[ItemWorldSpawner] Configured SpriteRenderer");
        }

        // Configure Item component (data + pickup delay)
        Item itemComponent = spawnedItem.GetComponent<Item>();
        if (itemComponent != null)
        {
            itemComponent.data = itemData;
            itemComponent.SetPickupDelay(pickupIgnoreDuration);

            if (showDebugLogs)
                Debug.Log($"[ItemWorldSpawner] Configured Item component with pickup delay {pickupIgnoreDuration}s");
        }
        else
        {
            Debug.LogWarning($"[ItemWorldSpawner] Spawned item '{spawnedItem.name}' has no Item component!");
        }
    }

    /// <summary>
    /// Spawn item at a specific position (for advanced use cases)
    /// </summary>
    public GameObject SpawnItemAtPosition(ItemData itemData, Vector3 position)
    {
        if (itemData == null)
        {
            Debug.LogError("[ItemWorldSpawner] Cannot spawn null ItemData");
            return null;
        }

        GameObject prefabToSpawn = GetPrefabForItem(itemData);
        if (prefabToSpawn == null)
        {
            Debug.LogError($"[ItemWorldSpawner] No prefab available for '{itemData.itemName}'");
            return null;
        }

        GameObject spawnedItem = Instantiate(prefabToSpawn, position, Quaternion.identity);
        ConfigureSpawnedItem(spawnedItem, itemData);

        if (showDebugLogs)
            Debug.Log($"[ItemWorldSpawner] Spawned '{itemData.itemName}' at custom position {position}");

        return spawnedItem;
    }
}
