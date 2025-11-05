// ...existing code...
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    private Vector2 movementInput;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.6f;
    private bool isSprinting = false;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float dashDuration = 0.12f;
    [SerializeField] private float dashCooldown = 1f;
    private bool isDashing = false;
    private float lastDashTime = -99f;

    [Header("Attack")]
    [Tooltip("Hierarchy의 AttackArea(자식) Collider2D를 할당하세요. Is Trigger 체크 필요")]
    [SerializeField] private Collider2D attackAreaCollider;
    [SerializeField] private float attackDuration = 0.12f;
    [SerializeField] private float attackCooldown = 0.3f;
    [SerializeField] private LayerMask enemyLayer;

    private bool isAttacking = false;
    private float lastAttackTime = -99f;
    private ContactFilter2D attackFilter;
    private readonly List<Collider2D> overlapResults = new List<Collider2D>();

    private void Awake()
    {
        if (GetComponent<PlayerInput>() == null)
            Debug.Log("⚠ PlayerInput 컴포넌트가 없습니다. 새 Input System 사용 시 PlayerInput 추가를 권장합니다.");

        attackFilter = new ContactFilter2D();
        attackFilter.SetLayerMask(enemyLayer);
        attackFilter.useTriggers = true;

        // Optional: 자동으로 비활성화(또는 활성화) 상태 조정하지 않음 — Inspector에서 관리 가능
        // if (attackAreaCollider != null) attackAreaCollider.enabled = false;
    }

    private void Update()
    {
        if (!isDashing && !isAttacking)
            MovePlayer();
    }

    // ===================== Input System 콜백 (InputAction.CallbackContext) =====================
    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            Attack();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
            TryDash();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed) isSprinting = true;
        else if (context.canceled) isSprinting = false;
    }

    // ===================== Send Messages 호환 오버로드 (PlayerInput Behavior: Send Messages) =====================
    public void OnMove(InputValue value) => movementInput = value.Get<Vector2>();
    public void OnAttack(InputValue value)
    {
        if (value.Get<float>() > 0f) Attack();
    }
    public void OnDash(InputValue value)
    {
        if (value.Get<float>() > 0f) TryDash();
    }
    public void OnSprint(InputValue value)
    {
        isSprinting = value.Get<float>() > 0f;
    }

    // ===================== 이동 =====================
    private void MovePlayer()
    {
        Vector3 dir = new Vector3(movementInput.x, movementInput.y, 0f);
        if (dir.sqrMagnitude < 0.0001f) return;

        dir = dir.normalized;
        float speed = moveSpeed * (isSprinting ? sprintMultiplier : 1f);
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }

    // ===================== 대시 =====================
    private void TryDash()
    {
        if (Time.time < lastDashTime + dashCooldown) return;

        Vector3 dir = new Vector3(movementInput.x, movementInput.y, 0f);
        if (dir.sqrMagnitude < 0.01f) return;

        StartCoroutine(Dash(dir.normalized));
    }

    private IEnumerator Dash(Vector3 direction)
    {
        isDashing = true;
        float start = Time.time;

        while (Time.time < start + dashDuration)
        {
            transform.Translate(direction * dashSpeed * Time.deltaTime, Space.World);
            yield return null;
        }

        isDashing = false;
        lastDashTime = Time.time;
    }

    // ===================== 공격 (AttackArea 사용) =====================
    private void Attack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;
        lastAttackTime = Time.time;

        StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        // (옵션) 공격 시작 시 콜라이더 활성화해서 트리거 이벤트를 사용하고 싶다면 여기서 활성화.
        // if (attackAreaCollider != null) attackAreaCollider.enabled = true;

        // 공격 지속 시간 동안 기다림 (애니메이션 동기화 용이)
        yield return new WaitForSeconds(attackDuration);

        // 공격 판정: AttackArea 콜라이더 범위 내의 적 검색
        if (attackAreaCollider != null)
        {
            overlapResults.Clear();
            attackAreaCollider.Overlap(attackFilter, overlapResults);

            foreach (var col in overlapResults)
            {
                if (col == null) continue;
                Debug.Log("공격 성공: " + col.name);
                // 예시: 적 오브젝트 파괴. 실제로는 적의 체력 시스템 호출 권장.
                Destroy(col.gameObject);
            }
        }
        else
        {
            // fallback: 기존 방식 (원하면 삭제)
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, 1.0f, enemyLayer);
            foreach (var enemy in hitEnemies)
            {
                Debug.Log("공격 성공 (fallback): " + enemy.name);
                Destroy(enemy.gameObject);
            }
        }

        // (옵션) 공격 끝나면 콜라이더 비활성화
        // if (attackAreaCollider != null) attackAreaCollider.enabled = false;

        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // 시각화: AttackArea 콜라이더가 있으면 그것을, 없으면 기본 반경 표시
        if (attackAreaCollider is CircleCollider2D cc)
        {
            Gizmos.DrawWireSphere(cc.transform.position + (Vector3)cc.offset, cc.radius * cc.transform.lossyScale.x);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, 1.0f);
        }
    }
}
// ...existing code...