using UnityEngine;

public class AppInit : MonoBehaviour
{
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = PlayerPrefs.GetInt("target_fps", 30);
    }
}