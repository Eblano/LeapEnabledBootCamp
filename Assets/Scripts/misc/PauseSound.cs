using UnityEngine;
using System.Collections;

[System.Serializable]
public class PauseSound : MonoBehaviour
{
    private bool _paused;
    private AudioSource[] _audioSources;
    public bool ZeroVolume;
    private float[] _currentVolume;
    public virtual void Start()
    {
        this._paused = false;
        Component[] c = this.gameObject.GetComponents(typeof(AudioSource)) as Component[];
        if ((c == null) || (c.Length <= 0))
        {
            if (this.GetComponent<AudioSource>() != null)
            {
                this._audioSources = new AudioSource[1];
                this._currentVolume = new float[1];
                this._audioSources[0] = this.GetComponent<AudioSource>();
            }
            else
            {
                UnityEngine.Object.Destroy(this);
            }
        }
        else
        {
            this._audioSources = new AudioSource[c.Length];
            this._currentVolume = new float[c.Length];
            int i = 0;
            while (i < c.Length)
            {
                if (c[i] == null)
                {
                    goto Label_for_57;
                }
                this._audioSources[i] = c[i] as AudioSource;
                this._currentVolume[i] = this._audioSources[i].volume;
                Label_for_57:
                i++;
            }
        }
    }

    public virtual void Update()
    {
        int i = 0;
        if (GameManager.pause)
        {
            if (!this._paused)
            {
                this._paused = true;
                i = 0;
                while (i < this._audioSources.Length)
                {
                    if (this._audioSources[i] == null)
                    {
                        goto Label_for_58;
                    }
                    if (!this.ZeroVolume)
                    {
                        this._audioSources[i].Pause();
                    }
                    else
                    {
                        this._audioSources[i].volume = 0f;
                    }
                    Label_for_58:
                    i++;
                }
            }
        }
        else
        {
            if (this._paused)
            {
                this._paused = false;
                i = 0;
                while (i < this._audioSources.Length)
                {
                    if (this._audioSources[i] == null)
                    {
                        goto Label_for_59;
                    }
                    if (!this.ZeroVolume)
                    {
                        this._audioSources[i].Play();
                    }
                    else
                    {
                        this._audioSources[i].volume = this._currentVolume[i];
                    }
                    Label_for_59:
                    i++;
                }
            }
        }
    }

}