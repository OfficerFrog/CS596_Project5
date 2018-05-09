using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : BasicPlayerController
{
    [HideInInspector]
    [SyncVar(hook = "OnNameChanged")]
    public string PlayerName;

    [HideInInspector]
    [SyncVar(hook = "OnColorChanged")]
    public Color PlayerColor;

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

    /// <summary>
    /// max amount of bullets player can hold
    /// </summary>
    [SerializeField]
    private int _bulletCapacity;

    [SerializeField]
    private int _killsToWin = 3;

    [SerializeField]
    private Text _playerCanvasNameText;


    [HideInInspector]
    protected override float EllapsedTimeBetweenUpdates { get; set; }

    /// <summary>
    /// for debugging purposes
    /// </summary>
    public Vector3 Destination;


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

        Destination = this.transform.position;

        // need to update here, since base class Update() isnt called
        EllapsedTimeBetweenUpdates += Time.deltaTime;

        UpdateMovement();

        if (Input.GetKeyDown(KeyCode.Return) && CanFire())
            WeaponShot();
        else if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("Menu");
    }

    private void EnablePlayer()
    {
        //OnToggleShared.Invoke(true);
        //OnToggleLocal.Invoke(this.isLocalPlayer);
        //OnToggleRemote.Invoke(!this.isLocalPlayer);

        if (isLocalPlayer)
            PlayerCanvas.canvasInstance.Initialize();
    }

    /// <summary>
    /// remove events when player dies, logs off, etc
    /// </summary>
    private void DisablePlayer()
    {
        //OnToggleShared.Invoke(false);
        //if (isLocalPlayer)
        //    OnToggleLocal.Invoke(false);
        //else
        //    OnToggleRemote.Invoke(false);
    }

    private void UpdateMovement()
    {
        float horizontalMovement = Input.GetAxis("Horizontal") * Time.deltaTime;
        float verticalMovement = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        // hasn't moved, so return
        if (horizontalMovement == 0.0f && verticalMovement == 0.0f)
            return;
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

        var pickupItem = collision.gameObject.GetComponent<PickupItemBase>();
        if (pickupItem != null)
        {
            bool wasItemPickedUp;
            switch (pickupItem.Type)
            {
                case PickupType.Ammo:
                    wasItemPickedUp = AddAmmoToPlayer(pickupItem);
                    break;

                case PickupType.Health:
                    wasItemPickedUp = AddHealthToPlayer(pickupItem);
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (wasItemPickedUp)
            {
                // let item respawner know it was picked up so it can start the timer to respawn
                pickupItem.ItemPickedUp();
                // remove picked up item
                Destroy(collision.gameObject);
            }
        }
    }

    /// <summary>
    /// returns True if ammo was added to stock
    /// </summary>
    private bool AddAmmoToPlayer(IPickupItem ammoItem)
    {
        if (_currentBulletAmmo >= _bulletCapacity)
            return false;

        _currentBulletAmmo += ammoItem.Amount;
        if (_currentBulletAmmo > _bulletCapacity)
            _currentBulletAmmo = _bulletCapacity;
        // add experience for picking up ammo
        _experience += ammoItem.Experience;
        return true;
    }

    /// <summary>
    /// returns True if health was added 
    /// </summary>
    private bool AddHealthToPlayer(IPickupItem healthItem)
    {
        // only gain experience if increased health
        if (this.GetComponent<Health>().TakeHeal(healthItem.Amount))
        {
            _experience += healthItem.Experience;
            return true;
        }
        return false;
    }

    #region SyncVar hooks

    void OnNameChanged(string value)
    {
        PlayerName = value;
        if(!isLocalPlayer)
            _playerCanvasNameText.text = value;
    }

    void OnColorChanged(Color value)
    {
        PlayerColor = value;
        foreach (Renderer rend in this.GetComponentsInChildren<Renderer>())
        {
            rend.material.color = PlayerColor;
        }
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


    #endregion SyncVar hooks

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
        _currentBulletAmmo = _bulletCapacity;
        var myHealth = this.GetComponent<Health>();
        myHealth.CurrentHealth = myHealth.MaxHealth;
        EnablePlayer();
    }
}
