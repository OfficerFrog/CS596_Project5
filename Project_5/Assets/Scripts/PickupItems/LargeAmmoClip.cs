using UnityEngine;

public class LargeAmmoClip : MonoBehaviour, IPickupItem
{
    public PickupType Type { get { return PickupType.Ammo; } }

    public uint Amount
    {
        get { return 50; }
    }

    public uint Experience
    {
        get { return 20; }
    }
}