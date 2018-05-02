

using UnityEngine;

/// <summary>
/// make UI elements face the player's camera
/// </summary>
public class LookAtCamera : MonoBehaviour
{
    /// <summary>
    /// camera's transform
    /// </summary>
    [HideInInspector]
    private Transform _mainCamera;

    void Start()
    {
        //Set the Main Camera as the target
        _mainCamera = Camera.main == null ? null : Camera.main.transform;
    }

    /// <summary>
    /// Update after all other updates have run
    /// </summary>
    void LateUpdate()
    {
        if (_mainCamera == null)
            return;

        //Apply the rotation needed to look at the camera. Note, since pointing a UI text element
        //at the camera makes it appear backwards, we are actually pointing this object
        //directly *away* from the camera.
        transform.rotation = Quaternion.LookRotation(transform.position - _mainCamera.position);
    }
}