using System;
using System.Collections;
using UnityEngine;

public class CopyExamProfessorPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    public Vector2 topRight = new Vector2(3.28f, 1.30f);
    public Vector2 bottomRight = new Vector2(3.28f, -3.06f);
    public Vector2 bottomLeft = new Vector2(-6.3f, -3.06f);
    public Vector2 topLeft = new Vector2(-6.3f, 1.36f);

    [Header("Movement")]
    public float speed = 2f;
    public float catchSpeed = 3f;
    public float reachDistance = 0.03f;

    [Header("Sorting Order")]
    public int topSortingOrder = 1;
    public int bottomSortingOrder = 5;
    public int catchSortingOrder = 3;

    [Header("Catch Points")]
    public Vector2 catchApproachPoint = new Vector2(0.34f, 1.36f);
    public Vector2 catchActionPoint = new Vector2(0.34f, 0.09f);

    [Header("Catch Animation")]
    public Animator professorAnimator;
    public string catchTriggerName = "catchCheating";
    public float catchAnimationDuration = 1.5f;

    private Vector2[] patrolPoints;
    private int currentPointIndex = 0;
    private SpriteRenderer spriteRenderer;
    private bool isCatching = false;

    public bool IsOnTopEdge
    {
        get
        {
            // Traseu în sensul acelor de ceasornic:
            // TopRight -> BottomRight -> BottomLeft -> TopLeft -> TopRight
            //
            // Profesorul este pe muchia de sus când merge din TopLeft spre TopRight.
            // În array, TopRight este index 0.
            return currentPointIndex == 0 && !isCatching;
        }
    }

    private void Start()
    {
        patrolPoints = new Vector2[]
        {
            topRight,
            bottomRight,
            bottomLeft,
            topLeft
        };

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (professorAnimator == null)
        {
            professorAnimator = GetComponent<Animator>();
        }

        // Profesorul începe din colțul dreapta sus.
        transform.position = patrolPoints[0];

        // Prima țintă este dreapta jos, deci patrula începe în sens orar.
        currentPointIndex = 1;

        UpdateSortingOrder();
    }

    private void Update()
    {
        if (isCatching)
            return;

        MoveToCurrentPoint();
        UpdateSortingOrder();
    }

    private void MoveToCurrentPoint()
    {
        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = patrolPoints[currentPointIndex];

        transform.position = Vector2.MoveTowards(
            currentPosition,
            targetPosition,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, targetPosition) <= reachDistance)
        {
            currentPointIndex++;

            if (currentPointIndex >= patrolPoints.Length)
            {
                currentPointIndex = 0;
            }
        }
    }

    private void UpdateSortingOrder()
    {
        if (spriteRenderer == null)
            return;

        // Muchia de sus: TopLeft -> TopRight
        if (currentPointIndex == 0)
        {
            spriteRenderer.sortingOrder = topSortingOrder;
        }
        // Muchia de jos: BottomRight -> BottomLeft
        else if (currentPointIndex == 2)
        {
            spriteRenderer.sortingOrder = bottomSortingOrder;
        }
    }

    public void StartCatchSequence(Action onProfessorCatchFinished)
    {
        if (isCatching)
            return;

        StartCoroutine(CatchSequence(onProfessorCatchFinished));
    }

    private IEnumerator CatchSequence(Action onProfessorCatchFinished)
    {
        isCatching = true;

        // Când se desprinde din patrulă și merge spre elev,
        // îl ținem pe layer-ul de sus.
        SetSortingOrder(topSortingOrder);

        // Merge până la primul punct.
        yield return MoveToPoint(catchApproachPoint, catchSpeed);

        // Apoi coboară până la punctul unde face animația de prins.
        yield return MoveToPoint(catchActionPoint, catchSpeed);

        // Fix când începe animația de prins copiat, trece pe order layer 3.
        SetSortingOrder(catchSortingOrder);

        if (professorAnimator != null)
        {
            professorAnimator.SetTrigger(catchTriggerName);
        }

        yield return new WaitForSeconds(catchAnimationDuration);

        // După animația de prins, revine pe order layer 1.
        SetSortingOrder(topSortingOrder);

        onProfessorCatchFinished?.Invoke();

        // Se întoarce la punctul de sus.
        yield return MoveToPoint(catchApproachPoint, catchSpeed);

        isCatching = false;
    }

    private IEnumerator MoveToPoint(Vector2 target, float moveSpeed)
    {
        while (Vector2.Distance(transform.position, target) > reachDistance)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = target;
    }

    private void SetSortingOrder(int order)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = order;
        }
    }
}