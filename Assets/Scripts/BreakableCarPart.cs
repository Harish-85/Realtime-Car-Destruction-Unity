using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableCarPart : MonoBehaviour
{
    public float health;
    protected bool isBroken = false;
    
    void Update()
    {
        if (isBroken)
            return;
        
        if(health <= 0)
        {
            OnBreak();
            isBroken = true;
        }
    }
    public virtual void OnBreak()
    {
        if (TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            GetComponent<Collider>().enabled = true;
        }
    }
}
