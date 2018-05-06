using System;
using UnityEngine;

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

    float RespawnTime { get; }
}

public abstract class PickupItemBase : MonoBehaviour, IPickupItem
{
    public event EventHandler ItemWasPickedUp;

    public abstract PickupType Type { get; }
    public abstract int Amount { get; }
    public abstract int Experience { get; }
    public abstract float RespawnTime { get; }

    public void ItemPickedUp()
    {
        if (ItemWasPickedUp != null)
            ItemWasPickedUp(this, new EventArgs());
    }
}