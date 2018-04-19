

public class EnemyController : BasicPlayerController
{
    // TODO: make experiene gained per enemy configurable (e.g. stronger enemies give more experience)
    public override ObjectWithExperience ExperienceData
    {
        get { return new ObjectWithExperience {Type = ObjectWithExperienceType.Player, Experience = 1000}; }
    }

    /// <summary>
    /// respawn player (in set time and place)
    /// </summary>
    public override void Respawn()
    {

    }

    public override void EnemyKilled(DismissibleObjectController enemy)
    {
        throw new System.NotImplementedException();
    }
}