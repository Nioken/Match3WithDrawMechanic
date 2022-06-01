using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class EditorUIManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer Background;
    [SerializeField] private List<Sprite> Backgrounds;

    [SerializeField] public Slider HeightSlider;
    [SerializeField] private TMP_Text HeightText;
    [SerializeField] public Slider WidthSlider;
    [SerializeField] private TMP_Text WidthText;
    public TMP_InputField StepsField;
    [SerializeField] public static EditorUIManager _editorUIManager;
    [SerializeField] public LevelConfig _levelConfig;

    [Header("UI ��������")]
    [SerializeField] public GameObject QuestSelectUI;
    [SerializeField] public Toggle ScoreQuestToggle;
    [SerializeField] public Toggle ItemQuestToggle;
    [SerializeField] public Toggle BarrierQuestToggle;


    private void OnEnable()
    {
        _editorUIManager = GetComponent<EditorUIManager>();
    }

    void Start()
    { 
        Background.sprite = Backgrounds[UnityEngine.Random.Range(0, Backgrounds.Count)];
    }

    public void UpdateHeight()
    {
        if (UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale.LocaleName == "English (en)")
        {
            HeightText.text = "Height: " + HeightSlider.value.ToString();
        }
        else
        {
            HeightText.text = "������: " + HeightSlider.value.ToString();
        }
        TileGenerator.Y = (int)HeightSlider.value;
    }

    public void UpdateWidth()
    {
        if (UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale.LocaleName == "English (en)")
        {
            WidthText.text = "Width: " + WidthSlider.value.ToString();
        }
        else
        {
            WidthText.text = "������: " + WidthSlider.value.ToString();
        }
        TileGenerator.X = (int)WidthSlider.value;
    }

    public void UpdateSteps()
    {
        EditorTileGenerator.Steps = Convert.ToInt32(StepsField.text);
    }

    public void UpdateCurrentBarrier(Barrier Barrier)
    {
        EditorTileGenerator.CurrentBarrier = Barrier;
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
        if (ScoreQuestToggle.isOn)
        {
            _levelConfig.ScoreQuest = true;
        }
        else
        {
            _levelConfig.ScoreQuest = false;
        }
    } 
    public void UpdateItemQuestToggle()
    {
        if (ItemQuestToggle.isOn)
        {
            _levelConfig.ItemQuest = true;
        }
        else
        {
            _levelConfig.ItemQuest = false;
        }
    }
    public void UpdateBarrierQuestToggle()
    {
        if (BarrierQuestToggle.isOn)
        {
            _levelConfig.BarrierQuest = true;
        }
        else
        {
            _levelConfig.BarrierQuest = false;
        }
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
