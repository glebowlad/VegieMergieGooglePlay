using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [Header("Links")]
    public RectTransform gameContainer;
    public Spawner spawner;
    public Counter counter;
    public BestScore bestScoreScript;

    private string savePath;

    [System.Serializable]
    public class VegData
    {
        public int prefabIndex;
        public Vector3 pos;
        public int type;
    }

    [System.Serializable]
    public class GameSaveData
    {
        public List<VegData> vegetables = new List<VegData>();
        public float mVol, sVol;
        public bool mMuted, sMuted;
        public int currentScore; // Имя должно совпадать везде
    }

    private void Awake()
    {
        savePath = Application.persistentDataPath + "/save.json";
        // Загружаем звук максимально рано
        PreloadAudioSettings();
    }

    private void PreloadAudioSettings()
    {
        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        // Если AudioManager еще не проснулся, ищем его принудительно
        var audioMan = AudioManager.Instance != null ? AudioManager.Instance : FindObjectOfType<AudioManager>();

        if (audioMan != null)
        {
            audioMan.InitLoadedSettings(data.mVol, data.sVol, data.mMuted, data.sMuted);
        }
    }

    private void Start()
    {
        Invoke("LoadOnlyVegetables", 0.1f);
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

        //Vegetable[] allVeg = FindObjectsOfType<Vegetable>();
        //foreach (var v in allVeg)
        //{
        //    if (v.gameObject.activeInHierarchy && v.transform.parent != spawner.transform)
        //    {
        //        string cleanName = v.name.Replace("(Clone)", "").Trim();
        //        int index = System.Array.FindIndex(spawner.GetPrefabsArray(), p => p.name == cleanName);

        //        if (index != -1)
        //        {
        //            RectTransform rt = v.GetComponent<RectTransform>();
        //            Vector3 posToSave = (rt != null) ? (Vector3)rt.anchoredPosition : v.transform.localPosition;

        //            data.vegetables.Add(new VegData { prefabIndex = index, pos =posToSave, type = (int)v.specialType });
        //            Debug.Log($"Сохранен: {cleanName} в позиции {posToSave}");
        //        }
        //        else
        //        {
        //            Debug.LogWarning($"Не нашел префаб с именем {cleanName} в списке Spawner!");
        //        }
        //    }
        //}
        File.WriteAllText(savePath, JsonUtility.ToJson(data));
    }

    public void LoadOnlyVegetables()
    {
        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        
        //if (counter != null) counter.LoadSavedScore(data.currentScore);
        //if (bestScoreScript != null) bestScoreScript.UpdateBestScore();

        //foreach (var v in data.vegetables)
        //{
        //    GameObject prefab = spawner.GetPrefabsArray()[v.prefabIndex];
        //    if (prefab == null) continue;

        //    GameObject newObj = Instantiate(prefab);
        //    Rigidbody2D rb = newObj.GetComponent<Rigidbody2D>();
        //    if (rb != null) rb.simulated = false;
        //    newObj.transform.SetParent(gameContainer, false);

        //    RectTransform rt = newObj.GetComponent<RectTransform>();
        //    if (rt != null)
        //    {
        //        // Устанавливаем сохраненные UI-координаты
        //        rt.anchoredPosition = new Vector2(v.pos.x, v.pos.y);
        //        rt.localScale = Vector3.one;
        //        rt.localRotation = Quaternion.identity;
        //        // Обнуляем Z
        //        rt.anchoredPosition3D = new Vector3(rt.anchoredPosition.x, rt.anchoredPosition.y, 0f);
        //    }

        //    var veg = newObj.GetComponent<Vegetable>();
        //    if (veg != null)
        //    {
        //        veg.HardResetForPool();
        //        veg.LoadState((Vegetable.VegetableType)v.type);
        //    }
        //    if (rb != null) rb.simulated = true;
        //    newObj.transform.SetAsLastSibling();
        //}
    }

    private void OnApplicationPause(bool pause) { if (pause) SaveGame(); }
    private void OnApplicationQuit() { SaveGame(); }
}