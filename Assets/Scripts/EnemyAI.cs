using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.Audio;

public class EnemyAI : MonoBehaviour
{
    public float speed = 2f;
    Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;
        transform.position = Vector2.MoveTowards(
            transform.position, player.position, speed * Time.deltaTime);
    }
}