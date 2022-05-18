using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.IO;
using System.Collections;

public class MenuScript : MonoBehaviour
{
    public LevelConfig _levelConfig;
    private AudioSource _audioSource;
    public List<Item> ItemsPrefabs;
    public SpriteRenderer Background;
    public List<Sprite> Backgrounds;

    void Start()
    {
        Background.sprite = Backgrounds[Random.Range(0, Backgrounds.Count - 1)];
        _audioSource = GetComponent<AudioSource>();
        if(PlayerPrefs.HasKey("IsMusic") && PlayerPrefs.GetInt("IsMusic") == 0)
        {
            _audioSource.enabled = false;
        }
        Instantiate(ItemsPrefabs[Random.Range(0, ItemsPrefabs.Count - 1)], new Vector3(0, 1.8f), Quaternion.identity).transform.localScale = new Vector3(2,2);
        SetLastConfig();
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
                    _levelConfig.isConfigured = true;
                    if (PlayerPrefs.HasKey("EditorX"))
                    {
                        _levelConfig.X = PlayerPrefs.GetInt("EditorX");
                        _levelConfig.Y = PlayerPrefs.GetInt("EditorY");
                        _levelConfig.Steps = PlayerPrefs.GetInt("EditorSteps");
                    }
                }
            }
            using (StreamReader sr = new StreamReader(Application.persistentDataPath + "/Barriers.json"))
            {
                if (sr != null)
                {
                    _levelConfig.AllBariers = JsonConvert.DeserializeObject<LevelConfig.BarrierInfo[,]>(sr.ReadToEnd());
                }
            }
        }
        catch (System.Exception)
        {
            _levelConfig.isConfigured = false;
        }
    }

    public void PlayRandomLevel()
    {
        _levelConfig.isConfigured = false;
        SceneManager.LoadScene("GameScene");
    }

    public void ConfigureLevel()
    {
        SceneManager.LoadScene("EditorScene");
    }

}
