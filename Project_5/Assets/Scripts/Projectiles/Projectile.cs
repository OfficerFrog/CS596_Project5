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
    public abstract float Velocity { get; }

    /// <summary>
    /// set the velocity of the projectile
    /// </summary>
    /// <param name="initalVelocity">velocity of entity that shot the projectile (e.g. user running should add velocity to total)</param>
    /// <param name="transformForward">directional vector</param>
    public void SetVelocity(float initalVelocity, Vector3 transformForward)
    {
        this.GetComponent<Rigidbody>().velocity = transformForward * (initalVelocity + Velocity);
    }
}