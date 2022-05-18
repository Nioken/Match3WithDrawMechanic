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

    [SerializeField] public static EditorUIManager _editorUIManager;
    public TMP_InputField StepsField;

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

    public void PlayWithConfig()
    {
        SceneManager.LoadScene("GameScene");
    }
}
