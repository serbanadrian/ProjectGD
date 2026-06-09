using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

public class StayAwakeMinigame : IMinigame
{
    public static StayAwakeMinigame Instance { get; private set; }

    private MinigameManager manager;
    private Action<bool> onComplete;

    private float sleepFillRate = 0.08f;
    private float wakeFillReduce = 0.25f;
    private float keyChangeInterval = 2f;
    private float totalDuration = 30f;

    private float sleepValue = 0f;
    private float timeRemaining;
    private float keyChangeTimer;
    private bool isRunning = false;

    private List<Key> possibleKeys = new List<Key>
    {
        Key.Space, Key.W, Key.A, Key.S, Key.D,
        Key.Q, Key.E, Key.R, Key.F, Key.G
    };

    private Key currentKey;

    public StayAwakeMinigame(MinigameManager manager)
    {
        this.manager = manager;
        Instance = this;
    }

    public void StartMinigame(Action<bool> onComplete)
    {
        this.onComplete = onComplete;
        sleepValue = 0f;
        timeRemaining = totalDuration;
        keyChangeTimer = 0f;
        isRunning = true;

        PickNewKey();
        manager.UpdateSleepBar(sleepValue);
        manager.UpdateTimer(timeRemaining);
    }

    public void UpdateMinigame()
    {
        if (!isRunning) return;

        sleepValue += sleepFillRate * Time.deltaTime;
        sleepValue = Mathf.Clamp01(sleepValue);

        timeRemaining -= Time.deltaTime;

        keyChangeTimer += Time.deltaTime;
        if (keyChangeTimer >= keyChangeInterval)
        {
            keyChangeTimer = 0f;
            PickNewKey();
        }

        manager.UpdateSleepBar(sleepValue);
        manager.UpdateTimer(timeRemaining);
        manager.UpdateOverlay(sleepValue);

        if (Keyboard.current[currentKey].wasPressedThisFrame)
        {
            sleepValue -= wakeFillReduce;
            sleepValue = Mathf.Clamp01(sleepValue);
            manager.ShowFeedback(true);
        }

        if (sleepValue >= 1f)
        {
            isRunning = false;
            EndMinigame(false);
        }
        else if (timeRemaining <= 0f)
        {
            isRunning = false;
            EndMinigame(true);
        }
    }

    void PickNewKey()
    {
        int index = UnityEngine.Random.Range(0, possibleKeys.Count);
        currentKey = possibleKeys[index];
        manager.ShowCurrentKey(currentKey.ToString());
    }

    public void EndMinigame(bool success)
    {
        isRunning = false;
        Instance = null;
        onComplete?.Invoke(success);
    }
}