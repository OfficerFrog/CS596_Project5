

using UnityEngine;
using UnityEngine.AI;

public class EnemyController : BasicPlayerController
{
    Transform playerPos;
    Health playerHealth;
    NavMeshAgent nav;

    private void Awake()
    {
        playerPos = GetNearestPlayer();
    }

    private void Update()
    {
        FollowPlayer(playerPos);
    }

    // TODO: make experiene gained per enemy configurable (e.g. stronger enemies give more experience)
    public override ObjectWithExperience ExperienceData
    {
        get { return new ObjectWithExperience { Type = ObjectWithExperienceType.Player, Experience = 500 }; }
    }

    // get enemy spawn location
    public override Transform GetSpawnLocation()
    {
        throw new System.NotImplementedException();
    }

    public override void OnRespawned()
    {
        throw new System.NotImplementedException();
    }

    public override void EnemyKilled(DismissibleObjectController enemy)
    {
        throw new System.NotImplementedException();
    }

    // returns location of nearest player
    public Transform GetNearestPlayer()
    {
        GameObject[] player;
        GameObject nearest = null;
        player = GameObject.FindGameObjectsWithTag("Player");
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;  
        foreach (GameObject go in player)
        {
            Vector3 diff = go.transform.position - position;
            float currDistance = diff.sqrMagnitude;
            if (currDistance < distance)
            {
                nearest = go;
                distance = currDistance;
            }
        }
        playerHealth = nearest.GetComponent<Health>();
        return nearest.transform;
    }

    public void FollowPlayer(Transform playerTrack)
    {
        if (playerHealth.CurrentHealth < 1){

            nav.SetDestination(playerTrack.position);
        }
        else
        {
            nav.enabled = false;
        }
    }
}