
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Controller inherited by all players and enemies
/// </summary>
public abstract class BasicPlayerController : DismissibleObjectController
{
    [SerializeField]
    protected Toggle.ToggleEvent OnToggleShared;
    [SerializeField]
    protected Toggle.ToggleEvent OnToggleLocal;
    [SerializeField]
    protected Toggle.ToggleEvent OnToggleRemote;

    [Tooltip("Minimum time between shots")]
    [SerializeField]
    protected float _shotCooldown = .3f;

    protected abstract float EllapsedTimeBetweenUpdates { get; set; }

    /// <summary>
    /// used to instantiate bullet
    /// </summary>
    [SerializeField]
    private GameObject _bulletPrefab;

    /// <summary>
    /// used to instantiate projectile's spawn point
    /// </summary>
    [SerializeField]
    private Transform _projectileSpawn;

    /// <summary>
    /// pseudo-unique id to differentiate players
    /// </summary>
    public string PlayerId { get; private set; }

    

    // Really TODO: not getting initialized correctly
    // TODO: keep track of players killed (via PlayerId)
    /// <summary>
    /// count of objects (enemies, walls, etc) destroyed/killed
    /// </summary>
    /// <remarks>
    /// Public to be accessible by the GameManager and/or end game
    /// </remarks>
    //public Dictionary<ObjectWithExperienceType, int> ObjectsDestroyedCounts;


    void Start()
    {
        if (!isLocalPlayer)
            return;

        PlayerId = new Guid().ToString();

        //ObjectsDestroyedCounts = Enum.GetValues(typeof(ObjectWithExperienceType))
        //    .OfType<ObjectWithExperienceType>()
        //    .ToDictionary(e => e, e => 0);
    }

    protected virtual bool CanFire()
    {
        if (EllapsedTimeBetweenUpdates >= _shotCooldown)
        {
            EllapsedTimeBetweenUpdates = 0;
            return true;
        }
        return false;
    }

    // [Command] code is called on the Client but ran on the Server
    [Command]
    protected void CmdFire()
    {
        // create an instane of the projectile we want to fire
        GameObject projectile = Instantiate(
            _bulletPrefab,
            _projectileSpawn.position,
            _projectileSpawn.rotation);

        // give the projectile some velocity
        float initialVelocity = this.GetComponent<Rigidbody>().velocity.magnitude;
        projectile.GetComponent<Bullet>().SetVelocity(initialVelocity, projectile.transform.forward);
        // associate projectile with this player
        projectile.GetComponent<Projectile>().FiringPlayer = this;

        // spawn the projectile on all of the connected Clients
        NetworkServer.Spawn(projectile);

        // destroy it after some arbitrary amount of time; 10 seconds seems good enough
        Destroy(projectile, 10.0f);
    }

    /// <summary>
    /// object or enemey was destroyed/killed
    /// </summary>
    public void ObjectDestroyed(DismissibleObjectController destroyedObject)
    {
        if (!isLocalPlayer)
            return;

        //ObjectsDestroyedCounts[destroyedObject.ExperienceData.Type] += 1;
        if (destroyedObject.ExperienceData.Type == ObjectWithExperienceType.Player)
            EnemyKilled(destroyedObject);
    }

    /// <summary>
    /// Do anything that needs to be done after killing an enemy
    /// </summary>
    public virtual void EnemyKilled(DismissibleObjectController enemy) { }

    /// <summary>
    /// respawn player (in set time and place)
    /// </summary>
    public override void Respawn(float inTime)
    {
        // spawn player in different position
        if (isLocalPlayer)
        {
            // get the spawn location of the player type
            Transform spawn = GetSpawnLocation();
            transform.position = spawn.position;
            transform.rotation = spawn.rotation;
        }

        OnRespawned();
    }

    /// <summary>
    /// Get distinct spawn location, based on if Enemy or Player, etc
    /// </summary>
    public abstract Transform GetSpawnLocation();

    /// <summary>
    /// Do anything that needs to be done after respawning
    /// </summary>
    public virtual void OnRespawned() { }
}