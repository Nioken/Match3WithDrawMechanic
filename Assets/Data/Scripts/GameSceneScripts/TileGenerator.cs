using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TileGenerator : MonoBehaviour
{
    [SerializeField] private protected LevelConfig _levelConfig;
    [SerializeField] private GameObject _rocketPrefab;
    [SerializeField] private GameObject _bombPrefab;
    [SerializeField] private List<GameObject> _itemsPrefabs;
    private static TileGenerator _tileGenerator;

    public static int X;
    public static int Y;
    public static Tile[,] AllTiles;
    public static Item[,] AllItems;
    public static Barrier[,] AllBariers;

    public GameObject TilePrefab;
    public List<Barrier> BarriersPrefabs;
    public Vector3 StartPosition;

    public Transform TilesObjectsTransform;
    public Transform ItemsObjectsTransform;
    public Transform BarriersObjectsTransform;

    private void Start()
    {
        _tileGenerator = GetComponent<TileGenerator>();
        if (_levelConfig.IsConfigured)
        {
            X = _levelConfig.X;
            Y = _levelConfig.Y;
            PlayerControl.PlayerSteps = _levelConfig.Steps;
            GenerateTilesByConfig(_levelConfig);
            return;
        }

        GenerateRandomTiles();
    }

    public void SearchStartPosition()
    {
        StartPosition = new Vector3(AllTiles.GetLength(0) * -1, Mathf.RoundToInt(0.5f * (AllTiles.GetLength(1) * -1) - 2f), 20);
    }

    public void GenerateRandomTiles()
    {
        AllTiles = new Tile[Random.Range(3,7), Random.Range(3,11)];
        X = AllTiles.GetLength(0);
        Y = AllTiles.GetLength(1);
        QuestsManager.SpawnScoreQuest(X * Y * 8);
        QuestsManager.SpawnItemQuest(X * Y * 2);
        AllBariers = new Barrier[X, Y];
        SearchStartPosition();
        for(var i = 0; i < X; i++)
        {
            for(var j = 0;j< Y; j++)
            {
                AllTiles[i, j] = Instantiate(TilePrefab, new Vector3
                    (StartPosition.x + 1f * i, StartPosition.y + 1f * j, 20),Quaternion.identity).GetComponent<Tile>();
                AllTiles[i, j].transform.SetParent(TilesObjectsTransform);
            }
        }
        AlignObjects();
        UpdateCameraSize();
        StartFillTiles();
        CreateBarriers();
        if (!MatchManager.CheckStepAvailable())
            _tileGenerator.RefillTiles();
    }

    public void GenerateTilesByConfig(LevelConfig config)
    {
        AllTiles = new Tile[X, Y];
        AllBariers = new Barrier[X, Y];
        if(config.ScoreQuest)
            QuestsManager.SpawnScoreQuest(X * Y * 8);

        if(config.ItemQuest)
            QuestsManager.SpawnItemQuest(X * Y * 2);

        SearchStartPosition();
        for (var i = 0; i < X; i++)
        {
            for (var j = 0; j < Y; j++)
            {
                AllTiles[i, j] = Instantiate(TilePrefab, new Vector3
                    (StartPosition.x + 1f * i, StartPosition.y + 1f * j, 20), Quaternion.identity).GetComponent<Tile>();
                EditorTileGenerator.ApplyConfigState(AllTiles[i, j], _levelConfig.AllTiles[i, j]);
                AllTiles[i, j].transform.SetParent(TilesObjectsTransform);
            }
        }

        AlignObjects();
        StartFillTiles();
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
                            AllBariers[i, j] = Instantiate(BarriersPrefabs[0], AllTiles[i, j].transform.position, Quaternion.identity)
                                .GetComponent<Barrier>();
                            AllBariers[i, j].X = i;
                            AllBariers[i, j].Y = j;
                        }
                        else
                        {
                            AllBariers[i, j] = Instantiate(BarriersPrefabs[1], AllTiles[i, j].transform.position, Quaternion.identity)
                                .GetComponent<Barrier>();
                            AllBariers[i, j].X = i;
                            AllBariers[i, j].Y = j;
                        }
                    }
                    else
                    {
                        if (_levelConfig.AllBariers[i, j].Heal == 2)
                        {
                            AllBariers[i, j] = Instantiate(BarriersPrefabs[2], AllTiles[i, j].transform.position, Quaternion.identity)
                                .GetComponent<Barrier>();
                            AllBariers[i, j].X = i;
                            AllBariers[i, j].Y = j;
                        }
                        else
                        {
                            AllBariers[i, j] = Instantiate(BarriersPrefabs[3], AllTiles[i, j].transform.position, Quaternion.identity)
                                .GetComponent<Barrier>();
                            AllBariers[i, j].X = i;
                            AllBariers[i, j].Y = j;
                        }
                    }
                }
            }
        }

        if(config.BarrierQuest)
            QuestsManager.SpawnBariersQuest();

        if (!MatchManager.CheckStepAvailable())
            _tileGenerator.RefillTiles();
    }

    public void AlignObjects()
    {
        float firstTilePos = AllTiles[0, 0].transform.position.x * -1;
        float lastTilePos = AllTiles[X - 1, Y - 1].transform.position.x;
        if(lastTilePos < 0)
            lastTilePos *= -1;

        while(firstTilePos != lastTilePos)
        {
            if(firstTilePos > lastTilePos)
            {
                for (var i = 0; i < X; i++)
                {
                    for (var j = 0; j < Y; j++)
                    {
                        AllTiles[i, j].transform.position = new Vector3
                            (AllTiles[i, j].transform.position.x + 0.5f, AllTiles[i, j].transform.position.y, 20);
                    }
                }
            }

            if(firstTilePos < lastTilePos)
            {
                for (var i = 0; i < X; i++)
                {
                    for (var j = 0; j < Y; j++)
                    {
                        AllTiles[i, j].transform.position = new Vector3
                            (AllTiles[i, j].transform.position.x - 0.5f, AllTiles[i, j].transform.position.y, 20);
                    }
                }
            }

            firstTilePos = AllTiles[0, 0].transform.position.x * -1;
            lastTilePos = AllTiles[X - 1, Y - 1].transform.position.x;
            if (lastTilePos < 0)
                lastTilePos *= -1;
        }
    }

    public void CreateBarriers()
    {
        for (var i = 0; i < X; i++)
        {
            for (var j = 0; j < Y; j++)
            {
                var randomValue = Random.Range(0, 100);
                if (randomValue > 70)
                {
                    AllBariers[i, j] = Instantiate(BarriersPrefabs[Random.Range(0, BarriersPrefabs.Count)], 
                        AllTiles[i, j].transform.position, Quaternion.identity).GetComponent<Barrier>();
                    AllBariers[i, j].X = i;
                    AllBariers[i, j].Y = j;
                    AllTiles[i, j].IsBarried = true;
                    AllBariers[i, j].transform.SetParent(BarriersObjectsTransform);
                }
            }
        }

        QuestsManager.SpawnBariersQuest();
    }
    
    public void StartFillTiles()
    {
        AllItems = new Item[X, Y];
        for (var i = 0; i < X; i++)
        {
            for (var j = 0; j < Y; j++)
            {
                if(AllTiles[i,j] != null)
                {
                    AllItems[i,j] = Instantiate(_itemsPrefabs[Random.Range(0, _itemsPrefabs.Count)], 
                        AllTiles[i, j].transform.position, Quaternion.identity).GetComponent<Item>();
                    AllItems[i, j].GetComponent<Item>().X = i;
                    AllItems[i, j].GetComponent<Item>().Y = j;
                    AllItems[i, j].transform.SetParent(ItemsObjectsTransform);
                }
            }
        }
    }
    
    public void FillEmptyTiles()
    {
        for (var i = 0; i < X; i++)
        {
            for (var j = 0; j < Y; j++)
            {
                if(AllItems[i,j] == null)
                {
                    AllItems[i, j] = Instantiate(_itemsPrefabs[Random.Range(0, _itemsPrefabs.Count)], 
                        AllTiles[i, j].transform.position, Quaternion.identity).GetComponent<Item>();
                    AllItems[i, j].transform.localScale = Vector3.zero;
                    AllItems[i, j].GetComponent<Item>().X = i;
                    AllItems[i, j].GetComponent<Item>().Y = j;
                    AllItems[i, j].transform.DOScale(Vector3.one, 0.5f);
                    AllItems[i, j].transform.SetParent(ItemsObjectsTransform);
                }
            }
        }
    }
    
    public static void SpawnBonus(Item.BonusType bonusType)
    {
        for(var i = 0; i < X; i++)
        {
            for(var j = 0; j < Y; j++)
            {
                if(AllItems[i,j] == null)
                {
                    if(bonusType == Item.BonusType.Rocket)
                        AllItems[i, j] = Instantiate(_tileGenerator._rocketPrefab, 
                            AllTiles[i, j].transform.position, Quaternion.identity).GetComponent<Item>();
                    else
                        AllItems[i, j] = Instantiate(_tileGenerator._bombPrefab, 
                            AllTiles[i, j].transform.position, Quaternion.identity).GetComponent<Item>();

                    AllItems[i, j].transform.DOScale(1, 0.5f);
                    AllItems[i, j].X = i;
                    AllItems[i,j].Y = j;
                    AllItems[i, j].transform.SetParent(_tileGenerator.ItemsObjectsTransform);
                    return;
                }
            }
        }
    }

    public static void FillAfterMatch()
    {
        var tmpY = 0;
        for (var i = 0; i < X; i++)
        {
            for (var j = 0; j < Y; j++)
            {
                if (AllItems[i, j] != null && AllItems[i, j].Y > 0 && AllItems[i, j - 1] == null)
                {
                    if (AllBariers[i, j] != null && AllBariers[i, j].Type == Barrier.BarrierType.Ice)
                        continue;

                    if (AllBariers[i, j - 1] != null && AllBariers[i, j - 1].Type == Barrier.BarrierType.Ice)
                        continue;

                    tmpY = j - 1;
                    while (j > 0)
                    {
                        if (AllItems[i, tmpY] == null)
                        {
                            if (tmpY == 0)
                            {
                                if (AllBariers[i, j] != null && AllBariers[i, j].Type == Barrier.BarrierType.Rock)
                                {
                                    AllBariers[i, j].transform.DOMove(AllTiles[i, tmpY].transform.position, 0.5f);
                                    AllBariers[i, j].Y = tmpY;
                                    AllBariers[i, tmpY] = AllBariers[i, j];
                                    AllBariers[i, j] = null;
                                    AllTiles[i, j].IsBarried = false;
                                    AllTiles[i, tmpY].IsBarried = true;
                                }

                                AllItems[i, j].transform.DOMove(AllTiles[i, tmpY].transform.position, 0.5f);
                                AllItems[i, j].Y = tmpY;
                                AllItems[i, tmpY] = AllItems[i, j];
                                AllItems[i, j] = null;
                                break;
                            }

                            tmpY--;
                        }
                        else
                        {
                            if (AllBariers[i, j] != null && AllBariers[i, j].Type == Barrier.BarrierType.Rock)
                            {
                                AllBariers[i, j].transform.DOMove(AllTiles[i, tmpY + 1].transform.position, 0.5f);
                                AllBariers[i, j].Y = tmpY + 1;
                                AllBariers[i, tmpY + 1] = AllBariers[i, j];
                                AllBariers[i, j] = null;
                                AllTiles[i, j].IsBarried = false;
                                AllTiles[i, tmpY + 1].IsBarried = true;
                            }

                            AllItems[i, j].transform.DOMove(AllTiles[i, tmpY + 1].transform.position, 0.5f);
                            AllItems[i, j].Y = tmpY + 1;
                            AllItems[i, tmpY + 1] = AllItems[i, j];
                            AllItems[i, j] = null;
                            break;
                        }
                    }
                }
            }
        }

        _tileGenerator.FillEmptyTiles();
        if (!MatchManager.CheckStepAvailable())
            _tileGenerator.RefillTiles();
    }

    public void RefillTiles()
    {
        for(var i = 0; i < X; i++)
        {
            for(var j = 0; j < Y; j++)
            {
                if (AllBariers[i, j] != null)
                    continue;

                MatchManager.DestroyAfterAnim(AllItems[i, j]);
                AllItems[i, j] = Instantiate(_itemsPrefabs[Random.Range(0, _itemsPrefabs.Count)], 
                    AllTiles[i, j].transform.position, Quaternion.identity).GetComponent<Item>();
                AllItems[i, j].transform.localScale = Vector3.zero;
                AllItems[i, j].GetComponent<Item>().X = i;
                AllItems[i, j].GetComponent<Item>().Y = j;
                AllItems[i, j].transform.DOScale(Vector3.one, 0.5f);
                AllItems[i, j].transform.SetParent(ItemsObjectsTransform);
            }
        }

        if (!MatchManager.CheckStepAvailable())
            RefillTiles();
    }
    
    public static void UpdateCameraSize()
    {
        Camera.main.orthographicSize = 3;
        Vector3 firstTilePos = Camera.main.WorldToScreenPoint(new Vector3
            (AllTiles[0, 0].transform.position.x-0.5f,AllTiles[0,0].transform.position.y - 0.5f));
        while(firstTilePos.x < 0 || firstTilePos.y < 0)
        {
            Camera.main.orthographicSize += 0.1f;
            firstTilePos = Camera.main.WorldToScreenPoint(new Vector3
                (AllTiles[0, 0].transform.position.x - 0.5f, AllTiles[0, 0].transform.position.y - 0.5f));
        }

        Camera.main.orthographicSize += 0.2f;
    }
}