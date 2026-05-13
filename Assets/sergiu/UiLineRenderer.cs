using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : MaskableGraphic
{
    public Vector2 pointA;
    public Vector2 pointB;
    public float thickness = 6f;
    public Color color = Color.white;

    public void SetPoints(Vector2 a, Vector2 b)
    {
        pointA = a;
        pointB = b;
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Vector2 dir = (pointB - pointA).normalized;
        Vector2 perp = new Vector2(-dir.y, dir.x) * (thickness / 2f);

        UIVertex v = UIVertex.simpleVert;
        v.color = color;

        v.position = pointA + perp; vh.AddVert(v);
        v.position = pointA - perp; vh.AddVert(v);
        v.position = pointB - perp; vh.AddVert(v);
        v.position = pointB + perp; vh.AddVert(v);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }
}