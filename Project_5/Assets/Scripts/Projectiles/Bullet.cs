
public class Bullet : Projectile
{
    protected override int Damage
    {
        get { return 10; }
    }

    public override float Velocity
    {
        get { return 7f; }
    }
}
