using System; 
using TMPro;  
using UnityEngine;
using System.Collections.Generic;

public class GameOverCheck : MonoBehaviour
{
    [SerializeField] private Drag drag;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI scoreText;

    public static event Action GameIsOver;
    public float thresholdTime = 1.5f;
    private bool isGameOver = false;

    private class FruitTrackingData {
        public float timer = 0f;
        public int collidersInside = 0;
        public SpriteRenderer[] renderers;
        public Color[] originalColors;
        public Vector3 originalPosition; // Для возврата после дрожания
    }

    private Dictionary<Rigidbody2D, FruitTrackingData> fruitData = new Dictionary<Rigidbody2D, FruitTrackingData>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isGameOver) return;
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null) return;

        if (!fruitData.ContainsKey(rb))
        {
            var data = new FruitTrackingData();
            data.renderers = rb.GetComponentsInChildren<SpriteRenderer>();
            data.originalColors = new Color[data.renderers.Length];
            data.originalPosition = rb.transform.position; // Запоминаем позицию
            
            for (int i = 0; i < data.renderers.Length; i++)
                data.originalColors[i] = data.renderers[i].color;

            fruitData.Add(rb, data);
        }
        fruitData[rb].collidersInside++;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isGameOver) return;
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null || !fruitData.ContainsKey(rb)) return;

        var data = fruitData[rb];
        data.timer += Time.deltaTime;

        float progress = data.timer / thresholdTime; // От 0 до 1

        // --- ПЛАВНОЕ ПОКРАСНЕНИЕ (начинается после 20% времени) ---
        float colorAlpha = Mathf.InverseLerp(0.2f, 1.0f, progress);
        for (int i = 0; i < data.renderers.Length; i++)
        {
            data.renderers[i].color = Color.Lerp(data.originalColors[i], Color.red, colorAlpha);
        }

        // --- ЛЕГКОЕ ДРОЖАНИЕ (усиливается к концу) ---
        if (progress > 0.4f) // Начинаем дрожать после половины времени
        {
            float shakeIntensity = (progress - 0.5f) * 0.10f; // Сила дрожания
            Vector2 shakeOffset = UnityEngine.Random.insideUnitCircle * shakeIntensity;
            rb.transform.position += (Vector3)shakeOffset;
        }

        if (data.timer >= thresholdTime)
        {
            GameOver();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null && fruitData.ContainsKey(rb))
        {
            var data = fruitData[rb];
            data.collidersInside--;

            if (data.collidersInside <= 0)
            {
                // Сброс цвета
                for (int i = 0; i < data.renderers.Length; i++)
                    data.renderers[i].color = data.originalColors[i];
                
                fruitData.Remove(rb);
            }
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("!!!!!!GAME OVER");
        GameIsOver?.Invoke();
        
        if (scoreText != null) scoreText.text = Counter.totalScore.ToString();
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (drag != null) drag.enabled = false;

        fruitData.Clear();
    }
}
