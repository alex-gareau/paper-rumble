using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public int _damage = 1;

    [SerializeField]
    private int _currentHealth = 1;

    [SerializeField]
    private SpriteRenderer _sprite;

    [Header("Animations and effects")]
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Animator _spriteAnimator;

    private bool _flipped;

    [SerializeField]
    private GameObject _deathParticles;

    [SerializeField]
    private GameObject _coinPrefab;

    private float _dyingCounter;
    private bool _isDead;

    public void SetupEnemy(int health, Transform entrancePostition, float moveSpeed)
    {
        _currentHealth = health;

        GetComponent<EnemyAI>().SetupEnemyMovement(entrancePostition, moveSpeed);

    }

    private void Update()
    {
        //Check if enemy is already dead
        if (_dyingCounter > 0)
        {
            _dyingCounter -= Time.deltaTime;

            if (_dyingCounter <= 0)
            {
                if (_deathParticles != null) Instantiate(_deathParticles, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }

        if (!_flipped && GetComponent<EnemyAI>().XVelocity < 0)
        {
            _flipped = true;
            _animator.SetTrigger("Flip");
        }
        else if (_flipped && GetComponent<EnemyAI>().XVelocity > 0)
        {
            _flipped = false;
            _animator.SetTrigger("Flip");
        }
    }

    public void TakeDamage(int damage, Vector3 hitPosition)
    {
        if (_isDead) return;

        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            OnDeath();
            return;
        }

        GetComponent<EnemyAI>().Knockback(hitPosition);
    }

    private void OnDeath()
    {
        _sprite.gameObject.GetComponent<BoxCollider>().enabled = false;
        GetComponent<EnemyAI>().EnemyKilled();

        _animator.enabled = false;
        _spriteAnimator.Play("death");
        _dyingCounter = 0.8f;

        for (int i = 0; i < 3; i++)
        {

            Collectable newCollectable = CollectableManager.instance.collectablesPool.Get();

            newCollectable.transform.position = transform.position + new Vector3(0, 1f, 0);

            newCollectable.transform.rotation = transform.rotation;

            newCollectable.GetComponent<Collectable>().LaunchCoin();
        }
    }
}
