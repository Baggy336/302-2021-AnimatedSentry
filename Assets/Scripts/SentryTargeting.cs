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

    float orbit = 0;

    private void Update()
    {
        if (!targetPlayer)
        {
            orbit += .5f;
            rotationPiece.localRotation = Quaternion.Euler(0, orbit, 0);
        }
    }
}
