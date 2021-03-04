using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    CameraOrbit camOrbit;

    /// <summary>
    /// This is the player's current target.
    /// </summary>
    public Transform target;

    public Transform armLeft;
    public Transform armRight;

    private Vector3 startPosArmLeft;
    private Vector3 startPosArmRight;

    public ParticleSystem prefabMuzzleFlash;
    public Projectile prefabProjectile;

    public Transform handRight;
    public Transform handLeft;

    /// <summary>
    /// This variable returns if the player wants to target an object or not.
    /// </summary>
    public bool wantsToTarget = false;

    public bool wantsToAttack = false;

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

    float cooldownShoot = 0;

    public float RPS = 4;

    void Start()
    {
        // Lock the mouse cursor to the edges of the screen
        Cursor.lockState = CursorLockMode.Locked;

        camOrbit = Camera.main.GetComponentInParent<CameraOrbit>();

        startPosArmLeft = armLeft.localPosition;
        startPosArmRight = armRight.localPosition;
    }
    void Update()
    {
        wantsToTarget = Input.GetButton("Fire2"); // Get the input for the right mouse click
        wantsToAttack = Input.GetButton("Fire1"); // Get the input for the left mouse click


        if (!wantsToTarget) target = null; // As soon as the player releases the mouse button, stop targeting.

        cooldownScan -= Time.deltaTime; // Count down every tick
        if (cooldownScan <= 0 || (!target && wantsToTarget)) ScanForTargets(); // Do this when the cooldown is over

        cooldownPick -= Time.deltaTime;
        if (cooldownPick <= 0) PickATarget();

        if (cooldownShoot > 0) cooldownShoot -= Time.deltaTime;

        // If we can't see the target, there is no target.
        if (target && CanSeeThing(target) == false) target = null;

        SlideArmsHome();

        DoAttack();
    }

    private void DoAttack()
    {
        Projectile projectile;

        if (cooldownShoot > 0) return;
        if (!wantsToTarget) return;
        if (!wantsToAttack) return;
        if (target == null) return;
        if (!CanSeeThing(target)) return;

        cooldownShoot = 1 / RPS;
        // Attack
        camOrbit.Shake(.5f);

        // Where to spawn the particle system
        if (handRight) Instantiate(prefabMuzzleFlash, handRight.position, handRight.rotation);
        if (handLeft) Instantiate(prefabMuzzleFlash, handLeft.position, handLeft.rotation);

        if (handRight)
        {
            projectile = Instantiate(prefabProjectile, handRight.position, handRight.rotation);
            projectile.target = target;
        }
        if (handLeft)
        {
            projectile = Instantiate(prefabProjectile, handLeft.position, handLeft.rotation);
            projectile.target = target;
        }

        // Trigger arm anim
        // Rotate the arms up
        armLeft.localEulerAngles += new Vector3(-20, 0, 0);
        armRight.localEulerAngles += new Vector3(-20, 0, 0);

        // Move the arms backwards
        armLeft.position += -armLeft.forward * .1f;
        armRight.position += -armRight.forward * .1f;
    }

    private void SlideArmsHome()
    {
        armLeft.localPosition = AnimMath.Slide(armLeft.localPosition, startPosArmLeft, .001f);
        armRight.localPosition = AnimMath.Slide(armRight.localPosition, startPosArmRight, .001f);

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
