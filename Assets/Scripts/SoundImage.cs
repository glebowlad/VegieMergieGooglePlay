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
    [Header("Sliders")]
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider SFXSlider;

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
        bool isMusicMuted = AudioManager.isMusicMuted;
        MusicButtonImage.sprite = isMusicMuted ? MusicOffSprite : MusicOnSprite;

        MusicSlider.value = isMusicMuted ? 0 : AudioManager.Instance.MusicVolume;
        MusicSlider.interactable = !isMusicMuted;

        bool isSFXMuted = AudioManager.isSFXMuted;
        SFXButtonImage.sprite = isSFXMuted ? SFXOffSprite : SFXOnSprite;

        SFXSlider.value = isSFXMuted ? 0 : AudioManager.Instance.SFXVolume;
        SFXSlider.interactable = !isSFXMuted;
    }
    private void OnDestroy()
    {
        AudioManager.Muted -= UpdateAppearance;
    }

    
}
