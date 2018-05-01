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
    public int Amount
    {
        get { return 30; }
    }

    public int Experience
    {
        get { return 30; }
    }
}

