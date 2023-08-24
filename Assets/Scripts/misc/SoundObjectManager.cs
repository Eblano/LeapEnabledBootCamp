using UnityEngine;
using System.Collections;

[System.Serializable]
public class SoundObjectManager : MonoBehaviour
{
    public float minSpeedToParticle;
    public GameObject genericParticle;
    public GameObject waterParticles;
    public LayerMask waterLayer;
    public float minSpeedSound;
    public float maxSpeedSound;
    public AudioClip defaultSound;
    public AudioClip defaultMetalSound;
    public AudioClip defaultWoodSound;
    public AudioClip defaultConcreteSound;
    public AudioClip[] additionalSounds;
    public static SoundObjectManager instance;
    public virtual void Awake()
    {
        SoundObjectManager.instance = this;
    }

    public SoundObjectManager()
    {
        this.minSpeedToParticle = 3f;
        this.minSpeedSound = 2f;
        this.maxSpeedSound = 10f;
    }

}