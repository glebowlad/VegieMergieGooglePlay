using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; 
    public WormController worm;

    public  Drag drag;
    // Метод срабатывает при сворачивании (pauseStatus = true) 
    // и при разворачивании (pauseStatus = false)
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            EnablePause();
        }
    }
    
    public void EnablePause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        drag.enabled = false;

        worm.EnablePauseMode();
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; 
        drag.enabled = true;

        worm.DisablePauseMode();
    }
}
