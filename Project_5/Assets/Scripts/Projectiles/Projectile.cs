using UnityEngine;
using UnityEngine.Networking;

public abstract class Projectile : NetworkBehaviour
{
    /// <summary>
    /// playerId of player that fired projectile
    /// </summary>
    [HideInInspector]
    public BasicPlayerController FiringPlayer { get; set; }

    /// <summary>
    /// the amount to take away from health when this projectile hits something
    /// </summary>
    protected abstract int Damage { get; }

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