using UnityEngine;
using System.Collections;

[System.Serializable]
public class MainMenuEffects : MonoBehaviour
{
    public ParticleSystem customParticleSystem;
    public virtual void Start()
    {
        if (this.customParticleSystem != null)
        {
            this.customParticleSystem.Simulate(10);
        }
    }

}