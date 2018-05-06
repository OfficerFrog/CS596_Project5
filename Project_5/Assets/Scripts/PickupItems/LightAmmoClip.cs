
public class LightAmmoClip : PickupItemBase
{
    public override PickupType Type { get { return PickupType.Ammo; } }

    public override int Amount
    {
        get { return 20; }
    }

    public override int Experience
    {
        get { return 10; }
    }

    public override float RespawnTime
    {
        get { return 15f; }
    }
}