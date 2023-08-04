using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class CollectableManager : MonoBehaviour
{

    public static CollectableManager instance;

    [SerializeField]
    private Collectable _collectablePrefab;

    [SerializeField]
    private Transform _collectablesParent;

    public IObjectPool<Collectable> collectablesPool;


    private void Awake()
    {
        instance = this;
        
        collectablesPool = new ObjectPool<Collectable>(
            CreateCollectable,
            OnGet,
            OnRelease,
            CallDestroy,
            maxSize: 25
        );
    }

    private Collectable CreateCollectable()
    {
        //Projectiles newProjectile = Instantiate(projectilePrefab, transform.position, new Quaternion(0, 0, 0, 0), projectilesParent);
        Collectable newCollectable = Instantiate(_collectablePrefab, _collectablesParent);
        newCollectable.SetPool(collectablesPool);
        return newCollectable;
    }

    private void OnGet(Collectable projectile)
    {
        projectile.gameObject.SetActive(true);
    }

    private void CallDestroy(Collectable projectiles)
    {
        Destroy(projectiles.gameObject);
    }

    private void OnRelease(Collectable projectiles)
    {
        projectiles.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
