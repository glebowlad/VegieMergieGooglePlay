using System;
using UnityEngine;

public class Drag : MonoBehaviour
{
    private Transform line;
    private RectTransform rectTransform;
    private Canvas canvas;
    private Spawner spawner;
    public RectTransform leftWall;
    public RectTransform rightWall;
    
    public RectTransform touchArea; 

    public event Action WhileDrag;
    public event Action OnDragFinished;

    private bool isDragging = false;
    private int currentTouchId = -1;
    private float minX;
    private float maxX;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        spawner = GetComponent<Spawner>();
        rectTransform = GetComponent<RectTransform>();
        line = transform.GetChild(0);
        line.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (canvas != null)
        {
            Vector3 leftLocal = canvas.transform.InverseTransformPoint(leftWall.position);
            Vector3 rightLocal = canvas.transform.InverseTransformPoint(rightWall.position);

            minX = leftLocal.x;
            maxX = rightLocal.x;
        }
    }

    private void Update()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (isDragging && touch.fingerId == currentTouchId)
            {
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    // Обновляем состояние линии
                    bool shouldShowLine = spawner.IsSpawned;
                    if (line.gameObject.activeSelf != shouldShowLine)
                    {
                        line.gameObject.SetActive(shouldShowLine);
                    }

                    // ПЕРЕДАЕМ touch напрямую, чтобы не искать его снова
                    MoveSpawner(touch.position);
                    WhileDrag?.Invoke();
                }

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    FinishDrag();
                }

                // Если мы нашли наш палец и обработали его, 
                // переходить к проверке TouchPhase.Began для него же не всегда нужно,
                // но для мультитача цикл продолжит работу.
                continue;
            }

            if (touch.phase == TouchPhase.Began && !isDragging)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(touchArea, touch.position, canvas.worldCamera))
                {
                    StartDrag(touch.fingerId);
                }
            }
        }
    }

    private void StartDrag(int fingerId)
    {
        isDragging = true;
        currentTouchId = fingerId;

    }

    private void FinishDrag()
    {
        isDragging = false;
        currentTouchId = -1;
        line.gameObject.SetActive(false);
        OnDragFinished?.Invoke();
    }

    private void MoveSpawner(Vector2 screenPosition)
    {
        // УДАЛЯЕМ СТРОКУ: Vector2 touchPosition = Input.GetTouch(currentTouchId).position;
        // Используем screenPosition, который пришел из Update

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPosition, // Используем переданный аргумент
            canvas.worldCamera,
            out Vector2 localPoint);

        float halfWidth = (spawner != null) ? spawner.CurrentItemWidth / 1.5f : 0;

        float clampedX = Mathf.Clamp(
            localPoint.x,
            minX + halfWidth,
            maxX - halfWidth
        );

        rectTransform.localPosition = new Vector3(clampedX, rectTransform.localPosition.y, rectTransform.localPosition.z);
    }
}
