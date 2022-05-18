using UnityEngine;
using DG.Tweening;

public class Barrier : MoveableObject
{
    [SerializeField]
    public BarrierType barrierType;
    [SerializeField]
    public Sprite RockBrokenSprite;
    public Sprite IceBrokenSprite;
    public int heal;

    private void Start()
    {
        if (barrierType == BarrierType.Ice)
        {
            gameObject.transform.DOScale(1.5f, 0.5f).OnComplete(() => gameObject.transform.DOScale(1.4f, 0.5f)).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }
    }

    public enum BarrierType
    {
        Ice,
        Rock
    }

}
