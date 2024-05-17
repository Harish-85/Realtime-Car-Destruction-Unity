using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableCarPart : MonoBehaviour
{
    public float health;

    private bool isBroken = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
