using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            //PlayerHealthController.instance.DamagePlayer();
            PlayerMovement.instance.Knockback(transform.position);
        }

        if (other.tag == "Weapons")
        {
            transform.parent.GetComponent<Enemy>().TakeDamage(1, other.transform.position);

        }
    }
}
