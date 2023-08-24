using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.ExecuteInEditMode]
[UnityEngine.RequireComponent(typeof(Camera))]
[UnityEngine.AddComponentMenu("Image Effects/Crease")]
public partial class Crease : PostEffectsBase
{
    public Shader blurShader;
    private Material _blurMaterial;
    public Shader depthFetchShader;
    private Material _depthFetchMaterial;
    public Shader creaseApplyShader;
    private Material _creaseApplyMaterial;
    public virtual void CreateMaterials()
    {
        if (!this._blurMaterial)
        {
            if (!this.CheckShader(this.blurShader))
            {
                this.enabled = false;
                return;
            }
            this._blurMaterial = new Material(this.blurShader);
            this._blurMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._depthFetchMaterial)
        {
            if (!this.CheckShader(this.depthFetchShader))
            {
                this.enabled = false;
                return;
            }
            this._depthFetchMaterial = new Material(this.depthFetchShader);
            this._depthFetchMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._creaseApplyMaterial)
        {
            if (!this.CheckShader(this.creaseApplyShader))
            {
                this.enabled = false;
                return;
            }
            this._creaseApplyMaterial = new Material(this.creaseApplyShader);
            this._creaseApplyMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
        {
            this.enabled = false;
            return;
        }
    }

    public override void Start()
    {
        this.CreateMaterials();
    }

    public virtual void OnEnable()
    {
        this.GetComponent<Camera>().depthTextureMode = this.GetComponent<Camera>().depthTextureMode | DepthTextureMode.Depth;
    }

    public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        this.CreateMaterials();
        RenderTexture hrTex = RenderTexture.GetTemporary(source.width, source.height, 0);
        RenderTexture lrTex1 = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0);
        RenderTexture lrTex2 = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0);
        Graphics.Blit(source, hrTex, this._depthFetchMaterial);
        Graphics.Blit(hrTex, lrTex1);
        int i = 0;
        while (i < this.softness)
        {
            this._blurMaterial.SetVector("offsets", new Vector4(0f, this.spread / lrTex1.height, 0f, 0f));
            Graphics.Blit(lrTex1, lrTex2, this._blurMaterial);
            this._blurMaterial.SetVector("offsets", new Vector4(this.spread / lrTex1.width, 0f, 0f, 0f));
            Graphics.Blit(lrTex2, lrTex1, this._blurMaterial);
            i++;
        }
        this._creaseApplyMaterial.SetTexture("_HrDepthTex", hrTex);
        this._creaseApplyMaterial.SetTexture("_LrDepthTex", lrTex1);
        this._creaseApplyMaterial.SetFloat("intensity", this.intensity);
        Graphics.Blit(source, destination, this._creaseApplyMaterial);
        RenderTexture.ReleaseTemporary(hrTex);
        RenderTexture.ReleaseTemporary(lrTex1);
        RenderTexture.ReleaseTemporary(lrTex2);
    }

    public float intensity;
    public int softness;
    public float spread;
    public Crease()
    {
        this.intensity = 0.5f;
        this.softness = 1;
        this.spread = 1f;
    }

}