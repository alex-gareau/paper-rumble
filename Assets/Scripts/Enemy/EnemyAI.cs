using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("AI variables")]
    [SerializeField]
    private float _detectionRange = 10f; // Range within which the enemy can detect the player
    [SerializeField]
    private float _movementSpeed = 3f; // Speed at which the enemy moves towards the player
    [SerializeField]
    private float _attackRange = 1.5f; // Range at which the enemy stops and attacks the player

    private Transform _entrancePoint;
    private Transform _player;
    private NavMeshAgent _navMeshAgent;
    private Rigidbody _rb;

    private bool _isDead;

    [Header("Knockback variables")]
    [SerializeField]
    private Vector3 _knockbackPower = new Vector3(3f, 5f, 3f);
    [SerializeField]
    private float _knockBackLength = 0.5f;
    private bool _isKnocking = false;
    private float _knockbackCounter = 0f;
    
    
    [Header("Invincibility and flashing variables")]
    [SerializeField]
    private float _flashTime = 0.1f;
    [SerializeField]
    private float _invincibilityLength = 1.5f;
    private float _flashCounter;
    private bool _isInvincible = false;
    private float _invincibilityCounter = 0f;

    public void SetupEnemyMovement(Transform entrance, float speed = 2f)
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();

        _navMeshAgent.updateRotation = false;

        AddEnemyTarget(FindFirstObjectByType<PlayerMovement>().gameObject.transform);

        //Set the current player
        _player = PlayerMovement.instance.transform;

        _entrancePoint = entrance;
        
        _navMeshAgent.speed = Random.Range(1f, 3f);
    }

    private void Update()
    {
        if (_player == null || _isDead) return;

        //if (_navMeshAgent.velocity != Vector3.zero)
        //{
        //    GetComponent<Enemy>().ToggleWalkAnimation(true);
        //} else
        //{
        //    GetComponent<Enemy>().ToggleWalkAnimation(false);
        //}

        if (_isKnocking)
        {
            //Decrement the knockback counter
            _knockbackCounter -= Time.deltaTime;

            if (_knockbackCounter <= 0)
            {
                _isKnocking = false;

                // Re-enable NavMeshAgent after knockback is over
                _navMeshAgent.enabled = true;
            }
        }

        if (_isInvincible)
        {
            //Decrement the invincibility counter
            _invincibilityCounter -= Time.deltaTime;

            
            _flashCounter -= Time.deltaTime;

            if (_flashCounter <= 0)
            {
                _flashCounter = _flashTime;

                ToggleRenderer();
            }

            if (_invincibilityCounter <= 0)
            {
                _isInvincible = false;
                ToggleRenderer(true);
            }
        }

        //Check if the player is within the detection range
        if (Vector3.Distance(transform.position, _player.position) <= _detectionRange && _navMeshAgent.enabled)
        {
            Vector3 direction = _player.position - transform.position;
            _navMeshAgent.destination = _player.position;
        }
    }

    /// <summary>
    /// Knockback the player relative to where the damage came from
    /// </summary>
    /// <param name="attack">Damage position</param>
    public void Knockback(Vector3 attackDirection)
    {
        if (!_isKnocking)
        {
            //Disable NavMeshAgent during knockback
            _navMeshAgent.enabled = false;

            _isKnocking = true;
            _knockbackCounter = _knockBackLength;
            _isInvincible = true;
            _invincibilityCounter = _invincibilityLength;

            //Calculate the knockback direction based on the attack direction
            Vector3 knockbackDirection = (transform.position - attackDirection).normalized;

            //Apply knockback force to the rigidbody
            Vector3 knockbackForce = new Vector3(knockbackDirection.x * _knockbackPower.x, _knockbackPower.y, knockbackDirection.z * _knockbackPower.z);
            _rb.velocity = knockbackForce;
        }
    }

    /// <summary>
    /// Handle the death of the enemy with the movement
    /// </summary>
    public void EnemyKilled()
    {
        _isDead = true;
        _rb.useGravity = false;
        _navMeshAgent.enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        _rb.velocity = Vector3.zero;
        ToggleRenderer(true);
    }

    /// <summary>
    /// Handle the toggle of the renderer
    /// </summary>
    /// <param name="display">If true, it force to display the renderer</param>
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


    public void AddEnemyTarget(Transform player)
    {
        _player = player;
    }
}
