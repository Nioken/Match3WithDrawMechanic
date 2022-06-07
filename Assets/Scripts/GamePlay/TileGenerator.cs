using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TileGenerator : MonoBehaviour
{
    [SerializeField] private protected Tile _tilePrefab;
    [SerializeField] private protected LevelConfig _levelConfig;
    [SerializeField] private Item _rocketPrefab;
    [SerializeField] private Item _bombPrefab;
    [SerializeField] private List<Item> _itemsPrefabs;
    private static TileGenerator _tileGenerator;

    public static int Width;
    public static int Height;
    public static Tile[,] AllTiles;
    public static Item[,] AllItems;
    public static Barrier[,] AllBariers;

    private const int WidthMin = 3;
    private const int WidthMax = 7;

    private const int HeightMin = 3;
    private const int HeightMax = 11;
    private protected const float TileDepthZ = 20.0f;

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
            Width = _levelConfig.X;
            Height = _levelConfig.Y;
            PlayerControl.PlayerSteps = _levelConfig.Steps;
            GenerateTilesByConfig(_levelConfig);
            return;
        }

        GenerateRandomTiles();
    }

    public void SearchStartPosition()
    {
        var HeightOffset = 2f;
        var HeightOffsetPerTile = 0.5f;
        StartPosition = new Vector3(Width * -1, Mathf.RoundToInt(HeightOffsetPerTile * (Height * -1) - HeightOffset), TileDepthZ);
    }

    private protected Tile CreateTile(Vector3 position)
    {
        var tile = Instantiate(_tilePrefab, position, Quaternion.identity);
        tile.transform.SetParent(TilesObjectsTransform);
        return tile;
    }

    private protected Barrier CreateBarrier(Tile positionTile, Barrier.BarrierType Type, int heal)
    {
        Barrier barrier;
        if(Type == Barrier.BarrierType.Rock)
        {
            if(heal == 2)
                barrier = Instantiate(BarriersPrefabs[0], positionTile.transform.position, Quaternion.identity);
            else
                barrier = Instantiate(BarriersPrefabs[1], positionTile.transform.position, Quaternion.identity);
        }
        else
        {
            if (heal == 2)
                barrier = Instantiate(BarriersPrefabs[2], positionTile.transform.position, Quaternion.identity);
            else
                barrier = Instantiate(BarriersPrefabs[3], positionTile.transform.position, Quaternion.identity);
        }

        barrier.X = positionTile.X;
        barrier.Y = positionTile.Y;
        positionTile.IsBarried = true;
        barrier.transform.SetParent(BarriersObjectsTransform);
        return barrier;
    }

    private protected Item CreateItem(Tile postionTile, Item item)
    {
        var spawedItem = Instantiate(item, postionTile.transform.position, Quaternion.identity);
        spawedItem.X = postionTile.X;
        spawedItem.Y = postionTile.Y;
        spawedItem.transform.SetParent(ItemsObjectsTransform);
        return spawedItem;
    }

    public static Item ReturnItem(GameObject itemObject)
    {
        for(var i = 0;i< Width; i++)
        {
            for(var j = 0; j < Height; j++)
            {
                if (AllItems[i, j] == null)
                    continue;

                if(itemObject == AllItems[i, j].gameObject)
                {
                    return AllItems[i, j];
                }
            }
        }
        return null;
    }

    public void GenerateRandomTiles()
    {
        Width = Random.Range(WidthMin, WidthMax);
        Height = Random.Range(HeightMin, HeightMax);
        AllTiles = new Tile[Width, Height];
        QuestsManager.SpawnScoreQuest(Width * Height * QuestsManager.ScoreProgressMultiplyer);
        QuestsManager.SpawnItemQuest(Width * Height * QuestsManager.ItemsProgressMultiplyer);
        AllBariers = new Barrier[Width, Height];
        SearchStartPosition();
        for(var i = 0; i < Width; i++)
        {
            for(var j = 0;j< Height; j++)
            {
                var position = new Vector3()
                {
                    x = StartPosition.x + 1 * i,
                    y = StartPosition.y + 1 * j,
                    z = TileDepthZ
                };

                AllTiles[i, j] = CreateTile(position);
                AllTiles[i, j].X = i;
                AllTiles[i, j].Y = j;
            }
        }

        AlignObjects();
        UpdateCameraSize();
        StartFillTiles();
        CreateRandomBarriers();
        if (!MatchManager.CheckStepAvailable())
            _tileGenerator.RefillTiles();
    }

    public void GenerateTilesByConfig(LevelConfig config)
    {
        AllTiles = new Tile[Width, Height];
        AllBariers = new Barrier[Width, Height];
        if(config.ScoreQuest)
            QuestsManager.SpawnScoreQuest(Width * Height * QuestsManager.ScoreProgressMultiplyer);

        if(config.ItemQuest)
            QuestsManager.SpawnItemQuest(Width * Height * QuestsManager.ItemsProgressMultiplyer);

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

                AllTiles[i,j] = CreateTile(position);
                EditorTileGenerator.ApplyConfigState(AllTiles[i, j], _levelConfig.AllTiles[i, j]);
            }
        }

        AlignObjects();
        StartFillTiles();
        UpdateCameraSize();
        for (var i = 0; i < Width; i++)
        {
            for (var j = 0; j < Height; j++)
            {
                if (AllTiles[i, j].IsBarried)
                {
                    AllBariers[i,j] = CreateBarrier(AllTiles[i, j], _levelConfig.AllBariers[i, j].Type, _levelConfig.AllBariers[i, j].Heal);
                }
            }
        }

        if(config.BarrierQuest)
            QuestsManager.SpawnBariersQuest();

        if (!MatchManager.CheckStepAvailable())
            _tileGenerator.RefillTiles();
    }

    public void CreateRandomBarriers()
    {
        for (var i = 0; i < Width; i++)
        {
            for (var j = 0; j < Height; j++)
            {
                var randomValue = Random.Range(0, 100);
                if (randomValue > 70)
                {
                    var randomType = (Barrier.BarrierType)Random.Range(0, 2);
                    var randomHeal = Random.Range(1, 3);
                    AllBariers[i,j] = CreateBarrier(AllTiles[i, j], randomType, randomHeal);
                }
            }
        }

        QuestsManager.SpawnBariersQuest();
    }
    
    public void StartFillTiles()
    {
        AllItems = new Item[Width, Height];
        for (var i = 0; i < Width; i++)
        {
            for (var j = 0; j < Height; j++)
            {
                if (AllTiles[i, j] != null)
                    AllItems[i, j] = CreateItem(AllTiles[i, j], _itemsPrefabs[Random.Range(0, _itemsPrefabs.Count)]);
            }
        }
    }
    
    public void FillEmptyTiles()
    {
        for (var i = 0; i < Width; i++)
        {
            for (var j = 0; j < Height; j++)
            {
                if(AllItems[i,j] == null)
                {
                    AllItems[i, j] = CreateItem(AllTiles[i, j], _itemsPrefabs[Random.Range(0, _itemsPrefabs.Count)]);
                    AllItems[i, j].transform.localScale = Vector3.zero;
                    AllItems[i, j].transform.DOScale(Vector3.one, 0.5f);
                }
            }
        }
    }
    
    public void AlignObjects()
    {
        float firstTilePos = AllTiles[0, 0].transform.position.x * -1;
        float lastTilePos = AllTiles[Width - 1, Height - 1].transform.position.x;
        if(lastTilePos < 0)
            lastTilePos *= -1;

        while(firstTilePos != lastTilePos)
        {
            if(firstTilePos > lastTilePos)
            {
                for (var i = 0; i < Width; i++)
                {
                    for (var j = 0; j < Height; j++)
                    {
                        AllTiles[i, j].transform.position = new Vector3
                            (AllTiles[i, j].transform.position.x + 0.5f, AllTiles[i, j].transform.position.y, TileDepthZ);
                    }
                }
            }

            if(firstTilePos < lastTilePos)
            {
                for (var i = 0; i < Width; i++)
                {
                    for (var j = 0; j < Height; j++)
                    {
                        AllTiles[i, j].transform.position = new Vector3
                            (AllTiles[i, j].transform.position.x - 0.5f, AllTiles[i, j].transform.position.y, TileDepthZ);
                    }
                }
            }

            firstTilePos = AllTiles[0, 0].transform.position.x * -1;
            lastTilePos = AllTiles[Width - 1, Height - 1].transform.position.x;
            if (lastTilePos < 0)
                lastTilePos *= -1;
        }
    }

    public static void SpawnBonus(Item.BonusType bonusType)
    {
        for(var i = 0; i < Width; i++)
        {
            for(var j = 0; j < Height; j++)
            {
                if(AllItems[i,j] == null)
                {
                    if (bonusType == Item.BonusType.Rocket)
                        AllItems[i, j] = _tileGenerator.CreateItem(AllTiles[i, j], _tileGenerator._rocketPrefab);
                    else
                        AllItems[i, j] = _tileGenerator.CreateItem(AllTiles[i, j], _tileGenerator._bombPrefab);

                    AllItems[i, j].transform.DOScale(1, 0.5f);
                    return;
                }
            }
        }
    }

    public static void FillAfterMatch()
    {
        var tmpY = 0;
        for (var i = 0; i < Width; i++)
        {
            for (var j = 0; j < Height; j++)
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
        for(var i = 0; i < Width; i++)
        {
            for(var j = 0; j < Height; j++)
            {
                if (AllBariers[i, j] != null)
                    continue;

                MatchManager.DestroyAfterAnim(AllItems[i, j]);
                AllItems[i, j] = CreateItem(AllTiles[i, j], _itemsPrefabs[Random.Range(0, _itemsPrefabs.Count)]);
                AllItems[i, j].transform.localScale = Vector3.zero;
                AllItems[i, j].transform.DOScale(Vector3.one, 0.5f);
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
