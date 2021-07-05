using UnityEngine;

public enum GroundType
{
    None,
    Soft,
    Hard
}

public class PlayerController2D : MonoBehaviour
{
    readonly Vector3 flippedScale = new Vector3(-1, 1, 1);
    readonly Quaternion flippedRotation = new Quaternion(0, 0, 1, 0);

    [Header("Character")]
    [SerializeField] Animator animator = null;
    [SerializeField] Transform puppet = null;
    [SerializeField] PlayerAudio audioPlayer = null;

    [Header("Movement")]
    [SerializeField] float acceleration = 0.0f;
    [SerializeField] float maxSpeed = 0.0f;
    [SerializeField] float jumpForce = 0.0f;
    [SerializeField] float minFlipSpeed = 0.1f;
    [SerializeField] float jumpGravityScale = 1.0f;
    [SerializeField] float fallGravityScale = 1.0f;
    [SerializeField] float groundedGravityScale = 1.0f;
    [SerializeField] bool resetSpeedOnLand = false;

    private Rigidbody2D controllerRigidbody;
    private Collider2D controllerCollider;
    private LayerMask softGroundMask;
    private LayerMask hardGroundMask;

    private Vector2 movementInput;
    private bool jumpInput;

    private Vector2 prevVelocity;
    private GroundType groundType;
    private bool isFlipped;
    private bool isJumping;
    private bool isFalling;

    private int animatorGroundedBool;
    private int animatorRunningSpeed;

    void Start()
    {
        controllerRigidbody = GetComponent<Rigidbody2D>();
        controllerCollider = GetComponent<Collider2D>();
        softGroundMask = LayerMask.GetMask("Ground Soft");
        hardGroundMask = LayerMask.GetMask("Ground Hard");

        animatorGroundedBool = Animator.StringToHash("grounded");
        animatorRunningSpeed = Animator.StringToHash("speed");

    }

    void Update()
    {
        // Remove all player control when we're in dialogue
        //if (FindObjectOfType<DialogueRunner>().IsDialogueRunning == true)
        //    return;

        // Horizontal movement
        float moveHorizontal = Input.GetAxis("Horizontal");

        movementInput = new Vector2(moveHorizontal, 0);

        // Jumping input
        if (!isJumping && Input.GetKeyDown(KeyCode.Space))
            jumpInput = true;
    }

    void FixedUpdate()
    {
        UpdateGrounding();
        UpdateVelocity();
        UpdateDirection();
        UpdateJump();
        UpdateGravityScale();

        prevVelocity = controllerRigidbody.velocity;
    }

    private void UpdateGrounding()
    {
        // Use character collider to check if touching ground layers
        if (controllerCollider.IsTouchingLayers(softGroundMask))
            groundType = GroundType.Soft;
        else if (controllerCollider.IsTouchingLayers(hardGroundMask))
            groundType = GroundType.Hard;
        else
            groundType = GroundType.None;

        // Update animator
        animator.SetBool(animatorGroundedBool, groundType != GroundType.None);
    }

    private void UpdateVelocity()
    {
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !isFalling && !isJumping)
            controllerRigidbody.velocity = new Vector2(0.0f,controllerRigidbody.velocity.y);

        Vector2 velocity = controllerRigidbody.velocity;
        // Apply acceleration directly as we'll want to clamp
        // prior to assigning back to the body.
        velocity += movementInput * acceleration * Time.deltaTime;

        // We've consumed the movement, reset it.
        movementInput = Vector2.zero;

        // Clamp horizontal speed.
        velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);

        // Assign back to the body.
        controllerRigidbody.velocity = velocity;

        // Update animator running speed
        var horizontalSpeedNormalized = Mathf.Abs(velocity.x) / maxSpeed;
        animator.SetFloat(animatorRunningSpeed, horizontalSpeedNormalized);

        // Play audio
        audioPlayer.PlaySteps(groundType, horizontalSpeedNormalized);
    }

    private void UpdateJump()
    {
        // Set falling flag
        if (isJumping && controllerRigidbody.velocity.y < 0)
            isFalling = true;

        // Jump
        if (jumpInput && groundType != GroundType.None)
        {
            // Jump using impulse force
            controllerRigidbody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

            // We've consumed the jump, reset it.
            jumpInput = false;

            // Set jumping flag
            isJumping = true;

            // Play audio
            audioPlayer.PlayJump();
        }

        // Landed
        else if (isJumping && isFalling && groundType != GroundType.None)
        {
            // Since collision with ground stops rigidbody, reset velocity
            if (resetSpeedOnLand)
            {
                prevVelocity.y = controllerRigidbody.velocity.y;
                controllerRigidbody.velocity = prevVelocity;
            }

            // Reset jumping flags
            isJumping = false;
            isFalling = false;

            // Play audio
            audioPlayer.PlayLanding(groundType);
        }
    }

    private void UpdateDirection()
    {
        // Use scale to flip character depending on direction
        if (controllerRigidbody.velocity.x > minFlipSpeed && isFlipped)
        {
            isFlipped = false;
            puppet.localScale = Vector3.one;
        }
        else if (controllerRigidbody.velocity.x < -minFlipSpeed && !isFlipped)
        {
            isFlipped = true;
            puppet.localScale = flippedScale;
        }
    }

    private void UpdateGravityScale()
    {
        // Use grounded gravity scale by default.
        var gravityScale = groundedGravityScale;

        if (groundType == GroundType.None)
        {
            // If not grounded then set the gravity scale according to upwards (jump) or downwards (falling) motion.
            gravityScale = controllerRigidbody.velocity.y > 0.0f && Input.GetKey(KeyCode.Space) ? jumpGravityScale : fallGravityScale;
        }

        controllerRigidbody.gravityScale = gravityScale;
    }
}
