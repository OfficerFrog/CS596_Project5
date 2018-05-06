
public class HeavyAmmoClip : PickupItemBase
{
    public override PickupType Type { get { return PickupType.Ammo; } }

    public override int Amount
    {
        get { return 50; }
    }

    public override int Experience
    {
        get { return 20; }
    }

    public override float RespawnTime
    {
        get { return 60f; }
    }
}