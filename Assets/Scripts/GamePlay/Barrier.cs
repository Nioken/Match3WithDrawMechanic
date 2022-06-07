using UnityEngine;
using DG.Tweening;

public class Barrier : MovableObject
{
    public BarrierType Type;
    public Sprite RockBrokenSprite;
    public Sprite IceBrokenSprite;
    public int Heal;

    private void Start()
    {
        if (Type == BarrierType.Ice)
        {
            gameObject.transform.DOScale(1.5f, 0.5f).OnComplete(() => 
                gameObject.transform.DOScale(1.4f, 0.5f)).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }
    }

    public enum BarrierType
    {
        Ice = 0,
        Rock
    }

}
