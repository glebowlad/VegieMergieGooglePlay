using UnityEngine;
using UnityEngine.UI;

public class FpsSwitcher : MonoBehaviour
{
    public Button button30;
    public Button button60;
    public Image button30Image;
    public Image button60Image;

    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(1f, 1f, 1f, 0.55f);

    private const string FpsKey = "target_fps";

    void Start()
    {
        button30.onClick.AddListener(Set30);
        button60.onClick.AddListener(Set60);

        int fps = PlayerPrefs.GetInt(FpsKey, 30);
        ApplyFps(fps);
    }

    public void Set30()
    {
        ApplyFps(30);
    }

    public void Set60()
    {
        ApplyFps(60);
    }

    void ApplyFps(int fps)
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = fps;

        PlayerPrefs.SetInt(FpsKey, fps);
        PlayerPrefs.Save();

        UpdateVisual(fps);
    }

    void UpdateVisual(int fps)
    {
        bool is30 = fps == 30;

        if (button30Image != null)
            button30Image.color = is30 ? activeColor : inactiveColor;

        if (button60Image != null)
            button60Image.color = is30 ? inactiveColor : activeColor;
    }
}