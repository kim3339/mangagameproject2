using UnityEngine;
using System.Collections;

public class Behavior : MonoBehaviour
{
    Character self;
    public virtual void Connect(Character self) { this.self = self; }
}
