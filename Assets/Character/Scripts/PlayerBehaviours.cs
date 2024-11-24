using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviours : MonoBehaviour
{
    //protected Animator anim;                     // Reference to the Animator component.
    protected int speedFloat;                      // Speed parameter on the Animator.
    protected bool canSprint;                      // Boolean to store if the behaviour allows the player to sprint.

    public float walkSpeed = 0.15f;                 // Default walk speed.
    public float runSpeed = 1.0f;                   // Default run speed.
    public float sprintSpeed = 2.0f;                // Default sprint speed.
    public float speedDampTime = 0.1f;              // Default damp time to change the animations based on current speed.
    private bool isSprinting;
    private float speed;
    private float speedSeeker;

    public string jumpButton = "Jump";              // Default jump button.
    public float jumpHeight = 1.5f;                 // Default jump height.
    public float jumpIntertialForce = 10f;          // Default horizontal inertial force when jumping.
    private int jumpBool;                           // Animator variable related to jumping.
    private int groundedBool;                       // Animator variable related to whether or not the player is on ground.
    private bool isGrounded;
    private bool jump;                              // Boolean to determine whether or not the player started a jump.

    private bool isColliding;                       // Boolean to determine if the player has collided with an obstacle.

    private Rigidbody rb;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        jumpBool = Animator.StringToHash("Jump");

        speedSeeker = runSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //move the player

        //rotate the player according to the camera

        //jump if needed

    }

    // Deal with the basic player movement
    void MovementManagement(float horizontal, float vertical)
    {
        // On ground, obey gravity.
        if (isGrounded)
            rb.useGravity = true;

        // Avoid takeoff when reached a slope end.
        else if (!anim.GetBool(jumpBool) && rb.velocity.y > 0)
        {
            RemoveVerticalVelocity();
        }

        // Call function that deals with player orientation.
        //Rotating(horizontal, vertical);

        // Set proper speed.
        Vector2 dir = new Vector2(horizontal, vertical);
        speed = Vector2.ClampMagnitude(dir, 1f).magnitude;
        // This is for PC only, gamepads control speed via analog stick.
        speedSeeker += Input.GetAxis("Mouse ScrollWheel");
        speedSeeker = Mathf.Clamp(speedSeeker, walkSpeed, runSpeed);
        speed *= speedSeeker;
        if (isSprinting)
        {
            speed = sprintSpeed;
        }

        anim.SetFloat(speedFloat, speed, speedDampTime, Time.deltaTime);
    }

    // Execute the idle and walk/run jump movements.
    void JumpManagement()
    {
        // Start a new jump.
        if (jump && !anim.GetBool(jumpBool) && isGrounded)
        {
            // Set jump related parameters.
            anim.SetBool(jumpBool, true);
            // Is a locomotion jump?
            if (anim.GetFloat(speedFloat) > 0.1)
            {
                // Temporarily change player friction to pass through obstacles.
                GetComponent<CapsuleCollider>().material.dynamicFriction = 0f;
                GetComponent<CapsuleCollider>().material.staticFriction = 0f;
                // Remove vertical velocity to avoid "super jumps" on slope ends.
                RemoveVerticalVelocity();
                // Set jump vertical impulse velocity.
                float velocity = 2f * Mathf.Abs(Physics.gravity.y) * jumpHeight;
                velocity = Mathf.Sqrt(velocity);
                rb.AddForce(Vector3.up * velocity, ForceMode.VelocityChange);
            }
        }
        // Is already jumping?
        else if (anim.GetBool(jumpBool))
        {
            // Keep forward movement while in the air.
            if (!isGrounded && !isColliding)
            {
                rb.AddForce(transform.forward * jumpIntertialForce * Physics.gravity.magnitude * sprintSpeed, ForceMode.Acceleration);
            }
            // Has landed?
            if ((rb.velocity.y < 0) && isGrounded)
            {
                anim.SetBool(groundedBool, true);
                // Change back player friction to default.
                GetComponent<CapsuleCollider>().material.dynamicFriction = 0.6f;
                GetComponent<CapsuleCollider>().material.staticFriction = 0.6f;
                // Set jump related parameters.
                jump = false;
                anim.SetBool(jumpBool, false);
            }
        }
    }

    // Remove vertical rigidbody velocity.
    private void RemoveVerticalVelocity()
    {
        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        rb.velocity = horizontalVelocity;
    }
}
