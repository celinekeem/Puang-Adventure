using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles inventory UI toggling via input.
/// Automatically closes inventory when scene changes.
/// </summary>
public class InventoryToggle : MonoBehaviour
{
    [SerializeField] private GameObject inventoryUI;
    private bool isOpen;
    private UIControls controls;

    private void Awake()
    {
        controls = new UIControls();
        controls.UI.InventoryToggle.performed += _ => ToggleInventory();

        // Subscribe to scene loading to close inventory on scene transition
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe from scene events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnEnable()
    {
        if (controls != null)
            controls.UI.Enable();
    }

    private void OnDisable()
    {
        if (controls != null)
            controls.UI.Disable();
    }

    private void Start()
    {
        // Ensure inventory is closed on start
        CloseInventory();
    }

    /// <summary>
    /// Called when a new scene is loaded - ensures inventory is closed
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find the inventory UI in the new scene if reference is lost
        if (inventoryUI == null)
        {
            // GameObject.Find only works on active objects, so we need to search all transforms
            Canvas[] allCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);

            foreach (Canvas canvas in allCanvases)
            {
                // Search for InventoryPanel in Canvas_UI children (even if inactive)
                Transform canvasTransform = canvas.transform;

                // Try common naming patterns
                Transform inventoryPanel = canvasTransform.Find("InventoryPanel");
                if (inventoryPanel == null)
                    inventoryPanel = canvasTransform.Find("Inventory_Panel");
                if (inventoryPanel == null)
                    inventoryPanel = canvasTransform.Find("Panel_Inventory");
                if (inventoryPanel == null)
                    inventoryPanel = canvasTransform.Find("InventoryUI");

                if (inventoryPanel != null)
                {
                    inventoryUI = inventoryPanel.gameObject;
                    Debug.Log($"✅ InventoryToggle: Found Inventory UI '{inventoryPanel.name}' in scene '{scene.name}' (Canvas: '{canvas.name}')");
                    break;
                }
            }

            if (inventoryUI == null)
            {
                Debug.LogWarning($"⚠ InventoryToggle: Could not find Inventory UI in scene '{scene.name}'. Searched all Canvas objects.");
            }
        }

        CloseInventory();
        Debug.Log($"🔄 InventoryToggle: Inventory closed on scene load '{scene.name}'");
    }

    private void ToggleInventory()
    {
        if (isOpen)
            CloseInventory();
        else
            OpenInventory();
    }

    /// <summary>
    /// Open the inventory UI
    /// </summary>
    private void OpenInventory()
    {
        isOpen = true;
        if (inventoryUI != null)
            inventoryUI.SetActive(true);
        Time.timeScale = 0;
        Debug.Log("📦 InventoryToggle: Inventory opened");
    }

    /// <summary>
    /// Close the inventory UI
    /// </summary>
    private void CloseInventory()
    {
        isOpen = false;
        if (inventoryUI != null)
            inventoryUI.SetActive(false);
        Time.timeScale = 1;
    }
}
