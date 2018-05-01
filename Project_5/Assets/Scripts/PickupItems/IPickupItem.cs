public enum PickupType
{
    Health,
    Ammo
}

public interface IPickupItem
{
    PickupType Type { get; }
    int Amount { get; }

    int Experience { get; }
}