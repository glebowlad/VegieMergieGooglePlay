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
    
    public RectTransform touchArea; 

    public event Action WhileDrag;
    public event Action OnDragFinished;

    private bool isDragging = false;
    private int numberOfClicks = 0;
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
        RectTransform parentRect = rectTransform.parent as RectTransform;

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
            
            if (isDragging)
            {
                if (touch.fingerId == currentTouchId)
                {
                    if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                       
                        MoveSpawner();
                        WhileDrag?.Invoke();

                    }

                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        FinishDrag();
                    }
                }
            }
            
            if (touch.phase == TouchPhase.Began)
            {
                bool isInsideArea = RectTransformUtility.RectangleContainsScreenPoint(touchArea, touch.position, canvas.worldCamera);

                if (isInsideArea)
                {
                    if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    {
                        GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
                        if (clickedObject != null && clickedObject.CompareTag("Buttons"))
                        {
                            continue;
                        }
                    }

                    StartDrag(touch.fingerId);
                   
                }
            }
        }
    }

    private void StartDrag(int fingerId)
    {
        isDragging = true;
        currentTouchId = fingerId;
        if (spawner.IsSpawned)
        {
            line.gameObject.SetActive(true);
        }
    }

    private void FinishDrag()
    {
        numberOfClicks++;
        isDragging = false;
        currentTouchId = -1;
        line.gameObject.SetActive(false);
        OnDragFinished?.Invoke();
    }

    private void MoveSpawner()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
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
