using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Camera cam;
    private CharacterController pawn;

    public float walkSpeed = 4;
    void Start()
    {
        cam = Camera.main;
        pawn = GetComponent<CharacterController>();
    }
    void Update()
    {
        float h = Input.GetAxis("Horizontal"); // Side to side
        float v = Input.GetAxis("Vertical"); // Front to back

        //float yawOfInput = Mathf.Atan2(v, h); // In radians
        //float yawOfCam = cam.transform.eulerAngles.y; // In Degrees

        bool isTryingToMove = (h != 0 || v != 0);
        if (isTryingToMove)
        {
            // Turn to face the direction based on input
            float camYaw = cam.transform.eulerAngles.y;
            transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(0, camYaw, 0), .01f);
        }

        // Use H and V to figure out how far the player moves per frame
        Vector3 inputDirection = transform.forward * v + transform.right * h;

        pawn.SimpleMove(inputDirection * walkSpeed);
    }
}
