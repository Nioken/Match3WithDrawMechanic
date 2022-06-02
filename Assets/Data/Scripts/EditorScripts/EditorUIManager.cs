using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;
using TMPro;

public class EditorUIManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _background;
    [SerializeField] private List<Sprite> _backgrounds;

    [SerializeField] private TMP_Text _heightText;
    [SerializeField] private TMP_Text _widthText;
    [SerializeField] private LevelConfig _levelConfig;
    [SerializeField] public static EditorUIManager _editorUIManager;
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
        _editorUIManager = GetComponent<EditorUIManager>();
    }

    private void Start()
    {
        _background.sprite = _backgrounds[UnityEngine.Random.Range(0, _backgrounds.Count)];
    }

    public void UpdateHeight()
    {
        _heightText.text = LocalizationSettings.SelectedLocale.LocaleName == "English (en)" ? _heightText.text = 
            "Height: " + HeightSlider.value.ToString() : _heightText.text = "Высота: " + HeightSlider.value.ToString();
        TileGenerator.Y = (int)HeightSlider.value;
    }

    public void UpdateWidth()
    {
        _widthText.text = LocalizationSettings.SelectedLocale.LocaleName == "English (en)" ? _widthText.text = 
            "Width: " + WidthSlider.value.ToString() : _widthText.text = "Ширина: " + WidthSlider.value.ToString();
        TileGenerator.X = (int)WidthSlider.value;
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
        _levelConfig.ScoreQuest = ScoreQuestToggle.isOn ? _levelConfig.ScoreQuest = true : _levelConfig.ScoreQuest = false;
    } 

    public void UpdateItemQuestToggle()
    {
        _levelConfig.ItemQuest = ItemQuestToggle.isOn ? _levelConfig.ItemQuest = true : _levelConfig.ItemQuest = false;
    }

    public void UpdateBarrierQuestToggle()
    {
        _levelConfig.BarrierQuest = BarrierQuestToggle.isOn ? _levelConfig.BarrierQuest = true : _levelConfig.BarrierQuest = false;
    }

    public void HideQuestSelect()
    {
        QuestSelectUI.SetActive(false);
    }

    public void PlayWithConfig()
    {
        SceneManager.LoadScene("GameScene");
    }
}
