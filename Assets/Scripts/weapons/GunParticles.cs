using UnityEngine;
using System.Collections;

[System.Serializable]
public class GunParticles : MonoBehaviour
{
    private bool cState;
    private object[] emitters;
    public virtual void Start()
    {
        this.cState = true;
        this.emitters = this.GetComponentsInChildren(typeof(ParticleEmitter));
        this.ChangeState(false);
    }

    public virtual void ChangeState(bool p_newState)
    {
        if (this.cState == p_newState)
        {
            return;
        }
        this.cState = p_newState;
        if (!(this.emitters == null))
        {
            int i = 0;
            while (i < this.emitters.Length)
            {
                (this.emitters[i] as ParticleSystem).emission.enabled = p_newState;
                i++;
            }
        }
    }

}