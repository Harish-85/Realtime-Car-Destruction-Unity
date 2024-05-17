using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCollDeformation : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private MeshFilter[] _meshFilters;
    [SerializeField] private float resolveForce = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Collision");
        foreach(var  _meshFilter in _meshFilters)
        {
            
            Vector3[] vertices = _meshFilter.mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vertex = transform.TransformPoint(vertices[i]);
                if (other.GetComponent<Collider>().bounds.Contains(vertex))
                {
                    
                    //get closest point on the surface of the collider
                    Vector3 closestPoint =transform.InverseTransformPoint(other.GetComponent<Collider>().ClosestPoint(vertex));
                    Debug.DrawLine(vertex, closestPoint, Color.green, 10f);
                    vertices[i] = transform.InverseTransformPoint(closestPoint);
                    //add a force to resolve the collision
                    _rigidbody
                        .AddForceAtPosition((vertex - closestPoint).normalized *resolveForce, vertex);
                    if(other.TryGetComponent(out Rigidbody r))
                        r.AddForceAtPosition((closestPoint - vertex).normalized * resolveForce, closestPoint);
                }
            }

            _meshFilter.mesh.vertices = vertices;
        }
    }
}
