using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.ExecuteInEditMode]
[UnityEngine.AddComponentMenu("Image Effects/Fisheye")]
public partial class Fisheye : PostEffectsBase
{
    public Shader fishEyeShader;
    private Material _fisheyeMaterial;
    public virtual void CreateMaterials()
    {
        if (!this._fisheyeMaterial)
        {
            if (!this.CheckShader(this.fishEyeShader))
            {
                this.enabled = false;
                return;
            }
            this._fisheyeMaterial = new Material(this.fishEyeShader);
            this._fisheyeMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    public override void Start()
    {
        this.CreateMaterials();
    }

    public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        this.CreateMaterials();
        float ar = (source.width * 1f) / (source.height * 1f);
        this._fisheyeMaterial.SetVector("intensity", new Vector4(this.strengthX * ar, this.strengthY * ar, this.strengthX * ar, this.strengthY * ar));
        Graphics.Blit(source, destination, this._fisheyeMaterial);
    }

    public float strengthX;
    public float strengthY;
    public Fisheye()
    {
        this.strengthX = 0.05f;
        this.strengthY = 0.05f;
    }

}