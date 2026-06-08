using UnityEngine;
using System;

public class ChaseMinigame : IMinigame
{
    public static ChaseMinigame Instance { get; private set; }

    private MinigameManager manager;
    private Action<bool> onComplete;

    private Transform chaser;
    private Transform player;

    private float chaseSpeed = 3f;
    private float catchRadius = 0.5f;
    private bool isRunning = false;
    private bool caught = false;

    public ChaseMinigame(MinigameManager manager)
    {
        this.manager = manager;
        Instance = this;
    }

    public void StartMinigame(Action<bool> onComplete)
    {
        this.onComplete = onComplete;
        isRunning = true;
        caught = false;

        chaser = manager.chaser.transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void UpdateMinigame()
    {
        if (!isRunning || caught) return;

        Vector3 direction = (player.position - chaser.position).normalized;
        chaser.position += direction * chaseSpeed * Time.deltaTime;

        float distance = Vector2.Distance(chaser.position, player.position);
        if (distance <= catchRadius)
        {
            caught = true;
            isRunning = false;
            EndMinigame(false);
        }
    }

    public void EndMinigame(bool success)
    {
        isRunning = false;
        Instance = null;
        onComplete?.Invoke(false);
    }
}