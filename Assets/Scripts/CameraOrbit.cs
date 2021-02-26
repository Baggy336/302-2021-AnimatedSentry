using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    /// <summary>
    /// This variable holds reference to the camera.
    /// </summary>
    private Camera cam;

    /// <summary>
    /// This variable holds reference to the PlayerTargeting script
    /// </summary>
    private PlayerTargeting targetScript;

    /// <summary>
    /// Hold reference to the PlayerMovement script.
    /// </summary>
    public PlayerMovement moveScript;

    /// <summary>
    /// This variable holds the camera's yaw.
    /// </summary>
    private float yaw = 0;

    /// <summary>
    /// This variable holds the camera's pitch.
    /// </summary>
    private float pitch = 0;

    public float shakeIntensity = 0;

    /// <summary>
    /// These two variables hold the camera's sensitivity.
    /// </summary>
    public float cameraSensitivityX = 10;
    public float cameraSensitivityY = 10;

    private void Start()
    {
        targetScript = moveScript.GetComponent<PlayerTargeting>();
        cam = GetComponentInChildren<Camera>();
        
    }

    void Update()
    {
        PlayerOrbitCamera();

        transform.position = moveScript.transform.position;

        // If aiming, set camera rotation to look at target.
        RotateCamToLookAtTarget();

        // Move the camera closer to the player.
        ZoomCamera();

        ShakeCamera();
    }

    public void Shake(float intensity = 1)
    {
        if (intensity > 1)
        {
            shakeIntensity = intensity;
        }
        else
        {
            shakeIntensity += intensity;
            if (shakeIntensity > 1) shakeIntensity = 1;
        }
    }
    private void ShakeCamera()
    {
        if (shakeIntensity < 0) shakeIntensity = 0;
        if (shakeIntensity > 0) shakeIntensity -= Time.deltaTime;
        else return; // shake intensity is 0

        // Pick a small random rotation
        Quaternion targetRot = AnimMath.Lerp(Random.rotation, Quaternion.identity, .995f);

        //cam.transform.localRotation *= targetRot;
        cam.transform.localRotation = AnimMath.Lerp(cam.transform.localRotation, cam.transform.localRotation * targetRot, shakeIntensity * shakeIntensity);

    }

    /// <summary>
    /// This function zooms the camera behind the player when they target something.
    /// </summary>
    private void ZoomCamera()
    {
        float dis = 10;
        if (IsTargeting()) dis = 3;

        // Slide the camera's position to the new zoomed position.
        cam.transform.localPosition = AnimMath.Slide(cam.transform.localPosition, new Vector3(0, 0, -dis), .001f);
    }

    /// <summary>
    /// This function checks to see if the player is targeting something or not.
    /// </summary>
    /// <returns></returns>
    private bool IsTargeting()
    {
        return (targetScript && targetScript.target != null && targetScript.wantsToTarget);
    }

    /// <summary>
    /// This function uses the MouseX and MouseY axis to rotate the camera's rig.
    /// </summary>
    private void PlayerOrbitCamera()
    {
        float mX = Input.GetAxisRaw("Mouse X");
        float mY = Input.GetAxisRaw("Mouse Y");

        yaw += mX * cameraSensitivityX;
        pitch += mY * cameraSensitivityY;

        if (IsTargeting()) // z-targeting
        {
            pitch = Mathf.Clamp(pitch, 15, 60);

            float playerYaw = moveScript.transform.eulerAngles.y;
            yaw = Mathf.Clamp(yaw, playerYaw - 40, playerYaw + 40);
        }
        else // Not targeting
        {
            pitch = Mathf.Clamp(pitch, -10, 89);
        }

        // Ease to the clamped range.
        transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .001f);
    }

    /// <summary>
    /// This function rotates the camera when the player is targeting something.
    /// </summary>
    private void RotateCamToLookAtTarget()
    {
        if (IsTargeting())
        {
            // If targeting, set rotation to look at target.
            Vector3 vToTarget = targetScript.target.position - cam.transform.position;

            Quaternion targetRot = Quaternion.LookRotation(vToTarget, Vector3.up);

            // Ease towards the target rotation
            cam.transform.rotation = AnimMath.Slide(cam.transform.rotation, targetRot, .001f);
        }
        else
        {
            // If not targeting, reset rotation.
            // Ease back to the identity.
            cam.transform.localRotation = AnimMath.Slide(cam.transform.localRotation, Quaternion.identity, .001f); 
        }

    }
}
