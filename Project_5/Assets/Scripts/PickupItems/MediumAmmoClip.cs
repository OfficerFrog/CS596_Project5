using UnityEngine;

public class MediumAmmoClip : MonoBehaviour, IPickupItem
{
    public PickupType Type { get { return PickupType.Ammo; } }

    public int Amount
    {
        get { return 20; }
    }

    public int Experience
    {
        get { return 10; }
    }
}