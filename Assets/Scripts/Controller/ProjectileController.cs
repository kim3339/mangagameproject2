using UnityEngine;

public class ProjectileController : MonoBehaviour 
{
    public LayerMask layer;
    CircleCollider2D collider;


    private void Awake() 
    {
        collider = GetComponent<CircleCollider2D>();
        collider = GetComponent<CircleCollider2D>();
    }
    public bool Move(Vector2 moveAmount)
    {
        var bounds = collider.bounds;
        var hit = Physics2D.CircleCast(bounds.center, collider.radius,moveAmount.normalized, moveAmount.magnitude, layer);
        if(hit)
        {
            transform.position = hit.centroid;
            return true;
        }
        else
        {
            transform.position += (Vector3)moveAmount;
            return false;
        }
    }
}