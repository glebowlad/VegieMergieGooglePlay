using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    [SerializeField] private BestScore bestScore;
    
    public void Restart()
    {
        if (bestScore != null) { bestScore.CheckBestScore(); }
        SceneManager.LoadScene(0);
        // реклама
        //InterstitialAdvShow();
    }
}
