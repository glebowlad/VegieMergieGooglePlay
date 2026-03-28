using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class Vegetable : MonoBehaviour
{
    private Drag drag;
    private Rigidbody2D rb; 
    private GameObject gameOverLine;
    private GameObject gameOverPanel;
    private TextMeshProUGUI scoreText;

    public float radiusOffset = 0f;
    public static event Action GameIsOver;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.simulated = false;
    }

    public void Initialize(Drag _drag, GameObject _gameOverLine, GameObject _gameOverPanel)
    {
        gameOverPanel = _gameOverPanel;
        gameOverLine = _gameOverLine;
        drag = _drag;
        if (drag != null)
        {
            drag.WhileDrag += Move;
            drag.OnDragFinished += Drop;
        }
    }

    private void Move()
    {
        transform.position = transform.parent.position;
        StopAllCoroutines();
    }

    private void Drop()
    {
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        
        rb.simulated = true;
        AudioManager.Instance.PlayDropSound();
        if (drag != null)
        {
            drag.WhileDrag -= Move;
            drag.OnDragFinished -= Drop;
        }
        this.enabled = false;
        StartCoroutine(CheckGameOver());
    }

    private IEnumerator CheckGameOver()
    {
        yield return new WaitForSeconds(3f);
        if (gameObject.transform.position.y > gameOverLine.transform.position.y)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER");
        GameIsOver?.Invoke();
        scoreText = gameOverPanel.transform.GetChild(1).GetChild(0).GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        scoreText.text = Counter.totalScore.ToString();
        gameOverPanel.SetActive(true);
        drag.enabled = false;

    }
}
