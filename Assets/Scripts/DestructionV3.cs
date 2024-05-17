using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionV3 : MonoBehaviour
{
    [SerializeField] private List<MeshFilter> meshFilters;
    [SerializeField] private float damageRadius = .4f;
    [SerializeField] private float minCollisionImpulse = 10000f;
    [SerializeField] private float fallOffMul = 1f;
    [SerializeField] private Vector2 damageImpRange = new Vector2(15000, 20000);
    [SerializeField] private float maxDamage = 10;
    
    [SerializeField] private float damageDisp = .5f; 
    [SerializeField] private Rigidbody rb;
    [SerializeField] private MeshCollider meshCollider;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collision: " + other.impulse);
        foreach (var contacts in other.contacts)
        {
            Vector3 normal = contacts.normal;
            Debug.DrawRay(contacts.point, normal, Color.red, 10f);

            if (other.impulse.magnitude > damageImpRange.x)
            {
                //loop through all the mesh filters
                DeformMesh(other, contacts, normal);
            }

            
        }
    }

    private void DeformMesh(Collision other, ContactPoint contacts, Vector3 normal)
    {
        foreach (var m in meshFilters)
        {
           bool didDamage = false;
            
            Vector3[] vertices = m.mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 worldPoint = m.transform.TransformPoint(vertices[i]);
                //Debug.DrawRay(worldPoint, Vector3.up, Color.green, 10f);
                float dist = Vector3.Distance(worldPoint, contacts.point);
                //Debug.Log(dist);
                //move the vertices within range in the direction of the normal 
                if (dist < damageRadius)
                {
                    
                    if (!didDamage && m.TryGetComponent(out BreakableCarPart br ))
                    {
                        didDamage = true;
                        br.health -=
                            Mathf.Clamp01(other.impulse.magnitude - damageImpRange.x / (damageImpRange.y - damageImpRange.x)) * maxDamage;
                    }
                    //adjust for distance and fall off and impulse and impulse range
                    vertices[i] += m.transform.InverseTransformDirection(normal) * 
                                   (damageRadius - dist) * fallOffMul *
                                   Mathf.Clamp01((other.impulse.magnitude - damageImpRange.x) /
                                                 (damageImpRange.y - damageImpRange.x));
                }
                        
            }
            m.mesh.vertices = vertices;
            //meshCollider.sharedMesh = m.mesh;
            m.mesh.RecalculateNormals();
            m.mesh.RecalculateBounds();
        }
    }
}