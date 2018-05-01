using UnityEngine;

public class LargeAmmoClip : MonoBehaviour, IPickupItem
{
    public PickupType Type { get { return PickupType.Ammo; } }

    public int Amount
    {
        get { return 50; }
    }

    public int Experience
    {
        get { return 20; }
    }
}