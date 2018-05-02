
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlayerController : BasicPlayerController
{
    /// <summary>
    /// max amount of bullets player can hold
    /// </summary>
    [SerializeField]
    private int _bulletCapacity;

    [Tooltip("Minimum time between shots")]
    [SerializeField]
    private float _shotCooldown = .3f;

    /// <summary>
    /// amount of bullets player currently has
    /// </summary>
    [SyncVar(hook = "OnCurrentBulletAmmoChanged")]
    private int _currentBulletAmmo;

    [SyncVar(hook = "OnEnemiesKilledChanged")]
    private int _enemiesKilled;

    /// <summary>
    /// the current experience the player has gained so far
    /// </summary>
    [SyncVar(hook = "OnExperienceChanged")]
    private int _experience;

    [SerializeField]
    private int _killsToWin = 3;

    [HideInInspector]
    private float _ellapsedTimeBetweenUpdates;

    public override ObjectWithExperience ExperienceData
    {
        get { return new ObjectWithExperience { Type = ObjectWithExperienceType.Player, Experience = 0 }; }
    }

    void Start()
    {
        EnablePlayer();

        if (!isLocalPlayer)
            return;

        _currentBulletAmmo = _bulletCapacity;
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        _ellapsedTimeBetweenUpdates += Time.deltaTime;

        UpdateMovement();

        if (Input.GetKeyDown(KeyCode.Return) && _ellapsedTimeBetweenUpdates >= _shotCooldown)
        {
            _ellapsedTimeBetweenUpdates = 0f;
            WeaponShot();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("Menu");
    }

    private void EnablePlayer()
    {
        if (isLocalPlayer)
            PlayerCanvas.canvasInstance.Initialize();
        
        //OnToggleShared.Invoke(true);
        //OnToggleLocal.Invoke(this.isLocalPlayer);
        //OnToggleRemote.Invoke(!this.isLocalPlayer);
    }

    /// <summary>
    /// remove events when player dies, logs off, etc
    /// </summary>
    private void DisablePlayer()
    {
        //OnToggleShared.Invoke(false);
        //OnToggleLocal.Invoke(false);
        //OnToggleRemote.Invoke(false);
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
            PlayerCanvas.canvasInstance.DisplayGameStatus("Out of Ammo!");
            return;
        }
        // update bullets  (TODO: if have different types of ammo, need to move this)
        _currentBulletAmmo -= 1;

        CmdFire();
    }

    /// <summary>
    /// player ran into something, see if it is an object it can pick up
    /// </summary>
    /// <remarks>
    /// NOTE: If enemies can pick up objects, move this method to base class
    /// </remarks>
    void OnTriggerEnter(Collider collision)
    {
        if (!isLocalPlayer)
            return;

        var pickupItem = collision.gameObject.GetComponent<IPickupItem>();
        if (pickupItem != null)
        {
            switch (pickupItem.Type)
            {
                case PickupType.Ammo:
                    AddAmmoToPlayer(pickupItem);
                    break;

                case PickupType.Health:
                    AddHealthToPlayer(pickupItem);
                    break;
                default:
                    return;
            }

            // remove picked up item
            Destroy(collision.gameObject);
        }
    }

    private void AddAmmoToPlayer(IPickupItem ammoItem)
    {
        if (_currentBulletAmmo >= _bulletCapacity)
            return;

        _currentBulletAmmo += ammoItem.Amount;
        if (_currentBulletAmmo > _bulletCapacity)
            _currentBulletAmmo = _bulletCapacity;
        // add experience for picking up ammo
        _experience += (int)ammoItem.Experience;
    }

    private void AddHealthToPlayer(IPickupItem healthItem)
    {
        // only gain experience if increased health
        if (this.GetComponent<Health>().TakeHeal(healthItem.Amount))
            _experience += (int)healthItem.Experience;
    }

    // called from "_currentBulletAmmo" syncvar
    void OnCurrentBulletAmmoChanged(int value)
    {
        _currentBulletAmmo = value;// update client value
        if (isLocalPlayer)
            PlayerCanvas.canvasInstance.SetAmmo(value);
    }

    // called from "_enemiesKilled" syncvar
    void OnEnemiesKilledChanged(int value)
    {
        _enemiesKilled = value;// update client value
        if (isLocalPlayer)
            PlayerCanvas.canvasInstance.SetKills(value);
    }

    // called from "_experience" syncvar
    void OnExperienceChanged(int value)
    {
        _experience = value;// update client value
        if (isLocalPlayer)
            PlayerCanvas.canvasInstance.SetExperience(value);
    }


    public override void EnemyKilled(DismissibleObjectController enemy)
    {
        if (!isLocalPlayer)
            return;

        _enemiesKilled++;
        _experience += enemy.ExperienceData.Experience;

        if (_enemiesKilled == _killsToWin)
            SceneManager.LoadScene("Game Win");
    }

    public override void OnKilled()
    {
        if (!isLocalPlayer)
            PlayerCanvas.canvasInstance.DisplayGameStatus("You Died!");

        DisablePlayer();
    }

    public override Transform GetSpawnLocation()
    {
        Transform spawn = NetworkManager.singleton.GetStartPosition();
        return spawn;
    }

    public override void OnRespawned()
    {
        EnablePlayer();
    }
}
