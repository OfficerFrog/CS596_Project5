using UnityEngine;

public class Projectile : MonoBehaviour
{
    // remove projectile from game when it collides with anything
    void OnCollisionEnter()
    {
        Destroy(gameObject);
    }
}