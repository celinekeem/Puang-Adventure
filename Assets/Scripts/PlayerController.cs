
using System;
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

    void Awake()
    {
        if (GetComponent<PlayerInput>() == null)
            Debug.Log("PlayerInput 컴포넌트가 없습니다. 새 Input System 사용 시 PlayerInput 추가를 권장합니다.");
    }

    void Update()
    {
        if (!isDashing)
            MovePlayer();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("공격!");
        }
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

    public void OnMove(InputValue value) => movementInput = value.Get<Vector2>();
    public void OnAttack(InputValue value)
    {
        if (value.Get<float>() > 0f) Debug.Log("공격!");
    }
    public void OnDash(InputValue value)
    {
        if (value.Get<float>() > 0f) TryDash();
    }
    public void OnSprint(InputValue value)
    {
        isSprinting = value.Get<float>() > 0f;
    }

    private void MovePlayer()
    {
        Vector3 dir = new Vector3(movementInput.x, movementInput.y, 0f);
        if (dir.sqrMagnitude < 0.0001f) return;

        dir = dir.normalized;
        float speed = moveSpeed * (isSprinting ? sprintMultiplier : 1f);
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }

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
}
