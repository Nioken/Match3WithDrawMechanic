using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerControl _playerControl;
    [SerializeField] private List<Sprite> _backgrounds;
    [SerializeField] private SpriteRenderer _background;

    [Header("UI")]
    [SerializeField] private Button _toMenuButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _ExitSettingsButton;
    [SerializeField] private Toggle _musicToggleButton;
    [SerializeField] private Toggle _soundsToggleButton;
    [SerializeField] private Button _winReplayBitton;
    [SerializeField] private Button _loseReplayButton;
    [SerializeField] private Button _winMenuButton;
    [SerializeField] private Button _loseMenuButton;
    [SerializeField] private TMP_Text _stepsText;
    [SerializeField] private GameObject _settingsUI;
    [SerializeField] private GameObject _winUI;
    [SerializeField] private GameObject _loseUI;
    [Space]
    private bool _isInteractive = true;

    private void Start()
    {
        AddUIListener();
        _background.sprite = _backgrounds[Random.Range(0, _backgrounds.Count - 1)];
        if (!Settings.IsSounds)
            _soundsToggleButton.isOn = false;

        if (!Settings.IsMusic)
            _musicToggleButton.isOn = false;
    }

    private void AddUIListener()
    {
        _toMenuButton.onClick.AddListener(ExitMenu);
        _settingsButton.onClick.AddListener(ShowSettings);
        _ExitSettingsButton.onClick.AddListener(HideSettings);
        _musicToggleButton.onValueChanged.AddListener((bool isOn) => ToggleMusic());
        _soundsToggleButton.onValueChanged.AddListener((bool isOn) => ToggleSounds());
        _winReplayBitton.onClick.AddListener(RestartLevel);
        _loseReplayButton.onClick.AddListener(RestartLevel);
        _winMenuButton.onClick.AddListener(ExitMenu);
        _loseMenuButton.onClick.AddListener(ExitMenu);
    }

    private void RemoveUIListeners()
    {
        _toMenuButton.onClick.RemoveAllListeners();
        _settingsButton.onClick.RemoveAllListeners();
        _ExitSettingsButton.onClick.RemoveAllListeners();
        _musicToggleButton.onValueChanged.RemoveAllListeners();
        _soundsToggleButton.onValueChanged.RemoveAllListeners();
        _winReplayBitton.onClick.RemoveAllListeners();
        _loseReplayButton.onClick.RemoveAllListeners();
        _winMenuButton.onClick.RemoveAllListeners();
        _loseMenuButton.onClick.RemoveAllListeners();
    }

    public void UpdateSteps(int steps)
    {
        _stepsText.text = LocalizationSettings.SelectedLocale.LocaleName == "English (en)" ? _stepsText.text = 
            "Steps: " + steps.ToString() : _stepsText.text = "ируш: " + steps.ToString();
    }

    public void ShowSettings()
    {
        if (_isInteractive)
        {
            _settingsUI.SetActive(true);
            _settingsUI.transform.DOScale(new Vector3(4.87f, 7.37f), 0.3f);
            _playerControl.enabled = false;
        }
    }

    public void HideSettings()
    {
        _settingsUI.transform.DOScale(0, 0.3f).OnComplete(() => _settingsUI.SetActive(false));
        _playerControl.enabled = true;
    }

    public void ToggleMusic()
    {
        if (_musicToggleButton.isOn)
        {
            Settings.IsMusic = true;
            _musicToggleButton.transform.GetChild(2).gameObject.SetActive(false);
            AudioManager.Manager.MusicSource.enabled = true;
        }
        else
        {
            Settings.IsMusic = false;
            _musicToggleButton.transform.GetChild(2).gameObject.SetActive(true);
            AudioManager.Manager.MusicSource.enabled = false;
        }
        Settings.WriteSettings();
    }

    public void ToggleSounds()
    {
        if (_soundsToggleButton.isOn)
        {
            Settings.IsSounds = true;
            _soundsToggleButton.transform.GetChild(2).gameObject.SetActive(false);
            AudioManager.Manager.SoundsSource.enabled = true;
        }
        else
        {
            Settings.IsSounds = false;
            _soundsToggleButton.transform.GetChild(2).gameObject.SetActive(true);
            AudioManager.Manager.SoundsSource.enabled = false;
        }
        Settings.WriteSettings();
    }

    public void ExitMenu()
    {
        QuestsManager.Quests.Clear();
        PlayerControl.PlayerSteps = 100;
        DOTween.Clear();
        SceneManager.LoadScene("MenuScene");
    }

    public static void UpdateQuestsUI()
    {
        for(var i = 0; i < QuestsManager.Quests.Count; i++)
        {
            QuestsManager.Quests[i].ProgressText.text = 
                QuestsManager.Quests[i].CurrentProgress.ToString() + "/" + QuestsManager.Quests[i].MaxProgress.ToString();
            QuestsManager.Quests[i].ProgressBar.fillAmount = 
                QuestsManager.Quests[i].ProgressFillPerOne * QuestsManager.Quests[i].CurrentProgress;
        }
    }

    public void ShowWinUI()
    {
        _winUI.SetActive(true);
        _winUI.transform.DOScale(new Vector3(4.87f, 7.37f), 0.3f);
        _playerControl.enabled = false;
        _isInteractive = false;
    }

    public void ShowLoseUI()
    {
        _loseUI.SetActive(true);
        _loseUI.transform.DOScale(new Vector3(4.87f, 7.37f), 0.3f);
        _playerControl.enabled = false;
        _isInteractive = false;
    }
    
    public void RestartLevel()
    {
        SceneManager.LoadScene("GameScene");
        QuestsManager.Quests.Clear();
        PlayerControl.PlayerSteps = 100;
    }

    private void OnDestroy()
    {
        RemoveUIListeners();
    }

}
