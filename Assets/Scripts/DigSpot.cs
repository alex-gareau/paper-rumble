using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigSpot : MonoBehaviour
{
    [SerializeField]
    private GameObject collectable;

    [SerializeField]
    private int numberOfCollectable;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

        DigOut();

    }


    private void DigOut()
    {
        for (int i = 0; i < numberOfCollectable; i++)
        {
            GameObject newCollectable = Instantiate(collectable, transform.position + new Vector3(0, 1f, 0), transform.rotation);
            newCollectable.GetComponent<Collectable>().LaunchCoin();
        }

        Destroy(gameObject);

    }

 

}
