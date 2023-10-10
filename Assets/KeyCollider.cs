using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCollider : MonoBehaviour
{
    public int num;
    public Piano piano;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        var selector = other.transform.GetComponent<KeySelector>();
        if (selector == null) return;
        piano.OnKeyColliderEnter(num, selector.handedness);
    }
}
