using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAt : MonoBehaviour
{
    /// <summary>
    /// This declares the player targeting class
    /// </summary>
    private PlayerTargeting playerTargeting;

    /// <summary>
    /// This variable holds the player's bones starting rotation.
    /// </summary>
    private Quaternion startingRot;

    /// <summary>
    /// These three booleans are used to lock the player's looking axis.
    /// </summary>
    public bool lockRotationX;
    public bool lockRotationY;
    public bool lockRotationZ;

    void Start()
    {
        startingRot = transform.localRotation;
        playerTargeting = GetComponentInParent<PlayerTargeting>();
        print(playerTargeting);
    }
    void Update()
    {
        TurnTowardsTarget();
    }

    /// <summary>
    /// This function calculates and turns the player towards the current target.
    /// </summary>
    private void TurnTowardsTarget()
    {
        if (playerTargeting && playerTargeting.target && playerTargeting.wantsToTarget)
        {
            // Get the distance from the player's current location to the target
            Vector3 disToTarget = playerTargeting.target.position - transform.position;

            // Store the look rotation in a quaternion
            Quaternion targetRot = Quaternion.LookRotation(disToTarget, Vector3.up);

            // Get local Euler angles before rotation
            Vector3 euler1 = transform.localEulerAngles;
            Quaternion prevRot = transform.rotation;
            transform.rotation = targetRot; // Set rotation
            Vector3 euler2 = transform.localEulerAngles; // Get local angles after rotation

            if (lockRotationX) euler2.x = euler1.x; // Revert X to previous value
            if (lockRotationY) euler2.y = euler1.y; // Revert Y to previous value
            if (lockRotationZ) euler2.z = euler1.z; // Revert Z to previous value

            transform.rotation = prevRot; // Revert rotation

            // Use the quaternion with our slide function to ease the rotation towards the target
            transform.localRotation = AnimMath.Slide(transform.localRotation, Quaternion.Euler(euler2), .01f);
        }
        else
        {
            print("Hello");
            // Figure out bone rotation when there is no target
            transform.localRotation = AnimMath.Slide(transform.localRotation, startingRot, .05f);
        }
    }
}
