using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretTargeting : MonoBehaviour
{
    public Transform rotationPiece;

    public Transform targetPlayer;

    public Transform targetSentry;

    public Transform nozzleLeft;
    public Transform nozzleRight;


    public Transform barrelLeft;
    public Transform barrelRight;

    public ParticleSystem prefabMuzzleFlash;
    public Projectile prefabProjectile;

    //private Projectile projectile;

    private Vector3 vToPlayer;

    private Vector3 vToSentry;

    private Vector3 startPosbarrelLeft;
    private Vector3 startPosbarrelRight;

    private Quaternion startRotbarrelLeft;
    private Quaternion startRotbarrelRight;

    float visDis = 10;

    float visCone = 360;

    float sentryRange = 20;

    float cooldownShoot = 0;

    float roundsPerSecond = .5f;

    private bool isTargetingPlayer = false;

    private void Start()
    {
        startPosbarrelLeft = barrelLeft.localPosition;
        startPosbarrelRight = barrelRight.localPosition;

        startRotbarrelLeft = barrelLeft.localRotation;
        startRotbarrelRight = barrelRight.localRotation;
    }

    void Update()
    {
        TargetPlayer();
        ShootAtPlayer();
        SlideBarrelsHome();
        if (cooldownShoot > 0) cooldownShoot -= Time.deltaTime;
    }

    private void SlideBarrelsHome()
    {
        barrelLeft.localPosition = AnimMath.Slide(barrelLeft.localPosition, startPosbarrelLeft, .001f);
        barrelRight.localPosition = AnimMath.Slide(barrelRight.localPosition, startPosbarrelRight, .001f);

        barrelLeft.localRotation = AnimMath.Slide(barrelLeft.localRotation, startRotbarrelLeft, .001f);
        barrelRight.localRotation = AnimMath.Slide(barrelRight.localRotation, startRotbarrelRight, .001f);
    }

    private void ShootAtPlayer()
    {
        Projectile projectile;

        if (cooldownShoot > 0) return;
        if (!isTargetingPlayer) return;
        if (!targetPlayer) return;
        if (!targetSentry) return;

        cooldownShoot = 1 / roundsPerSecond;

        // Where to spawn the particle system
        if (nozzleRight) Instantiate(prefabMuzzleFlash, nozzleRight.position, nozzleRight.rotation);
        if (nozzleLeft) Instantiate(prefabMuzzleFlash, nozzleLeft.position, nozzleLeft.rotation);

        // Where to spawn the projectile
        if (nozzleRight)
        {
            projectile = Instantiate(prefabProjectile, nozzleRight.position, nozzleRight.rotation);
            projectile.target = targetPlayer;
        }
        if (nozzleLeft)
        {
            projectile = Instantiate(prefabProjectile, nozzleLeft.position, nozzleLeft.rotation);
            projectile.target = targetPlayer;
        }



        // Trigger arm anim
        // Rotate the arms up
        barrelLeft.localEulerAngles += new Vector3(-20, 0, 0);
        barrelRight.localEulerAngles += new Vector3(-20, 0, 0);

        // Move the arms backwards
        barrelLeft.position += -nozzleLeft.forward * .1f;
        barrelRight.position += -nozzleRight.forward * .1f;
    }

    private void TargetPlayer()
    {
        if (!targetPlayer) return;
        if (!targetSentry) return;

        vToPlayer = targetPlayer.transform.position - transform.position;
        vToSentry = targetSentry.transform.position - transform.position;

        if (vToSentry.sqrMagnitude > sentryRange * sentryRange) isTargetingPlayer = false;
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
    }
}
