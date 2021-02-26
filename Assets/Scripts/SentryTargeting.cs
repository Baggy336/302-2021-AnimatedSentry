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

    /// <summary>
    /// How far away the sentry can see
    /// </summary>
    float visDis = 10;

    /// <summary>
    /// The angle in which the sentry can see.
    /// </summary>
    float visCone = 50;

    float cooldownScan = 0;

    float cooldownPick = 0;

    private List<TargetPlayer> players = new List<TargetPlayer>();

    private void Update()
    {
        if (!targetPlayer)
        {
            orbit += .5f;
            rotationPiece.localRotation = Quaternion.Euler(0, orbit, 0);
        }
    }
    /// <summary>
    /// This function returns true/false based on if the sentry can see the player.
    /// </summary>
    /// <returns></returns>
    private bool CanSeePlayer(Transform player)
    {
        if (!player) return false; // If there is no player.

        // Get the current direction to the player.
        Vector3 vectorToPlayer = player.position - transform.position;

        // Check distance
        if (vectorToPlayer.sqrMagnitude > visDis * visDis) return false; // Too far away

        // Check the vision cone
        if (Vector3.Angle(transform.forward, vectorToPlayer) > visCone) return false; //The player is outside of the vision cone.

        return true;
    }

    /// <summary>
    /// This function scans for the player.
    /// </summary>
    private void LookForPlayer()
    {
        cooldownScan = 1; // Do next scan in 1 second

        players.Clear(); // empty the list.

        TargetPlayer[] things = GameObject.FindObjectsOfType<TargetPlayer>();

        foreach (TargetPlayer player in things)
        {
            if (CanSeePlayer(player.transform))
            {
                players.Add(player);
            }
        }
    }
}
