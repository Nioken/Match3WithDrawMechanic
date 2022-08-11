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
        var lastItem = SelectedItems[^1];
        var neighbours = ReturnNeighbours(lastItem.X, lastItem.Y);
        
        foreach (var neighbour in neighbours)
            if (neighbour == checkObject)
                return true;

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

                var checkedItem = TileGenerator.AllItems[i, j];
                var objectNeibours = ReturnNeighbours(i, j);
                
                foreach (var neighbour in objectNeibours)
                {
                    if (neighbour == TileGenerator.AllItems[i, j] || TileGenerator.AllBariers[neighbour.X,neighbour.Y] != null) 
                        continue;

                    if (!neighbour.CompareTag(TileGenerator.AllItems[i, j].tag) &&
                        !neighbour.CompareTag("Bonus")) 
                        continue;
                    
                    var nextObjectNeibours = ReturnNeighbours(neighbour.X, neighbour.Y);
                        
                    foreach (var nextNeighbour in nextObjectNeibours)
                    {
                        if (nextNeighbour == neighbour || TileGenerator.AllBariers[nextNeighbour.X, nextNeighbour.Y] != null) 
                            continue;

                        if(nextNeighbour != checkedItem && nextNeighbour.CompareTag(checkedItem.tag) || nextNeighbour.CompareTag("Bonus"))
                            return true;
                    }
                }
            }
        }

        return false;
    }

    private static void CheckBarriersMatch()
    {
        for (var i = 0; i < TileGenerator.Width; i++)
        for (var j = 0; j < TileGenerator.Height; j++)
        {
            if (TileGenerator.AllBariers[i, j] == null)
                continue;

            var neighbours = ReturnBarrierNeighbours(i, j);

            foreach (var neighbour in neighbours)
            {
                if (neighbour != null)
                    continue;

                HitBarrier(TileGenerator.AllBariers[i, j]);

                break;
            }
        }
    }

    private static void HitBarrier(Barrier barrier)
    {
        barrier.Heal--;

        if (barrier.Heal <= 0)
        {
            TileGenerator.AllTiles[barrier.X, barrier.Y].IsBarried = false;
            TileGenerator.AllBariers[barrier.X, barrier.Y] = null;

            QuestsManager.UpdateBarrierProgress(barrier);

            barrier.GetComponent<SpriteRenderer>().DOFade(0, 0.6f).OnComplete(() => Object.Destroy(barrier.gameObject));

            for (var i = 0; i < TileGenerator.Width; i++)
            for (var j = 0; j < TileGenerator.Height; j++)
                if (TileGenerator.AllBariers[i, j] == null)
                    TileGenerator.AllTiles[i, j].IsBarried = false;
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

    private static void ActivateBonus(Item bonus)
    {
        var destroyCounter = 0;
        
        if (bonus.Type == Item.BonusType.Rocket)
        {
            AudioManager.PlayRocketSound();
            var rocketY = bonus.Y;
            
            for (var i = 0; i < TileGenerator.Width; i++)
            for (var j = 0; j < TileGenerator.Height; j++)
            {
                if (j != rocketY)
                    continue;

                if (TileGenerator.AllBariers[i, j] != null)
                {
                    HitBarrier(TileGenerator.AllBariers[i, j]);
                    continue;
                }

                destroyCounter++;

                var itemToDestroy = TileGenerator.AllItems[i, j];
                QuestsManager.UpdateItemProgress(TileGenerator.AllItems[i, j]);
                TileGenerator.AllItems[i, j] = null;

                DestroyAfterAnim(itemToDestroy);
            }
        }
        else
        {
            AudioManager.PlayBombSound();
            var bombX = bonus.X;
            var bombY = bonus.Y;
            
            for (var i = 0; i < TileGenerator.Width; i++)
            for (var j = 0; j < TileGenerator.Height; j++)
            {
                if (i != bombX && i != bombX - 1 && i != bombX - 2 && i != bombX + 1 && i != bombX + 2)
                    continue;

                if (j != bombY && j != bombY - 1 && j != bombY - 2 && j != bombY + 1 && j != bombY + 2)
                    continue;

                TileGenerator.AllTiles[i, j].transform.DOShakePosition(0.3f, 0.2f);

                if (TileGenerator.AllBariers[i, j] != null)
                {
                    HitBarrier(TileGenerator.AllBariers[i, j]);
                    continue;
                }

                var itemToDestroy = TileGenerator.AllItems[i, j];
                QuestsManager.UpdateItemProgress(TileGenerator.AllItems[i, j]);

                TileGenerator.AllItems[i, j] = null;

                DestroyAfterAnim(itemToDestroy);
                destroyCounter++;
            }
        }

        TileGenerator.FillAfterMatch();
        QuestsManager.UpdateScoreProgress(destroyCounter);
    }

    public static void CheckMatch()
    {
        if (SelectedItems.Count < 3)
        {
            foreach (var item in SelectedItems.Where(item => item != null && item.IsBonus))
                ActivateBonus(item);

            foreach (var item in SelectedItems.Where(item => item != null))
                item.transform.DOScale(1, 0.5f);

            SelectedItems.Clear();
            CurrentTag = null;
        }
        else
        {
            PlayerControl.PlayerSteps--;
            QuestsManager.UpdateScoreProgress(SelectedItems.Count);
            
            foreach (var item in SelectedItems.Where(item => SelectedItems != null && item.IsBonus))
                ActivateBonus(item);

            foreach (var item in SelectedItems)
            {
                for (var i = 0; i < TileGenerator.Width; i++)
                for (var j = 0; j < TileGenerator.Height; j++)
                {
                    if (item == null || item != TileGenerator.AllItems[i, j]) 
                        continue;
                    
                    QuestsManager.UpdateItemProgress(TileGenerator.AllItems[i, j]);
                    TileGenerator.AllItems[i, j] = null;
                }

                if (item != null)
                    DestroyAfterAnim(item);
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

    private static List<Item> ReturnNeighbours(int refx, int refy)
    {
        var neighbours = from x in Enumerable.Range(refx - 1, 3)
                         from y in Enumerable.Range(refy - 1, 3)
                         where x >= 0 && y >= 0 && x < TileGenerator.AllItems.GetLength(0) && y < TileGenerator.AllItems.GetLength(1)
                         select TileGenerator.AllItems[x,y];
        return neighbours.ToList();
    }

    private static List<Item> ReturnBarrierNeighbours(int refx, int refy)
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