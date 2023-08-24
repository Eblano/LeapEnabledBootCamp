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
    public virtual void Start()
    {
        this.cc = (CharacterController) this.GetComponent(typeof(CharacterController));
        this.t = this.transform;
    }

    public virtual void OnFootStrike()
    {
        if (Time.time < 0.5f)
        {
            return;
        }
        if (this.cc != null)
        {
            volume = Mathf.Clamp01(0.1f + (this.cc.velocity.magnitude * 0.3f));
        }
        else
        {
            volume = 1;
        }
        this.footAudioSource.PlayOneShot(this.GetAudio(), volume);
    }

    public virtual AudioClip GetAudio()
    {
        RaycastHit hit = default(RaycastHit);
        //Debug.DrawRay(t.position + new Vector3(0, 0.5, 0), -Vector3.up * 5.0);
        if (Physics.Raycast(this.t.position + new Vector3(0, 0.5f, 0), -Vector3.up, out hit, Mathf.Infinity, (int) this.hitLayer))
        {
            this.cTag = hit.collider.tag.ToLower();
        }
        if (this.cTag == "wood")
        {
            return this.woodSteps[Random.Range(0, this.woodSteps.Length)];
        }
        else
        {
            if (this.cTag == "metal")
            {
                return this.metalSteps[Random.Range(0, this.metalSteps.Length)];
            }
            else
            {
                if (this.cTag == "concrete")
                {
                    volume = 0.8f;
                    return this.concreteSteps[Random.Range(0, this.concreteSteps.Length)];
                }
                else
                {
                    if (this.cTag == "dirt")
                    {
                        volume = 1f;
                        return this.sandSteps[Random.Range(0, this.sandSteps.Length)];
                    }
                    else
                    {
                        if (this.cTag == "sand")
                        {
                            volume = 1f;
                            return this.sandSteps[Random.Range(0, this.sandSteps.Length)];
                        }
                        else
                        {
                            volume = 1f;
                            return this.sandSteps[Random.Range(0, this.sandSteps.Length)];
                        }
                    }
                }
            }
        }
    }

}