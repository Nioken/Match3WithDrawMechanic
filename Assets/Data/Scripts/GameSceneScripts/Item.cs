public class Item : MoveableObject
{
    public BonusType bonusType;
    public bool IsBonus;

    public enum BonusType
    {
        Rocket,
        Bomb
    }
}
