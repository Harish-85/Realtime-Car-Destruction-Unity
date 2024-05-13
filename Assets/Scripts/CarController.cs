using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class CarController : MonoBehaviour
{
    public Wheel[] frontWheels;
    public Wheel[] rearWheels;

    [SerializeField] private Camera _camera;

    public float health = 1000;
    public float collisionDamageMultiplier = .001f;

    // Start is called before the first frame update
    void Start()
    {
 

    }

    private void Update()
    {
        OnForwardBackward(Input.GetAxis("Vertical"));
        //OnBrake(Input.GetAxis("Vertical") * -1);
        OnSteer(Input.GetAxis("Horizontal"));
    }

    private void OnBrake(float arg0)
        {
            foreach (var w in rearWheels)
            {
                if (arg0 > 0)
                {
                    w.SetBrake(1);
                }
                else
                {
                    w.SetBrake(0);
                }
            }
        }



        public void OnForwardBackward(float value)
        {
            foreach (var w in rearWheels)
            {
                w.SetDriveInput(value);
//            Debug.Log(value);
            }
        }



        public void OnSteer(float value)
        {
            foreach (var w in frontWheels)
            {
                w.SetSteering(value);
            }
        }


    }
