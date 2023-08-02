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

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private GameObject _deathParticles;

    [SerializeField]
    private GameObject _coinPrefab;

    private float _dyingCounter;
    private bool _isDead;

    public void SetupEnemy(int health, Transform entrancePostition, float moveSpeed)
    {
        _currentHealth = health;

        GetComponent<EnemyMovement>().SetupEnemyMovement(entrancePostition, moveSpeed);

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

        GetComponent<EnemyMovement>().Knockback(hitPosition);
    }

    private void OnDeath()
    {
        _sprite.gameObject.GetComponent<BoxCollider>().enabled = false;
        GetComponent<EnemyMovement>().EnemyKilled();

        _animator.Play("death");
        _dyingCounter = 0.8f;

        for (int i = 0; i < 3; i++)
        {

            Collectable newCollectable = GameManager.instance.collectablesPool.Get();

            newCollectable.transform.position = transform.position + new Vector3(0, 1f, 0);

            newCollectable.GetComponent<Collectable>().LaunchCoin();
        }
    }
}
