using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;
    [SerializeField] 
    private Drag drag;
    [SerializeField] 
    private GameObject startPanel;

    [Header("Clips")]
    [SerializeField]
    private AudioClip bgSound;
    [SerializeField] 
    private AudioClip shakeSound;
    [SerializeField]
    private AudioClip[] mergeSounds;
    [SerializeField]
    private AudioClip[] dropSounds;

    [Header("Audio Sources")]
    [SerializeField ] private  AudioSource MusicSource;
    [SerializeField] private AudioSource SFXSource;

    public float MusicVolume {  get; private set; }
    public float SFXVolume { get; private set; }
    public static bool isMusicMuted= false;
    public static bool isSFXMuted = false;

    public static event Action Muted;
    //private static bool isStart = true;

   
    void Awake()
    {

        if (Instance == null)
        { 
            Instance = this; 
        }
        else 
        { 
            Destroy(gameObject); 
            return; 
        }
        DontDestroyOnLoad(gameObject);

        MusicSource.clip= bgSound;
        Subscribe();
    }
    void Start()
    {
        //startPanel.SetActive(isStart);
        //drag.enabled = false;
       // if(!isStart) { return; }
        //AudioListener.pause = true;
        //if (MusicSource.isPlaying) 
        SFXVolume =SFXSource.volume;
        MusicVolume=MusicSource.volume;
            MusicSource.Play(); 
    }

    //public void StartGame()
    //{
    //    //isStart = false;
    //    //AudioListener.pause = false;
    //    if (!MusicSource.isPlaying)
    //    {
    //        MusicSource.Play();
    //    }
    //    startPanel.SetActive(isStart);
    //    drag.enabled = true;

    //}

    private void Subscribe()
    {
        
        Merge.Merged += PlayMergeSound;
    }
  
    
    public void MusicMute()
    {
        isMusicMuted = !isMusicMuted;
        MusicSource.mute = isMusicMuted;
        Muted?.Invoke();
    }

    public void SFXMute()
    {
        isSFXMuted = !isSFXMuted;
        SFXSource.mute = isSFXMuted;
        Muted?.Invoke();
    }

    public void PlayMergeSound(int level)
    {
        if (mergeSounds.Length > 0)
            PlayRandomSfx(mergeSounds);
    }

    public void PlayDropSound()
    {
        if (dropSounds.Length > 0)
            PlayRandomSfx(dropSounds);
    }

    private void PlayRandomSfx(AudioClip[] clips)
    {
        AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        SFXSource.PlayOneShot(clip);
    }

    public void PlayShakeSound()
    {
        if (shakeSound != null)
            SFXSource.PlayOneShot(shakeSound);
    }

    private void OnDestroy()
    {
        
        Merge.Merged -= PlayMergeSound;
    }
}
