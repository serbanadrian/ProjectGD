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
    private int totalObjects = 10;      // Cate obiecte trebuie sa prinzi
    private int livesMax = 3;
    private float spawnInterval = 1.5f; // Interval intre obiecte

    // State
    private int livesRemaining;
    private int caughtCount = 0;
    private bool isRunning = false;
    private FallingObject currentObject = null;

    // Pozitii de spawn (sus, pe coloane diferite)
    private Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(-3f, 6f, 0f),
        new Vector3(-1f, 6f, 0f),
        new Vector3( 1f, 6f, 0f),
        new Vector3( 3f, 6f, 0f),
    };

    // Mapare tip → coloana de spawn
    private Dictionary<FallingObject.ObjectType, int> typeToColumn =
        new Dictionary<FallingObject.ObjectType, int>
    {
        { FallingObject.ObjectType.A, 0 },
        { FallingObject.ObjectType.S, 1 },
        { FallingObject.ObjectType.W, 2 },
        { FallingObject.ObjectType.D, 3 },
    };

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
            // Corect!
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
        // Mic delay la inceput
        yield return new WaitForSeconds(0.5f);

        while (isRunning && caughtCount < totalObjects)
        {
            // Asteapta sa nu fie un obiect activ
            yield return new WaitUntil(() => currentObject == null || !isRunning);

            if (!isRunning) yield break;

            yield return new WaitForSeconds(spawnInterval);

            if (!isRunning) yield break;

            SpawnObject();
        }
    }

    void SpawnObject()
    {
        // Alege tip random
        var types = (FallingObject.ObjectType[])Enum.GetValues(typeof(FallingObject.ObjectType));
        FallingObject.ObjectType randomType = types[UnityEngine.Random.Range(0, types.Length)];

        // Spawn pe coloana corespunzatoare
        Vector3 spawnPos = spawnPositions[typeToColumn[randomType]];
        GameObject obj = manager.SpawnFallingObject(spawnPos);

        FallingObject fo = obj.GetComponent<FallingObject>();
        fo.objectType = randomType;

        currentObject = fo;

        // Arata hint pe UI
        manager.ShowKeyHint(randomType.ToString());
    }

    public void EndMinigame(bool success)
    {
        isRunning = false;
        Instance = null;

        // Distruge obiectul curent daca mai exista
        if (currentObject != null)
        {
            currentObject.HandleMissed();
            currentObject = null;
        }

        onComplete?.Invoke(success);
    }
}