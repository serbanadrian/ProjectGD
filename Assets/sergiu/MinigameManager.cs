using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class WireMinigameManager : MonoBehaviour
{
    public static WireMinigameManager Instance;

    [Header("UI References")]
    public GameObject minigamePanel;
    public RectTransform dotsContainer;   // Parintele tuturor punctelor
    public RectTransform linesContainer;  // Parintele tuturor liniilor

    [Header("Prefabs")]
    public GameObject dotPrefab;          // Cerc colorat
    public GameObject linePrefab;         // Image cu LineRenderer sau UI Line

    [Header("Settings")]
    public int pairCount = 4;
    public float dotRadius = 30f;

    [Header("Colors")]
    public Color[] pairColors = new Color[]
    {
        Color.red,
        Color.blue,
        Color.green,
        new Color(1f, 0.5f, 0f), // portocaliu
        Color.magenta,
    };

    // State intern
    private List<DotNode> allDots = new List<DotNode>();
    private DotNode selectedDot = null;
    private GameObject currentLineObj = null;
    private UILineRenderer currentLine = null;
    private int completedPairs = 0;
    private Action<bool> onComplete;
    private bool isPlaying = false;

    void Awake() => Instance = this;

    public void StartMinigame(Action<bool> callback)
    {
        if (isPlaying) return;
        onComplete = callback;
        completedPairs = 0;
        isPlaying = true;
        minigamePanel.SetActive(true);

        FindObjectOfType<PlayerMovement>().enabled = false;

        SpawnDots();
    }

    void SpawnDots()
    {
        // Curata vechi
        foreach (Transform t in dotsContainer) Destroy(t.gameObject);
        foreach (Transform t in linesContainer) Destroy(t.gameObject);
        allDots.Clear();

        // Pozitii posibile intr-un grid 4x4
        List<Vector2> positions = GenerateGridPositions();
        Shuffle(positions);

        for (int i = 0; i < pairCount; i++)
        {
            Color c = pairColors[i % pairColors.Length];

            // Cream 2 dot-uri cu aceeasi culoare
            DotNode dotA = SpawnDot(positions[i * 2],     c, i);
            DotNode dotB = SpawnDot(positions[i * 2 + 1], c, i);

            dotA.partner = dotB;
            dotB.partner = dotA;
        }
    }

    DotNode SpawnDot(Vector2 anchoredPos, Color color, int pairIndex)
    {
        GameObject obj = Instantiate(dotPrefab, dotsContainer);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(dotRadius * 2, dotRadius * 2);

        Image img = obj.GetComponent<Image>();
        img.color = color;

        DotNode node = obj.GetComponent<DotNode>();
        node.pairIndex = pairIndex;
        node.color = color;
        node.manager = this;
        allDots.Add(node);

        return node;
    }

    List<Vector2> GenerateGridPositions()
    {
        var list = new List<Vector2>();
        float spacing = 120f;
        float startX = -180f;
        float startY = -180f;

        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                list.Add(new Vector2(startX + x * spacing, startY + y * spacing));

        return list;
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    // Apelat de DotNode la click
    public void OnDotClicked(DotNode dot)
    {
        if (!isPlaying) return;

        // Daca era un fir activ, sterge-l
        if (currentLineObj != null)
        {
            Destroy(currentLineObj);
            currentLineObj = null;
            currentLine = null;
        }

        // Daca dot-ul e deja conectat, deconecteaza-l
        if (dot.isConnected)
        {
            dot.Disconnect();
            dot.partner.Disconnect();
            completedPairs--;
            selectedDot = null;
            return;
        }

        selectedDot = dot;

        // Creeaza linie noua
        currentLineObj = Instantiate(linePrefab, linesContainer);
        currentLine = currentLineObj.GetComponent<UILineRenderer>();
        currentLine.color = dot.color;
        currentLine.SetPoints(dot.GetComponent<RectTransform>().anchoredPosition,
                              dot.GetComponent<RectTransform>().anchoredPosition);
    }

    void Update()
    {
        if (!isPlaying) return;

        // Urmareste mouse-ul pentru linia activa
        if (selectedDot != null && currentLine != null)
        {
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                dotsContainer,
                Input.mousePosition,
                null,
                out localPos
            );
            currentLine.SetPoints(
                selectedDot.GetComponent<RectTransform>().anchoredPosition,
                localPos
            );
        }

        // Escape anuleaza
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentLineObj != null) { Destroy(currentLineObj); currentLineObj = null; }
            selectedDot = null;
        }
    }

    // Apelat de DotNode cand se termina drag-ul pe un alt dot
    public void TryConnect(DotNode target)
    {
        if (selectedDot == null || target == selectedDot) { CancelCurrentLine(); return; }

        // Trebuie sa fie aceeasi culoare
        if (target.pairIndex != selectedDot.pairIndex)
        {
            CancelCurrentLine();
            selectedDot = null;
            return;
        }

        // Trebuie sa fie partner-ul direct
        if (target != selectedDot.partner) { CancelCurrentLine(); selectedDot = null; return; }

        // Conecteaza
        currentLine.SetPoints(
            selectedDot.GetComponent<RectTransform>().anchoredPosition,
            target.GetComponent<RectTransform>().anchoredPosition
        );

        selectedDot.Connect(currentLineObj);
        target.Connect(currentLineObj);
        currentLineObj = null;
        currentLine = null;
        selectedDot = null;
        completedPairs++;

        if (completedPairs >= pairCount)
            EndMinigame(true);
    }

    void CancelCurrentLine()
    {
        if (currentLineObj != null) Destroy(currentLineObj);
        currentLineObj = null;
        currentLine = null;
    }

    void EndMinigame(bool success)
    {
        isPlaying = false;
        minigamePanel.SetActive(false);
        FindObjectOfType<PlayerMovement>().enabled = true;
        onComplete?.Invoke(success);
    }
}