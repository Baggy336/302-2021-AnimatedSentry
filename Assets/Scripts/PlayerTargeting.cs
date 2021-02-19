using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    /// <summary>
    /// This is the player's current target.
    /// </summary>
    public Transform target;

    /// <summary>
    /// This variable returns if the player wants to target an object or not.
    /// </summary>
    public bool wantsToTarget = false;

    /// <summary>
    /// This variable is how far away an object can be for the player to see it.
    /// </summary>
    public float visionDis = 10;

    /// <summary>
    /// This variable is the player's vision cone.
    /// </summary>
    public float visionAngle = 45;

    /// <summary>
    /// A list of all potential targets for the player in the scene.
    /// </summary>
    private List<TargetableThing> potentialTargets = new List<TargetableThing>();

    /// <summary>
    /// This variable sets how much time is between each scan.
    /// </summary>
    float cooldownScan = 0;

    /// <summary>
    /// This variable holds how much time the player has to wait in between targets.
    /// </summary>
    float cooldownPick = 0;

    void Start()
    {
        // Lock the mouse cursor to the edges of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        wantsToTarget = Input.GetButton("Fire2"); // Get the input for the right mouse click

        if (!wantsToTarget) target = null; // As soon as the player releases the mouse button, stop targeting.

        cooldownScan -= Time.deltaTime; // Count down every tick
        if (cooldownScan <= 0 || (!target && wantsToTarget)) ScanForTargets(); // Do this when the cooldown is over

        cooldownPick -= Time.deltaTime;
        if (cooldownPick <= 0) PickATarget();

        // If we can't see the target, there is no target.
        if (target && CanSeeThing(target) == false) target = null;
    }

    /// <summary>
    /// Check to see if the player can see a target or not.
    /// This function returns true or false.
    /// </summary>
    /// <param name="thing"></param>
    /// <returns></returns>
    private bool CanSeeThing(Transform thing) // Returns true or false if we can/can't see a thing
    {
        if (!thing) return false;

        Vector3 vectorToThing = thing.position - transform.position;

        // Check distance
        if (vectorToThing.sqrMagnitude > visionDis * visionDis) return false; // Too far away to see.

        // Check direction
        if (Vector3.Angle(transform.forward, vectorToThing) > visionAngle) return false; // Out of vision cone.

        // TODO: check occlusion

        return true;
    }

    /// <summary>
    /// This function scans the scene for potential targets every 1 second.
    /// </summary>
    private void ScanForTargets()
    {
        cooldownScan = 1; // Do the next scan in 1 seconds

        potentialTargets.Clear(); // Empty the list

        // Refill the list
        TargetableThing[] things = GameObject.FindObjectsOfType<TargetableThing>();

        foreach (TargetableThing thing in things)
        {
            // If we can see it
            // Add target to potentialTargets
            if (CanSeeThing(thing.transform))
            {
               potentialTargets.Add(thing);
            }       
        }
    }

    /// <summary>
    /// This function returns the target that is closest to the player.
    /// </summary>
    void PickATarget()
    {
        cooldownPick = .25f;

        //if (target) return; // We already have a target
        target = null;

        float closestDisSoFar = 0;

        // Loop throught the targets, and find the closest TargetableThing
        foreach (TargetableThing pt in potentialTargets)
        {
            float dd = (pt.transform.position - transform.position).sqrMagnitude;

            if (dd < closestDisSoFar || target == null)
            {
                target = pt.transform;
                closestDisSoFar = dd;
            }
        }
    }
}
