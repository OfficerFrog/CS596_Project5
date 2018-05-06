
public class HealthOrb : PickupItemBase
{
    public override PickupType Type
    {
        get { return PickupType.Health; }
    }

    /// <summary>
    /// how much health should be gained when picked up
    /// </summary>
    public override int Amount
    {
        get { return 30; }
    }

    public override int Experience
    {
        get { return 30; }
    }

    public override float RespawnTime
    {
        get { return 20f; }
    }
}

