using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;
    [SerializeField]
    private AudioClip[] mergeSounds;
    [SerializeField]
    private AudioClip[] dropSounds;
    [SerializeField]
    private Drag drag;
    private AudioSource source;
    public static bool isMuted;
    public static event Action Muted;
    void Awake()
    {

        if (Instance == null)
        { 
            Instance = this; 
        }
        else 
        { 
            Destroy(gameObject); 
        }
        DontDestroyOnLoad(gameObject);

        source = GetComponent<AudioSource>();
        isMuted=source.mute;
        Subscribe();
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
    private void OnDestroy()
    {
        
        Merge.Merged -= PlayMergeSound;
    }
}
