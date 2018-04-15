using UnityEngine;

/// <summary>
/// force this to always be facing the main camera
/// </summary>
public class Billboard : MonoBehaviour
{

    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
