public class Item : MovableObject
{
    public BonusType Type;
    public bool IsBonus;

    public enum BonusType
    {
        Rocket,
        Bomb
    }
}
