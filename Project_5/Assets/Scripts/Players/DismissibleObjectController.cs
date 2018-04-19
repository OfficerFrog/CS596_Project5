
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

    public abstract ObjectWithExperience ExperienceData { get; }

    /// <summary>
    /// actions to do once the object has no more health, respawn or be destroyed
    /// </summary>
    public void OnZeroHealth()
    {
        if (_doesRespawn)
            Respawn();
        else
            Destroy(gameObject);
    }

    public abstract void Respawn();
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