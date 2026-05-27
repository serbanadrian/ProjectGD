using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 4)]
    public string text;
    public float displayDuration = 3f;  // cat timp ramane pe ecran
    public bool isLastLine = false;      // ultima linie declanseaza minigame-ul
}