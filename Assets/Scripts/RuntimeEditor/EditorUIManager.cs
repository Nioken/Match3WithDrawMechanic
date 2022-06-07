using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;
using TMPro;
using System.Globalization;

public class EditorUIManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _toMenuButton;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _showQuestUIButton;
    [SerializeField] private Button _hideQuestUIButton;
    [SerializeField] private Button _rockButton;
    [SerializeField] private Button _rockBrockenButton;
    [SerializeField] private Button _iceButton;
    [SerializeField] private Button _iceBrockenButton;
    [Space]

    [SerializeField] private SpriteRenderer _background;
    [SerializeField] private List<Sprite> _backgrounds;

    [SerializeField] private TMP_Text _heightText;
    [SerializeField] private TMP_Text _widthText;

    [SerializeField] private LevelConfig _levelConfig;
    [SerializeField] private EditorTileGenerator _editorTileGenerator;
    public static EditorUIManager EditorUI;

    public Slider HeightSlider;
    public Slider WidthSlider;
    public TMP_InputField StepsField;

    [Header("UI Квествов")]
    public GameObject QuestSelectUI;
    public Toggle ScoreQuestToggle;
    public Toggle ItemQuestToggle;
    public Toggle BarrierQuestToggle;

    private void OnEnable()
    {
        EditorUI = GetComponent<EditorUIManager>();
        AddUIListeners();
    }

    private void Start()
    {
        _background.sprite = _backgrounds[UnityEngine.Random.Range(0, _backgrounds.Count)];
    }

    private void AddUIListeners()
    {
        ScoreQuestToggle.onValueChanged.AddListener((bool isOn) => UpdateScoreQuestToggle());
        ScoreQuestToggle.onValueChanged.AddListener((bool isOn) => _editorTileGenerator.UpdateConfig());
        ItemQuestToggle.onValueChanged.AddListener((bool isOn) => UpdateItemQuestToggle());
        ItemQuestToggle.onValueChanged.AddListener((bool isOn) => _editorTileGenerator.UpdateConfig());
        BarrierQuestToggle.onValueChanged.AddListener((bool inOn) => UpdateBarrierQuestToggle());
        BarrierQuestToggle.onValueChanged.AddListener((bool isOn) => _editorTileGenerator.UpdateConfig());
        HeightSlider.onValueChanged.AddListener((float value) => UpdateHeight());
        HeightSlider.onValueChanged.AddListener((float value) => _editorTileGenerator.UpdateSize());
        WidthSlider.onValueChanged.AddListener((float value) => UpdateWidth());
        WidthSlider.onValueChanged.AddListener((float value) => _editorTileGenerator.UpdateSize());
        _toMenuButton.onClick.AddListener(ToMenu);
        _playButton.onClick.AddListener(PlayWithConfig);
        _showQuestUIButton.onClick.AddListener(ShowQuestSelect);
        _hideQuestUIButton.onClick.AddListener(HideQuestSelect);
        _rockButton.onClick.AddListener(() => UpdateCurrentBarrier(_editorTileGenerator.BarriersPrefabs[0]));
        _rockBrockenButton.onClick.AddListener(() => UpdateCurrentBarrier(_editorTileGenerator.BarriersPrefabs[1]));
        _iceButton.onClick.AddListener(() => UpdateCurrentBarrier(_editorTileGenerator.BarriersPrefabs[2]));
        _iceBrockenButton.onClick.AddListener(() => UpdateCurrentBarrier(_editorTileGenerator.BarriersPrefabs[3]));
        StepsField.onValueChanged.AddListener((string value) => UpdateSteps());
        StepsField.onValueChanged.AddListener((string value) => _editorTileGenerator.UpdateConfig());
    }

    private void RemoveUIListeners()
    {
        ScoreQuestToggle.onValueChanged.RemoveAllListeners();
        ScoreQuestToggle.onValueChanged.RemoveAllListeners();
        ItemQuestToggle.onValueChanged.RemoveAllListeners();
        ItemQuestToggle.onValueChanged.RemoveAllListeners();
        BarrierQuestToggle.onValueChanged.RemoveAllListeners();
        BarrierQuestToggle.onValueChanged.RemoveAllListeners();
        HeightSlider.onValueChanged.RemoveAllListeners();
        HeightSlider.onValueChanged.RemoveAllListeners();
        WidthSlider.onValueChanged.RemoveAllListeners();
        WidthSlider.onValueChanged.RemoveAllListeners();
        _toMenuButton.onClick.RemoveAllListeners();
        _playButton.onClick.RemoveAllListeners();
        _showQuestUIButton.onClick.RemoveAllListeners();
        _hideQuestUIButton.onClick.RemoveAllListeners();
        _rockButton.onClick.RemoveAllListeners();
        _rockBrockenButton.onClick.RemoveAllListeners();
        _iceButton.onClick.RemoveAllListeners();
        _iceBrockenButton.onClick.RemoveAllListeners();
        StepsField.onValueChanged.RemoveAllListeners();
        StepsField.onValueChanged.RemoveAllListeners();
    }

    public void UpdateHeight()
    {
        _heightText.text =
            LocalizationSettings.SelectedLocale.LocaleName == "English (en)"
            ? $"Height: {HeightSlider.value.ToString(CultureInfo.InvariantCulture)}"

            : $"Высота: {HeightSlider.value.ToString(CultureInfo.InvariantCulture)}";

        TileGenerator.Height = (int)HeightSlider.value;
    }

    public void UpdateWidth()
    {
        _widthText.text =
            LocalizationSettings.SelectedLocale.LocaleName == "English (en)"
            ? $"Width: {WidthSlider.value.ToString(CultureInfo.InvariantCulture)}" 
            
            : $"Ширина: {WidthSlider.value.ToString(CultureInfo.InvariantCulture)}";

        TileGenerator.Width = (int)WidthSlider.value;
    }

    public void UpdateSteps()
    {
        EditorTileGenerator.Steps = Convert.ToInt32(StepsField.text);
    }

    public void UpdateCurrentBarrier(Barrier barrier)
    {
        EditorTileGenerator.CurrentBarrier = barrier;
    }

    public void ToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void ShowQuestSelect()
    {
        QuestSelectUI.SetActive(true);
    }
    
    public void UpdateScoreQuestToggle()
    {
        _levelConfig.ScoreQuest = ScoreQuestToggle.isOn;
    } 

    public void UpdateItemQuestToggle()
    {
        _levelConfig.ItemQuest = ItemQuestToggle.isOn;
    }

    public void UpdateBarrierQuestToggle()
    {
        _levelConfig.BarrierQuest = BarrierQuestToggle.isOn;
    }

    public void HideQuestSelect()
    {
        QuestSelectUI.SetActive(false);
    }

    public void PlayWithConfig()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void OnDestroy()
    {
        RemoveUIListeners();
    }
}
