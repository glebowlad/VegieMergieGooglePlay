using UnityEngine;
using System.Collections;

public class AuraEffect : MonoBehaviour
{
    private SpriteRenderer sr;
    private SimplePool<AuraEffect> pool;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetPool(SimplePool<AuraEffect> sourcePool)
    {
        pool = sourcePool;
    }

    // Метод настройки (для Вспышки)
    public void PlayFlash(Sprite sprite, Color color, float radius)
    {
        StopAllCoroutines();
        sr.sprite = sprite;
        sr.color = color;

        // 1. ПРОВЕРКА: Узнаем реальный размер картинки
        float spriteFullWidth = sr.sprite.bounds.size.x;
        
        // Если по какой-то причине размер 0 (ошибка импорта), ставим 1, чтобы не было деления на 0
        if (spriteFullWidth <= 0) spriteFullWidth = 1f;

        // 2. РАСЧЕТ: (Диаметр / Ширина спрайта)
        float finalScale = (radius * 2f) / spriteFullWidth;

        // 3. ЗАПУСК: Передаем это число в анимацию
        StartCoroutine(FlashRoutine(finalScale));
    }

    private IEnumerator FlashRoutine(float targetScale)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Color startColor = sr.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // ВНИМАНИЕ СЮДА: Масштаб должен расти до targetScale!
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * targetScale, t);

            Color c = startColor;
            c.a = Mathf.Lerp(startColor.a, 0f, t);
            sr.color = c;

            yield return null;
        }       
        pool.Release(this);
    }
}
