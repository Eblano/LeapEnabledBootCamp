using UnityEngine;
using System.Collections;

[System.Serializable]
public class SyncAudioListener : MonoBehaviour
{
    private AudioListener listener;
    public virtual void Start()
    {
        this.listener = this.gameObject.GetComponent("AudioListener") as AudioListener;
        if (this.listener == null)
        {
            this.enabled = false;
        }
        this.listener.enabled = false;
    }

    public virtual void Update()
    {
        this.listener.enabled = this.GetComponent<Camera>().enabled;
    }

}