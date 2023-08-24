using UnityEngine;
using System.Collections;

[System.Serializable]
public class CutsceneBirds : MonoBehaviour
{
    public AudioClip[] sounds;
    public float soundFrequency;
    public float animationSpeed;
    public float minSpeed;
    public float turnSpeed;
    public float randomFreq;
    public float randomForce;
    public float toOriginForce;
    public float toOriginRange;
    public float damping;
    public float gravity;
    public float avoidanceRadius;
    public float avoidanceForce;
    public float followVelocity;
    public float followRadius;
    public float bankTurn;
    public bool raycast;
    public float bounce;
    public CutsceneBirds()
    {
        this.sounds = new AudioClip[0];
        this.soundFrequency = 1f;
        this.animationSpeed = 1f;
        this.bounce = 0.8f;
    }

}