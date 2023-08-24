using UnityEngine;
using System.Collections;

[System.Serializable]
public class FadeLight : MonoBehaviour
{
    public float delay;
    public float fadeTime;
    private float fadeSpeed;
    private float intensity;
    private Color color;
    public virtual void Start()//alpha = 1.0;
    {
        if (this.GetComponent<Light>() == null)
        {
            UnityEngine.Object.Destroy(this);
            return;
        }
        this.intensity = this.GetComponent<Light>().intensity;
        this.fadeTime = Mathf.Abs(this.fadeTime);
        if (this.fadeTime > 0f)
        {
            this.fadeSpeed = this.intensity / this.fadeTime;
        }
        else
        {
            this.fadeSpeed = this.intensity;
        }
    }

    public virtual void Update()
    {
        if (this.delay > 0f)
        {
            this.delay = this.delay - Time.deltaTime;
        }
        else
        {
            if (this.intensity > 0f)
            {
                this.intensity = this.intensity - (this.fadeSpeed * Time.deltaTime);
                this.GetComponent<Light>().intensity = this.intensity;
            }
        }
    }

}