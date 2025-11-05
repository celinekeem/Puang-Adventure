// ...existing code...
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float hitCooldown = 0.5f;

    private float lastHitTime = -99f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryHit(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHit(other.gameObject);
    }

    private void TryHit(GameObject target)
    {
        if (Time.time < lastHitTime + hitCooldown) return;
        if (!target.CompareTag("Player")) return;

        lastHitTime = Time.time;

        var ph = target.GetComponent<PlayerHealth>() ?? target.GetComponentInParent<PlayerHealth>();
        if (ph != null)
        {
            ph.TakeDamage(damage);
            Debug.Log($"Enemy hit Player for {damage} dmg");
        }
        else
        {
            Debug.LogWarning("EnemyAttack: PlayerHealth component not found on Player object.");
        }
    }
}
// ...existing code...