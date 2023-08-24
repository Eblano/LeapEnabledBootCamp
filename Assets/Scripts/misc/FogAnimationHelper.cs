using UnityEngine;
using System.Collections;

[System.Serializable]
public class FogAnimationHelper : MonoBehaviour
{
    public Color fogColor;
    public float fogDensity;
    public virtual void Start()
    {
        this.fogDensity = RenderSettings.fogDensity;
        this.fogColor = RenderSettings.fogColor;
    }

    public virtual void Update()
    {
        this.fogDensity = Mathf.Clamp(this.fogDensity, 0f, 1f);
        if (RenderSettings.fogDensity != this.fogDensity)
        {
            if (!RenderSettings.fog)
            {
                RenderSettings.fog = true;
            }
            RenderSettings.fogDensity = this.fogDensity;
        }
        if (RenderSettings.fogColor != this.fogColor)
        {
            if (!RenderSettings.fog)
            {
                RenderSettings.fog = true;
            }
            RenderSettings.fogColor = this.fogColor;
        }
    }

}