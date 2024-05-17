using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDeformer : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    // Start is called before the first frame update
    void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        // check all the vertex and move the vertices to stayout of the collision object
        Vector3[] vertices = _meshFilter.mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = transform.TransformPoint(vertices[i]);
            if (other.GetComponent<Collider>().bounds.Contains(vertex))
            {
               //get closest point on the surface of the collider
               Vector3 closestPoint = other.GetComponent<Collider>().ClosestPoint(vertex);
               vertices[i] = transform.InverseTransformPoint(closestPoint);
               
            }
        }
        _meshFilter.mesh.vertices = vertices;
    }
}
