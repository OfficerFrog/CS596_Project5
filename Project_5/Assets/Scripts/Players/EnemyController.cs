

using UnityEngine;

public class EnemyController : BasicPlayerController
{
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
}