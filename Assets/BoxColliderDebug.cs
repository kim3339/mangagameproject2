using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxColliderDebug : MonoBehaviour
{
    public BoxCollider2D col;
    public Color color = new Color(1, 0, 0, 0.2f);
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawCube(col.bounds.center, col.bounds.size);
    }
}
