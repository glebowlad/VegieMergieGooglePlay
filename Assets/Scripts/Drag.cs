using System;
using UnityEngine;
using UnityEngine.EventSystems;
using YG;

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

        if (Input.GetMouseButtonDown(0))
        {
            // Проверяем, кликнули ли мы по кнопкам
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Получаем объект, по которому кликнули
                GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
                
                // Если кликнули по UI, и это НЕ наша зона ввода — игнорируем (выходим)
                // Здесь замени "InputArea" на точное имя твоего объекта в иерархии
                if (clickedObject != null && clickedObject.name != "InputArea") 
                {
                    return;
                }
            }

            isDragging = true;
            line.gameObject.SetActive(true);
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            MoveSpawner();
            WhileDrag?.Invoke();
        }

        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            line.gameObject.SetActive(false);
            if(Counter.totalMergedItems == 30)
            {
                YG2.InterstitialAdvShow();
            }
            OnDragFinished?.Invoke();
            AudioManager.Instance.PlayDropSound();
        }
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

        // Берем половину ширины овоща. Если овоща нет, отступ 0.
        float halfWidth = (spawner != null) ? spawner.CurrentItemWidth / 2f : 0;

        float clampedX = Mathf.Clamp(
            localPoint.x,
            leftWallLocal.x + halfWidth, 
            rightWallLocal.x - halfWidth
        );

        rectTransform.localPosition = new Vector3(clampedX, rectTransform.localPosition.y, rectTransform.localPosition.z);
    }


}
