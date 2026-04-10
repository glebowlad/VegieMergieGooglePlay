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
    [SerializeField] private AudioClip shakeSound;
    [SerializeField] private AudioClip[] mergeSounds;
    [SerializeField] private AudioClip[] dropSounds;

    // Теперь храним реальную громкость
    public float MusicVolume { get; private set; } = 1f;
    public float SFXVolume { get; private set; } = 1f;

    public static bool isMusicMuted = false;
    public static bool isSFXMuted = false;

    public static event Action Muted;

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

    public void PlayMergeSound(int level) => PlayRandomSfx(mergeSounds);
    public void PlayDropSound() => PlayRandomSfx(dropSounds);
    public void PlayShakeSound() { if (shakeSound != null) SFXSource?.PlayOneShot(shakeSound); }

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
