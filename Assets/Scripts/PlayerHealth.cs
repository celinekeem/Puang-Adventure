using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.Audio;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Slider hpSlider; // Canvas_UI 안의 Slider 할당

    private int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHealth;
            hpSlider.value = currentHealth;
        }
    }

    public void TakeDamage(int amount)
    {
        Debug.Log($"[PlayerHealth] 데미지 {amount} 받음, 남은 체력: {currentHealth - amount}");
        if (amount <= 0) return;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        UpdateUI();
        if (currentHealth == 0) Die();
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (hpSlider != null) hpSlider.value = currentHealth;
    }

    private void Die()
    {
        Debug.Log("Player died");
        GameManager.I?.OnPlayerDeath();
        // 추가: 리스폰/게임오버 UI 로직은 GameManager에서 처리
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }
}