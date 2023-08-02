using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public enum EnemyState { enter, chasing };

    [Header("Enemy state and type")]
    public EnemyState currentState;

    [Header("Enemy movements and physics variables")]
    public float moveSpeed;
    public float chaseDistance;
    public float chaseSpeed;
    public float hopForce;
    public float waitChaseTime;
    public float dyingTime = 0.5f;

    public Rigidbody rb;

    private Vector3 _moveDirection;
    private Vector3 _lookTarget;

    private float _yStore;

    private float _waitChaseCounter;

    [Header("Enemy effects and patrol points")]

    public GameObject deathEffect;
    public Transform[] patrolPoints;
    private Transform _entrancePoint;

    private PlayerMovement _player;

    private bool _isDead;

    [Header("Knockback variables")]
    public bool isKnocking;
    public float knockBackLength = 0.5f;
    public Vector3 knockbackPower;

    private float knockbackCounter;
    private Vector3 knockbackPosition;

    [Header("Ground check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;

    [Header("Invincibility variables")]
    public float invincibilityLength = 1f;

    public GameObject[] modelDisplay;

    public float flashTime = 0.1f;

    private float flashCounter;

    private float invincibilityCounter;

    public void SetupEnemyMovement(Transform entrance, float speed = 2f)
    {
        //Set the current player
        _player = PlayerMovement.instance;

        _entrancePoint = entrance;

        currentState = EnemyState.enter;

        moveSpeed = Random.Range(1f, 3f);
    }

    // Update is called once per frame
    void Update()
    {

        if(!_isDead)
        {

            if (isKnocking)
            {
                // Apply knockback force in the opposite direction
                Vector3 knockbackForce = new Vector3(knockbackPosition.x * knockbackPower.x, knockbackPower.y, knockbackPosition.z * knockbackPower.z);

                Debug.Log("In the update " + knockbackForce);

                rb.velocity = knockbackForce;

                knockbackCounter -= Time.deltaTime;

                if (knockbackCounter <= 0)
                {
                    isKnocking = false;
                }
            }
            else
            {

                // Ground check
                bool isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

                switch (currentState)
                {
                    //Moving to the current patrol point
                    case EnemyState.enter:
                        _yStore = rb.velocity.y;
                        _moveDirection = _entrancePoint.position - transform.position;

                        _moveDirection.y = 0f;
                        _moveDirection.Normalize();

                        rb.velocity = _moveDirection * moveSpeed;
                        rb.velocity = new Vector3(rb.velocity.x, _yStore, rb.velocity.z);

                        if (Vector3.Distance(transform.position, _entrancePoint.position) <= 0.1f)
                        {
                            currentState = EnemyState.chasing;
                        }
                        break;

                    //Chasing the player
                    case EnemyState.chasing:

                        if (_waitChaseCounter > 0)
                        {
                            _waitChaseCounter -= Time.deltaTime;
                        }
                        else if (isGrounded) // Only chase when on the ground
                        {
                            _yStore = rb.velocity.y;
                            _moveDirection = _player.transform.position - transform.position;
                            _moveDirection.y = 0f;
                            _moveDirection.Normalize();
                            rb.velocity = _moveDirection * moveSpeed;
                            rb.velocity = new Vector3(rb.velocity.x, _yStore, rb.velocity.z);
                        }


                        break;

                }

            }
            


            _lookTarget.y = transform.position.y;

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

    /// <summary>
    /// Knockback the player relative to where the damage came from
    /// </summary>
    /// <param name="attack">Damage position</param>
    public void Knockback(Vector3 attackDirection)
    {
        attackDirection = PlayerMovement.instance.transform.position;

        isKnocking = true;
        knockbackCounter = knockBackLength;

        // Calculate the opposite direction
        Vector3 knockbackDirection = (transform.position - attackDirection).normalized;

        // Scale the knockback direction by the knockback power
        knockbackPosition = knockbackDirection;

        invincibilityCounter = invincibilityLength;
    }

    public void EnemyKilled()
    {
        _isDead = true;
        rb.useGravity = false;
        GetComponent<BoxCollider>().enabled = false;
        rb.velocity = Vector3.zero;
    }

    private void ToggleRenderer(bool display = false)
    {
        Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (display)
            {
                renderer.enabled = display;
            }
            else
            {
                renderer.enabled = !renderer.enabled;
            }
        }
    }
}
