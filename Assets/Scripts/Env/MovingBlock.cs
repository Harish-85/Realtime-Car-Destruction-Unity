using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    
    [SerializeField] private float downwardForce = 100f;
    [SerializeField] private float yHighPos = 10;
    [SerializeField] private float downwardForceTime = 1f;
    
    private Rigidbody rb;
    
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Invoke(nameof(ResetIsReturning), downwardForceTime);
        
    }
    public bool isReturning = false;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if(! isReturning)
            rb.AddForce(Vector3.down * downwardForce, ForceMode.Acceleration);
        else
        {
            rb.velocity = Vector3.up*19;
            
            if (transform.position.y >= yHighPos)
            {
                
                isReturning = false;
                Invoke(nameof(ResetIsReturning), downwardForceTime);
            }
            
        }
        
    }
    
    void ResetIsReturning()
    {
        isReturning = true;
    }
}
