using UnityEngine;

public class WormController : MonoBehaviour
{
    [Header("UI")]
    public RectTransform wormHead;
    public RectTransform pauseMenuRect;
    public Canvas canvas;

    [Header("Tail")]
    public RectTransform[] tailSegments;
    public float segmentSpacing = 35f;
    public float tailFollowSpeed = 900f;

    [Header("Speed")]
    public float chaseSpeed = 700f;
    public float restoreSpeed = 650f;
    public float returnSpeed = 250f;

    [Header("Delays")]
    public float pauseInputLockTime = 1f;   // после открытия паузы игнор касаний
    public float idleDelay = 1f;           // после отпускания вернуть домой
    public float segmentStartDelay = 0.08f;

    [Header("Rotate")]
    public float rotateSpeed = 720f;
    public float snapDistance = 8f;

    private bool startPoseInitialized = false;

    private enum WormState
    {
        Restore,
        Chase
    }

    

    private WormState state = WormState.Restore;

    private Camera uiCamera;
    private RectTransform parentRect;

    private Vector2 headStartPosition;
    private Quaternion headStartRotation;

    private Vector2[] tailStartPositions;
    private Quaternion[] tailStartRotations;

    private float restoreTimer = 0f;

    private float pauseStartedAt = 0f;
    private bool inputUnlocked = false;

    void Awake()
    {
        if (canvas != null)
            uiCamera = canvas.worldCamera;

        if (wormHead != null)
            parentRect = wormHead.parent as RectTransform;
    }

    void InitStartPoseIfNeeded()
    {
        if (startPoseInitialized) return;

        SaveStartPose();
        startPoseInitialized = true;
    }

    void Update()
    {
        if (!gameObject.activeInHierarchy) return;
        if (wormHead == null || pauseMenuRect == null || parentRect == null) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        bool pressed = Input.GetMouseButton(0);
        Vector2 fingerScreen = Input.mousePosition;
#else
        bool pressed = Input.touchCount > 0;
        Vector2 fingerScreen = pressed ? Input.GetTouch(0).position : Vector2.zero;
#endif

        if (!inputUnlocked)
        {
            if (Time.unscaledTime - pauseStartedAt >= pauseInputLockTime)
                inputUnlocked = true;
        }

        bool canChase =
            inputUnlocked &&
            pressed &&
            !InsideMenu(fingerScreen);

        if (canChase)
        {
            state = WormState.Chase;
            restoreTimer = 0f;

            Vector2 target = ScreenToParentLocal(fingerScreen);

            MoveHead(target, chaseSpeed);
            MoveTailFollow();
        }
        else
        {
            restoreTimer += Time.unscaledDeltaTime;

            if (restoreTimer >= idleDelay)
                state = WormState.Restore;

            if (state == WormState.Restore)
                RestoreSequential();
        }
    }

    public void EnablePauseMode()
    {
        InitStartPoseIfNeeded();

        restoreTimer = 0f;
        state = WormState.Restore;

        pauseStartedAt = Time.unscaledTime;
        inputUnlocked = false;

        RestoreImmediate();

        wormHead.gameObject.SetActive(true);

        for (int i = 0; i < tailSegments.Length; i++)
        {
            if (tailSegments[i] != null)
                tailSegments[i].gameObject.SetActive(true);
        }
    }

    public void DisablePauseMode()
    {
        restoreTimer = 0f;
    }

    void SaveStartPose()
    {
        if (wormHead != null)
        {
            headStartPosition = wormHead.anchoredPosition;
            headStartRotation = wormHead.localRotation;
        }

        if (tailSegments == null) return;

        tailStartPositions = new Vector2[tailSegments.Length];
        tailStartRotations = new Quaternion[tailSegments.Length];

        for (int i = 0; i < tailSegments.Length; i++)
        {
            if (tailSegments[i] == null) continue;

            tailStartPositions[i] = tailSegments[i].anchoredPosition;
            tailStartRotations[i] = tailSegments[i].localRotation;
        }
    }

    void RestoreImmediate()
    {
        wormHead.anchoredPosition = headStartPosition;
        wormHead.localRotation = headStartRotation;

        for (int i = 0; i < tailSegments.Length; i++)
        {
            if (tailSegments[i] == null) continue;

            tailSegments[i].anchoredPosition = tailStartPositions[i];
            tailSegments[i].localRotation = tailStartRotations[i];
        }
    }

    void MoveHead(Vector2 target, float speed)
    {
        Vector2 oldPos = wormHead.anchoredPosition;

        wormHead.anchoredPosition = Vector2.MoveTowards(
            wormHead.anchoredPosition,
            target,
            speed * Time.unscaledDeltaTime
        );

        RotateByMove(wormHead, wormHead.anchoredPosition - oldPos);
    }

    void MoveTailFollow()
    {
        RectTransform prev = wormHead;

        for (int i = 0; i < tailSegments.Length; i++)
        {
            RectTransform current = tailSegments[i];
            if (current == null) continue;

            Vector2 dir = prev.anchoredPosition - current.anchoredPosition;
            float dist = dir.magnitude;

            if (dist > segmentSpacing)
            {
                Vector2 targetPos =
                    prev.anchoredPosition - dir.normalized * segmentSpacing;

                Vector2 oldPos = current.anchoredPosition;

                current.anchoredPosition = Vector2.MoveTowards(
                    current.anchoredPosition,
                    targetPos,
                    tailFollowSpeed * Time.unscaledDeltaTime
                );

                RotateByMove(current, current.anchoredPosition - oldPos);
            }

            prev = current;
        }
    }

    void RestoreSequential()
    {
        MoveSegmentToPose(
            wormHead,
            headStartPosition,
            headStartRotation
        );

        for (int i = 0; i < tailSegments.Length; i++)
        {
            float needTime = idleDelay + segmentStartDelay * (i + 1);

            if (restoreTimer < needTime)
                break;

            if (tailSegments[i] == null) continue;

            MoveSegmentToPose(
                tailSegments[i],
                tailStartPositions[i],
                tailStartRotations[i]
            );
        }
    }

    void MoveSegmentToPose(
        RectTransform segment,
        Vector2 targetPos,
        Quaternion targetRot)
    {
        segment.anchoredPosition = Vector2.MoveTowards(
            segment.anchoredPosition,
            targetPos,
            restoreSpeed * Time.unscaledDeltaTime
        );

        segment.localRotation = Quaternion.RotateTowards(
            segment.localRotation,
            targetRot,
            rotateSpeed * Time.unscaledDeltaTime
        );

        if (Vector2.Distance(segment.anchoredPosition, targetPos) <= snapDistance)
            segment.anchoredPosition = targetPos;
    }

    void RotateByMove(RectTransform segment, Vector2 moveDir)
    {
        if (moveDir.sqrMagnitude < 0.001f) return;

        float angle =
            Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;

        segment.localRotation =
            Quaternion.Euler(0f, 0f, angle + 90f);
    }

    bool InsideMenu(Vector2 screenPoint)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            pauseMenuRect,
            screenPoint,
            uiCamera
        );
    }

    Vector2 ScreenToParentLocal(Vector2 screenPoint)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            screenPoint,
            uiCamera,
            out Vector2 localPoint
        );

        return localPoint;
    }
}