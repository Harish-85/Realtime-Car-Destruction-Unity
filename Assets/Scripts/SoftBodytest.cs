using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftBodytest : MonoBehaviour
{
    public MeshFilter softBodyMesh;
    public MeshFilter actualMesh;
    
    struct SoftBodyVertex
    {
        public Vector3 position;
        public Vector3 velocity;
        public float drag;
        public SoftBodyVertex[] neighbours;
        public List<int> vertices;
        public List<float> vertWeights;
        public float stiffness;
    }
    
    private SoftBodyVertex[] vertices;
    
    // Start is called before the first frame update
    void Start()
    {
        vertices = new SoftBodyVertex[softBodyMesh.mesh.vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 pos = softBodyMesh.mesh.vertices[i];
            //apply rotation
            Vector3 fixedLocal = softBodyMesh.transform.rotation * pos;
            
            vertices[i].position = fixedLocal;
            vertices[i].velocity = Vector3.zero;
            vertices[i].drag = .5f;
            vertices[i].stiffness = 5f;
        }
        //loop through all triangles and create the neighbours
        for (int i = 0; i < softBodyMesh.mesh.triangles.Length; i+=3)
        {
            int a = softBodyMesh.mesh.triangles[i];
            int b = softBodyMesh.mesh.triangles[i + 1];
            int c = softBodyMesh.mesh.triangles[i + 2];
            
            vertices[a].neighbours = new SoftBodyVertex[]{vertices[b], vertices[c]};
            vertices[b].neighbours = new SoftBodyVertex[]{vertices[a], vertices[c]};
            vertices[c].neighbours = new SoftBodyVertex[]{vertices[a], vertices[b]};
        }
        //get all nearby vertices depending on the distance
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].vertices = new List<int>();
            vertices[i].vertWeights = new List<float>();
            for (int j = 0; j < vertices.Length; j++)
            {
                if (i != j)
                {
                    Vector3 worldVert = transform.position + vertices[i].position;
                    float dist = Vector3.Distance(worldVert, vertices[j].position);
                    if (dist < 1f)
                    {
                        vertices[i].vertices.Add(j);
                        vertices[i].vertWeights.Add(1-dist);
                    }
                }
            }
        }
       
    }

    public void PrevPointVelocity()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].velocity = GetComponent<Rigidbody>().velocity;
        }
    }
    
    //this will be called after collsition for 2 seconds
    public void UpdateSoftBodyForce()
    {
        
    }
    public void UpdateVertices()
    {
        //update the vertices depending on the stiffness
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 force = Vector3.zero;
            for (int j = 0; j < vertices[i].vertices.Count; j++)
            {
                force += (vertices[vertices[i].vertices[j]].position - vertices[i].position) * vertices[i].vertWeights[j];
            }
            force *= vertices[i].stiffness;
            vertices[i].velocity += force;
            vertices[i].velocity *= vertices[i].drag;
            vertices[i].position += vertices[i].velocity * Time.fixedDeltaTime;
            //update actual mesh
            for (int j = 0; j < vertices[i].vertices.Count; j++)
            {
                actualMesh.mesh.vertices[vertices[i].vertices[j]] = vertices[i].position ;
            }
        }
        
        
        
    }
    bool isDeforming = false;
    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (isDeforming)
        {
            UpdateVertices();
        }
        else
        {
            PrevPointVelocity();
        }
    }

    private void OnDrawGizmos()
    {
        if(vertices == null) return;
        
        Gizmos.color = Color.red;
        for (int i = 0; i < vertices.Length; i++)
        {
            //Gizmos.DrawSphere(vertices[i].position, .1f);
            if (vertices[i].neighbours != null)
            {
                for (int j = 0; j < vertices[i].neighbours.Length; j++)
                {
                    Gizmos.DrawLine(vertices[i].position, vertices[i].neighbours[j].position);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
           
        isDeforming = true;
        
    }
}
