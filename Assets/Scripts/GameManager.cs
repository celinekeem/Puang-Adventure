using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.Audio;
public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("UI")]
    [SerializeField] private Slider playerHpSlider; // optional: 자동 연결 원하면 PlayerHealth에서 drag
    [SerializeField] private Text scoreText; // optional
    [SerializeField] private GameObject gameOverPanel; // optional: 활성화하면 게임오버 UI 보여줌

    private int score = 0;

    void Awake()
    {
        if (I == null) { I = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        UpdateScoreUI();
    }

    public void OnPlayerDeath()
    {
        Debug.Log("Game Over");
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnEnemyKilled(int value)
    {
        score += Mathf.Max(0, value);
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = $"Score: {score}";
    }

    // 유틸: 재시작/종료 버튼에서 호출
    public void Restart()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}