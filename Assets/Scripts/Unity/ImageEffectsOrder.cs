using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.RequireComponent(typeof(Camera))]
[UnityEngine.ExecuteInEditMode]
public partial class ImageEffectsOrder : MonoBehaviour
{
    private RenderTexture[] _tex;
    public virtual void OnEnable()
    {
        if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
        {
            this.enabled = false;
        }
    }

    public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        this._tex[0] = source;
        this._tex[1] = RenderTexture.GetTemporary(source.width, source.height);
        RenderTexture releaseMe = this._tex[1];
        int index = 0;
        object[] sorted = new object[0];
        int i = 0;
        foreach (PostEffectsBase fx in this.GetComponents(typeof(PostEffectsBase)))
        {
            if (fx && fx.enabled)
            {
                sorted[i++] = fx;
            }
        }
        while (sorted.Length != 0)
        {
            int indexToUse = 0;
            int orderValue = -1;
            i = 0;
            while (i < sorted.Length)
            {
                if (sorted[i].order > orderValue)
                {
                    orderValue = (int) sorted[i].order;
                    indexToUse = i;
                }
                i++;
            }
            PostEffectsBase effect = (PostEffectsBase) sorted[indexToUse];
            if (effect.PreferRenderImage3())
            {
                effect.OnRenderImage3(this._tex[index], this._tex[1 - index]);
            }
            else
            {
                effect.OnRenderImage2(this._tex[index], this._tex[1 - index]);
                index = 1 - index;
            }
            sorted.RemoveAt(indexToUse);
        }
        Graphics.Blit(this._tex[index], destination);
        RenderTexture.ReleaseTemporary(releaseMe);
    }

    public ImageEffectsOrder()
    {
        this._tex = new RenderTexture[2];
    }

}