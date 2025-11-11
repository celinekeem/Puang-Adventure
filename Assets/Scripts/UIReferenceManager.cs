using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Central UI reference manager for the new UI structure.
/// Manages references to HUD_Canvas and UI_MasterPanel elements.
/// Singleton pattern ensures consistent access across all scripts.
///
/// NEW STRUCTURE:
/// - HUD_Canvas (Sort Order 0, Always Active)
///   └─ HPBar, STBar, DialoguePanel, Hotbar
/// - UI_MasterPanel (Sort Order 1, Tab/ESC Toggle)
///   └─ InventoryPanel, MapPanel, SettingsPanel, SavePanel
/// </summary>
public class UIReferenceManager : MonoBehaviour
{
    public static UIReferenceManager Instance { get; private set; }

    [Header("=== HUD Canvas References (Always Visible) ===")]
    [Tooltip("Main HUD Canvas - contains HP, Stamina, Hotbar, Dialogue")]
    public Canvas hudCanvas;

    [Tooltip("HP Slider in HUD_Canvas/HPBar")]
    public Slider hpSlider;

    [Tooltip("Stamina Slider in HUD_Canvas/STBar")]
    public Slider staminaSlider;

    [Tooltip("Hotbar parent transform (contains Hotbar slots)")]
    public Transform hotbarParent;

    [Tooltip("Dialogue Panel (optional)")]
    public GameObject dialoguePanel;

    [Header("=== Master Panel References (Tab/ESC Toggle) ===")]
    [Tooltip("Main Master Panel GameObject - parent of all tabbed panels")]
    public GameObject masterPanel;

    [Tooltip("Inventory Panel (contains SlotGrid)")]
    public GameObject inventoryPanel;

    [Tooltip("Inventory Slots Parent (SlotGrid)")]
    public Transform inventorySlotGrid;

    [Tooltip("Map Panel")]
    public GameObject mapPanel;

    [Tooltip("Settings Panel")]
    public GameObject settingsPanel;

    [Tooltip("Save Panel")]
    public GameObject savePanel;

    [Header("=== Top Buttons (Panel Switchers) ===")]
    [Tooltip("Button to switch to Map Panel")]
    public Button buttonMap;

    [Tooltip("Button to switch to Inventory Panel")]
    public Button buttonInventory;

    [Tooltip("Button to switch to Settings Panel")]
    public Button buttonSettings;

    [Tooltip("Button to switch to Save Panel")]
    public Button buttonSave;

    [Header("=== Settings Panel UI Elements ===")]
    [Tooltip("Brightness slider in Settings Panel")]
    public Slider sliderBrightness;

    [Tooltip("BGM volume slider in Settings Panel")]
    public Slider sliderBGM;

    [Tooltip("SFX volume slider in Settings Panel")]
    public Slider sliderSFX;

    [Header("=== Save Panel UI Elements ===")]
    [Tooltip("Save button in Save Panel")]
    public Button buttonSaveGame;

    [Tooltip("Load button in Save Panel")]
    public Button buttonLoadGame;

    [Header("=== Debug Settings ===")]
    [SerializeField] private bool showDebugLogs = true;
    [SerializeField] private bool autoFindReferences = true;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            LogDebug("✅ UIReferenceManager: Initialized as singleton");

            if (autoFindReferences)
            {
                AutoFindReferences();
            }

            ValidateReferences();
        }
        else
        {
            LogDebug("⚠ UIReferenceManager: Duplicate instance detected - destroying");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Automatically find UI references in the scene (useful for debugging)
    /// </summary>
    private void AutoFindReferences()
    {
        LogDebug("🔍 UIReferenceManager: Auto-finding UI references...");

        // Find HUD_Canvas
        if (hudCanvas == null)
        {
            GameObject hudCanvasObj = GameObject.Find("HUD_Canvas");
            if (hudCanvasObj != null)
            {
                hudCanvas = hudCanvasObj.GetComponent<Canvas>();
                LogDebug($"✅ Found HUD_Canvas: {hudCanvasObj.name}");
            }
        }

        // Find HPBar
        if (hpSlider == null)
        {
            GameObject hpBarObj = GameObject.Find("HUD_Canvas/HPBar");
            if (hpBarObj != null)
            {
                hpSlider = hpBarObj.GetComponent<Slider>();
                LogDebug($"✅ Found HPBar slider");
            }
            else
            {
                // Alternative search
                Slider[] sliders = FindObjectsByType<Slider>(FindObjectsSortMode.None);
                foreach (var slider in sliders)
                {
                    if (slider.name.Contains("HP") || slider.name.Contains("Health"))
                    {
                        hpSlider = slider;
                        LogDebug($"✅ Found HPBar slider (alternative): {slider.name}");
                        break;
                    }
                }
            }
        }

        // Find STBar
        if (staminaSlider == null)
        {
            GameObject stBarObj = GameObject.Find("HUD_Canvas/STBar");
            if (stBarObj != null)
            {
                staminaSlider = stBarObj.GetComponent<Slider>();
                LogDebug($"✅ Found STBar slider");
            }
            else
            {
                // Alternative search
                Slider[] sliders = FindObjectsByType<Slider>(FindObjectsSortMode.None);
                foreach (var slider in sliders)
                {
                    if (slider.name.Contains("ST") || slider.name.Contains("Stamina"))
                    {
                        staminaSlider = slider;
                        LogDebug($"✅ Found STBar slider (alternative): {slider.name}");
                        break;
                    }
                }
            }
        }

        // Find Hotbar
        if (hotbarParent == null)
        {
            GameObject hotbarObj = GameObject.Find("HUD_Canvas/Hotbar");
            if (hotbarObj != null)
            {
                hotbarParent = hotbarObj.transform;
                LogDebug($"✅ Found Hotbar parent");
            }
        }

        // Find UI_MasterPanel
        if (masterPanel == null)
        {
            masterPanel = GameObject.Find("UI_MasterPanel");
            if (masterPanel != null)
            {
                LogDebug($"✅ Found UI_MasterPanel");
            }
        }

        // Find panels under MasterPanel
        if (masterPanel != null)
        {
            Transform masterTransform = masterPanel.transform;

            if (inventoryPanel == null)
            {
                Transform invPanel = masterTransform.Find("InventoryPanel");
                if (invPanel != null)
                {
                    inventoryPanel = invPanel.gameObject;
                    LogDebug($"✅ Found InventoryPanel");

                    // Find SlotGrid
                    if (inventorySlotGrid == null)
                    {
                        Transform slotGrid = invPanel.Find("SlotGrid");
                        if (slotGrid != null)
                        {
                            inventorySlotGrid = slotGrid;
                            LogDebug($"✅ Found InventoryPanel/SlotGrid");
                        }
                    }
                }
            }

            if (mapPanel == null)
            {
                Transform map = masterTransform.Find("MapPanel");
                if (map != null)
                {
                    mapPanel = map.gameObject;
                    LogDebug($"✅ Found MapPanel");
                }
            }

            if (settingsPanel == null)
            {
                Transform settings = masterTransform.Find("SettingsPanel");
                if (settings != null)
                {
                    settingsPanel = settings.gameObject;
                    LogDebug($"✅ Found SettingsPanel");
                }
            }

            if (savePanel == null)
            {
                Transform save = masterTransform.Find("SavePanel");
                if (save != null)
                {
                    savePanel = save.gameObject;
                    LogDebug($"✅ Found SavePanel");
                }
            }

            // Find Top Buttons
            Transform topButtons = masterTransform.Find("TopButtonsPanel");
            if (topButtons != null)
            {
                if (buttonMap == null)
                {
                    Transform btn = topButtons.Find("Button_Map");
                    if (btn != null) buttonMap = btn.GetComponent<Button>();
                }
                if (buttonInventory == null)
                {
                    Transform btn = topButtons.Find("Button_Inventory");
                    if (btn != null) buttonInventory = btn.GetComponent<Button>();
                }
                if (buttonSettings == null)
                {
                    Transform btn = topButtons.Find("Button_Settings");
                    if (btn != null) buttonSettings = btn.GetComponent<Button>();
                }
                if (buttonSave == null)
                {
                    Transform btn = topButtons.Find("Button_Save");
                    if (btn != null) buttonSave = btn.GetComponent<Button>();
                }
            }
        }

        LogDebug("✅ UIReferenceManager: Auto-find complete");
    }

    /// <summary>
    /// Validate that all critical references are assigned
    /// </summary>
    private void ValidateReferences()
    {
        bool allValid = true;

        // HUD Canvas validation
        if (hudCanvas == null)
        {
            Debug.LogError("❌ UIReferenceManager: hudCanvas is not assigned!");
            allValid = false;
        }

        if (hpSlider == null)
        {
            Debug.LogWarning("⚠ UIReferenceManager: hpSlider is not assigned! PlayerHealth will not update UI.");
            allValid = false;
        }

        if (staminaSlider == null)
        {
            Debug.LogWarning("⚠ UIReferenceManager: staminaSlider is not assigned! PlayerStamina will not update UI.");
            allValid = false;
        }

        // Master Panel validation
        if (masterPanel == null)
        {
            Debug.LogError("❌ UIReferenceManager: masterPanel is not assigned!");
            allValid = false;
        }

        if (inventoryPanel == null)
        {
            Debug.LogWarning("⚠ UIReferenceManager: inventoryPanel is not assigned!");
        }

        if (inventorySlotGrid == null)
        {
            Debug.LogWarning("⚠ UIReferenceManager: inventorySlotGrid is not assigned! InventoryUI will not work.");
        }

        if (allValid)
        {
            LogDebug("✅ UIReferenceManager: All critical references validated successfully");
        }
        else
        {
            Debug.LogWarning("⚠ UIReferenceManager: Some references are missing. Check Inspector or enable autoFindReferences.");
        }
    }

    /// <summary>
    /// Public API: Get HP Slider for PlayerHealth
    /// </summary>
    public Slider GetHPSlider()
    {
        if (hpSlider == null)
        {
            Debug.LogWarning("⚠ UIReferenceManager.GetHPSlider: hpSlider is null!");
        }
        return hpSlider;
    }

    /// <summary>
    /// Public API: Get Stamina Slider for PlayerStamina
    /// </summary>
    public Slider GetStaminaSlider()
    {
        if (staminaSlider == null)
        {
            Debug.LogWarning("⚠ UIReferenceManager.GetStaminaSlider: staminaSlider is null!");
        }
        return staminaSlider;
    }

    /// <summary>
    /// Public API: Get Inventory Slot Grid for InventoryUI
    /// </summary>
    public Transform GetInventorySlotGrid()
    {
        if (inventorySlotGrid == null)
        {
            Debug.LogWarning("⚠ UIReferenceManager.GetInventorySlotGrid: inventorySlotGrid is null!");
        }
        return inventorySlotGrid;
    }

    /// <summary>
    /// Public API: Get Master Panel GameObject
    /// </summary>
    public GameObject GetMasterPanel()
    {
        return masterPanel;
    }

    /// <summary>
    /// Helper method to log debug messages
    /// </summary>
    private void LogDebug(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log(message);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Debug: Validate All References")]
    private void DebugValidateReferences()
    {
        AutoFindReferences();
        ValidateReferences();
    }

    [ContextMenu("Debug: Log All References")]
    private void DebugLogAllReferences()
    {
        Debug.Log("=== UIReferenceManager References ===");
        Debug.Log($"HUD Canvas: {(hudCanvas != null ? hudCanvas.name : "NULL")}");
        Debug.Log($"HP Slider: {(hpSlider != null ? hpSlider.name : "NULL")}");
        Debug.Log($"Stamina Slider: {(staminaSlider != null ? staminaSlider.name : "NULL")}");
        Debug.Log($"Hotbar Parent: {(hotbarParent != null ? hotbarParent.name : "NULL")}");
        Debug.Log($"Master Panel: {(masterPanel != null ? masterPanel.name : "NULL")}");
        Debug.Log($"Inventory Panel: {(inventoryPanel != null ? inventoryPanel.name : "NULL")}");
        Debug.Log($"Inventory Slot Grid: {(inventorySlotGrid != null ? inventorySlotGrid.name : "NULL")}");
        Debug.Log($"Map Panel: {(mapPanel != null ? mapPanel.name : "NULL")}");
        Debug.Log($"Settings Panel: {(settingsPanel != null ? settingsPanel.name : "NULL")}");
        Debug.Log($"Save Panel: {(savePanel != null ? savePanel.name : "NULL")}");
        Debug.Log("====================================");
    }
#endif
}
