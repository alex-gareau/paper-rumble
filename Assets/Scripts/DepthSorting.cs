using UnityEngine;

public class DepthSorting : MonoBehaviour
{
    private Renderer spriteRenderer;
    private Transform cameraTransform;

    private void Start()
    {
        spriteRenderer = GetComponent<Renderer>();
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        // Calculate the distance in the Z-axis only
        float distance = Mathf.Abs(cameraTransform.position.z - transform.position.z);

        // Set the sorting order based on the distance
        spriteRenderer.sortingOrder = Mathf.RoundToInt(distance * -50);
    }
}
