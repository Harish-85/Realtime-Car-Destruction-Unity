using System;
using Unity.VisualScripting;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable]
    public struct CarPart
    {
        public MeshFilter meshFilter;
        public float health;
        public bool isBreakable;
        
    }
    
    public class Deformation : MonoBehaviour
    {
        public float deformRadius = .4f;
        public float maxDerform = .3f;
        public float damageFallOff = 1;
        public float minDamage = 1;
        public float damageMultiplier = 1;
        public float maxDamage = 100;
        
        public CarPart[] meshList;
        

        private void OnCollisionEnter(Collision other)
        {
            print("Coll");
            float collisionForce = other.impulse.magnitude;

            if (collisionForce > minDamage)
            {
                Vector3 relativeVelocity = other.relativeVelocity;
                
                Vector3 colPointToCenter = transform.position - other.contacts[0].point;
                //see if teh collision is in the front of the car or sideways
                float colStrength = relativeVelocity.magnitude * Vector3.Dot(colPointToCenter.normalized, other.contacts[0].normal);
                

                for (int i = 0; i < meshList.Length; i++)
                {
                    Vector3[] vertices = meshList[i].meshFilter.mesh.vertices;
                    
                    for(int j=0;j<vertices.Length;j++)
                    {
                        
                        //get the vertex in world space
                        Vector3 pointColl = transform.InverseTransformPoint(other.contacts[0].point);
                        float distance = Vector3.Distance(vertices[j], pointColl);
                        if (distance < deformRadius)
                        {
                            print("vertex moved");
                            float fallOff = 1 - (distance / deformRadius) *damageFallOff;
                            
                            
                            
                            Vector3 deform = new Vector3(
                                Mathf.Clamp(pointColl.x * fallOff,0,maxDerform),
                                Mathf.Clamp(pointColl.y * fallOff,0,maxDerform),
                                Mathf.Clamp(pointColl.z * fallOff,0,maxDerform)
                                );

                            vertices[j] -= deform * damageMultiplier ;
                        }

                        
                    }
                    meshList[i].meshFilter.mesh.vertices = vertices;
                    
                }
                
            }
        }
    }
}