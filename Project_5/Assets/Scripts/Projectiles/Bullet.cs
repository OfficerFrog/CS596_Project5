
public class Bullet : Projectile
{
    protected override uint Damage
    {
        get { return 10; }
    }

    public override int Velocity
    {
        get { return 7; }
    }
}
