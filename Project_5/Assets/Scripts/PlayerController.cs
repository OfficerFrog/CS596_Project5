
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    void Update()
    {
        if (!isLocalPlayer)
            return;

        float horizontalMovement = Input.GetAxis("Horizontal") * Time.deltaTime;
        float verticalMovement = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        
        // if has both movements, means player is Strafing
        if (horizontalMovement != 0.0f && verticalMovement != 0.0f)
        {
            // horizontal movement equals rotation
            float horizontalRotation = horizontalMovement * 150.0f;
            transform.Rotate(0, horizontalRotation, 0);
            transform.Translate(0, 0, verticalMovement);
        }
        else
        {
            // forward/back or left/right
            float newLeftRightMovement = horizontalMovement * 3.0f;  // want left/right speed to be equal to front/back
            transform.Translate(newLeftRightMovement, 0, verticalMovement);

        }
    }
}
