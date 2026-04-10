using UnityEngine;
using UnityEngine.UI;

public class SoundImage : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite MusicOnSprite;
    [SerializeField] private Sprite MusicOffSprite;
    [SerializeField] private Sprite SFXOnSprite;
    [SerializeField] private Sprite SFXOffSprite;
    [Header("Buttons")]
    [SerializeField] private Image MusicButtonImage;
    [SerializeField] private Image SFXButtonImage;

    void Start()
    {
        UpdateAppearance();
        AudioManager.Muted += UpdateAppearance;

    }

    public void Mute()
    {
        AudioManager.Instance.MusicMute();
    }
    void UpdateAppearance()
    {
        if (AudioManager.isMusicMuted)
        {
            MusicButtonImage.sprite = MusicOffSprite;
        }
        else
        {
            MusicButtonImage.sprite = MusicOnSprite;
        }
        if (AudioManager.isSFXMuted)
        {
            SFXButtonImage.sprite = SFXOffSprite;
        }
        else
        {
            SFXButtonImage.sprite = SFXOnSprite;
        }
    }
    private void OnDestroy()
    {
        AudioManager.Muted -= UpdateAppearance;
    }

    
}
