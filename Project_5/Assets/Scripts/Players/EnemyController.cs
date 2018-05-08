﻿using System.Collections.Generic;
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

    private void Start()
    {
        //if (!isLocalPlayer)
        //    return;
        _navMesh = this.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //if (!isLocalPlayer)
        //    return;

        // need to update here, since base class Update() isnt called
        EllapsedTimeBetweenUpdates += Time.deltaTime;

        GameObject player = GetNearestPlayer();
        if (player == null)
        {
            // continue cruising along (OR go back to main spot)
        }
        else
        {
            FollowPlayer(player);
            if (CanFire())
                CmdFire();
        }
    }

    public override ObjectWithExperience ExperienceData
    {
        get { return new ObjectWithExperience { Type = ObjectWithExperienceType.Player, Experience = 500 }; }
    }

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
        if(closestPlayer == null)
            return null;
        return closestPlayer.player.gameObject;
        //return closestPlayer == null ? null : closestPlayer.player.gameObject;
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

    public void FollowPlayer(GameObject player)
    {
        if (player == null)
            _navMesh.isStopped = true;
        else
            _navMesh.SetDestination(player.transform.position);
    }

    // TODO:  get enemy spawn location
    public override Transform GetSpawnLocation()
    {
        //TODO change this to only getting enemy specific spawn location
        Transform spawn = NetworkManager.singleton.GetStartPosition();
        return spawn;
    }
}