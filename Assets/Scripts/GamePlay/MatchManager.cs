using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public static class MatchManager
{
    public static List<Item> SelectedItems;
    public static string CurrentTag;

    public static bool CheckNear(Item checkObject)
    {
        var lastItem = SelectedItems[SelectedItems.Count - 1];
        var neighbours = ReturnNeighbours(lastItem.X, lastItem.Y);
        for(var i = 0; i < neighbours.Count; i++)
        {
            if(neighbours[i] == checkObject)
                return true;
        }

        return false;
    }

    public static bool CheckStepAvailable()
    {
        for(var i = 0; i < TileGenerator.Width; i++)
        {
            for(var j = 0; j < TileGenerator.Height; j++)
            {
                if (TileGenerator.AllItems[i, j].IsBonus)
                    return true;

                if (TileGenerator.AllItems[i, j].tag == "Bonus" || TileGenerator.AllBariers[i, j] != null)
                    continue;

                var checkedItem = TileGenerator.AllItems[i, j];
                var objectNeibours = ReturnNeighbours(i, j);
                for(var t = 0;t < objectNeibours.Count; t++)
                {
                    if (objectNeibours[t] == TileGenerator.AllItems[i, j] || TileGenerator.AllBariers[objectNeibours[t].X,objectNeibours[t].Y] != null) 
                        continue;

                    if(objectNeibours[t].tag == TileGenerator.AllItems[i, j].tag || objectNeibours[t].tag == "Bonus")
                    {
                        var nextObjectNeibours = ReturnNeighbours(objectNeibours[t].X, objectNeibours[t].Y);
                        for(var n = 0; n < nextObjectNeibours.Count; n++)
                        {
                            if (nextObjectNeibours[n] == objectNeibours[t] || TileGenerator.AllBariers[nextObjectNeibours[n].X, nextObjectNeibours[n].Y] != null) 
                                continue;

                            if(nextObjectNeibours[n] != checkedItem && nextObjectNeibours[n].tag == checkedItem.tag || nextObjectNeibours[n].tag == "Bonus")
                                return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    public static void CheckBarriersMatch()
    {
        for (var i = 0; i < TileGenerator.Width; i++)
        {
            for (var j = 0; j < TileGenerator.Height; j++)
            {
                if (TileGenerator.AllBariers[i, j] != null)
                {
                    var neighbours = ReturnBarrierNeighbours(i, j);
                    for(var n = 0; n < neighbours.Count; n++)
                    {
                        if(neighbours[n] == null)
                        {
                            HitBarrier(TileGenerator.AllBariers[i, j]);
                            break;
                        }
                    }
                }
            }
        }
    }

    public static void HitBarrier(Barrier barrier)
    {
        barrier.Heal--;
        if (barrier.Heal <= 0)
        {
            TileGenerator.AllTiles[barrier.X, barrier.Y].IsBarried = false;
            TileGenerator.AllBariers[barrier.X, barrier.Y] = null;
            QuestsManager.UpdateBarrierProgress(barrier);
            barrier.GetComponent<SpriteRenderer>().DOFade(0, 0.6f).OnComplete(() => Object.Destroy(barrier.gameObject));
            for (var i = 0; i < TileGenerator.Width; i++)
            {
                for (var j = 0; j < TileGenerator.Height; j++)
                {
                    if (TileGenerator.AllBariers[i, j] == null)
                        TileGenerator.AllTiles[i, j].IsBarried = false;
                }
            }
        }

        if (barrier.Heal == 1 && barrier.Type == Barrier.BarrierType.Rock)
        {
            barrier.transform.DOShakePosition(0.5f, 0.2f);
            barrier.GetComponent<SpriteRenderer>().sprite = barrier.RockBrokenSprite;
        }

        if (barrier.Heal == 1 && barrier.Type == Barrier.BarrierType.Ice)
        {
            barrier.transform.DOShakePosition(0.5f, 0.2f);
            barrier.GetComponent<SpriteRenderer>().sprite = barrier.IceBrokenSprite;
        }

        AudioManager.PlayHitSound();
    }

    public static void DestroyAfterAnim(Item item)
    {
        item.transform.DOScale(0, 0.1f).OnComplete(() => 
        {
            DOTween.Kill(item);
            Object.Destroy(item.gameObject); 
        });
    }

    public static void ActivateBonus(Item bonus)
    {
        var destroyCounter = 0;
        if (bonus.Type == Item.BonusType.Rocket)
        {
            AudioManager.PlayRocketSound();
            var rocketY = bonus.Y;
            for (var i = 0; i < TileGenerator.Width; i++)
            {
                for (var j = 0; j < TileGenerator.Height; j++)
                {
                    if (j == rocketY)
                    {
                        if (TileGenerator.AllBariers[i, j] != null)
                        {
                            HitBarrier(TileGenerator.AllBariers[i, j]);
                            continue;
                        }

                        destroyCounter++;
                        Item ItemToDestroy = TileGenerator.AllItems[i, j];
                        QuestsManager.UpdateItemProgress(TileGenerator.AllItems[i, j]);
                        TileGenerator.AllItems[i, j] = null;
                        DestroyAfterAnim(ItemToDestroy);
                    }
                }
            }
        }
        else
        {
            AudioManager.PlayBombSound();
            var bombX = bonus.X;
            var bombY = bonus.Y;
            for (var i = 0; i < TileGenerator.Width; i++)
            {
                for (var j = 0; j < TileGenerator.Height; j++)
                {
                    if (i == bombX || i == bombX - 1 || i == bombX - 2 || i == bombX + 1 || i == bombX + 2)
                    {
                        if (j == bombY || j == bombY - 1 || j == bombY - 2 || j == bombY + 1 || j == bombY + 2)
                        {
                            TileGenerator.AllTiles[i, j].transform.DOShakePosition(0.3f, 0.2f);
                            if (TileGenerator.AllBariers[i, j] != null)
                            {
                                HitBarrier(TileGenerator.AllBariers[i, j]);
                                continue;
                            }

                            Item ItemToDestroy = TileGenerator.AllItems[i, j];
                            QuestsManager.UpdateItemProgress(TileGenerator.AllItems[i, j]);
                            TileGenerator.AllItems[i, j] = null;
                            DestroyAfterAnim(ItemToDestroy);
                            destroyCounter++;
                        }
                    }
                }
            }
        }

        TileGenerator.FillAfterMatch();
        QuestsManager.UpdateScoreProgress(destroyCounter);
    }

    public static void CheckMatch()
    {
        if (SelectedItems.Count < 3)
        {
            for (var i = 0; i < SelectedItems.Count; i++)
            {
                if (SelectedItems[i] != null && SelectedItems[i].IsBonus)
                    ActivateBonus(SelectedItems[i]);
            }

            for (var i = 0; i < SelectedItems.Count; i++)
            {
                if (SelectedItems[i] != null)
                    SelectedItems[i].transform.DOScale(1, 0.5f);
            }

            SelectedItems.Clear();
            CurrentTag = null;
        }
        else
        {
            PlayerControl.PlayerSteps--;
            QuestsManager.UpdateScoreProgress(SelectedItems.Count);
            for (var i = 0; i < SelectedItems.Count; i++)
            {
                if (SelectedItems != null && SelectedItems[i].IsBonus)
                    ActivateBonus(SelectedItems[i]);
            }

            for (var t = 0; t < SelectedItems.Count; t++)
            {
                for (var i = 0; i < TileGenerator.Width; i++)
                {
                    for (var j = 0; j < TileGenerator.Height; j++)
                    {
                        if (SelectedItems[t] != null && SelectedItems[t] == TileGenerator.AllItems[i, j])
                        {
                            QuestsManager.UpdateItemProgress(TileGenerator.AllItems[i, j]);
                            TileGenerator.AllItems[i, j] = null;
                        }
                    }
                }

                if (SelectedItems[t] != null)
                    DestroyAfterAnim(SelectedItems[t]);
            }

            if (SelectedItems.Count >= 5 && SelectedItems.Count <= 7)
                TileGenerator.SpawnBonus(Item.BonusType.Rocket);

            if (SelectedItems.Count >= 8)
                TileGenerator.SpawnBonus(Item.BonusType.Bomb);

            SelectedItems.Clear();
            CurrentTag = null;
            CheckBarriersMatch();
            TileGenerator.FillAfterMatch();
            CheckStepAvailable();
            AudioManager.PlayMatchedSound();
        }
    }

    public static List<Item> ReturnNeighbours(int refx, int refy)
    {
        var neighbours = from x in Enumerable.Range(refx - 1, 3)
                         from y in Enumerable.Range(refy - 1, 3)
                         where x >= 0 && y >= 0 && x < TileGenerator.AllItems.GetLength(0) && y < TileGenerator.AllItems.GetLength(1)
                         select TileGenerator.AllItems[x,y];
        return neighbours.ToList();
    }

    public static List<Item> ReturnBarrierNeighbours(int refx, int refy)
    {
        var neighbours = from x in Enumerable.Range(refx - 1, 3)
                         from y in Enumerable.Range(refy, 1)
                         where x >= 0 && y >= 0 && x < TileGenerator.AllItems.GetLength(0) && y < TileGenerator.AllItems.GetLength(1)
                         select TileGenerator.AllItems[x, y];
        return neighbours.Union(from x in Enumerable.Range(refx, 1)
                                from y in Enumerable.Range(refy - 1, 3)
                                where (x >= 0 && y >= 0 && x < TileGenerator.AllItems.GetLength(0) && y < TileGenerator.AllItems.GetLength(1))
                                select TileGenerator.AllItems[x, y]).ToList();
    }

}