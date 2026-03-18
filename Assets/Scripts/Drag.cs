using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour
{
    private Transform line;
    private RectTransform rectTransform;
    private Canvas canvas;
    private Spawner spawner;
    public RectTransform leftWall;
    public RectTransform rightWall;

    public event Action WhileDrag;
    public event Action OnDragFinished;

    private bool isDragging = false;
    private int numberOfClicks = 0;
    private int currentTouchId = -1;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        spawner = GetComponent<Spawner>();
        rectTransform = GetComponent<RectTransform>();
        line = transform.GetChild(0);
        line.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!Spawner.IsSpawned) return;

        // Обрабатываем все касания
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            // Если уже есть активный драг, работаем только с ним
            if (isDragging)
            {
                // Проверяем, что это именно наше касание
                if (touch.fingerId == currentTouchId)
                {
                    // Обрабатываем движение
                    if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        MoveSpawner();
                        WhileDrag?.Invoke();
                    }

                    // Обрабатываем окончание касания
                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        FinishDrag();
                    }
                }

            }
            // Если драга нет, проверяем начало нового касания
            if (touch.phase == TouchPhase.Began)
            {
                // Проверяем, кликнули ли мы по UI элементу
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    GameObject clickedObject = EventSystem.current.currentSelectedGameObject;

                    // Если это кнопка - игнорируем это касание
                    if (clickedObject != null && clickedObject.CompareTag("Buttons"))
                    {
                        continue; // Пропускаем это касание, но продолжаем проверять другие
                    }
                }

                // Если дошли сюда - касание не по кнопке, начинаем драг
                StartDrag(touch.fingerId);
            }
        }
    }

    private void StartDrag(int fingerId)
    {
        isDragging = true;
        currentTouchId = fingerId;
        line.gameObject.SetActive(true);
    }

    private void FinishDrag()
    {
        numberOfClicks++;
        isDragging = false;
        currentTouchId = -1;
        line.gameObject.SetActive(false);

        if (numberOfClicks != 0 && numberOfClicks % 70 == 0)
        {
            // Реклама
            AdsManager.Instance.interstitialAds.ShowInterstitialAd();
        }
        OnDragFinished?.Invoke();
        AudioManager.Instance.PlayDropSound();
    }

    private void MoveSpawner()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out Vector2 localPoint);

        Vector2 leftWallLocal = canvas.transform.InverseTransformPoint(leftWall.position);
        Vector2 rightWallLocal = canvas.transform.InverseTransformPoint(rightWall.position);

        float halfWidth = (spawner != null) ? spawner.CurrentItemWidth / 2f : 0;

        float clampedX = Mathf.Clamp(
            localPoint.x,
            leftWallLocal.x + halfWidth,
            rightWallLocal.x - halfWidth
        );

        rectTransform.localPosition = new Vector3(clampedX, rectTransform.localPosition.y, rectTransform.localPosition.z);
    }
}