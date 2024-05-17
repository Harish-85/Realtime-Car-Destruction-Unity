using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBreakable : BreakableCarPart
{
    public ParticleSystem glassParticle;
    //    // Start is called before the first frame update
    void Start()
    {
        
    }

  
    public override void OnBreak()
    {
        glassParticle.Play();
        GetComponent<MeshRenderer>().enabled = false;
    }
}
