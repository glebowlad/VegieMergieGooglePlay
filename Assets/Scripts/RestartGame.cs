using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    [SerializeField] private BestScore bestScore;
    [SerializeField] private PauseManager pauseManager;
    [SerializeField] public SaveManager saveManager;
    public void Restart()
    {
        if (bestScore != null) { bestScore.CheckBestScore(); }
        AdsManager.Instance?.ShowAdOnRestart();
        Counter.totalScore = 0;
        saveManager.SaveGame();
        SceneManager.LoadScene(0);
        pauseManager.ResumeGame();

        
    }
}
