using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Collectable : MonoBehaviour
{
    private enum collectables { Coin, Health, Live };

    [SerializeField]
    private SpriteRenderer sprite;

    [SerializeField]
    private collectables collectableType;

    [SerializeField]
    private int value = 1;

    [SerializeField]
    private int lifeTime = 10;

    private float lifeCounter;

    public float flashTime = 0.1f;

    private float flashCounter;

    [SerializeField]
    private bool pickable;

    private bool launched;

    private IObjectPool<Collectable> collectablePool;

    public void SetPool(IObjectPool<Collectable> pool)
    {
        collectablePool = pool;
    }

    // Update is called once per frame
    void Update()
    {
        if (launched && GetComponent<Rigidbody>().velocity.y == 0 && !pickable)
        {
            pickable = true;
            ToggleTrigger(true);
        }

        //Handle the item duration before flashing and disapearing
        if (lifeCounter > 0)
        {
            lifeCounter -= Time.deltaTime;

            if (lifeCounter < 3)
            {
                flashCounter -= Time.deltaTime;

                if (flashCounter <= 0)
                {
                    flashCounter = flashTime;

                    ToggleRenderer();
                }

                if (lifeCounter <= 0)
                {
                    //TODO should reset coin for pooling
                    collectablePool.Release(this);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        switch (collectableType)
        {
            case collectables.Coin:
                Debug.Log("Collected a coin");
                //GameManager.instance.AddCoins(value);
                break;
            case collectables.Health:
                Debug.Log("Collected a health item");
                break;

            case collectables.Live:
                Debug.Log("Collected a extra live");
                break;
            default:

                break;
        }

        collectablePool.Release(this);

    }

    public void LaunchCoin()
    {
        lifeCounter = lifeTime;
        launched = false;
        pickable = false;
        ToggleTrigger(false);
        GetComponent<Rigidbody>().AddForce(Random.Range(-2, 2), Random.Range(4, 6), Random.Range(-2, 2), ForceMode.Impulse);
        Invoke("CanCheckVelocity", 0.3f);
        
    }

    private void CanCheckVelocity()
    {
        launched = true;
    }

    //Use for coins when launched
    public void ToggleTrigger(bool value)
    {
        gameObject.layer = value ? 0 : 12;

        GetComponent<Collider>().isTrigger = value;
        GetComponent<Rigidbody>().isKinematic = value;

    }

    private void ToggleRenderer(bool display = false)
    {
        Renderer renderer = GetComponent<Renderer>();

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
