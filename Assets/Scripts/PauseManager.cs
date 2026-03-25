using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; 

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
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; 
        drag.enabled = true;
    }
}
