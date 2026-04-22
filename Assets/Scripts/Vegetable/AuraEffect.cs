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

    public void PlayFlash(Sprite sprite, Color color, float radius, AuraAnimType animType)
    {
        if (sr == null) return;
        if (sprite == null) return;
        if (radius <= 0f) return;
        StopAllCoroutines();
        sr.sprite = sprite;
        sr.color = color;

        float spriteWidth = sr.sprite.bounds.size.x;
        float targetScale = (radius * 2f) / spriteWidth;

        StartCoroutine(FlashRoutine(targetScale, animType));
    }

    private IEnumerator FlashRoutine(float targetScale, AuraAnimType animType)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Color startColor = sr.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // --- ГИБКАЯ ЛОГИКА АНИМАЦИИ ---
            switch (animType)
            {
                case AuraAnimType.SharpRotate:
                    float swingEnd = 0.3f; // 30% времени на замах, 70% на удар

                    if (t <= swingEnd)
                    {
                        float t1 = t / swingEnd;
                        transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * targetScale, t1);
                        // Замах: от 45 еще дальше назад до 80 градусов
                        float angle = Mathf.Lerp(45f, 80f, t1);
                        transform.rotation = Quaternion.Euler(0, 0, angle);
                        
                        // В фазе замаха коса полностью непрозрачна
                        sr.color = startColor;
                    }
                    else
                    {
                        float t2 = (t - swingEnd) / (1f - swingEnd);
                        // Удар: от 80 до -135. Используем t2 * t2 для ускорения
                        float strikeAngle = Mathf.Lerp(80f, -135f, t2 * t2);
                        transform.rotation = Quaternion.Euler(0, 0, strikeAngle);

                        // ИСЧЕЗНОВЕНИЕ: Начинаем таять только после 70% пути удара
                        float alphaT = Mathf.Clamp01((t2 - 0.7f) / 0.3f); 
                        Color c = startColor;
                        c.a = Mathf.Lerp(startColor.a, 0f, alphaT);
                        sr.color = c;
                    }
                    break;

                case AuraAnimType.Pulse:
                    // Фаза расширения (первые 40% времени) — делаем её более плавной
                    float growEnd = 0.4f; 
                    
                    if (t <= growEnd)
                    {
                        float t1 = t / growEnd;
                        // Плавное расширение с использованием SmoothStep для мягкости
                        float smoothT = Mathf.SmoothStep(0f, 1f, t1);
                        transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * targetScale, smoothT);
                        
                        sr.color = startColor;
                    }
                    else
                    {
                        // Фаза удержания и вибрации (оставшиеся 60% времени)
                        float t2 = (t - growEnd) / (1f - growEnd);
                        
                        // Вибрация: очень быстрая, но мелкоамплитудная
                        // Мы используем Sin от времени, чтобы создать "марево"
                        float shake = Mathf.Sin(Time.time * 50f) * 0.02f; 
                        transform.localScale = Vector3.one * targetScale * (1f + shake);
                        
                        // Плавное затухание начинается только во второй половине этой фазы
                        float alphaT = Mathf.Clamp01((t2 - 0.2f) / 0.8f);
                        Color c = startColor;
                        c.a = Mathf.Lerp(startColor.a, 0f, alphaT);
                        sr.color = c;
                    }
                    
                    // Медленное вращение добавляет эффекту объема
                    transform.Rotate(0, 0, 20f * Time.deltaTime);
                    break;

                case AuraAnimType.MagicSpell:
                    float splitPoint = 0.6f; // Точка перехода от роста к рассеиванию

                    if (t <= splitPoint)
                    {
                        // ФАЗА 1 (0.6): Плавный рост и проявление
                        float t1 = t / splitPoint;
                        // Используем SmoothStep для максимальной плавности
                        float smoothT1 = Mathf.SmoothStep(0f, 1f, t1);
                        
                        // Растем от 0.4 до 1.0 (чтобы не прыгало из точки)
                        transform.localScale = Vector3.one * targetScale * Mathf.Lerp(0.4f, 1.0f, smoothT1);
                        
                        // Проявляемся до 0.7 прозрачности
                        Color c = startColor;
                        c.a = Mathf.Lerp(0f, 0.7f, smoothT1);
                        sr.color = c;
                    }
                    else
                    {
                        // ФАЗА 2 (0.4): Рассеивание
                        float t2 = (t - splitPoint) / (1f - splitPoint);
                        // Используем t2 * t2 для ускорения ухода (эффект взрывной волны)
                        float accelT2 = t2 * t2;

                        // Расширяемся от 1.0 до 4.0
                        transform.localScale = Vector3.one * targetScale * Mathf.Lerp(1.0f, 4.0f, accelT2);
                        
                        // Растворяемся от 0.7 до 0
                        Color c = startColor;
                        c.a = Mathf.Lerp(0.7f, 0f, accelT2);
                        sr.color = c;
                    }
                    break;


                default:
                    float sharpT = Mathf.Pow(t, 0.4f); 
                    transform.localScale = Vector3.one * targetScale * sharpT;
                    Color iceColor = startColor;
                    iceColor.a = Mathf.Lerp(startColor.a, 0f, t * t * t);               
                    sr.color = iceColor;
                    break;
            }
            yield return null;
        }
        pool.Release(this);
    }

}
