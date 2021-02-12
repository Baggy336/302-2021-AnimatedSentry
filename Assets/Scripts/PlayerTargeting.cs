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

    public bool wantsToTarget = false;

    public float visionDis = 10;

    private List<TargetableThing> potentialTargets = new List<TargetableThing>();

    float cooldownScan = 0;
    float cooldownPick = 0;

    void Start()
    {
        
    }
    void Update()
    {
        wantsToTarget = Input.GetButton("Fire2"); // Get the input for the right mouse click

        cooldownScan -= Time.deltaTime; // Count down every tick
        if (cooldownScan <= 0) ScanForTargets(); // Do this when the cooldown is over

        cooldownPick -= Time.deltaTime;
        if (cooldownPick <= 0) PickATarget();
    }

    private void ScanForTargets()
    {
        cooldownScan = 1; // Do the next scan in 2 seconds

        potentialTargets.Clear(); // Empty the list

        // Refill the list
        TargetableThing[] things = GameObject.FindObjectsOfType<TargetableThing>();

        foreach (TargetableThing thing in things)
        {
            // Check how far away thing is
            Vector3 disToThing = thing.transform.position - transform.position;

            // Check which direction thing is in
            if (disToThing.sqrMagnitude < visionDis * visionDis) // Does pythagorean theorum without square root
            {
                if (Vector3.Angle(transform.forward, disToThing) < 45)
                {
                    potentialTargets.Add(thing);
                }
            } 
        }
    }
    void PickATarget()
    {
        cooldownPick = .25f;

        if (target) return; // We already have a target

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
