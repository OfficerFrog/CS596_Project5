
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Linq;

/// <summary>
/// Controller inherited by all players and enemies
/// </summary>
public abstract class BasicPlayerController : DismissibleObjectController
{
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

    /// <summary>
    /// the current experience the player has gained so far
    /// </summary>
    /// <remarks>
    /// Public to be accessible by the GameManager and/or end game
    /// </remarks>
    [SyncVar]
    public int CurrentExperience;

    // TODO: keep track of players killed (via PlayerId)
    /// <summary>
    /// count of objects (enemies, walls, etc) destroyed/killed
    /// </summary>
    /// <remarks>
    /// Public to be accessible by the GameManager and/or end game
    /// </remarks>
    public Dictionary<ObjectWithExperienceType, int> ObjectsDestroyedCounts;


    void Start()
    {
        PlayerId = new Guid().ToString();
        // defaul all to 0
        ObjectsDestroyedCounts = Enum.GetValues(typeof(ObjectWithExperienceType))
            .OfType<ObjectWithExperienceType>()
            .ToDictionary(e => e, e => 0);
       
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

        // destroy it after some arbitrary amount of time; 2 seconds seems good enough
        Destroy(projectile, 2.0f);
    }

    /// <summary>
    /// object or enemey was destroyed/killed
    /// </summary>
    public void ObjectDestroyed(DismissibleObjectController destroyedObject)
    {
        CurrentExperience += destroyedObject.ExperienceData.Experience;
        ObjectsDestroyedCounts[destroyedObject.ExperienceData.Type] += 1;
        if (destroyedObject.ExperienceData.Type == ObjectWithExperienceType.Player)
            EnemyKilled(destroyedObject);
    }

    public abstract void EnemyKilled(DismissibleObjectController enemy);
}