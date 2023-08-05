using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    [Header("Movements and physics variables")]
    public float moveSpeed = 4f;
    public float jumpForce = 4f;
    public float gravityScale = 5f;
    public float rotateSpeed = 10f;
    public float bounceForce;
    public bool lastGrounded;

    public CharacterController charController;

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

    public Equipment helm;

    public bool stopMoving;
    public Transform cameraTarget;
    private Camera camera;

    [Header("Special attack")]

    [SerializeField]
    private GameObject slamSphere;

    [SerializeField]
    private GameObject slamParticles;

    [Header("Dashing's related variables")]
    public bool isDashing;
    private bool canDash = true;

    public const float maxDashTime = 1.0f;
    public float dashDistance = 5;
    public float dashStoppingSpeed = 0.25f;
    private float currentDashTime = maxDashTime;

    private bool movingBackward;
    private bool flipped;

    [Header("Knockback variables")]
    public bool isKnocking;
    public float knockBackLength = 0.5f;
    public Vector2 knockbackPower;

    private float knockbackCounter;
    private Vector3 knockbackPosition;

    public float invincibilityLength = 1f;

    public GameObject[] modelDisplay;

    public float flashTime = 0.1f;

    private float flashCounter;


    private float invincibilityCounter;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        camera = FindObjectOfType<Camera>();

        regularGravity = gravityScale;

        gravityScale = 13f;

        lastGrounded = true;

        charController.Move(new Vector3(0f, Physics.gravity.y * gravityScale * Time.deltaTime, 0f));
    }

    void FixedUpdate()
    {
        if (!charController.isGrounded)
        {
            moveAmount.y = moveAmount.y + externalMoveSpeed.y + (Physics.gravity.y * gravityScale * Time.fixedDeltaTime);
        }
        else
        {
            moveAmount.y = Physics.gravity.y * gravityScale * Time.fixedDeltaTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Check if level has ended or game is paused
        //if (Time.timeScale == 0 || LevelManager.instance.levelComplete || stopMoving) return;

        storedY = moveAmount.y;

        //Check if the player is being knockback
        if (isKnocking)
        {
            knockbackCounter -= Time.deltaTime;

            Vector3 direction = (transform.position - knockbackPosition);

            moveAmount = (direction * knockbackPower.x);

            moveAmount.y = knockbackPower.y;

            charController.Move(moveAmount * Time.deltaTime * moveSpeed);

            if (knockbackCounter <= 0)
            {
                isKnocking = false;

            }

        }
        else
        {

            moveAmount = (camera.transform.forward * Input.GetAxisRaw("Vertical")) + (camera.transform.right * Input.GetAxisRaw("Horizontal"));
            moveAmount.y = 0f;
            moveAmount = moveAmount.normalized;

            moveAmount.y = storedY;

            if (charController.isGrounded)
            {
                //Display landing particles on landing
                if (!lastGrounded && externalMoveSpeed == Vector3.zero)
                {
                    if (landingParticles != null) landingParticles.SetActive(true);
                    //canDash = true;
                    gravityScale = 13f;
                }

                //Jump action
                if (Input.GetButtonDown("Jump"))
                {
                    moveAmount.y = jumpForce;
                }
            }
            else
            {
                gravityScale = regularGravity;
                //Dash action
                //if (Input.GetButtonDown("Fire1") && canDash)
                //{
                //    currentDashTime = 0;
                //    canDash = false;
                //    isDashing = true;
                //}
            }

            //if (currentDashTime < maxDashTime)
            //{
            //    moveAmount = transform.forward * dashDistance;
            //    currentDashTime += dashStoppingSpeed;
            //}
            //else
            //{
            //    isDashing = false;
            //}

            lastGrounded = charController.isGrounded;

            spriteAnimator.SetFloat("moveSpeed", charController.velocity.magnitude);

            spriteAnimator.SetBool("onGround", lastGrounded);

            if (Input.GetButtonDown("Fire1"))
            {
                Attack(sprite.flipX);
            }

            if (Input.GetButtonDown("Fire2"))
            {
                SlamAttack();
            }

            spriteAnimator.SetBool("movingBackward", movingBackward);

            charController.Move((new Vector3(moveAmount.x * moveSpeed, moveAmount.y, moveAmount.z * moveSpeed)) * Time.deltaTime);

            if (!flipped && moveAmount.x < 0)
            {
                flipped = true;
                playerAnimator.SetTrigger("Flip");
            }
            else if (flipped && moveAmount.x > 0)
            {
                flipped = false;
                playerAnimator.SetTrigger("Flip");
            }

            if (!movingBackward && moveAmount.z > 0)
            {
                helm.DisplayBack();
                movingBackward = true;
            }
            else if (movingBackward && moveAmount.z < 0)
            {
                helm.DisplayFront();
                movingBackward = false;
            }

            ShieldPosition();
        }


        if (invincibilityCounter > 0)
        {
            invincibilityCounter -= Time.deltaTime;


            flashCounter -= Time.deltaTime;

            if (flashCounter <= 0)
            {
                flashCounter = flashTime;

                ToggleRenderer();
            }

            if (invincibilityCounter <= 0)
            {
                ToggleRenderer(true);
            }

        }

    }

    private void ShieldPosition()
    {
        if (flipped && movingBackward)
        {
            //weapon.position = weaponLeftPosition.position;
            shield.position = new Vector3(shieldFrontLeftPosition.position.x, shieldBackRightPosition.position.y, shieldFrontLeftPosition.position.z);
        } else if (flipped)
        {
            //weapon.position = weaponRightPosition.position;
            shield.position = new Vector3(shieldFrontLeftPosition.position.x, shieldFrontRightPosition.position.y, shieldBackLeftPosition.position.z);
        } else if (!flipped && movingBackward)
        {
            //weapon.position = weaponRightPosition.position;
            shield.position = new Vector3(shieldBackLeftPosition.position.x, shieldBackLeftPosition.position.y, shieldBackLeftPosition.position.z);
        } else
        {
            //weapon.position = weaponRightPosition.position;
            shield.position = new Vector3(shieldFrontLeftPosition.position.x, shieldFrontLeftPosition.position.y, shieldFrontLeftPosition.position.z);
        }
    }

    private void Attack(bool moveLeft)
    {
        if (moveLeft)
        {
            weapon.GetComponent<Animator>().Play("SwingLeft");
        }
        else
        {
            weapon.GetComponent<Animator>().Play("SwingRight");
        }
    }

    /// <summary>
    /// Make the player bounce
    /// </summary>
    public void Bounce()
    {
        currentDashTime = maxDashTime;
        moveAmount.y = bounceForce;

        charController.Move(Vector3.up * bounceForce * Time.deltaTime);
    }

    /// <summary>
    /// Knockback the player relative to where the damage came from
    /// </summary>
    /// <param name="attack">Damage position</param>
    public void Knockback(Vector3 attackDirection)
    {
        if (invincibilityCounter > 0) return;

        isKnocking = true;
        knockbackCounter = knockBackLength;

        knockbackPosition = attackDirection;

        invincibilityCounter = invincibilityLength;
    }

    private void ToggleRenderer(bool display = false)
    {
        Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (display)
            {
                renderer.enabled = display;
            } else
            {
                renderer.enabled = !renderer.enabled;
            }
        }
    }

    private void SlamAttack()
    {
        slamSphere.SetActive(true);
        slamParticles.SetActive(true);
        slamSphere.transform.DOScale(new Vector3(4, 4, 4), 0.5f).OnComplete(ResetSlamAttack);
    }

    private void ResetSlamAttack()
    {
        slamSphere.transform.localScale = new Vector3(1, 1, 1);
        slamSphere.SetActive(false);
    }
}
