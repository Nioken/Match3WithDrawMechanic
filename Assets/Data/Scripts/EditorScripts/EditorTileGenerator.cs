using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class EditorTileGenerator : TileGenerator
{
    public static int Steps = 100;
    public static Barrier CurrentBarrier;
    private RaycastHit _hit;
    private bool _canResize = true;

    private void Start()
    {
        if (AllTiles != null)
        {
            AllTiles = null;
            AllBariers = null;
            AllItems = null;
        }

        if (_levelConfig.IsConfigured)
        {
            X = _levelConfig.X;
            Y = _levelConfig.Y;
            Steps = _levelConfig.Steps;
            _canResize = false;
            EditorUIManager._editorUIManager.StepsField.text = Steps.ToString();
            EditorUIManager._editorUIManager.HeightSlider.value = Y;
            EditorUIManager._editorUIManager.WidthSlider.value = X;
            UpdateSizeByConfig();
            if (!_levelConfig.ScoreQuest) 
               EditorUIManager._editorUIManager.ScoreQuestToggle.isOn = false;

            if (!_levelConfig.ItemQuest)
                EditorUIManager._editorUIManager.ItemQuestToggle.isOn = false;

            if (!_levelConfig.BarrierQuest)
                EditorUIManager._editorUIManager.BarrierQuestToggle.isOn = false;

            return;
        }

        _levelConfig.ScoreQuest = true;
        _levelConfig.ItemQuest = true;
        _levelConfig.BarrierQuest = true;
        UpdateSize();
    }

    public void UpdateSize()
    {
        if (!_levelConfig.IsConfigured)
        {
            X = 3;
            Y = 3;
        }

        if (!_canResize)
            return;

        if (AllTiles != null)
        {
            for (var i = 0; i < AllTiles.GetLength(0); i++)
            {
                for (var j = 0; j < AllTiles.GetLength(1); j++)
                {
                    if (AllBariers[i, j] != null)
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
        for (var i = 0; i < X; i++)
        {
            for (var j = 0; j < Y; j++)
            {
                AllTiles[i, j] = Instantiate(TilePrefab, new Vector3
                    (StartPosition.x + 1f * i, StartPosition.y + 1f * j, 20), Quaternion.identity).GetComponent<Tile>();
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
        for (var i = 0; i < X; i++)
        {
            for (var j = 0; j < Y; j++)
            {
                AllTiles[i, j] = Instantiate(TilePrefab, new Vector3
                    (StartPosition.x + 1f * i, StartPosition.y + 1f * j, 20), Quaternion.identity).GetComponent<Tile>();
                ApplyConfigState(AllTiles[i, j], _levelConfig.AllTiles[i, j]);
                AllTiles[i, j].gameObject.AddComponent<BoxCollider>();
                AllTiles[i, j].transform.SetParent(TilesObjectsTransform);
            }
        }

        AlignObjects();
        UpdateCameraSize();
        for (var i = 0; i < X; i++)
        {
            for (var j = 0; j < Y; j++)
            {
                if (AllTiles[i, j].IsBarried)
                {
                    if (_levelConfig.AllBariers[i, j].Type == Barrier.BarrierType.Rock)
                    {
                        if (_levelConfig.AllBariers[i, j].Heal == 2)
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
                        if (_levelConfig.AllBariers[i, j].Heal == 2)
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

        _canResize = true;
    }

    private void PlayerInput()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _hit))
        {
            if (_hit.collider.tag == "Tile" && CurrentBarrier != null)
            {
                for (var i = 0; i < X; i++)
                {
                    for (var j = 0; j < Y; j++)
                    {
                        if (_hit.collider.gameObject == AllTiles[i, j].gameObject)
                        {
                            if (AllBariers[i, j] != null)
                                return;

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

    public void UpdateConfig()
    {
        _levelConfig.IsConfigured = true;
        _levelConfig.X = X;
        _levelConfig.Y = Y;
        _levelConfig.Steps = Steps;
        _levelConfig.AllTiles = new LevelConfig.TileInfo[X, Y];
        _levelConfig.AllBariers = new LevelConfig.BarrierInfo[X, Y];
        for (var i = 0; i < X; i++)
        {
            for (var j = 0; j < Y; j++)
            {
                LevelConfig.TileInfo tmp = new LevelConfig.TileInfo(AllTiles[i, j].X, AllTiles[i, j].Y, AllTiles[i, j].IsBarried);
                _levelConfig.AllTiles[i, j] = tmp;
                if (AllBariers[i, j] != null)
                {
                    LevelConfig.BarrierInfo tmpBarrier = new LevelConfig.BarrierInfo
                        (AllBariers[i, j].X, AllBariers[i, j].Y, AllBariers[i, j].Heal, AllBariers[i, j].Type);
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
        using (StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/LevelInfo.json"))
        {
            string SerializeConfig = JsonConvert.SerializeObject(new LevelConfig.LevelInfo(_levelConfig.Steps,_levelConfig.X,_levelConfig.Y,_levelConfig.ScoreQuest,_levelConfig.ItemQuest,_levelConfig.BarrierQuest));
            sw.Write(SerializeConfig);
        }
    }

    public static void ApplyConfigState(Tile tile, LevelConfig.TileInfo savedSate)
    {
        tile.X = savedSate.X;
        tile.Y = savedSate.Y;
        tile.IsBarried = savedSate.IsBarried;
    }
    
    public void Update()
    {

        #region AndoridInput

#if UNITY_ANDROID
        if (Input.touchCount > 0 && (Input.touches[0].phase == TouchPhase.Began || Input.touches[0].phase == TouchPhase.Moved))
            PlayerInput();
#endif

        #endregion

        #region EditorInput

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
            PlayerInput();
#endif

        #endregion

    }
}
