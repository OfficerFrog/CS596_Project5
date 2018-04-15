
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public GameObject ProjectilePrefab;
    public Transform ProjectileSpawn;

    void Update()
    {
        if (!isLocalPlayer)
            return;

        UpdateMovement();

        if (Input.GetKeyDown(KeyCode.Space))
            Fire();
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

    void Fire()
    {
        // create an instane of the projectile we want to fire
        GameObject projectile = Instantiate(
            ProjectilePrefab,
            ProjectileSpawn.position,
            ProjectileSpawn.rotation);

        // give the projectile some velocity
        projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * 7;

        // destroy it after some arbitrary amount of time; 2 seconds seems good enough
        Destroy(projectile, 2.0f);
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }
}
