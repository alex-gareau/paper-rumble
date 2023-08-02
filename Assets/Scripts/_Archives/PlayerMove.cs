using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public Rigidbody rb;

    public float moveSpeed;
    public float jumpForce;

    private Vector2 moveInput;

    public LayerMask groundLayer;
    public Transform groundCheck;

    private bool isGrounded;

    public Animator playerAnimator;

    public Animator spriteAnimator;

    public SpriteRenderer sprite;

    private bool movingBackward;

    [Header("Knockback variables")]
    public bool isKnocking;
    public float knockBackLength = 0.5f;
    public Vector2 knockbackPower;
    public float knockbackUpForce = 5f;

    private float knockbackCounter;
    private Vector3 knockbackDirection;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (isKnocking)
        {
            // Apply knockback force in the opposite direction

            Vector3 knockbackForce = new Vector3(knockbackDirection.x * knockbackPower.x, knockbackUpForce, knockbackDirection.z * knockbackPower.y);

            rb.velocity = knockbackForce;

            knockbackCounter -= Time.deltaTime;

            if (knockbackCounter <= 0)
            {
                isKnocking = false;
            }
        }
        else
        {
            moveInput.x = Input.GetAxis("Horizontal");
            moveInput.y = Input.GetAxis("Vertical");

            moveInput.Normalize();

            rb.velocity = new Vector3(moveInput.x * moveSpeed, rb.velocity.y, moveInput.y * moveSpeed);

            spriteAnimator.SetFloat("moveSpeed", rb.velocity.magnitude);

            RaycastHit hit;

            if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, 0.3f, groundLayer))
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rb.velocity += new Vector3(0, jumpForce, 0);
            }

            spriteAnimator.SetBool("onGround", isGrounded);

            if (!sprite.flipX && moveInput.x < 0)
            {
                sprite.flipX = true;
                playerAnimator.SetTrigger("Flip");
            }
            else if (sprite.flipX && moveInput.x > 0)
            {
                sprite.flipX = false;
                playerAnimator.SetTrigger("Flip");
            }

            if (!movingBackward && moveInput.y > 0)
            {
                movingBackward = true;
                playerAnimator.SetTrigger("Flip");
            }
            else if (movingBackward && moveInput.y < 0)
            {
                movingBackward = false;
                playerAnimator.SetTrigger("Flip");
            }

            spriteAnimator.SetBool("movingBackward", movingBackward);
        }
    }

    // Call this method to apply knockback to the player
    public void Knockback(Vector3 direction)
    {
        isKnocking = true;
        knockbackCounter = knockBackLength;
        knockbackDirection = direction;
    }
}
