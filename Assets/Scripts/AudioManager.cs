using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;
    [SerializeField]
    private AudioClip bgSound;
    [SerializeField]
    private AudioClip[] mergeSounds;
    [SerializeField]
    private AudioClip[] dropSounds;

    [SerializeField] 
    private AudioClip shakeSound;
    [SerializeField] private Drag drag;

    public static AudioSource source;
    public static bool isMuted= false;
    public static event Action Muted;
    private static bool isStart = true;

    [SerializeField] private GameObject startPanel;
   
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

        source = GetComponent<AudioSource>();
        source.clip= bgSound;
        Subscribe();
    }
    void Start()
    {
        startPanel.SetActive(isStart);
        drag.enabled = false;
        if(!isStart) { return; }
        AudioListener.pause = true;
        if (source.isPlaying) { source.Stop(); }
    }

    public void StartGame()
    {
        isStart = false;
        AudioListener.pause = false;
        if (!source.isPlaying)
        {
            source.Play();
        }
        startPanel.SetActive(isStart);
        drag.enabled = true;

    }

    private void Subscribe()
    {
        
        Merge.Merged += PlayMergeSound;
    }
  
    
    public void Mute()
    {
        isMuted = !isMuted;
        source.mute = isMuted;
        Muted?.Invoke();
    }

    public void PlayMergeSound(int level)
    {
        if (mergeSounds == null || mergeSounds.Length == 0)
        {
            return;
        }
        AudioClip randomSound = mergeSounds[UnityEngine.Random.Range(0, mergeSounds.Length)];
        source.PlayOneShot(randomSound);
    }

    public void PlayDropSound()
    {
        if (dropSounds == null || dropSounds.Length == 0)
        {
            return;
        }
        AudioClip randomSound = dropSounds[UnityEngine.Random.Range(0, dropSounds.Length)];
        source.PlayOneShot(randomSound);
    }
    public void PlayShakeSound()
    {
        if (shakeSound == null )
        {
            return;
        }
        source.PlayOneShot(shakeSound);
    }
    private void OnDestroy()
    {
        
        Merge.Merged -= PlayMergeSound;
    }
}
