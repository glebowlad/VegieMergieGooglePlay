using TMPro;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameOverCheck : MonoBehaviour
{
    [SerializeField] private Drag drag;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI scoreText;

    public static event Action GameIsOver;
    private Coroutine gameOverCoroutine;
    private int timer;
    private List<Collider2D> objectsInZone = new List<Collider2D>();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        objectsInZone.Add(collision);
        // Если это первый объект в зоне — запускаем обратный отсчет
        //if (objectsInZone.Count > 0 )
        //{
           // gameOverCoroutine = StartCoroutine(CheckGameOver());
        //}
    }

    //private void Update()
    //{
    //    if (objectsInZone.Count > 0 && gameOverCoroutine==null)
    //    {
    //        Debug.Log($"{objectsInZone.Count}");
    //        gameOverCoroutine = StartCoroutine(CheckGameOver());
    //    }
    //    else
    //    {
    //        if(gameOverCoroutine != null)
    //        {
    //            StopCoroutine(gameOverCoroutine);
    //        }
    //    }
    //}
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (objectsInZone.Count > 0)
        
            if (timer >= 70)
        {
            GameOver();
        }
        timer++;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (objectsInZone.Contains(collision))
        //{
            objectsInZone.Remove(collision);
        //    if (objectsInZone.Count== 0 )
        //    {
        //        StopCoroutine(gameOverCoroutine);
        //        gameOverCoroutine = null;
        //        Debug.Log("Timer OFF");
        //    }
        //}


    }

    private IEnumerator CheckGameOver()
    {
        yield return new WaitForSeconds(1.5f);
        if (objectsInZone.Count > 0)
        {
            GameOver();
        }
    }
    private void GameOver()
    {
        Debug.Log("!!!!!!GAME OVER");
        GameIsOver?.Invoke();
        scoreText.text = Counter.totalScore.ToString();
        gameOverPanel.SetActive(true);
        drag.enabled = false;
    }
}

