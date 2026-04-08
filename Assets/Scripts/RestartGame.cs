using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    [SerializeField] private BestScore bestScore;
    
    public void Restart()
    {
        if (bestScore != null) { bestScore.CheckBestScore(); }
        AdsManager.Instance.ShowAdOnRestart();
        SceneManager.LoadScene(0);
        
    }
}
