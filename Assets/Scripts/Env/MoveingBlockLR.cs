using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveingBlockLR : MonoBehaviour
{
    
    [SerializeField] private float startPosition;
    [SerializeField] private float endPosition;
    
    [SerializeField] private float force = 1000f;
    
    private Rigidbody rb;
    
    private bool isMovingToEnd = true;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isMovingToEnd)
        {
            rb.AddForce(-Vector3.right * force, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(Vector3.right *force, ForceMode.Acceleration);
        }
        
        if (transform.position.x <= endPosition)
        {
            isMovingToEnd = false;
        }
        else if (transform.position.x >= startPosition)
        {
            isMovingToEnd = true;
        }
    }
}