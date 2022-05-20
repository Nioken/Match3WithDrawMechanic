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

    [Header("UI  вествов")]
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
        HeightText.text = "Height: " + HeightSlider.value.ToString();
        TileGenerator.Y = (int)HeightSlider.value;
    }

    public void UpdateWidth()
    {
        WidthText.text = "Width: " + WidthSlider.value.ToString();
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
            PlayerPrefs.SetInt("ScoreQuest", 1);
        }
        else
        {
            PlayerPrefs.SetInt("ScoreQuest", 0);
        }
    } 
    public void UpdateItemQuestToggle()
    {
        if (ItemQuestToggle.isOn)
        {
            PlayerPrefs.SetInt("ItemQuest", 1);
        }
        else
        {
            PlayerPrefs.SetInt("ItemQuest", 0);
        }
    }
    public void UpdateBarrierQuestToggle()
    {
        if (BarrierQuestToggle.isOn)
        {
            PlayerPrefs.SetInt("BarrierQuest", 1);
        }
        else
        {
            PlayerPrefs.SetInt("BarrierQuest", 0);
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
