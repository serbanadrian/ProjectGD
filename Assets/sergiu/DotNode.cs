using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DotNode : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    public int pairIndex;
    public Color color;
    public DotNode partner;
    public WireMinigameManager manager;

    public bool isConnected { get; private set; }
    private GameObject connectedLine;

    public void OnPointerClick(PointerEventData eventData)
    {
        manager.OnDotClicked(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Daca userul trage (buton apasat), incearca conectarea
        if (Input.GetMouseButton(0))
            manager.TryConnect(this);
    }

    public void Connect(GameObject line)
    {
        isConnected = true;
        connectedLine = line;
        // Feedback vizual - adauga un inel
        GetComponent<Image>().color = new Color(color.r, color.g, color.b, 1f);
        transform.localScale = Vector3.one * 1.2f;
    }

    public void Disconnect()
    {
        isConnected = false;
        if (connectedLine != null) Destroy(connectedLine);
        connectedLine = null;
        transform.localScale = Vector3.one;
    }
}