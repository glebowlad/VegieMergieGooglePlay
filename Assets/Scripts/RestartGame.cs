using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    [SerializeField] private BestScore bestScore;
    [SerializeField] private PauseManager pauseManager;
    public void Restart()
    {
        if (bestScore != null) { bestScore.CheckBestScore(); }
        AdsManager.Instance.ShowAdOnRestart();
        SceneManager.LoadScene(0);
        pauseManager.ResumeGame();

        
    }
}
