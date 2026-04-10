using TMPro;
using UnityEngine;
using System.Collections;

public class Counter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreTextInGame;
    [SerializeField] private TextMeshProUGUI scoreTextInFinish;
    [SerializeField] private ParticleSystem jackpotParticles;
    private int displayedScore = 0;
    public static int totalScore = 0;
    public static int totalMergedItems = 0;
    
    private float duration = 0.5f;
    private Coroutine countCoroutine;

    private void Awake()
    {
        totalScore = 0;
        scoreTextInGame.text = "00000";
        Merge.Merged += OnVeggiesMerged;
        GameOverCheck.GameIsOver += ShowScoreOnFinish;
    }

    private void OnDestroy()
    {
        Merge.Merged -= OnVeggiesMerged;
        GameOverCheck.GameIsOver -= ShowScoreOnFinish;
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
        RectTransform textRect = scoreTextInGame.GetComponent<RectTransform>();
        Vector3 originalScale = Vector3.one;

        float maxPunchScale = 1f + (level * level * 0.0030f); 

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            displayedScore = (int)Mathf.Lerp(initialScore, targetScore, t);
            scoreTextInGame.text = displayedScore.ToString().PadLeft(5, '0');

            float pulse = Mathf.Sin(t * Mathf.PI) * (maxPunchScale - 1f);
            textRect.localScale = originalScale * (1f + pulse);

            yield return null;
        }

        scoreTextInGame.text = targetScore.ToString().PadLeft(5, '0');
        scoreTextInGame.transform.localScale = originalScale;
    }
    private void ShowScoreOnFinish()
    {
        scoreTextInFinish.text = totalScore.ToString().PadLeft(5, '0');
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
