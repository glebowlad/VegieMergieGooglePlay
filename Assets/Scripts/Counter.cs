using TMPro;
using UnityEngine;
using System.Collections;

public class Counter : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    private int displayedScore = 0;
    public static int totalScore = 0;
    public static int totalMergedItems = 0;
    
    [SerializeField] private float duration = 0.5f;
    private Coroutine countCoroutine;

    private void Awake()
    {
        totalScore = 0;
        scoreText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        scoreText.text = "00000";
        Merge.Merged += OnVeggiesMerged;
    }

    private void OnDestroy()
    {
        Merge.Merged -= OnVeggiesMerged;
    }


    private void OnVeggiesMerged(int level)
    {
        int pointsToAdd = (level + 1) + (level * 2);
        totalScore += pointsToAdd;
        totalMergedItems++;
        Debug.Log($"totalMergedItems {totalMergedItems}");

        if (countCoroutine != null) StopCoroutine(countCoroutine);
        countCoroutine = StartCoroutine(AnimateScore(totalScore));
    }

    private IEnumerator AnimateScore(int targetScore)
    {
        int initialScore = displayedScore;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            displayedScore = (int)Mathf.Lerp(initialScore, targetScore, elapsed / duration);
            scoreText.text = displayedScore.ToString().PadLeft(5, '0');
            yield return null;
        }
       
    }
}
