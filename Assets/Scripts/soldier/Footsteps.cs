using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Footsteps : MonoBehaviour
{
    public AudioSource footAudioSource;

    public AudioClip[] woodSteps;
    public AudioClip[] metalSteps;
    public AudioClip[] concreteSteps;
    public AudioClip[] sandSteps;

    private CharacterController cc;
    private Transform t;

    public LayerMask hitLayer;
    private string cTag;
    private float volume;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        t = transform;
    }

    public void OnFootStrike()
    {
        if (Time.time < 0.5f)
            return;

        if (cc != null)
        {
            volume = Mathf.Clamp01(0.1f + cc.velocity.magnitude * 0.3f);
        }
        else
        {
            volume = 1f;
        }

        footAudioSource.PlayOneShot(GetAudio(), volume);
    }

    private AudioClip GetAudio()
    {
        RaycastHit hit;

        if (Physics.Raycast(t.position + new Vector3(0, 0.5f, 0), -Vector3.up, out hit, Mathf.Infinity, hitLayer))
        {
            cTag = hit.collider.tag.ToLower();
        }

        if (cTag == "wood")
        {
            return woodSteps[Random.Range(0, woodSteps.Length)];
        }
        else if (cTag == "metal")
        {
            return metalSteps[Random.Range(0, metalSteps.Length)];
        }
        else if (cTag == "concrete")
        {
            volume = 0.8f;
            return concreteSteps[Random.Range(0, concreteSteps.Length)];
        }
        else if (cTag == "dirt")
        {
            volume = 1.0f;
            return sandSteps[Random.Range(0, sandSteps.Length)];
        }
        else if (cTag == "sand")
        {
            volume = 1.0f;
            return sandSteps[Random.Range(0, sandSteps.Length)];
        }
        else
        {
            volume = 1.0f;
            return sandSteps[Random.Range(0, sandSteps.Length)];
        }
    }
}