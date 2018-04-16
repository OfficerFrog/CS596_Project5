using UnityEngine;

public class HealthOrb : MonoBehaviour, IPickupItem
{
    public PickupType Type
    {
        get { return PickupType.Health; }
    }

    /// <summary>
    /// how much health should be gained when picked up
    /// </summary>
    public uint Amount
    {
        get { return 30; }
    }
}

