using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerControl _playerControl;
    [SerializeField] private TMP_Text StepsText;
    [SerializeField] private GameObject SettingsUI;
    [SerializeField] private GameObject WinUI;
    [SerializeField] private GameObject LoseUI;
    [SerializeField] private Toggle MusicToggleButton;
    [SerializeField] private Toggle SoundsToggleButton;
    [SerializeField] private List<Sprite> Backgrounds;
    [SerializeField] private SpriteRenderer Background;
    private bool isInteractive = true;

    private void Start()
    {
        Background.sprite = Backgrounds[Random.Range(0, Backgrounds.Count - 1)];
        if (PlayerPrefs.HasKey("IsSound") && PlayerPrefs.GetInt("IsSound") == 0)
        {
            SoundsToggleButton.isOn = false;
        }
        if (PlayerPrefs.HasKey("IsMusic") && PlayerPrefs.GetInt("IsMusic") == 0)
        {
            MusicToggleButton.isOn = false;
        }
    }

    public void UpdateSteps(int steps)
    {
        if (UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale.LocaleName == "English (en)")
        {
            StepsText.text = "Steps: " + steps.ToString();
        }
        else
        {
            StepsText.text = "ируш: " + steps.ToString();
        }
    }

    public void ShowSettings()
    {
        if (isInteractive)
        {
            SettingsUI.SetActive(true);
            SettingsUI.transform.DOScale(new Vector3(4.87f, 7.37f), 0.3f);
            _playerControl.enabled = false;
        }
        
    }

    public void HideSettings()
    {
        SettingsUI.transform.DOScale(0, 0.3f).OnComplete(() => SettingsUI.SetActive(false));
        _playerControl.enabled = true;
    }

    public void ToggleMusic()
    {
        if (MusicToggleButton.isOn)
        {
            PlayerPrefs.SetInt("IsMusic", 1);
            MusicToggleButton.transform.GetChild(2).gameObject.SetActive(false);
            AudioManager._audioManager.MusicSource.enabled = true;
        }
        else
        {
            PlayerPrefs.SetInt("IsMusic", 0);
            MusicToggleButton.transform.GetChild(2).gameObject.SetActive(true);
            AudioManager._audioManager.MusicSource.enabled = false;
        }
    }

    public void ToggleSounds()
    {
        if (SoundsToggleButton.isOn)
        {
            PlayerPrefs.SetInt("IsSound", 1);
            SoundsToggleButton.transform.GetChild(2).gameObject.SetActive(false);
            AudioManager._audioManager.SoundsSource.enabled = true;
        }
        else
        {
            PlayerPrefs.SetInt("IsSound", 0);
            SoundsToggleButton.transform.GetChild(2).gameObject.SetActive(true);
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
        for(int i = 0; i < QuestsManager.Quests.Count; i++)
        {
            QuestsManager.Quests[i].ProgressText.text = QuestsManager.Quests[i].CurrentProgress.ToString() + "/" + QuestsManager.Quests[i].MaxProgress.ToString();
            QuestsManager.Quests[i].ProgressBar.fillAmount = QuestsManager.Quests[i].ProgressFillPerOne * QuestsManager.Quests[i].CurrentProgress;
        }
    }

    public void ShowWinUI()
    {
        WinUI.SetActive(true);
        WinUI.transform.DOScale(new Vector3(4.87f, 7.37f), 0.3f);
        _playerControl.enabled = false;
        isInteractive = false;
    }

    public void ShowLoseUI()
    {
        LoseUI.SetActive(true);
        LoseUI.transform.DOScale(new Vector3(4.87f, 7.37f), 0.3f);
        _playerControl.enabled = false;
        isInteractive = false;
    }
    
    public void RestartLevel()
    {
        SceneManager.LoadScene("GameScene");
        QuestsManager.Quests.Clear();
        PlayerControl.PlayerSteps = 100;
    }

}
