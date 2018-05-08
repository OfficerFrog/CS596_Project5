using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class EnemyController : BasicPlayerController
{
    [Tooltip("Max distance enemy should engage player")]
    [SerializeField]
    private float _engagementRange = 5f;

    [HideInInspector]
    private NavMeshAgent _navMesh;

    protected override float EllapsedTimeBetweenUpdates { get; set; }

    /// <summary>
    /// for debugging purposes only
    /// </summary>
    public Vector3 Destination;

    /// <summary>
    /// player that enemy is currently shooting/chasing
    /// </summary>
    private GameObject _engagedPlayer;

    protected float EllapsedTimeBetweenMovementUpdates { get; set; }

    private void Start()
    {
        _navMesh = this.GetComponent<NavMeshAgent>();
        _navMesh.updateRotation = true;
    }

    private void Update()
    {
        // need to update here, since base class Update() isnt called
        EllapsedTimeBetweenUpdates += Time.deltaTime;

        _engagedPlayer = GetNearestPlayer();
        if (_engagedPlayer != null)
        {
            if (CanFire())
                CmdFire();
        }
    }

    /// <summary>
    /// reduce the frequency that the enemy is updated, as too often will lead to bad things
    /// </summary>
    void FixedUpdate()
    {
        EllapsedTimeBetweenMovementUpdates += Time.deltaTime;
        if (EllapsedTimeBetweenMovementUpdates >= .25f)
        {
            EllapsedTimeBetweenMovementUpdates = 0;
            MoveEnemy(_engagedPlayer);
        }
    }

    public override ObjectWithExperience ExperienceData
    {
        get { return new ObjectWithExperience { Type = ObjectWithExperienceType.Player, Experience = 500 }; }
    }

    /// <summary>
    /// returns nearest player GameObject, or Null if none
    /// </summary>
    public GameObject GetNearestPlayer()
    {
        List<GameObject> players = GameObject.FindObjectsOfType<PlayerController>()
                                                                            .Select(p => p.gameObject)
                                                                            .ToList();
        if (!players.Any())
            return null;

        Vector3 myPosition = transform.position;
        // order by the distance from enemy to each player, smallest first; pick the smallest that is within the max distance to engage
        var closestPlayer = players.Select(player => new {player, dist = GetDistanceToPlayer(player, myPosition)}) // create new obj so only have to call GetDistanceToPlayer() onces
            // sort smallest to largest
            .OrderBy(playerDist => playerDist.dist) 
            // pick the first that is within distance & has health
            .FirstOrDefault(playerDist => playerDist.dist <= _engagementRange && DoesPlayerHaveHealth(playerDist.player));
            // if none within range, return null
        return closestPlayer == null ? null : closestPlayer.player.gameObject;
    }

    private float GetDistanceToPlayer(GameObject player, Vector3 myPosition)
    {
        Vector3 diff = player.transform.position - myPosition;
        // dont care about Y-axis
        float distance = Mathf.Sqrt(diff.x * diff.x + diff.z * diff.z);
        return distance;
    }

    private bool DoesPlayerHaveHealth(GameObject player)
    {
        var playerHealth = player.GetComponent<Health>();
        if (playerHealth == null || playerHealth.CurrentHealth <= 0)
            return false;
        return true;
    }

    public void MoveEnemy(GameObject player)
    {
        if (player == null)
            PossiblyMoveToMeshLocation(this.transform.position);
        else
            PossiblyMoveToMeshLocation(player.transform.position);
    }

    /// <summary>
    /// get a location on the NavMesh, and move to it
    /// </summary>
    private void PossiblyMoveToMeshLocation(Vector3 destination)
    {
        NavMeshHit navMeshLocation;
        if (NavMesh.SamplePosition(destination, out navMeshLocation, 5f, 1))
        {
            Destination = navMeshLocation.position;
            _navMesh.SetDestination(navMeshLocation.position);
        }
    }

    // TODO:  get enemy spawn location
    public override Transform GetSpawnLocation()
    {
        //TODO change this to only getting enemy specific spawn location
        Transform spawn = NetworkManager.singleton.GetStartPosition();
        return spawn;
    }
}