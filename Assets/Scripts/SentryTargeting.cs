using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryTargeting : MonoBehaviour
{
    /// <summary>
    /// This is what is being rotated around.
    /// </summary>
    public Transform rotationPiece;

    /// <summary>
    /// This is the player, that the sentry wants to target.
    /// </summary>
    public Transform targetPlayer;

    public PlayerMovement pm;

    private Vector3 vToPlayer;

    float orbit = 0;

    /// <summary>
    /// How far away the sentry can see
    /// </summary>
    float visDis = 10;

    /// <summary>
    /// The angle in which the sentry can see.
    /// </summary>
    float visCone = 360;

    public bool isTargetingPlayer = false;

    private void Update()
    {
        TargetThePlayer();

    }

    private void TargetThePlayer()
    {
        vToPlayer = targetPlayer.transform.position - transform.position;

        if (vToPlayer.sqrMagnitude > visDis * visDis) isTargetingPlayer = false;
        if (Vector3.Angle(transform.forward, vToPlayer) > visCone) isTargetingPlayer = false;

        if (vToPlayer.sqrMagnitude < visDis * visDis && Vector3.Angle(transform.forward, vToPlayer) < visCone) isTargetingPlayer = true;

        if (isTargetingPlayer)
        {
            // Store the look rotation in a quaternion
            Quaternion targetRot = Quaternion.LookRotation(vToPlayer, Vector3.up);

            // Get local Euler angles before rotation
            Vector3 euler1 = rotationPiece.transform.localEulerAngles;
            Quaternion prevRot = rotationPiece.transform.rotation;
            rotationPiece.transform.rotation = targetRot; // Set rotation
            Vector3 euler2 = rotationPiece.transform.localEulerAngles; // Get local angles after rotation

            rotationPiece.transform.rotation = prevRot; // Revert rotation

            // Use the quaternion with our slide function to ease the rotation towards the target
            rotationPiece.transform.localRotation = AnimMath.Slide(rotationPiece.transform.localRotation, Quaternion.Euler(euler2), .01f);
        }

        else
        {
            orbit += .5f;
            rotationPiece.localRotation = Quaternion.Euler(0, orbit, 0);
        }
    }
}
