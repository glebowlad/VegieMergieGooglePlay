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
    [SerializeField] private Button musicBtn;
    [SerializeField] private Button sfxBtn;
    [SerializeField] private Image MusicButtonImage;
    [SerializeField] private Image SFXButtonImage;

    [Header("Sliders")]
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider SFXSlider;

    void Start()
    {
        // Подписка кнопок
        musicBtn.onClick.RemoveAllListeners();
        sfxBtn.onClick.RemoveAllListeners();
        musicBtn.onClick.AddListener(() => AudioManager.Instance.MusicMute());
        sfxBtn.onClick.AddListener(() => AudioManager.Instance.SFXMute());

        // Подписка слайдеров (динамическое изменение громкости)
        MusicSlider.onValueChanged.RemoveAllListeners();
        SFXSlider.onValueChanged.RemoveAllListeners();
        MusicSlider.onValueChanged.AddListener((val) => AudioManager.Instance.SetMusicVolume(val));
        SFXSlider.onValueChanged.AddListener((val) => AudioManager.Instance.SetSFXVolume(val));

        UpdateAppearance();
        AudioManager.Muted += UpdateAppearance;
        AudioManager.SettingsLoaded += UpdateAppearance;
    }

    void UpdateAppearance()
    {
        if (this == null || MusicButtonImage == null) return;

        // Музыка
        bool isMusicMuted = AudioManager.isMusicMuted;
        MusicButtonImage.sprite = isMusicMuted ? MusicOffSprite : MusicOnSprite;

        // ВСЕГДА ставим реальное значение громкости (оно не занулится при мьюте)
        MusicSlider.value = AudioManager.Instance.MusicVolume;
        // Просто запрещаем двигать слайдер, если звук выключен
        MusicSlider.interactable = !isMusicMuted;

        // SFX
        bool isSFXMuted = AudioManager.isSFXMuted;
        SFXButtonImage.sprite = isSFXMuted ? SFXOffSprite : SFXOnSprite;

        SFXSlider.value = AudioManager.Instance.SFXVolume;
        SFXSlider.interactable = !isSFXMuted;
    }

    private void OnDestroy()
    {
        AudioManager.Muted -= UpdateAppearance;
        AudioManager.SettingsLoaded -= UpdateAppearance;
    }
}
