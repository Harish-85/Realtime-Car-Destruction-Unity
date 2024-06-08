using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class DestructionV3Job : MonoBehaviour
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
           NativeArray<int> didDamage = new NativeArray<int>(1, Allocator.TempJob);
           
               NativeArray<Vector3> v = new NativeArray<Vector3>(verticesList[m].ToArray(), Allocator.TempJob);
               
                DeformJob job = new DeformJob()
                {
                    impulse = other.impulse.magnitude,
                    contacts = contacts,
                    normal = meshFilters[m].transform.InverseTransformDirection(normal),
                    vertices = v.Reinterpret<float3>(),
                    damageRadius = damageRadius,
                    localToWorldMatrix = meshFilters[m].transform.localToWorldMatrix,
                    didDamage = didDamage,
                    damageImpRange = damageImpRange,
                    fallOffMul = fallOffMul
                };
                JobHandle jobHandle= job.Schedule(verticesList[m].Count,64);
                jobHandle.Complete();
                job.vertices.Reinterpret<Vector3>().CopyTo(v);
                

                if (job.didDamage[0] == 1)
                {
                    if (meshFilters[m].TryGetComponent(out BreakableCarPart br))
                    {
                        
                        br.health -=
                            Mathf.Clamp01(other.impulse.magnitude - damageImpRange.x / (damageImpRange.y - damageImpRange.x)) *
                            maxDamage;
                    }
                }

                verticesList[m] = new List<Vector3>(v.ToArray());
            meshFilters[m].mesh.SetVertices(v);
            //meshCollider.sharedMesh = m.mesh;
            meshFilters[m].mesh.RecalculateNormals();
            meshFilters[m].mesh.RecalculateBounds();
              
        }
    }


}

[BurstCompile]
struct DeformJob : IJobParallelFor
{
    public float impulse;
    public ContactPoint contacts;
    public float3 normal;
    public NativeArray<float3> vertices;
    public float damageRadius;
    public Matrix4x4 localToWorldMatrix;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> didDamage;
    public float2 damageImpRange;
    public float fallOffMul;
    
    public void Execute(int index)
    {
        //Vector3 worldPoint = meshFilters[m].transform.TransformPoint(vertices[i]);
        Vector3 worldPoint = localToWorldMatrix.MultiplyPoint(vertices[index]);
        //Debug.DrawRay(worldPoint, Vector3.up, Color.green, 10f);
        float dist = Vector3.Distance(worldPoint, contacts.point);
        //Debug.Log(dist);
        //move the vertices within range in the direction of the normal 
        if (dist < damageRadius)
        {
            didDamage[0] = 1;
            //adjust for distance and fall off and impulse and impulse range
            vertices[index] += (normal) * 
                           (damageRadius - dist) * fallOffMul *
                           Mathf.Clamp01((impulse - damageImpRange.x) /
                                         (damageImpRange.y - damageImpRange.x));
        }

    }
}