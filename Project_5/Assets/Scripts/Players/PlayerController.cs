
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : BasicObjectController
{
    // TODO: add ability to handle multiple weapons

    public GameObject BulletPrefab;
    public Transform ProjectileSpawn;

    void Update()
    {
        if (!isLocalPlayer)
            return;

        UpdateMovement();

        if (Input.GetKeyDown(KeyCode.Space))
            CmdFire();
    }

    private void UpdateMovement()
    {
        float horizontalMovement = Input.GetAxis("Horizontal") * Time.deltaTime;
        float verticalMovement = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;


        // if has both movements, means player is turning in circle
        if (horizontalMovement != 0.0f && verticalMovement != 0.0f)
        {
            // horizontal movement equals rotation
            float horizontalRotation = horizontalMovement * 150.0f;
            transform.Rotate(0, horizontalRotation, 0);
            transform.Translate(0, 0, verticalMovement);
        }
        else
        {
            // forward/back or left/right
            float newLeftRightMovement = horizontalMovement * 3.0f;  // want left/right speed to be equal to front/back
            transform.Translate(newLeftRightMovement, 0, verticalMovement);

        }
    }

    // [Command] code is called on the Client but ran on the Server
    [Command]
    void CmdFire()
    {
        this.GetComponent<Health>().TakeDamage(10);

        // create an instane of the projectile we want to fire
        GameObject projectile = Instantiate(
            BulletPrefab,
            ProjectileSpawn.position,
            ProjectileSpawn.rotation);

        // TODO: Would be better if projectile did itself when instantiated
        // give the projectile some velocity; 
        projectile.GetComponent<Bullet>().SetVelocity(projectile.transform.forward);

        // spawn the projectile on al of the connected Clients
        NetworkServer.Spawn(projectile);

        // destroy it after some arbitrary amount of time; 2 seconds seems good enough
        Destroy(projectile, 2.0f);
    }



    /// <summary>
    /// display the player of the current user as Blue
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        this.GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    /// <summary>
    /// player ran into something, see if it is an object it can pick up
    /// </summary>
    /// <remarks>
    /// NOTE: If enemies can pick up objects, move this method to base class
    /// </remarks>
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "HealthObject")
        {
            AddHealthToPlayer(collision.gameObject.GetComponent<HealthOrb>().Health);
            Destroy(collision.gameObject);
        }
    }

    private void AddHealthToPlayer(uint healthAmount)
    {
        this.GetComponent<Health>().TakeHeal(healthAmount);
    }

    /// <summary>
    /// respawn player (in set time and place)
    /// </summary>
    public override void Respawn()
    {
        
    }
}
