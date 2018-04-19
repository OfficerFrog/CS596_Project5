
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : BasicPlayerController
{
    // TODO: add ability to handle multiple weapons

    /// <summary>
    /// max amount of bullets player can hold
    /// </summary>
    [SerializeField]
    private uint _bulletCapacity;

    /// <summary>
    /// amount of bullets player currently has
    /// </summary>
    private uint _currentBulletAmmo;

    // TODO: remove and let GameManager handle status of Enemies
    private int _enemyCount;

    public override ObjectWithExperience ExperienceData
    {
        get { return new ObjectWithExperience { Type = ObjectWithExperienceType.Player, Experience = 0 }; }
    }

    /// <summary>
    /// used instead of constructor (which doesnt work for setting fields)
    /// </summary>
    void Awake()
    {
        _currentBulletAmmo = _bulletCapacity;
        // get EnemySpawner found in scene
        _enemyCount = GameObject.FindObjectOfType<EnemySpawner>().NumberOfEnemies;
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        UpdateMovement();

        if (Input.GetKeyDown(KeyCode.Return))
            WeaponShot();
        else  if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("Menu");
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

    private void WeaponShot()
    {
        if (_currentBulletAmmo == 0)
        {
            // TODO: notify user
            return;
        }
        // update bullets  (TODO: if have different types of ammo, need to move this)
        _currentBulletAmmo -= 1;

        CmdFire();
    }


    /// <summary>
    /// display the player of the current user as Blue
    /// </summary>
    //public override void OnStartLocalPlayer()
    //{
    //    this.GetComponent<MeshRenderer>().material.color = Color.blue;
    //}

    /// <summary>
    /// player ran into something, see if it is an object it can pick up
    /// </summary>
    /// <remarks>
    /// NOTE: If enemies can pick up objects, move this method to base class
    /// </remarks>
    void OnTriggerEnter(Collider collision)
    {
        var pickupItem = collision.gameObject.GetComponent<IPickupItem>();
        if (pickupItem != null)
        {
            switch (pickupItem.Type)
            {
                case PickupType.Ammo:
                    AddAmmoToPlayer(pickupItem.Amount);
                    break;

                case PickupType.Health:
                    AddHealthToPlayer(pickupItem.Amount);
                    break;
            }
            // remove picked up item
            Destroy(collision.gameObject);
        }
    }

    private void AddAmmoToPlayer(uint ammoAmount)
    {
        _currentBulletAmmo += ammoAmount;
        if (_currentBulletAmmo > _bulletCapacity)
            _currentBulletAmmo = _bulletCapacity;
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

    public override void EnemyKilled(DismissibleObjectController enemy)
    {
        // TODO: remove and let GameManager handle status of Enemies
        // ends game if all enemies are defeated (* assumes only one player killed all enemies)
        if (ObjectsDestroyedCounts[enemy.ExperienceData.Type] == _enemyCount)
            SceneManager.LoadScene("Game Win");
    }
}
