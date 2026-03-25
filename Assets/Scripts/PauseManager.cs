using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // Перетащите сюда панель паузы в инспекторе

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
        Time.timeScale = 0f; // Останавливает время в игре
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Запускает время снова
    }
}
