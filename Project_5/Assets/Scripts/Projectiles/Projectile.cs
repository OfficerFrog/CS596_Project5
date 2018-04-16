using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        // if object collided with has health, reduce it
        GameObject collisionObject = collision.gameObject;
        var health = collisionObject.GetComponent<Health>();
        if (health != null)
            health.TakeDamage(Damage);

        // remove projectile from game when it collides with anything
        Destroy(gameObject);
    }

    /// <summary>
    /// the amount to take away from health when this projectile hits something
    /// </summary>
    protected abstract uint Damage { get; }

    /// <summary>
    /// how fast this projectile should go after being fired
    /// </summary>
    public abstract int Velocity { get; }

    public void SetVelocity(Vector3 transformForward)
    {
        this.GetComponent<Rigidbody>().velocity = transformForward * Velocity;
    }
}