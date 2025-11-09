using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Portal trigger for scene transitions.
/// When player enters the trigger, loads the target scene and optionally spawns at a specific location.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PortalTrigger : MonoBehaviour
{
    [Header("Portal Settings")]
    [Tooltip("Name of the scene to load (must be added in Build Settings)")]
    [SerializeField] private string targetSceneName = "TutorialScene";

    [Tooltip("Optional: Specific spawn position in target scene. If empty, uses PlayerSpawn tag.")]
    [SerializeField] private Vector3 spawnPosition = Vector3.zero;

    [Tooltip("Use specific spawn position instead of PlayerSpawn tag")]
    [SerializeField] private bool useCustomSpawnPosition = false;

    [Header("Portal Visual (Optional)")]
    [Tooltip("Optional visual effect or sprite renderer to highlight the portal")]
    [SerializeField] private SpriteRenderer portalVisual;

    [Header("Debug")]
    [SerializeField] private bool showDebugMessages = true;

    private bool isTransitioning = false;

    private void Awake()
    {
        // Ensure the collider is set to trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
            Debug.LogWarning($"⚠ PortalTrigger: Collider on '{gameObject.name}' was not set to IsTrigger. Auto-fixing.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player") && !isTransitioning)
        {
            if (showDebugMessages)
                Debug.Log($"🌀 PortalTrigger: Player entered portal '{gameObject.name}' → Loading scene '{targetSceneName}'");

            TriggerSceneTransition(other.gameObject);
        }
    }

    /// <summary>
    /// Trigger the scene transition
    /// </summary>
    private void TriggerSceneTransition(GameObject player)
    {
        isTransitioning = true;

        // Save player's current position before transitioning (optional)
        PlayerPersistent playerPersistent = player.GetComponent<PlayerPersistent>();
        if (playerPersistent != null)
        {
            // If using custom spawn position, set it before scene loads
            if (useCustomSpawnPosition)
            {
                playerPersistent.SaveCurrentPosition();
                // The new position will be set after scene loads via OnSceneLoaded in PlayerPersistent
            }
        }

        // Load the target scene
        LoadTargetScene();
    }

    /// <summary>
    /// Load the target scene
    /// </summary>
    private void LoadTargetScene()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError($"❌ PortalTrigger: Target scene name is empty on '{gameObject.name}'");
            isTransitioning = false;
            return;
        }

        // Check if scene exists in build settings
        if (Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError($"❌ PortalTrigger: Scene '{targetSceneName}' not found in Build Settings! Add it via File → Build Settings.");
            isTransitioning = false;
        }
    }

    /// <summary>
    /// Get the spawn position for this portal
    /// </summary>
    public Vector3 GetSpawnPosition()
    {
        return useCustomSpawnPosition ? spawnPosition : Vector3.zero;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Draw portal area in editor
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.5f);

            if (col is BoxCollider2D boxCol)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(boxCol.offset, boxCol.size);
            }
            else if (col is CircleCollider2D circleCol)
            {
                Gizmos.DrawSphere(transform.position + (Vector3)circleCol.offset, circleCol.radius);
            }
        }

        // Draw arrow pointing up to indicate portal
        Gizmos.color = Color.cyan;
        Vector3 arrowStart = transform.position;
        Vector3 arrowEnd = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawLine(arrowStart, arrowEnd);
        Gizmos.DrawLine(arrowEnd, arrowEnd + Vector3.left * 0.15f + Vector3.down * 0.15f);
        Gizmos.DrawLine(arrowEnd, arrowEnd + Vector3.right * 0.15f + Vector3.down * 0.15f);
    }

    private void OnDrawGizmosSelected()
    {
        // Show target spawn position when selected
        if (useCustomSpawnPosition)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnPosition, 0.3f);
            Gizmos.DrawLine(transform.position, spawnPosition);

            UnityEditor.Handles.Label(spawnPosition + Vector3.up * 0.5f, $"Spawn Position\n{spawnPosition}");
        }

        // Show scene name label
        UnityEditor.Handles.color = Color.cyan;
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1f, $"Portal → {targetSceneName}");
    }
#endif
}
