using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;
using Newtonsoft.Json;

public class MenuScript : MonoBehaviour
{
    [Header("UI ��������")]
    [SerializeField] private Button RandomLevelButton;
    [SerializeField] private Button ConfigureLevelButton;
    [SerializeField] private Button RussianLanguageButton;
    [SerializeField] private Button EnglishLanguageButton;
    [Space]

    public LevelConfig _levelConfig;
    private AudioSource _audioSource;
    public List<Item> ItemsPrefabs;
    public SpriteRenderer Background;
    public List<Sprite> Backgrounds;


    private IEnumerator Start()
    {
        AddUIListeners();
        Settings.InitializeSettings();
        _audioSource = Camera.main.GetComponent<AudioSource>();
        if(!Settings.IsMusic)
            _audioSource.enabled = false;

        Background.sprite = Backgrounds[Random.Range(0, Backgrounds.Count - 1)];
        Instantiate(ItemsPrefabs[Random.Range(0, ItemsPrefabs.Count - 1)], 
            new Vector3(0, 1.8f), Quaternion.identity).transform.localScale = new Vector3(2, 2);
        SetLastConfig();
        yield return LocalizationSettings.InitializationOperation;
        if (Settings.Language == "ru")
            SetRussinLanguage();
        else
            SetEnglishLanguage();
    }

    private void AddUIListeners()
    {
        RandomLevelButton.onClick.AddListener(PlayRandomLevel);
        ConfigureLevelButton.onClick.AddListener(ConfigureLevel);
        RussianLanguageButton.onClick.AddListener(SetRussinLanguage);
        EnglishLanguageButton.onClick.AddListener(SetEnglishLanguage);
    }

    private void SetRussinLanguage()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
        Settings.Language = "ru";
    }

    private void SetEnglishLanguage()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        Settings.Language = "en";
    }

    private void SetLastConfig()
    {
        try
        {
            using (var sr = new StreamReader(Application.persistentDataPath + "/Tiles.json"))
            {
                if (sr != null)
                {
                    _levelConfig.AllTiles = JsonConvert.DeserializeObject<LevelConfig.TileInfo[,]>(sr.ReadToEnd());
                    _levelConfig.IsConfigured = true;
                }
            }
            
            using (var sr = new StreamReader(Application.persistentDataPath + "/Barriers.json"))
                if (sr != null)
                    _levelConfig.AllBariers = JsonConvert.DeserializeObject<LevelConfig.BarrierInfo[,]>(sr.ReadToEnd());
            
            using (var sr = new StreamReader(Application.persistentDataPath + "/LevelInfo.json"))
            {
                if (sr == null)
                    return;
                
                var info = JsonConvert.DeserializeObject<LevelConfig.LevelInfo>(sr.ReadToEnd());
                _levelConfig.X = info.X;
                _levelConfig.Y = info.Y;
                _levelConfig.Steps = info.Steps;
                _levelConfig.ItemQuest = info.ItemQuest;
                _levelConfig.ScoreQuest = info.ScoreQuest;
                _levelConfig.BarrierQuest = info.BarrierQuest;
            }
        }
        catch (System.Exception)
        {
            _levelConfig.IsConfigured = false;
        }
    }

    private void PlayRandomLevel()
    {
        _levelConfig.IsConfigured = false;
        SceneManager.LoadScene("GameScene");
    }

    private void ConfigureLevel()
    {
        SceneManager.LoadScene("EditorScene");
    }

    private void RemoveUIListeners()
    {
        RandomLevelButton.onClick.RemoveAllListeners();
        ConfigureLevelButton.onClick.RemoveAllListeners();
        RussianLanguageButton.onClick.RemoveAllListeners();
        EnglishLanguageButton.onClick.RemoveAllListeners();
    }

    private void OnDestroy()
    {
        RemoveUIListeners();
        Settings.WriteSettings();
    }
}