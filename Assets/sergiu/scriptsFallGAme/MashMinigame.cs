using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;

public class MashMinigame : IMinigame
{
    public static MashMinigame Instance { get; private set; }

    private MinigameManager manager;
    private Action<bool> onComplete;

    // Settings
    private int totalObjects = 10;
    private int livesMax = 3;
    private float spawnInterval = 2f;

    // Limite spawn pe X (aleator intre acestea)
    private float minX = -3f;
    private float maxX = 3f;
    private float spawnY = 6f;

    // State
    private int livesRemaining;
    private int caughtCount = 0;
    private bool isRunning = false;
    private FallingObject currentObject = null;

    public MashMinigame(MinigameManager manager)
    {
        this.manager = manager;
        Instance = this;
    }

    public void StartMinigame(Action<bool> onComplete)
    {
        this.onComplete = onComplete;
        livesRemaining = livesMax;
        caughtCount = 0;
        isRunning = true;
        currentObject = null;

        manager.UpdateLives(livesRemaining, livesMax);
        manager.UpdateProgress(caughtCount, totalObjects);
        manager.StartCoroutine(SpawnLoop());
    }

    public void UpdateMinigame()
    {
        if (!isRunning || currentObject == null) return;
        CheckInput();
    }

    void CheckInput()
    {
        FallingObject.ObjectType? pressed = null;

        if (Keyboard.current.wKey.wasPressedThisFrame) pressed = FallingObject.ObjectType.W;
        if (Keyboard.current.aKey.wasPressedThisFrame) pressed = FallingObject.ObjectType.A;
        if (Keyboard.current.sKey.wasPressedThisFrame) pressed = FallingObject.ObjectType.S;
        if (Keyboard.current.dKey.wasPressedThisFrame) pressed = FallingObject.ObjectType.D;

        if (pressed == null) return;

        if (pressed == currentObject.objectType)
        {
            currentObject.HandleCorrect();
            currentObject = null;
            caughtCount++;
            manager.UpdateProgress(caughtCount, totalObjects);
            manager.ShowFeedback(true);

            if (caughtCount >= totalObjects)
            {
                isRunning = false;
                EndMinigame(true);
            }
        }
        else
        {
            // Tasta gresita
            manager.ShowFeedback(false);
        }
    }

    public void OnObjectMissed(FallingObject obj)
    {
        if (!isRunning) return;

        currentObject = null;
        obj.HandleMissed();
        livesRemaining--;
        manager.UpdateLives(livesRemaining, livesMax);
        manager.ShowFeedback(false);

        if (livesRemaining <= 0)
        {
            isRunning = false;
            EndMinigame(false);
        }
    }

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(0.5f);

        while (isRunning && caughtCount < totalObjects)
        {
            yield return new WaitUntil(() => currentObject == null || !isRunning);

            if (!isRunning) yield break;

            yield return new WaitForSeconds(spawnInterval);

            if (!isRunning) yield break;

            SpawnObject();
        }
    }

    void SpawnObject()
    {
        // Tip aleator
        var types = (FallingObject.ObjectType[])Enum.GetValues(typeof(FallingObject.ObjectType));
        FallingObject.ObjectType randomType = types[UnityEngine.Random.Range(0, types.Length)];

        // Pozitie X aleatoare
        float randomX = UnityEngine.Random.Range(minX, maxX);
        Vector3 spawnPos = new Vector3(randomX, spawnY, 0f);

        // Spawneaza obiectul corespunzator tipului
        GameObject obj = manager.SpawnFallingObject(randomType, spawnPos);
        FallingObject fo = obj.GetComponent<FallingObject>();
        fo.objectType = randomType;
        currentObject = fo;
    }

    public void EndMinigame(bool success)
    {
        isRunning = false;
        Instance = null;

        if (currentObject != null)
        {
            currentObject.HandleMissed();
            currentObject = null;
        }

        onComplete?.Invoke(success);
    }
}