using UnityEngine;
using UnityEngine.UI;

public class FpsSwitcher : MonoBehaviour
{
    public Button button30;
    public Button button60;

    public Image image30;
    public Image image60;

    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(1f, 1f, 1f, 0.55f);

    private const string KEY = "target_fps";
    private int currentFps;

    void Start()
    {
        button30.onClick.AddListener(On30Click);
        button60.onClick.AddListener(On60Click);

        Apply(PlayerPrefs.GetInt(KEY, 60));
    }

    void On30Click()
    {
        if (currentFps == 30) return;
        Apply(30);
    }

    void On60Click()
    {
        if (currentFps == 60) return;
        Apply(60);
    }

    void Apply(int fps)
    {
        currentFps = fps;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = fps;

        PlayerPrefs.SetInt(KEY, fps);
        PlayerPrefs.Save();

        bool is30 = fps == 30;

        if (image30 != null) image30.color = is30 ? activeColor : inactiveColor;
        if (image60 != null) image60.color = is30 ? inactiveColor : activeColor;
    }
}