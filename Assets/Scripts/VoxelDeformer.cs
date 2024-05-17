using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteAlways]
public class VoxelDeformer : MonoBehaviour
{
    [SerializeField] private Vector3 boxCenter = Vector3.zero;
    [SerializeField] private Vector3 boxSize = new Vector3(10,10,10);
    [SerializeField] private float voxelSize = 1f;
    [SerializeField] private Transform collisionObject;
    
    public bool InitializeVerteices = true;
    private MeshFilter _meshFilter;
    [Serializable]
    struct Voxel
    {
        public Bounds bounds;
        public List<Vector3> vertices;
        public BoxCollider boxCollider;
    }
    
    private Voxel[,,] _voxelVertices;
    
    // Start is called before the first frame update
    void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    void InitializeVoxelVertices()
    {
        _voxelVertices = new Voxel[(int)(boxSize.x / voxelSize), (int)(boxSize.y / voxelSize), (int)(boxSize.z / voxelSize)];
        
        
        
        //create voxel grid
        for (int i = 0; i < _voxelVertices.GetLength(0); i++)
        {
            for (int j = 0; j < _voxelVertices.GetLength(1); j++)
            {
                for (int k = 0; k < _voxelVertices.GetLength(2); k++)
                {
                    _voxelVertices[i, j, k].bounds = new Bounds(new Vector3(i * voxelSize, j * voxelSize, k * voxelSize) + boxCenter, Vector3.one * voxelSize);
                    //get all the vertices in the voxel
                    _voxelVertices[i, j, k].vertices = new List<Vector3>();
                    for (int l = 0; l < _meshFilter.mesh.vertices.Length; l++)
                    {
                        if (_voxelVertices[i, j, k].bounds.Contains(_meshFilter.mesh.vertices[l]))
                        {
                            _voxelVertices[i, j, k].vertices.Add(_meshFilter.mesh.vertices[l]);
                        }
                    }
                  
                }
            }
        }
        
        
        
    }
    
    void InitializeBoxCollider()
    {
        //destroy all the box colliders
        foreach (var boxCollider in collisionObject.GetComponents<Collider>())
        {
            DestroyImmediate(boxCollider);
        }
        
        
        //Initalize a box collider for each voxel
        for (int i = 0; i < _voxelVertices.GetLength(0); i++)
        {
            for (int j = 0; j < _voxelVertices.GetLength(1); j++)
            {
                for (int k = 0; k < _voxelVertices.GetLength(2); k++)
                {
                    if (_voxelVertices[i, j, k].vertices.Count > 0)
                    {
                        //add a box collider to the collision object
                        //BoxCollider boxCollider = collisionObject.gameObject.AddComponent<BoxCollider>();
                        //boxCollider.center = _voxelVertices[i, j, k].bounds.center;
                        //boxCollider.size = _voxelVertices[i, j, k].bounds.size;
                        //_voxelVertices[i, j, k].boxCollider = boxCollider;
                        //Add a sphere collider to the collision object
                        SphereCollider sphereCollider = collisionObject.gameObject.AddComponent<SphereCollider>();
                        sphereCollider.center = _voxelVertices[i, j, k].bounds.center;
                        sphereCollider.radius = voxelSize / 4;
                    }
                }
            }
        }
        
        
    }

    public void ColliderFloodFill(Vector3Int voxel)
    {
        //get neighbors without a collider
        List<Voxel> neighbors = new List<Voxel>();
        neighbors.Add(_voxelVertices[voxel.x + 1, voxel.y, voxel.z]);
        neighbors.Add(_voxelVertices[voxel.x - 1, voxel.y, voxel.z]);
        neighbors.Add(_voxelVertices[voxel.x, voxel.y + 1, voxel.z]);
        neighbors.Add(_voxelVertices[voxel.x, voxel.y - 1, voxel.z]);
        neighbors.Add(_voxelVertices[voxel.x, voxel.y, voxel.z + 1]);
        neighbors.Add(_voxelVertices[voxel.x, voxel.y, voxel.z - 1]);
        
        bool isSurrounded = true;
        //check if they have a box collider or not
        foreach (var n in neighbors)
        {
            if (n.boxCollider == null)
            {
                isSurrounded = false;
                break;
            }
        }
        
        
        
    }

    private void OnDrawGizmos()
    {
        if (InitializeVerteices || _voxelVertices == null)
        {
            InitializeVerteices = false;
            InitializeVoxelVertices();
            InitializeBoxCollider();
        }

        //draw voxel grid
       for (int i = 0; i < _voxelVertices.GetLength(0); i++)
       {
           for(int j = 0; j < _voxelVertices.GetLength(1); j++)
           {
               for (int k = 0; k < _voxelVertices.GetLength(2); k++)
               {
                   Random.InitState(i+j+k);
                   Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                     Gizmos.color = color;
                   Gizmos.DrawWireCube(_voxelVertices[i, j, k].bounds.center, _voxelVertices[i, j, k].bounds.size);
                   //foreach vertex in the voxel draw a sphere
                   foreach (var vertex in _voxelVertices[i, j, k].vertices)
                   {
                       Gizmos.DrawSphere(vertex, 0.01f);
                   }
               }
           }
       }
        
    }
}
