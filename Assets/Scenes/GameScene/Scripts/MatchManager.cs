using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MatchManager : MonoBehaviour
{
    public static List<Item> SelectedItems;
    public static string CurrentTag;

    public static bool CheckNear(Item checkObject)
    {
        Item lastItem = SelectedItems[SelectedItems.Count - 1];
        Item thisItem = checkObject;
        if (thisItem.X + 1 == lastItem.X && thisItem.Y == lastItem.Y)
        {
            return true;
        }
        if (thisItem.X - 1 == lastItem.X && thisItem.Y == lastItem.Y)
        {
            return true;
        }

        if (thisItem.X == lastItem.X && thisItem.Y - 1 == lastItem.Y)
        {
            return true;
        }
        if (thisItem.X == lastItem.X && thisItem.Y + 1 == lastItem.Y)
        {
            return true;
        }
        if (thisItem.X - 1 == lastItem.X && thisItem.Y - 1 == lastItem.Y)
        {
            return true;
        }
        if (thisItem.X + 1 == lastItem.X && thisItem.Y + 1 == lastItem.Y)
        {
            return true;
        }
        if (thisItem.X - 1 == lastItem.X && thisItem.Y + 1 == lastItem.Y)
        {
            return true;
        }
        if (thisItem.X + 1 == lastItem.X && thisItem.Y - 1 == lastItem.Y)
        {
            return true;
        }
        return false;
    }

    public static void CheckBarriersMatch()
    {
        for(int i = 0; i < TileGenerator.X; i++)
        {
            for(int j = 0; j < TileGenerator.Y; j++)
            {
                if(TileGenerator.AllBariers[i,j] != null)
                {
                    if(i == 0)
                    {
                        if(j == 0)
                        {
                            if(TileGenerator.AllItems[i+1,j] == null || TileGenerator.AllItems[i, j + 1] == null)
                            {
                                HitBarrier(TileGenerator.AllBariers[i, j]);
                            }
                        }
                        if(j == TileGenerator.Y-1)
                        {
                            if (TileGenerator.AllItems[i + 1, j] == null || TileGenerator.AllItems[i, j - 1] == null)
                            {
                                HitBarrier(TileGenerator.AllBariers[i, j]);
                            }
                        }
                        if(j != 0 && j != TileGenerator.Y-1)
                        {
                            if (TileGenerator.AllItems[i + 1, j] == null || TileGenerator.AllItems[i, j - 1] == null || TileGenerator.AllItems[i, j + 1] == null)
                            {
                                HitBarrier(TileGenerator.AllBariers[i, j]);
                            }
                        }
                    }
                    if (i == TileGenerator.X-1)
                    {
                        if (j == 0)
                        {
                            if (TileGenerator.AllItems[i - 1, j] == null || TileGenerator.AllItems[i, j + 1] == null)
                            {
                                HitBarrier(TileGenerator.AllBariers[i,j]);
                            }
                        }
                        if (j == TileGenerator.Y-1)
                        {
                            if (TileGenerator.AllItems[i - 1, j] == null || TileGenerator.AllItems[i, j - 1] == null)
                            {
                                HitBarrier(TileGenerator.AllBariers[i, j]);
                            }
                        }
                        if (j != 0 && j != TileGenerator.Y-1)
                        {
                            if (TileGenerator.AllItems[i - 1, j] == null || TileGenerator.AllItems[i, j - 1] == null || TileGenerator.AllItems[i, j + 1] == null)
                            {
                                HitBarrier(TileGenerator.AllBariers[i, j]);
                            }
                        }
                    }
                    if(i != 0 && i != TileGenerator.X-1)
                    {
                        if (j == 0)
                        {
                            if (TileGenerator.AllItems[i - 1, j] == null || TileGenerator.AllItems[i, j + 1] == null || TileGenerator.AllItems[i + 1, j] == null)
                            {
                                HitBarrier(TileGenerator.AllBariers[i, j]);
                            }
                        }
                        if (j == TileGenerator.Y-1)
                        {
                            if (TileGenerator.AllItems[i - 1, j] == null || TileGenerator.AllItems[i, j - 1] == null || TileGenerator.AllItems[i + 1, j] == null)
                            {
                                HitBarrier(TileGenerator.AllBariers[i, j]);
                            }
                        }
                        if (j != 0 && j != TileGenerator.Y-1)
                        {
                            if (TileGenerator.AllItems[i - 1, j] == null || TileGenerator.AllItems[i, j - 1] == null || TileGenerator.AllItems[i, j + 1] == null || TileGenerator.AllItems[i + 1, j] == null)
                            {
                                HitBarrier(TileGenerator.AllBariers[i, j]);
                            }
                        }
                    }
                }
            }
        }
    }

    public static void HitBarrier(Barrier barrier)
    {
        barrier.heal--;
        if(barrier.heal <= 0)
        {
            TileGenerator.AllTiles[barrier.X, barrier.Y].IsBarried = false;
            TileGenerator.AllBariers[barrier.X, barrier.Y] = null;
            QuestsManager.UpdateBarrierProgress(barrier);
            barrier.GetComponent<SpriteRenderer>().DOFade(0, 0.6f).OnComplete(() => Destroy(barrier.gameObject));
            for (int i = 0; i < TileGenerator.X; i++)
            {
                for(int j = 0; j < TileGenerator.Y; j++)
                {
                    if(TileGenerator.AllBariers[i,j] == null)
                    {
                        TileGenerator.AllTiles[i, j].IsBarried = false;
                    }
                }
            }
            
        }
        if(barrier.heal == 1 && barrier.barrierType == Barrier.BarrierType.Rock)
        {
            barrier.transform.DOShakePosition(0.5f, 0.2f);
            barrier.GetComponent<SpriteRenderer>().sprite = barrier.RockBrokenSprite;
        }
        if (barrier.heal == 1 && barrier.barrierType == Barrier.BarrierType.Ice)
        {
            barrier.transform.DOShakePosition(0.5f, 0.2f);
            barrier.GetComponent<SpriteRenderer>().sprite = barrier.IceBrokenSprite;
        }
        AudioManager.PlayHitSound();
    }

    public static void DestroyAfterAnim(Item item)
    {
        item.transform.DOScale(0, 0.1f).OnComplete(() => { DOTween.Kill(item); Destroy(item.gameObject); });
    }

    public static void ActivateBonus(Item bonus)
    {
        int destroyCounter = 0;
        if(bonus.bonusType == Item.BonusType.Rocket)
        {
            AudioManager.PlayRocketSound();
            int rocketY = bonus.Y;
            for(int i = 0; i < TileGenerator.X; i++)
            {
                for(int j = 0; j < TileGenerator.Y; j++)
                {
                    if(j == rocketY)
                    {
                        if (TileGenerator.AllBariers[i,j] != null)
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
            int bombX = bonus.X;
            int bombY = bonus.Y;
            for (int i = 0; i < TileGenerator.X; i++)
            {
                for (int j = 0; j < TileGenerator.Y; j++)
                {
                    if(i == bombX || i == bombX - 1 || i == bombX - 2 || i == bombX + 1 || i == bombX + 2)
                    {
                        if(j == bombY || j == bombY - 1 || j == bombY - 2 || j == bombY + 1 || j == bombY + 2)
                        {
                            TileGenerator.AllTiles[i,j].transform.DOShakePosition(0.3f, 0.2f);
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
        if(SelectedItems.Count < 3)
        {
            for (int i = 0; i < SelectedItems.Count; i++)
            {
                if (SelectedItems[i].IsBonus)
                    ActivateBonus(SelectedItems[i]);
            }
            for (int i = 0; i < SelectedItems.Count; i++)
            {
                SelectedItems[i].transform.DOScale(1, 0.5f);
            }
            SelectedItems.Clear();
            CurrentTag = null;
        }
        else
        {
            PlayerControl.PlayerSteps--;
            QuestsManager.UpdateScoreProgress(SelectedItems.Count);
            for (int i = 0; i < SelectedItems.Count; i++)
            {
                if (SelectedItems[i].IsBonus)
                    ActivateBonus(SelectedItems[i]);
            }
            for (int t = 0; t < SelectedItems.Count; t++)
            {
                for (int i = 0; i < TileGenerator.X; i++)
                {
                    for (int j = 0; j < TileGenerator.Y; j++)
                    {
                        if (SelectedItems[t] == TileGenerator.AllItems[i, j])
                        {
                            QuestsManager.UpdateItemProgress(TileGenerator.AllItems[i, j]);
                            TileGenerator.AllItems[i, j] = null;
                        }
                    }
                }
                DestroyAfterAnim(SelectedItems[t]);
            }
            if (SelectedItems.Count >= 5 && SelectedItems.Count <= 7)
            {
                TileGenerator.SpawnBonus(Item.BonusType.Rocket);
            }
            if (SelectedItems.Count >= 8)
            {
                TileGenerator.SpawnBonus(Item.BonusType.Bomb);
            }
            SelectedItems.Clear();
            CurrentTag = null;
            CheckBarriersMatch();
            TileGenerator.FillAfterMatch();
            AudioManager.PlayMatchedSound();
        }
    }

}
