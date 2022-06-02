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
    [SerializeField] private TMP_Text _stepsText;
    [SerializeField] private GameObject _settingsUI;
    [SerializeField] private GameObject _winUI;
    [SerializeField] private GameObject _loseUI;
    [SerializeField] private Toggle _musicToggleButton;
    [SerializeField] private Toggle _soundsToggleButton;
    [SerializeField] private List<Sprite> _backgrounds;
    [SerializeField] private SpriteRenderer _background;
    private bool _isInteractive = true;

    private void Start()
    {
        _background.sprite = _backgrounds[Random.Range(0, _backgrounds.Count - 1)];
        if (PlayerPrefs.HasKey("IsSound") && PlayerPrefs.GetInt("IsSound") == 0)
            _soundsToggleButton.isOn = false;

        if (PlayerPrefs.HasKey("IsMusic") && PlayerPrefs.GetInt("IsMusic") == 0)
            _musicToggleButton.isOn = false;
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
            PlayerPrefs.SetInt("IsMusic", 1);
            _musicToggleButton.transform.GetChild(2).gameObject.SetActive(false);
            AudioManager._audioManager.MusicSource.enabled = true;
        }
        else
        {
            PlayerPrefs.SetInt("IsMusic", 0);
            _musicToggleButton.transform.GetChild(2).gameObject.SetActive(true);
            AudioManager._audioManager.MusicSource.enabled = false;
        }
    }

    public void ToggleSounds()
    {
        if (_soundsToggleButton.isOn)
        {
            PlayerPrefs.SetInt("IsSound", 1);
            _soundsToggleButton.transform.GetChild(2).gameObject.SetActive(false);
            AudioManager._audioManager.SoundsSource.enabled = true;
        }
        else
        {
            PlayerPrefs.SetInt("IsSound", 0);
            _soundsToggleButton.transform.GetChild(2).gameObject.SetActive(true);
            AudioManager._audioManager.SoundsSource.enabled = false;
        }
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

}
