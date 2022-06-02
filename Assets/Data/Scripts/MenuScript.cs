using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;
using Newtonsoft.Json;

public class MenuScript : MonoBehaviour
{
    public LevelConfig _levelConfig;
    private AudioSource _audioSource;
    public List<Item> ItemsPrefabs;
    public SpriteRenderer Background;
    public List<Sprite> Backgrounds;

    private IEnumerator Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if(PlayerPrefs.HasKey("IsMusic") && PlayerPrefs.GetInt("IsMusic") == 0)
            _audioSource.enabled = false;

        Background.sprite = Backgrounds[Random.Range(0, Backgrounds.Count - 1)];
        Instantiate(ItemsPrefabs[Random.Range(0, ItemsPrefabs.Count - 1)], 
            new Vector3(0, 1.8f), Quaternion.identity).transform.localScale = new Vector3(2, 2);
        SetLastConfig();
        yield return LocalizationSettings.InitializationOperation;
        if (PlayerPrefs.HasKey("GameLanguage"))
        {
            if(PlayerPrefs.GetString("GameLanguage") == "ru")
                SetRussinLanguage();
            else
                SetEnglishLanguage();
        }
    }

    public void SetRussinLanguage()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
        PlayerPrefs.SetString("GameLanguage", "ru");
    }

    public void SetEnglishLanguage()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        PlayerPrefs.SetString("GameLanguage", "en");
    }

    public void SetLastConfig()
    {
        try
        {
            using (StreamReader sr = new StreamReader(Application.persistentDataPath + "/Tiles.json"))
            {
                if (sr != null)
                {
                    _levelConfig.AllTiles = JsonConvert.DeserializeObject<LevelConfig.TileInfo[,]>(sr.ReadToEnd());
                    _levelConfig.IsConfigured = true;
                }
            }
            using (StreamReader sr = new StreamReader(Application.persistentDataPath + "/Barriers.json"))
            {
                if (sr != null)
                {
                    _levelConfig.AllBariers = JsonConvert.DeserializeObject<LevelConfig.BarrierInfo[,]>(sr.ReadToEnd());
                }
            }
            using (StreamReader sr = new StreamReader(Application.persistentDataPath + "/LevelInfo.json"))
            {
                if (sr != null)
                {
                    LevelConfig.LevelInfo info = JsonConvert.DeserializeObject<LevelConfig.LevelInfo>(sr.ReadToEnd());
                    _levelConfig.X = info.X;
                    _levelConfig.Y = info.Y;
                    _levelConfig.Steps = info.Steps;
                    _levelConfig.ItemQuest = info.ItemQuest;
                    _levelConfig.ScoreQuest = info.ScoreQuest;
                    _levelConfig.BarrierQuest = info.BarrierQuest;
                }
            }
        }
        catch (System.Exception)
        {
            _levelConfig.IsConfigured = false;
        }
    }

    public void PlayRandomLevel()
    {
        _levelConfig.IsConfigured = false;
        SceneManager.LoadScene("GameScene");
    }

    public void ConfigureLevel()
    {
        SceneManager.LoadScene("EditorScene");
    }
}