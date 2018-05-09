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

    protected override float EllapsedTimeBetweenUpdates { get; set; }

    [Tooltip("Perimeter in which to find a random point")]
    [SerializeField]
    private float _randomLocationDistanceThreshold = 50;

    [HideInInspector]
    private NavMeshAgent _navMesh;

    [HideInInspector]
    private bool _isMovingRandomly;
       
    /// <summary>
    /// point on navMesh that the enemy is currently moving towards
    /// </summary>
    [HideInInspector]
    private Vector3 _destination;

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
                _projectile.CmdFire(_projectileSpawn.position, _projectileSpawn.forward);
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

    private float GetDistanceToPlayer(GameObject player, Vector3 position)
    {
        Vector3 diff = player.transform.position - position;
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
        {
            // find new random point to move to, if not moving randomly or within range of random point
            // distance must be same distance used for NavMesh.SampleDistance()
            if (!_isMovingRandomly || (_isMovingRandomly && GetDistanceToPlayer(this.gameObject, _destination) < 5f))
            {
                // keep trying to find path on NavMesh
                bool foundPath = false;
                while (!foundPath)
                {
                    Vector3 newDestination = FindRandomLocation(this.transform.position, _randomLocationDistanceThreshold);
                    foundPath = PossiblyMoveToMeshLocation(newDestination, NavMesh.AllAreas);
                }
                
                _isMovingRandomly = true;
            }
        }
        else
        {
            // for some reason our player's transform has a negaitve y-axis sometimes
            var newDestination = new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z);
            PossiblyMoveToMeshLocation(newDestination, NavMesh.AllAreas);
            _isMovingRandomly = false;
        }
    }

    private void ShowDebugPosition(Vector3 point)
    {
        Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
    }
    /// <summary>
    /// get a location on the NavMesh, and move to it
    /// </summary>
    private bool PossiblyMoveToMeshLocation(Vector3 destination, int layermask)
    {
        NavMeshHit navMeshLocation;
        if (NavMesh.SamplePosition(destination, out navMeshLocation, 5f, layermask))
        {
            _destination = navMeshLocation.position; // update for debugging
            _navMesh.destination = navMeshLocation.position;
            ShowDebugPosition(navMeshLocation.position);
            return true;
        }
        return false;
    }
    /// <summary>
    /// find random location X distance from my current location
    /// </summary>
    public static Vector3 FindRandomLocation(Vector3 myLocation, float distance)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;

        randomDirection += myLocation;

        return randomDirection;
    }


    // TODO:  get enemy spawn location
    public override Transform GetSpawnLocation()
    {
        //TODO change this to only getting enemy specific spawn location
        Transform spawn = NetworkManager.singleton.GetStartPosition();
        return spawn;
    }

    public override void OnRespawned()
    {
        _isMovingRandomly = false;
        _destination = this.transform.position;
        _engagedPlayer = null;
        EllapsedTimeBetweenMovementUpdates = 0;

        var myHealth = this.GetComponent<Health>();
        myHealth.CurrentHealth = myHealth.MaxHealth;
    }
}