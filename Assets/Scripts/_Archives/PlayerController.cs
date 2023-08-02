using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Header("Movements and physics variables")]
    public float moveSpeed = 4f;
    public float jumpForce = 4f;
    public float gravityScale = 5f;
    public float rotateSpeed = 10f;
    public float bounceForce;
    public bool lastGrounded;

    public LayerMask groundLayer;
    public Transform groundCheck;

    private bool isGrounded;

    public Rigidbody rigidbody;

    private float storedY;
    private Vector3 moveAmount;
    private float regularGravity;

    [Header("Externals movements from moving platform")]
    //public MovingPlatform currentPlatform;
    public Vector3 externalMoveSpeed;

    [Header("Animations and effects")]
    public Animator playerAnimator;
    public Animator spriteAnimator;

    public SpriteRenderer sprite;

    public Transform weapon;

    public Transform weaponLeftPosition;
    public Transform weaponRightPosition;

    public Transform shield;

    public Transform shieldFrontLeftPosition;
    public Transform shieldFrontRightPosition;

    public Transform shieldBackLeftPosition;
    public Transform shieldBackRightPosition;

    public GameObject jumpParticles;
    public GameObject landingParticles;

    public bool stopMoving;
    public Transform cameraTarget;
    private Camera camera;

    [Header("Dashing's related variables")]
    public bool isDashing;
    private bool canDash = true;

    public const float maxDashTime = 1.0f;
    public float dashDistance = 5;
    public float dashStoppingSpeed = 0.25f;
    private float currentDashTime = maxDashTime;

    private bool movingBackward;

    [Header("Knockback variables")]
    public bool isKnocking;
    public float knockBackLength = 0.5f;
    public Vector2 knockbackPower;

    private float knockbackCounter;
    private Transform knockbackPosition;

    // Rest of the variables...

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Get the Rigidbody component
        rigidbody = GetComponent<Rigidbody>();

        regularGravity = gravityScale;

        lastGrounded = true;
    }

    private void FixedUpdate()
    {
        // Apply gravity to the Rigidbody
        //rigidbody.AddForce(new Vector3(0f, Physics.gravity.y * gravityScale * Time.fixedDeltaTime, 0f));

        if (!IsGrounded())
        {
            moveAmount.y = moveAmount.y + externalMoveSpeed.y + (Physics.gravity.y * gravityScale * Time.fixedDeltaTime);
        }
        else
        {
            moveAmount.y = Physics.gravity.y * gravityScale * Time.fixedDeltaTime;
        }

    }

    private void Update()
    {
        // Check if the level has ended or the game is paused
        // if (Time.timeScale == 0 || LevelManager.instance.levelComplete || stopMoving) return;

        storedY = moveAmount.y;

        // Check if the player is being knocked back
        if (isKnocking)
        {
            knockbackCounter -= Time.deltaTime;

            Vector3 direction = (transform.position - knockbackPosition.position);

            moveAmount = (direction * knockbackPower.x);

            moveAmount.y = knockbackPower.y;

            // Apply the knockback force to the Rigidbody
            rigidbody.velocity = moveAmount;

            if (knockbackCounter <= 0)
            {
                isKnocking = false;
            }
        }
        else
        {
            moveAmount = new Vector3(Input.GetAxisRaw("Horizontal"), moveAmount.y, Input.GetAxisRaw("Vertical"));

            moveAmount = moveAmount.normalized;

            moveAmount.y = storedY;

            if (Input.GetButtonDown("Jump") && lastGrounded)
            {
                // Apply jump force to the Rigidbody
                rigidbody.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
            }

            lastGrounded = IsGrounded();

            // Rest of the logic...

            // Move the Rigidbody
            rigidbody.velocity = new Vector3(moveAmount.x * moveSpeed, rigidbody.velocity.y, moveAmount.z * moveSpeed);
        }
    }

    private bool IsGrounded()
    {
        RaycastHit hit;

        if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, 0.3f, groundLayer))
        {
            return true;
        }

        return false;
    }

    // Rest of the methods...

    /// <summary>
    /// Knockback the player relative to where the damage came from
    /// </summary>
    /// <param name="attack">Damage position</param>
    public void Knockback(GameObject attack)
    {
        isKnocking = true;
        knockbackCounter = knockBackLength;

        knockbackPosition = attack.transform;
    }
}
