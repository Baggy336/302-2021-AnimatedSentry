using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Hold reference to the camera in the scene.
    /// </summary>
    private Camera cam;

    /// <summary>
    /// Hold reference to the character controller.
    /// </summary>
    private CharacterController pawn;

    /// <summary>
    /// Hold reference to the leg bones.
    /// </summary>
    public Transform leg1;
    public Transform leg2;

    private Vector3 inputDirection = new Vector3();

    /// <summary>
    /// How fast the player is currently moving, vertically in m/s.
    /// </summary>
    private float verticalVel = 0;

    /// <summary>
    /// This variable holds how fast the player can move.
    /// </summary>
    public float walkSpeed = 4;
    void Start()
    {
        cam = Camera.main;
        pawn = GetComponent<CharacterController>();
    }
    void Update()
    {
        MovePlayer();
        WiggleLegs();
    }

    /// <summary>
    /// This function gets player input and moves the player from that input.
    /// </summary>
    private void MovePlayer()
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
        inputDirection = transform.forward * v + transform.right * h;

        if (inputDirection.sqrMagnitude > 1) inputDirection.Normalize(); // Make the value 1

        // Apply gravity
        verticalVel += 10 * Time.deltaTime;

        // Pass all movement information to the CharacterController
        pawn.Move(inputDirection * walkSpeed * Time.deltaTime + verticalVel * Vector3.down * Time.deltaTime);

        if (pawn.isGrounded) // If the player is touching the ground, set verticalVel to 0.
        {
            verticalVel = 0;
        }
    }

    /// <summary>
    /// This function animates the player's legs.
    /// </summary>
    private void WiggleLegs()
    {
        float degrees = 45;

        float speed = 10;


        // Get the vector perpendicular to the input direction.
        Vector3 inputDirectionLocal = transform.InverseTransformDirection(inputDirection);
        Vector3 axis = Vector3.Cross(inputDirectionLocal, Vector3.up);

        // Check the alignment of inputDirLocal against forward vector
        float alignment = Vector3.Dot(inputDirectionLocal, Vector3.forward);
        //if (alignment < 0) alignment *= -1; // flips the nmumber
        alignment = Mathf.Abs(alignment); // flips negative numbers

        /*
        1 = lots of movement
        0 = no movement
        -1 = lots of movement
        */
        // Remap alignment from .25 to 1, and multiply it by degrees.
        degrees *= AnimMath.Lerp(0.25f, 1, alignment);

        // Set the local rotation of the legs as the player moves.
        float wave = Mathf.Sin(Time.time * speed) * degrees;

        leg1.localRotation = AnimMath.Slide(leg1.localRotation, Quaternion.AngleAxis(wave, axis), .001f);
        leg2.localRotation = AnimMath.Slide(leg2.localRotation, Quaternion.AngleAxis(-wave, axis), .001f);
    }
}
