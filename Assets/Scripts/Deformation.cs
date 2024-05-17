using System;
using System.Collections.Generic;
using UnityEngine;

public class Deformation : MonoBehaviour
{
    
    float minCollisionImpulse = 10000f;
    
    [SerializeField] private List<MeshFilter> meshFilters;
    [SerializeField] private float damageRadius;
    [SerializeField] private float randomizeVertices = 1f;
    [SerializeField] private float damageMultiplier = 1f;
    
    [SerializeField] private Transform middlePoint;
    List<Vector3[]> originalMeshVerts = new List<Vector3[]>();


    private void Start()
    {
        for (int i = 0; i < meshFilters.Count; i++)
        {
            originalMeshVerts.Add(meshFilters[i].mesh.vertices);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.impulse.magnitude > minCollisionImpulse)
        {
            DeformMesh(other, other.impulse.magnitude);
        }
    }

    private bool deforming = false;
    private bool deformed = false;
    [SerializeField] private float maximumDamage =.5f;

    
    List<Vector3[]> damagedMeshVerts = new List<Vector3[]>();
    private void DeformMesh(Collision coll, float impulseMagnitude)
    {
        
        
        
        damagedMeshVerts.Clear();
        for (int i = 0; i < meshFilters.Count; i++)
        { 
            Vector3[] vertices = meshFilters[i].mesh.vertices;
            foreach (var contactPoint in coll.contacts)
            {
                Vector3 collDir = contactPoint.point - middlePoint.position;
                //Vector3 collDir = -contactPoint.normal;
                collDir = -collDir.normalized;
                
                Vector3 point = meshFilters[i].transform.InverseTransformPoint(contactPoint.point);
                
                for (int j = 0; j < vertices.Length; j++)
                {
                    
                    //i need to adjust for the scalse and rotation of the mesh .
                    //THis caused me so much head ache
                    Vector3 scaledVert = Vector3.Scale(vertices[j], meshFilters[i].transform.localScale);
                    Vector3 vertWorldPos = meshFilters[i].transform.position + (meshFilters[i].transform.rotation * scaledVert);
                    
                    
                    if ((point - vertWorldPos).sqrMagnitude < damageRadius * damageRadius)
                    {
                        deforming = true;
                        deformed = false;
                        
                        Vector3 randomizedVector =  new Vector3(UnityEngine.Random.Range(-randomizeVertices, randomizeVertices), UnityEngine.Random.Range(-randomizeVertices, randomizeVertices), UnityEngine.Random.Range(-randomizeVertices, randomizeVertices));

                        if(randomizeVertices > 0)
                            collDir += randomizedVector/1000f;
                        vertices[j] += transform.InverseTransformDirection(collDir) * impulseMagnitude * damageMultiplier/50f;
                        
                        if (maximumDamage > 0 && ((vertices[j] - originalMeshVerts[i][j]).magnitude) > maximumDamage)
                            vertices[j] = originalMeshVerts[i][j] + (vertices[j] - originalMeshVerts[i][j]).normalized * (maximumDamage);
                        
                    }
                }
                
            }
            damagedMeshVerts.Add(vertices);
            
        }
        Damage();

    }
    private void Damage()
    {
        for(int k = 0; k < meshFilters.Count; k++)
        {
            Vector3[] vertices = meshFilters[k].mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                    vertices[i] += (damagedMeshVerts[k][i] - vertices[i]);
            }
            meshFilters[k].mesh.vertices = vertices;
            meshFilters[k].mesh.RecalculateNormals();
            meshFilters[k].mesh.RecalculateBounds();
            meshFilters[k].mesh.Optimize();
            if (meshFilters[k].GetComponent<MeshCollider>() != null)
            {
                meshFilters[k].GetComponent<MeshCollider>().sharedMesh = meshFilters[k].mesh;
                
            }
        }

        
    }
}