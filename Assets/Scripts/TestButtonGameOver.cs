using UnityEngine;
using TMPro; // Добавь это, чтобы работать с TextMeshPro

public class TestButtonGameOver : MonoBehaviour
{
    public GameObject targetObject;
    public TextMeshProUGUI textScore; // Сюда перетащишь текстовый объект с окна финала
    [SerializeField] private Drag drag;

    private void Awake()
    {
        targetObject.SetActive(false);
    }

    public void Toggle()
    {
        if (targetObject != null)
        {
            bool isActive = targetObject.activeSelf;
            targetObject.SetActive(!isActive);
            drag.enabled = isActive;
            // Если окно открывается (становится активным)
            if (targetObject.activeSelf)
            {
                // Берем статическую переменную прямо из класса Counter
                textScore.text = Counter.totalScore.ToString();
            }
        }
    }
}
