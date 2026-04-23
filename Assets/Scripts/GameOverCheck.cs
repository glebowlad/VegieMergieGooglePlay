using System;
using UnityEngine;
using System.Collections.Generic;

public class GameOverCheck : MonoBehaviour
{
    [SerializeField] private Drag drag;
    [SerializeField] private GameObject gameOverPanel;
    
    public float thresholdTime = 1.5f;
    private bool isGameOver = false;

    // СОБЫТИЕ ДЛЯ BestScore (исправляет ошибку со скрина)
    public static Action GameIsOver;

    // Список овощей, которые СЕЙЧАС в зоне (теперь используем Vegetable)
    private List<Vegetable> activeVegetables = new List<Vegetable>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isGameOver) return;
        // Ищем компонент Vegetable (объединенный)
        var veg = other.attachedRigidbody?.GetComponent<Vegetable>();
        if (veg != null)
        {
            if (!activeVegetables.Contains(veg)) activeVegetables.Add(veg);
            veg.OnZoneEnter();
        }
    }

    private void Update()
    {
        if (isGameOver) return;

        for (int i = activeVegetables.Count - 1; i >= 0; i--)
        {
            var veg = activeVegetables[i];
            if (veg == null) {
                activeVegetables.RemoveAt(i);
                continue;
            }

            if (veg.IsInHazardZone())
            {
                veg.hazardTimer += Time.deltaTime;
                veg.UpdateHazardVisuals(veg.hazardTimer / thresholdTime);

                if (veg.hazardTimer >= thresholdTime) GameOver();
            }
            else
            {
                activeVegetables.RemoveAt(i);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var veg = other.attachedRigidbody?.GetComponent<Vegetable>();
        if (veg != null) veg.OnZoneExit();
    }

    private void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        AudioManager.Instance.PlayFinishSound();
        if (drag != null) drag.enabled = false;

      
        GameIsOver?.Invoke();

        Debug.Log("Game Over triggered by " + name);
    }
}
