using UnityEngine;
using UnityEngine.Networking;

public abstract class Projectile : NetworkBehaviour
{
    /// <summary>
    /// playerId of player that fired projectile
    /// </summary>
    [HideInInspector]
    public BasicPlayerController FiringPlayer { get; set; }

    // [Command] code is called on the Client but ran on the Server
    [Command]
    public void CmdFire(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;

        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(ray.origin, ray.direction * 3f, Color.red, 1f);

        bool result = Physics.Raycast(ray, out hit, 50f);

        if (result)
        {
            Health enemy = hit.transform.GetComponent<Health>();

            if (enemy != null)
            {
                enemy.TakeDamage(FiringPlayer, Damage);
            }
        }


    }

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