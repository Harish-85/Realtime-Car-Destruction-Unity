using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Input = UnityEngine.Windows.Input;

public class Wheel : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private GameObject wheelMesh;
    
    [Header("Suspension")]
    public float restLength;
    public float springTravel;
    public float stiffness;
    public float damperStiffness;
    
    
    private float minLength;
    private float maxLength;
    private float lastLength;
    private float springLength;
    private float springForce;
    private float springVelocity;
    private float damperForce;
    
    [Header("Acceleration and Deceleration")]
    public float acceleration;

    public float friction = .1f;
    private float input;
    
    public float brakeFriciton = 1;
    
    [Header("Wheel")]
    public float wheelRadius;
    


    [Header("Steering")] public float maxSteerAngle;
    public float antiSkid = 0.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //move this to start
        minLength = restLength - springTravel;
        maxLength = restLength + springTravel;
        WheelPhysics();   
        
    }
    Vector3 prevVelocity;
    
    
    private void WheelPhysics(){
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, maxLength + wheelRadius))
        {
            
            lastLength = springLength;
            springLength = hit.distance - wheelRadius;
            springLength = Mathf.Clamp(springLength,minLength, maxLength);
            springVelocity = (lastLength - springLength) / Time.fixedDeltaTime;
            springForce = stiffness * (restLength - springLength);

            damperForce = damperStiffness * springVelocity;
            
            Vector3 suspensionForce = transform.up * (springForce + damperForce);
            
             //calc forwarrd force based on input
            Vector3 forwardForce = transform.forward * input * acceleration;
            Vector3 vel = (rb.GetPointVelocity(hit.point));
            //friction force
            Vector3 forwardFriction;
            Vector3 rightForce;
            rightForce = transform.right * Vector3.Dot(vel,transform.right) ;
            Vector3 antiSkidForce = -rightForce * antiSkid ;
            if (brakeInput > 0.5)
            {
                forwardFriction = -transform.forward * Vector3.Dot(vel, transform.forward) * brakeFriciton;
                
            }
            else
            {
                forwardFriction = -transform.forward * Vector3.Dot(vel,transform.forward) * friction ;
            }
            //anti skid force
            
            
            
            
            Debug.DrawLine(hit.point,hit.point + suspensionForce/rb.mass,Color.green);
            Debug.DrawLine(hit.point,hit.point + forwardForce,Color.red);
            Debug.DrawLine(hit.point,hit.point + antiSkidForce,Color.blue);
            Debug.DrawLine(hit.point,hit.point + forwardFriction,Color.yellow);
            
            rb.AddForceAtPosition(suspensionForce,hit.point);
            rb.AddForceAtPosition(forwardForce + antiSkidForce + forwardFriction,hit.point,ForceMode.Acceleration);

            prevVelocity = vel;
        }
        UpdateWheelMesh(springLength);
    }
    
    public void SetDriveInput(float driveInput){
       input = driveInput;
    }

    public void SetSteering(float steerInput)
    { 
        transform.localEulerAngles = new Vector3(0, maxSteerAngle * steerInput, 0);
    }

    float brakeInput;
    public void SetBrake(float brakeInput)
    {
        this.brakeInput = brakeInput;
    }

    private void UpdateWheelMesh(float wheelHeight)
    {
        wheelMesh.transform.position = transform.position - transform.up * wheelHeight;
        
        //rotate wheel mesh
        int dir = Vector3.Dot(transform.forward,rb.velocity) > 0 ? 1 : -1;
        wheelMesh.transform.Rotate(Vector3.forward,rb.velocity.magnitude *dir* Time.deltaTime * 360 / (2 * Mathf.PI * wheelRadius));
        
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position,transform.position - transform.up * (springLength + wheelRadius));
        Gizmos.DrawWireSphere(transform.position - (springLength*transform.up),wheelRadius);
        Gizmos.DrawLine(transform.position,transform.position + transform.forward * input * 2);
    }
}
