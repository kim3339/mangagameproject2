using UnityEngine;
using System.Collections.Generic;
using System;

public class CameraController : MonoBehaviour 
{
    
    public CameraSetting settings;
    public Vector2 TotalFollow => total;
    public List<CameraFollow> follows = new List<CameraFollow>();
    Vector2 total;
    new Transform transform;
    private void Awake() {
        transform = base.transform;
    }
    private void LateUpdate() {
        if(follows.Count == 0) return;

        total = Vector2.zero;
        var totalstr = Vector2.zero;
        follows.ForEach((f) => totalstr += f.strength);
        follows.ForEach((f) => total += (Vector2)f.follow.position);
        total /= totalstr;

        transform.position += (Vector3)(total - (Vector2)transform.position) * 0.3f;
    }
}