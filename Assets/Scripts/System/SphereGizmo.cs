using UnityEngine;
using System.Collections;

public class SphereGizmo : MonoBehaviour
{
    [SerializeField]
    private Color gizmoColor = Color.white;

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x);
        //Gizmos.DrawWireCube(transform.position, transform.localScale.x);
        
    }
}