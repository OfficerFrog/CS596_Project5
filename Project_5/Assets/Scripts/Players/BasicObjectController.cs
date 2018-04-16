
using UnityEngine;
using UnityEngine.Networking;

public abstract class BasicObjectController : NetworkBehaviour
{
    [Tooltip("Should this object respawn or be destroyed")]
    [SerializeField]
    private bool DoesRespawn;

    /// <summary>
    /// actions to do once the object has no more health, respawn or be destroyed
    /// </summary>
    public void OnZeroHealth()
    {
        if (DoesRespawn)
        {

        }
        else
            Destroy(gameObject);
    }
}