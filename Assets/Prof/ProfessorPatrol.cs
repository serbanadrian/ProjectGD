using System;
using System.Collections;
using UnityEngine;

public class ProfessorPatrol : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;

    [Header("Rectangle points")]
    public Vector2 topRight = new Vector2(4.14f, 0.91f);
    public Vector2 bottomRight = new Vector2(4.14f, -2.01f);
    public Vector2 bottomLeft = new Vector2(-6.05f, -2.01f);
    public Vector2 topLeft = new Vector2(-6.05f, 0.91f);

    [Header("Catch Points")]
    public Vector2 catchApproachPoint = new Vector2(2.216773f, -1.61f);
    public Vector2 catchActionPoint = new Vector2(1.85f, -1.71f);

    [Header("Catch Animation")]
    public Animator professorAnimator;
    public string catchTriggerName = "catchCheating";
    public float catchAnimationDuration = 0.3f;

    [Header("Sorting Order")]
    public int topSortingOrder = 1;
    public int bottomSortingOrder = 7;
    public float bottomYThreshold = -0.5f;

    private Vector2[] points;
    private int currentPointIndex = 0;
    private SpriteRenderer spriteRenderer;
    private bool isCatching = false;

public bool IsInBottomInterval
{
    get
    {
        // Profesorul poate prinde studentul doar când merge pe latura de jos:
        // bottomLeft -> bottomRight
        return currentPointIndex == 3 && !isCatching;
    }
}

    private void Start()
    {
        // Traseu:
        // dreapta sus -> stânga sus -> stânga jos -> dreapta jos -> dreapta sus
        points = new Vector2[]
        {
            topRight,
            topLeft,
            bottomLeft,
            bottomRight
        };

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (professorAnimator == null)
            professorAnimator = GetComponent<Animator>();

        transform.position = points[0];
    }

    private void Update()
    {
        if (!isCatching)
        {
            MoveBetweenPoints();
        }

        UpdateSortingOrder();
    }

    private void MoveBetweenPoints()
    {
        Vector2 target = points[currentPointIndex];

        transform.position = Vector2.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        float distance = Vector2.Distance(transform.position, target);

        if (distance < 0.01f)
        {
            currentPointIndex++;

            if (currentPointIndex >= points.Length)
            {
                currentPointIndex = 0;
            }
        }
    }

    private void UpdateSortingOrder()
    {
        if (spriteRenderer == null)
            return;

        if (transform.position.y > bottomYThreshold && !isCatching)
        {
            spriteRenderer.sortingOrder = topSortingOrder;
        }
        else
        {
            spriteRenderer.sortingOrder = bottomSortingOrder;
        }
    }

    public void StartCatchSequence(Action onCatchAnimationFinished)
    {
        if (isCatching)
            return;

        StartCoroutine(CatchSequence(onCatchAnimationFinished));
    }

    private IEnumerator CatchSequence(Action onCatchAnimationFinished)
    {
        isCatching = true;

        yield return MoveToPoint(catchApproachPoint);
        yield return MoveToPoint(catchActionPoint);

        if (professorAnimator != null)
        {
            professorAnimator.SetTrigger(catchTriggerName);
        }

        yield return new WaitForSeconds(catchAnimationDuration);

        onCatchAnimationFinished?.Invoke();

        yield return MoveToPoint(catchApproachPoint);

        isCatching = false;
    }

    private IEnumerator MoveToPoint(Vector2 target)
    {
        while (Vector2.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                target,
                speed * Time.deltaTime
            );

            UpdateSortingOrder();

            yield return null;
        }

        transform.position = target;
    }
}