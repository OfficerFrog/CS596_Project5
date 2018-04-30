public enum PickupType
{
    Health,
    Ammo
}

public interface IPickupItem
{
    PickupType Type { get; }
    uint Amount { get; }

    uint Experience { get; }
}