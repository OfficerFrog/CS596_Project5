using UnityEngine;

public class MediumAmmoClip : MonoBehaviour, IPickupItem
{
    public PickupType Type { get { return PickupType.Ammo; } }

    public uint Amount
    {
        get { return 20; }
    }

    public uint Experience
    {
        get { return 10; }
    }
}