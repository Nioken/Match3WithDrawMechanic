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
            Width = _levelConfig.X;
            Height = _levelConfig.Y;
            Steps = _levelConfig.Steps;
            _canResize = false;
            EditorUIManager.EditorUI.StepsField.text = Steps.ToString();
            EditorUIManager.EditorUI.HeightSlider.value = Height;
            EditorUIManager.EditorUI.WidthSlider.value = Width;
            UpdateSizeByConfig();
            if (!_levelConfig.ScoreQuest) 
               EditorUIManager.EditorUI.ScoreQuestToggle.isOn = false;

            if (!_levelConfig.ItemQuest)
                EditorUIManager.EditorUI.ItemQuestToggle.isOn = false;

            if (!_levelConfig.BarrierQuest)
                EditorUIManager.EditorUI.BarrierQuestToggle.isOn = false;

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
            Width = 3;
            Height = 3;
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

        AllTiles = new Tile[Width, Height];
        AllBariers = new Barrier[Width, Height];
        
        SearchStartPosition();
        
        for (var i = 0; i < Width; i++)
        {
            for (var j = 0; j < Height; j++)
            {
                var position = new Vector3()
                {
                    x = StartPosition.x + 1 * i,
                    y = StartPosition.y + 1 * j,
                    z = TileDepthZ
                };

                AllTiles[i, j] = CreateTile(position);
                AllTiles[i, j].gameObject.AddComponent<BoxCollider>();
                AllTiles[i, j].X = i;
                AllTiles[i, j].Y = j;
            }
        }

        AlignObjects();
        UpdateCameraSize();
        UpdateConfig();
    }

    public void UpdateSizeByConfig()
    {
        AllTiles = new Tile[Width, Height];
        AllBariers = new Barrier[Width, Height];
        SearchStartPosition();
        for (var i = 0; i < Width; i++)
        {
            for (var j = 0; j < Height; j++)
            {
                var position = new Vector3()
                {
                    x = StartPosition.x + 1 * i,
                    y = StartPosition.y + 1 * j,
                    z = TileDepthZ
                };

                AllTiles[i, j] = CreateTile(position);
                ApplyConfigState(AllTiles[i, j], _levelConfig.AllTiles[i, j]);
                AllTiles[i, j].gameObject.AddComponent<BoxCollider>();
            }
        }

        AlignObjects();
        UpdateCameraSize();
        for (var i = 0; i < Width; i++)
        {
            for (var j = 0; j < Height; j++)
            {
                if (AllTiles[i, j].IsBarried)
                    AllBariers[i, j] = CreateBarrier(AllTiles[i, j], _levelConfig.AllBariers[i, j].Type, _levelConfig.AllBariers[i, j].Heal);
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
                for (var i = 0; i < Width; i++)
                {
                    for (var j = 0; j < Height; j++)
                    {
                        if (_hit.collider.gameObject == AllTiles[i, j].gameObject)
                        {
                            if (AllBariers[i, j] != null)
                                return;

                            AllBariers[i, j] = CreateBarrier(AllTiles[i, j], CurrentBarrier.Type, CurrentBarrier.Heal);
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
        _levelConfig.X = Width;
        _levelConfig.Y = Height;
        _levelConfig.Steps = Steps;
        _levelConfig.AllTiles = new LevelConfig.TileInfo[Width, Height];
        _levelConfig.AllBariers = new LevelConfig.BarrierInfo[Width, Height];
        for (var i = 0; i < Width; i++)
        {
            for (var j = 0; j < Height; j++)
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
