using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;
    [Header("Sources")]
    [SerializeField] private AudioSource MusicSource;
    [SerializeField] private AudioSource SFXSource;

    [Header("Clips")]
    [SerializeField] private AudioClip bgSound;
    [SerializeField]private AudioClip lastVegSound;
    [SerializeField] private AudioClip finishSound;
    [SerializeField] private AudioClip shakeSound;
    [SerializeField] private AudioClip[] mergeSounds;
    [SerializeField] private AudioClip[] dropSounds;

    // Теперь храним реальную громкость
    public float MusicVolume { get; private set; } 
    public float SFXVolume { get; private set; }

    public static bool isMusicMuted = false;
    public static bool isSFXMuted = false;

    public static event Action Muted;
    public static event Action SettingsLoaded;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Инициализация при самом первом запуске игры
            MusicVolume = MusicSource != null ? MusicSource.volume : 1f;
            SFXVolume = SFXSource != null ? SFXSource.volume : 1f;

            if (MusicSource != null && bgSound != null)
            {
                MusicSource.clip = bgSound;
                MusicSource.Play();
            }
            Subscribe();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

   

    public void InitLoadedSettings(float mVol, float sVol, bool mMuted, bool sMuted)
    {
        MusicVolume = mVol;
        SFXVolume = sVol;
        isMusicMuted = mMuted;
        isSFXMuted = sMuted;

        ApplySettings();
        SettingsLoaded?.Invoke(); // Уведомляем SoundImage, что нужно обновить слайдеры
    }

    // Метод для изменения громкости музыки из слайдера
    public void SetMusicVolume(float volume)
    {
        MusicVolume = volume;
        if (!isMusicMuted && MusicSource != null)
            MusicSource.volume = volume;
    }

    // Метод для изменения громкости эффектов из слайдера
    public void SetSFXVolume(float volume)
    {
        SFXVolume = volume;
        if (!isSFXMuted && SFXSource != null)
            SFXSource.volume = volume;
    }

    public void ApplySettings()
    {
        if (MusicSource != null)
        {
            MusicSource.mute = isMusicMuted;
            MusicSource.volume = isMusicMuted ? 0 : MusicVolume;
        }
        if (SFXSource != null)
        {
            SFXSource.mute = isSFXMuted;
            SFXSource.volume = isSFXMuted ? 0 : SFXVolume;
        }
    }

    public void MusicMute()
    {
        isMusicMuted = !isMusicMuted;
        ApplySettings();
        Muted?.Invoke();
    }

    public void SFXMute()
    {
        isSFXMuted = !isSFXMuted;
        ApplySettings();
        Muted?.Invoke();
    }
    public void PlayLastVegSound() { if (lastVegSound != null) SFXSource?.PlayOneShot(lastVegSound); }
    public void PlayMergeSound(int level)
    {
        if (level == 9)
        {
            PlayLastVegSound();
        }
        else
        {
            PlayRandomSfx(mergeSounds);

        }
    }
    public void PlayDropSound() => PlayRandomSfx(dropSounds);
    public void PlayShakeSound() { if (shakeSound != null) SFXSource?.PlayOneShot(shakeSound); }
    public void PlayFinishSound() { if (shakeSound != null) SFXSource?.PlayOneShot(finishSound); }
    public void PlayEffectSound(AudioClip effect) { if (effect != null) SFXSource?.PlayOneShot(effect); }
    private void PlayRandomSfx(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0 || SFXSource == null || isSFXMuted) return;
        AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        SFXSource.PlayOneShot(clip, SFXVolume); // Проигрываем с учетом громкости
    }

    private void Subscribe() => Merge.Merged += PlayMergeSound;

    private void OnDestroy()
    {
        if (Instance == this) Merge.Merged -= PlayMergeSound;
    }
}
