using UnityEngine;
using Cinemachine;
using System.Collections;

public class AimBehaviourBasic : GenericBehaviour
{
    [SerializeField]
    private CinemachineFreeLook freeLookCamera; // Reference to the Cinemachine FreeLook Camera.

    public string aimButton = "Aim", shoulderButton = "Aim Shoulder";     // Default aim and switch shoulders buttons.
    public Texture2D crosshair;                                           // Crosshair texture.
    public float aimTurnSmoothing = 0.15f;                                // Speed of turn response when aiming to match camera facing.
    public float aimSensitivity = 2f;                                     // Sensitivity when aiming.
    private float originalSensitivity;                                    // Stores original camera sensitivity.

    private int aimBool;                                                  // Animator variable related to aiming.
    private bool aim;                                                     // Boolean to determine whether or not the player is aiming.

    // Start is always called after any Awake functions.
    void Start()
    {
        // Set up the references.
        aimBool = Animator.StringToHash("Aim");

        if (freeLookCamera != null)
        {
            originalSensitivity = freeLookCamera.m_XAxis.m_MaxSpeed; // Save original sensitivity.
        }
        else
        {
            Debug.LogWarning("CinemachineFreeLook Camera is not assigned in AimBehaviourBasic.");
        }
    }

    // Update is used to set features regardless of the active behaviour.
    void Update()
    {
        // Activate/deactivate aim by input.
        if (Input.GetAxisRaw(aimButton) != 0 && !aim)
        {
            StartCoroutine(ToggleAimOn());
        }
        else if (aim && Input.GetAxisRaw(aimButton) == 0)
        {
            StartCoroutine(ToggleAimOff());
        }

        // No sprinting while aiming.
        canSprint = !aim;

        // Toggle shoulder camera position.
        if (aim && Input.GetButtonDown(shoulderButton))
        {
            SwapShoulder();
        }

        // Set aim boolean on the Animator Controller.
        behaviourManager.GetAnim.SetBool(aimBool, aim);
    }

    // Coroutine to start aiming mode with delay.
    private IEnumerator ToggleAimOn()
    {
        yield return new WaitForSeconds(0.05f);

        if (behaviourManager.GetTempLockStatus(this.behaviourCode) || behaviourManager.IsOverriding(this))
            yield return false;

        aim = true;

        if (freeLookCamera != null)
        {
            // Adjust Cinemachine sensitivity for aiming.
            freeLookCamera.m_XAxis.m_MaxSpeed = aimSensitivity;
        }

        yield return new WaitForSeconds(0.1f);
        behaviourManager.GetAnim.SetFloat(speedFloat, 0);
        behaviourManager.OverrideWithBehaviour(this); // Override the current behaviour.
    }

    // Coroutine to end aiming mode with delay.
    private IEnumerator ToggleAimOff()
    {
        aim = false;

        if (freeLookCamera != null)
        {
            // Reset Cinemachine sensitivity.
            freeLookCamera.m_XAxis.m_MaxSpeed = originalSensitivity;
        }

        yield return new WaitForSeconds(0.3f);
        behaviourManager.RevokeOverridingBehaviour(this); // Revoke the current behaviour.
    }

    // Swaps the shoulder camera view by flipping offsets.
    private void SwapShoulder()
    {
        if (freeLookCamera != null)
        {
            var middleRig = freeLookCamera.GetRig(1); // Get the middle rig.
            if (middleRig != null)
            {
                var composer = middleRig.GetCinemachineComponent<CinemachineComposer>();
                if (composer != null)
                {
                    composer.m_TrackedObjectOffset.x *= -1; // Flip the X offset.
                }
            }
        }
    }

    // LocalFixedUpdate overrides the virtual function of the base class.
    public override void LocalFixedUpdate()
    {
        if (aim)
        {
            AimManagement();
        }
    }

    // LocalLateUpdate: manager is called here to set player rotation after camera rotates, avoiding flickering.
    public override void LocalLateUpdate()
    {
        AimManagement();
    }

    // Handle aim parameters when aiming is active.
    private void AimManagement()
    {
        // Deal with the player orientation when aiming.
        Rotating();
    }

    // Rotate the player to match correct orientation, according to the camera.
    private void Rotating()
    {
        if (freeLookCamera == null) return;

        Vector3 forward = freeLookCamera.transform.forward;
        forward.y = 0.0f;
        forward = forward.normalized;

        Quaternion targetRotation = Quaternion.LookRotation(forward);
        float minSpeed = Quaternion.Angle(transform.rotation, targetRotation) * aimTurnSmoothing;

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, minSpeed * Time.deltaTime);
    }

    // Draw the crosshair when aiming.
    void OnGUI()
    {
        if (aim && crosshair)
        {
            GUI.DrawTexture(new Rect(Screen.width / 2 - (crosshair.width * 0.5f),
                                     Screen.height / 2 - (crosshair.height * 0.5f),
                                     crosshair.width, crosshair.height), crosshair);
        }
    }
}
