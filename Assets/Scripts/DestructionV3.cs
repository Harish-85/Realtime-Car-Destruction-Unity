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
    
    [SerializeField] private List<AudioSource> impactSounds;
    
    List<List<Vector3>> verticesList = new List<List<Vector3>>();
    
    void Start()
    {
        verticesList = new List<List<Vector3>>();
        int i = 0;
        foreach (var m in meshFilters)
        {
            verticesList.Add(new List<Vector3>());
            m.mesh.GetVertices(verticesList[i]);
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision other)
    {
        foreach (var contacts in other.contacts)
        {
            

            if (other.impulse.magnitude > damageImpRange.x)
            {
                DeformMesh(other, contacts, contacts.normal);
            }

            
        }
    }

    private void DeformMesh(Collision other, ContactPoint contacts, Vector3 normal)
    {
        var audioSource = impactSounds[UnityEngine.Random.Range(0, impactSounds.Count)];
        audioSource.pitch = UnityEngine.Random.Range(.8f, 1.2f);
        audioSource.volume = Mathf.Clamp01((other.impulse.magnitude - damageImpRange.x) /
                                           (damageImpRange.y - damageImpRange.x)) + .3f;
        audioSource.Play();
        
        
        for(int m = 0; m < meshFilters.Count; m++)
        {
           bool didDamage = false;

           List<Vector3> vertices = verticesList[m];
           
            for (int i = 0; i < vertices.Count; i++)
            {
                Vector3 worldPoint = meshFilters[m].transform.TransformPoint(vertices[i]);
                //Debug.DrawRay(worldPoint, Vector3.up, Color.green, 10f);
                float dist = Vector3.Distance(worldPoint, contacts.point);
                //Debug.Log(dist);
                //move the vertices within range in the direction of the normal 
                if (dist < damageRadius)
                {
                    
                    if (!didDamage && meshFilters[m].TryGetComponent(out BreakableCarPart br ))
                    {
                        didDamage = true;
                        br.health -=
                            Mathf.Clamp01(other.impulse.magnitude - damageImpRange.x / (damageImpRange.y - damageImpRange.x)) * maxDamage;
                    }
                    //adjust for distance and fall off and impulse and impulse range
                    vertices[i] += meshFilters[m].transform.InverseTransformDirection(normal) * 
                                   (damageRadius - dist) * fallOffMul *
                                   Mathf.Clamp01((other.impulse.magnitude - damageImpRange.x) /
                                                 (damageImpRange.y - damageImpRange.x));
                }
                        
            }
            meshFilters[m].mesh.SetVertices(vertices);
            //meshCollider.sharedMesh = m.mesh;
            meshFilters[m].mesh.RecalculateNormals();
            meshFilters[m].mesh.RecalculateBounds();
        }
    }
}