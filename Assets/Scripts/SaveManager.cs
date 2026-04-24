using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private string savePath;
    public static Action<GameSaveData> OnDataLoaded;

    [System.Serializable]
    public class GameSaveData
    {
        public float mVol, sVol;
        public bool mMuted, sMuted;
        public int currentScore; 
    }

    private void Awake()
    {
        savePath = Application.persistentDataPath + "/save.json";
       
    }
    void Start()
    {
        LoadAndNotify();
    }

    private void LoadAndNotify()
    {
        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
        OnDataLoaded?.Invoke(data);
    }

    public GameSaveData GetSavedData()
    {
        if (!File.Exists(savePath)) return null;
        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<GameSaveData>(json);
    }
    public void SaveGame()
    {
        GameSaveData data = new GameSaveData();
        if (AudioManager.Instance != null)
        {
            data.mVol = AudioManager.Instance.MusicVolume;
            data.sVol = AudioManager.Instance.SFXVolume;
            data.mMuted = AudioManager.isMusicMuted;
            data.sMuted = AudioManager.isSFXMuted;
        }

        data.currentScore = Counter.totalScore;
        File.WriteAllText(savePath, JsonUtility.ToJson(data));
    }

    public void LoadOnlyVegetables()
    {
        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
    }

    private void OnApplicationPause(bool pause) { if (pause) SaveGame(); }
    private void OnApplicationQuit() { SaveGame(); }
}