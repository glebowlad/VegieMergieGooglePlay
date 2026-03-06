using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BestScore : MonoBehaviour
{
    private int bestScore = 0;

    private TextMeshProUGUI bestScoreSceneText;
    [SerializeField] private TextMeshProUGUI bestScoreGOText;
    void Awake()
    {
        bestScoreSceneText = GetComponentInChildren<TextMeshProUGUI>();
        LoadBestScore();
        Vegetable.GameIsOver += CheckBestScore;
    }


    private void OnDestroy()
    {
        Vegetable.GameIsOver -= CheckBestScore;
    }
    private void LoadBestScore()
    {
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
       ShowBestScore();
    }

    private void ShowBestScore()
    {
        bestScoreSceneText.text = bestScore.ToString().PadLeft(5, '0');
        bestScoreGOText.text = bestScore.ToString().PadLeft(5, '0');
        
    }
    private void SaveBestScore()
    {
        PlayerPrefs.SetInt("BestScore", bestScore);
        PlayerPrefs.Save();
    }
    // метод для сброса рекорда для тестов
    public void ResetScore()
    {
        PlayerPrefs.SetInt("BestScore", 0);
        PlayerPrefs.Save();
        ShowBestScore();
    }
    private void CheckBestScore()
    {
        int currentScore = Counter.totalScore;
        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            SaveBestScore();
        }
            ShowBestScore();
    }
}