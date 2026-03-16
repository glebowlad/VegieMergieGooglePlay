using UnityEngine;
using System.Collections;
using TMPro;


public class Shaker : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI shakeCounterText;
    [SerializeField]
    private GameObject AddVideo;
    [SerializeField]
    private GameObject counterObject;
    
    public float duration = 0.2f;
    public float magnitude = 10f;
    public int maxShakes = 5;
    private bool isShaking = false;

    private int currentShakeCount;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private RectTransform rectTransform;
    private string rewardID;

    void Start()
    {
        AddVideo.SetActive(false);
        AddVideo.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text ="+"+ maxShakes.ToString();
        shakeCounterText.enabled = true;
        rectTransform = GetComponent<RectTransform>();
        currentShakeCount = maxShakes;


        UpdateCounter();
    }

    public void OnButtonClick()
    {
        if (isShaking) {return;}
        UpdateCounter();
        if (currentShakeCount > 0)
        {
            currentShakeCount--;
            StartShake();
            UpdateCounter();
        }
        else
        {
            MyRewardAdvShow();
        }

    }

    private void UpdateCounter()
    {
        if (shakeCounterText != null)
        {
            if (currentShakeCount > 0)
            {
                HideAdvImage();
                shakeCounterText.text = currentShakeCount.ToString();
            }
            else
            {
                ShowAddImage();
            }
        }
    }
    private void ShowAddImage() 
    {
        if (counterObject != null) counterObject.SetActive(false);
        AddVideo.SetActive(true);
    }
    private void MyRewardAdvShow()
    {
        // Реклама за награду
        //YG2.RewardedAdvShow(rewardID, () =>
        //{
        //    currentShakeCount += maxShakes;
        //    UpdateCounter();
        //});
    }

    private void HideAdvImage()
    {
        AddVideo.SetActive(false);
        if (counterObject != null) counterObject.SetActive(true);
    }

    public void StartShake()
    {
        if (rectTransform == null) return;
        StopAllCoroutines();
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        isShaking = true;
        AudioManager.Instance.PlayShakeSound();
        originalPosition = rectTransform.localPosition;
        originalRotation = rectTransform.localRotation;
        
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Твоя прошлая формула перемещения
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            rectTransform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            // Добавленное вращение (половина от силы magnitude для мягкости)
            float z = Random.Range(-1f, 1f) * (magnitude * 0.5f);
            rectTransform.localRotation = Quaternion.Euler(0, 0, originalRotation.eulerAngles.z + z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.localPosition = originalPosition;
        rectTransform.localRotation = originalRotation;
        isShaking=false;
    }
}
