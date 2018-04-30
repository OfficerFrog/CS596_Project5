
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// controller for objects that can be respawned or deleted from the game
/// </summary>
public abstract class DismissibleObjectController : NetworkBehaviour
{
    [Tooltip("Should this object respawn or be destroyed")]
    [SerializeField]
    private bool _doesRespawn;

    [SerializeField]
    protected float RespawnTime = 5f;

    public abstract ObjectWithExperience ExperienceData { get; }

    /// <summary>
    /// actions to do once the object has no more health, respawn or be destroyed
    /// </summary>
    public void OnZeroHealth()
    {
        OnKilled();

        if (_doesRespawn)
            Respawn(RespawnTime);
        else
            Destroy(gameObject);
    }

    public virtual void OnKilled() { }

    public abstract void Respawn(float inTime);
}

/// <summary>
/// Data (type, experience) of object that can be killed or similar
/// </summary>
public class ObjectWithExperience
{
    public ObjectWithExperienceType Type { get; set; }
    public int Experience { get; set; }
}

/// <summary>
/// objects that give experience
/// </summary>
public enum ObjectWithExperienceType
{
    /// <summary>
    /// Enemy player
    /// </summary>
    Player
    // Wall/Structure
}