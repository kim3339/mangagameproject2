using UnityEngine;
using System.Collections;

public class Behaviour : MonoBehaviour
{
    protected Character self;
    public virtual void Connect(Character self) { this.self = self; }
}
