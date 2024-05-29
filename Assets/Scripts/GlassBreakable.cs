using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBreakable : BreakableCarPart
{
    public ParticleSystem glassParticle;
   
    public override void OnBreak()
    {
        isBroken = true;
        glassParticle.Play();
        GetComponent<MeshRenderer>().enabled = false;
    }
}
