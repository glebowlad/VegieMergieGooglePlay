using TMPro;
using UnityEngine;
using System.Collections;

public class Counter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private ParticleSystem jackpotParticles;
    private int displayedScore = 0;
    public static int totalScore = 0;
    public static int totalMergedItems = 0;
    
    private float duration = 0.5f;
    private Coroutine countCoroutine;

    private void Awake()
    {
        totalScore = 0;
        scoreText.text = "00000";
        Merge.Merged += OnVeggiesMerged;
    }

    private void OnDestroy()
    {
        Merge.Merged -= OnVeggiesMerged;
    }


    private void OnVeggiesMerged(int level)
    {
        int pointsToAdd = 0;
        if (level < 7) 
        {
            pointsToAdd = (level + 1) * (level + 2); 
        }
        else 
        {
            int[] elitePoints = { 100, 200, 500 }; 
            pointsToAdd = elitePoints[level - 7];
            TriggerJackpot(level);
        }

        totalScore += pointsToAdd;

        if (countCoroutine != null) StopCoroutine(countCoroutine);
        countCoroutine = StartCoroutine(AnimateScore(totalScore, level));
    }


    private IEnumerator AnimateScore(int targetScore, int level)
    {
        int initialScore = displayedScore;
        float elapsed = 0f;
        RectTransform textRect = scoreText.GetComponent<RectTransform>();
        Vector3 originalScale = Vector3.one;

        float maxPunchScale = 1f + (level * level * 0.0030f); 

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            displayedScore = (int)Mathf.Lerp(initialScore, targetScore, t);
            scoreText.text = displayedScore.ToString().PadLeft(5, '0');

            float pulse = Mathf.Sin(t * Mathf.PI) * (maxPunchScale - 1f);
            textRect.localScale = originalScale * (1f + pulse);

            yield return null;
        }

        scoreText.text = targetScore.ToString().PadLeft(5, '0');
        scoreText.transform.localScale = originalScale;
    }


    private void TriggerJackpot(int level)
    {
        if (jackpotParticles != null)
        {
            // Чем выше уровень, тем больше частиц (например, для тыквы - ливень)
            var main = jackpotParticles.main;
            int count = (level - 6) * 40; 

            jackpotParticles.Emit(count); 
        }
    }

}
