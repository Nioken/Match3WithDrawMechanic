using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class EditorTileGenerator : TileGenerator
{
    public static int Steps = 100;
    public static Barrier CurrentBarrier;
    private RaycastHit _hit;
    private bool CanResize = true;

    private void Start()
    {
        if(AllTiles != null)
        {
            AllTiles = null;
            AllBariers = null;
            AllItems = null;
        }
        if (_levelConfig.isConfigured)
        {
            X = _levelConfig.X;
            Y = _levelConfig.Y;
            Steps = _levelConfig.Steps;
            CanResize = false;
            EditorUIManager._editorUIManager.StepsField.text = Steps.ToString();
            EditorUIManager._editorUIManager.HeightSlider.value = Y;
            EditorUIManager._editorUIManager.WidthSlider.value = X;
            UpdateSizeByConfig();
            if (PlayerPrefs.HasKey("ScoreQuest") && PlayerPrefs.GetInt("ScoreQuest") == 0)
            {
                EditorUIManager._editorUIManager.ScoreQuestToggle.isOn = false;
            }
            if (PlayerPrefs.HasKey("ItemQuest") && PlayerPrefs.GetInt("ItemQuest") == 0)
            {
                EditorUIManager._editorUIManager.ItemQuestToggle.isOn = false;
            }
            if (PlayerPrefs.HasKey("BarrierQuest") && PlayerPrefs.GetInt("BarrierQuest") == 0)
            {
                EditorUIManager._editorUIManager.BarrierQuestToggle.isOn = false;
            }
            return;
        }
        PlayerPrefs.SetInt("ScoreQuest", 1);
        PlayerPrefs.SetInt("ItemQuest", 1);
        PlayerPrefs.SetInt("BarrierQuest", 1);
        UpdateSize();
    }

    public void UpdateSize()
    {
        if (!_levelConfig.isConfigured)
        {
            X = 3;
            Y = 3;
        }
        if (!CanResize) return;
        if(AllTiles != null)
        {
            for (int i = 0; i < AllTiles.GetLength(0); i++)
            {
                for (int j = 0; j < AllTiles.GetLength(1); j++)
                {
                    if(AllBariers[i,j] != null)
                    {
                        Destroy(AllBariers[i, j].gameObject);
                    }
                    Destroy(AllTiles[i, j].gameObject);
                }
            }
            AllTiles = null;
        }
        AllTiles = new Tile[X, Y];
        AllBariers = new Barrier[X, Y];
        SearchStartPosition();
        for (int i = 0; i < X; i++)
        {
            for (int j = 0; j < Y; j++)
            {
                AllTiles[i, j] = Instantiate(TilePrefab, new Vector3(StartPosition.x + 1f * i, StartPosition.y + 1f * j, 20), Quaternion.identity).GetComponent<Tile>();
                AllTiles[i, j].gameObject.AddComponent<BoxCollider>();
                AllTiles[i, j].X = i;
                AllTiles[i, j].Y = j;
                AllTiles[i, j].transform.SetParent(TilesObjectsTransform);
            }
        }
        AlignObjects();
        UpdateCameraSize();
        UpdateConfig();
    }

    public void UpdateSizeByConfig()
    {
        AllTiles = new Tile[X, Y];
        AllBariers = new Barrier[X, Y];
        SearchStartPosition();
        for (int i = 0; i < X; i++)
        {
            for (int j = 0; j < Y; j++)
            {
                AllTiles[i,j] = Instantiate(TilePrefab, new Vector3(StartPosition.x + 1f * i, StartPosition.y + 1f * j, 20), Quaternion.identity).GetComponent<Tile>();
                ApplyConfigState(AllTiles[i, j], _levelConfig.AllTiles[i, j]);
                AllTiles[i, j].gameObject.AddComponent<BoxCollider>();
                AllTiles[i, j].transform.SetParent(TilesObjectsTransform);
            }
        }
        AlignObjects();
        UpdateCameraSize();
        for(int i = 0; i < X; i++)
        {
            for(int j = 0; j < Y; j++)
            {
                if (AllTiles[i, j].IsBarried)
                {
                    if (_levelConfig.AllBariers[i, j].barrierType == Barrier.BarrierType.Rock)
                    {
                        if (_levelConfig.AllBariers[i, j].heal == 2)
                        {
                            AllBariers[i, j] = Instantiate(BarriersPrefabs[0], AllTiles[i, j].transform.position, Quaternion.identity);
                            AllBariers[i, j].X = i;
                            AllBariers[i, j].Y = j;
                        }
                        else
                        {
                            AllBariers[i, j] = Instantiate(BarriersPrefabs[1], AllTiles[i, j].transform.position, Quaternion.identity);
                            AllBariers[i, j].X = i;
                            AllBariers[i, j].Y = j;
                        }
                    }
                    else
                    {
                        if (_levelConfig.AllBariers[i, j].heal == 2)
                        {
                            AllBariers[i, j] = Instantiate(BarriersPrefabs[2], AllTiles[i, j].transform.position, Quaternion.identity);
                            AllBariers[i, j].X = i;
                            AllBariers[i, j].Y = j;
                        }
                        else
                        {
                            AllBariers[i, j] = Instantiate(BarriersPrefabs[3], AllTiles[i, j].transform.position, Quaternion.identity);
                            AllBariers[i, j].X = i;
                            AllBariers[i, j].Y = j;
                        }
                    }
                    AllBariers[i, j].transform.SetParent(BarriersObjectsTransform);
                }
            }
        }
        CanResize = true;
    }

    public void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began || Input.touches[0].phase == TouchPhase.Moved)
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.touches[0].position), out _hit))
                {
                    if (_hit.collider.tag == "Tile")
                    {
                        if (CurrentBarrier != null)
                        {
                            for (int i = 0; i < X; i++)
                            {
                                for (int j = 0; j < Y; j++)
                                {
                                    if (_hit.collider.gameObject == AllTiles[i, j].gameObject)
                                    {
                                        if (AllBariers[i, j] != null) return;
                                        AllBariers[i, j] = Instantiate(CurrentBarrier, AllTiles[i, j].transform.position, Quaternion.identity);
                                        AllBariers[i, j].X = i;
                                        AllBariers[i, j].Y = j;
                                        AllTiles[i, j].IsBarried = true;
                                        AllBariers[i, j].transform.SetParent(BarriersObjectsTransform);
                                        UpdateConfig();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void UpdateConfig()
    {
        //UpdateQuestConfig();
        _levelConfig.isConfigured = true;
        _levelConfig.X = X;
        _levelConfig.Y = Y;
        _levelConfig.Steps = Steps;
        _levelConfig.ScoreQuest = PlayerPrefs.GetInt("ScoreQuest");
        _levelConfig.ItemQuest = PlayerPrefs.GetInt("ItemQuest");
        _levelConfig.BarrierQuest = PlayerPrefs.GetInt("BarrierQuest");
        PlayerPrefs.SetInt("EditorX", _levelConfig.X);
        PlayerPrefs.SetInt("EditorY", _levelConfig.Y);
        PlayerPrefs.SetInt("EditorSteps", _levelConfig.Steps);
        _levelConfig.AllTiles = new LevelConfig.TileInfo[X,Y];
        _levelConfig.AllBariers = new LevelConfig.BarrierInfo[X,Y];
        for(int i = 0; i < X; i++)
        {
            for(int j = 0; j < Y; j++)
            {
                LevelConfig.TileInfo tmp = new LevelConfig.TileInfo(AllTiles[i,j].X,AllTiles[i,j].Y,AllTiles[i,j].IsBarried);
                _levelConfig.AllTiles[i, j] = tmp;
                if(AllBariers[i,j] != null)
                {
                    LevelConfig.BarrierInfo tmpBarrier = new LevelConfig.BarrierInfo(AllBariers[i, j].X, AllBariers[i, j].Y, AllBariers[i, j].heal, AllBariers[i, j].barrierType);
                    _levelConfig.AllBariers[i, j] = tmpBarrier;
                }
            }
        }
        using (StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/Tiles.json"))
        {
            string SerializeConfig = JsonConvert.SerializeObject(_levelConfig.AllTiles);
            sw.Write(SerializeConfig);
        }
        using (StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/Barriers.json"))
        {
            string SerializeConfig = JsonConvert.SerializeObject(_levelConfig.AllBariers);
            sw.Write(SerializeConfig);
        }
    }

    public static void ApplyConfigState(Tile tile, LevelConfig.TileInfo SavedSate)
    {
        tile.X = SavedSate.X;
        tile.Y = SavedSate.Y;
        tile.IsBarried = SavedSate.IsBarried;
    }
}
