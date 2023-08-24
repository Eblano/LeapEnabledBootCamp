using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.ExecuteInEditMode]
[UnityEngine.RequireComponent(typeof(Camera))]
[UnityEngine.AddComponentMenu("Image Effects/Contrast Enhance (Unsharp Mask)")]
public partial class ContrastEnhance : PostEffectsBase
{
    public virtual void CreateMaterials()
    {
        if (!this._contrastCompositeMaterial)
        {
            if (!this.CheckShader(this.contrastCompositeShader))
            {
                this.enabled = false;
                return;
            }
            this._contrastCompositeMaterial = new Material(this.contrastCompositeShader);
            this._contrastCompositeMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._separableBlurMaterial)
        {
            if (!this.CheckShader(this.separableBlurShader))
            {
                this.enabled = false;
                return;
            }
            this._separableBlurMaterial = new Material(this.separableBlurShader);
            this._separableBlurMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    public override void Start()
    {
        this.CreateMaterials();
    }

    public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        this.CreateMaterials();
        // get render targets
        RenderTexture halfRezColor = RenderTexture.GetTemporary((int) (source.width / 2f), (int) (source.height / 2f), 0);
        RenderTexture quarterRezColor = RenderTexture.GetTemporary((int) (source.width / 4f), (int) (source.height / 4f), 0);
        RenderTexture secondQuarterRezColor = RenderTexture.GetTemporary((int) (source.width / 4f), (int) (source.height / 4f), 0);
        // do the downsample and stuff
        Graphics.Blit(source, halfRezColor);
        Graphics.Blit(halfRezColor, quarterRezColor);
        // blurring
        this._separableBlurMaterial.SetVector("offsets", new Vector4(0f, (this.sepBlurSpread * 1f) / quarterRezColor.height, 0f, 0f));
        Graphics.Blit(quarterRezColor, secondQuarterRezColor, this._separableBlurMaterial);
        this._separableBlurMaterial.SetVector("offsets", new Vector4((this.sepBlurSpread * 1f) / quarterRezColor.width, 0f, 0f, 0f));
        Graphics.Blit(secondQuarterRezColor, quarterRezColor, this._separableBlurMaterial);
        // comp
        this._contrastCompositeMaterial.SetTexture("_MainTexBlurred", quarterRezColor);
        this._contrastCompositeMaterial.SetFloat("intensity", this.intensity);
        this._contrastCompositeMaterial.SetFloat("threshhold", this.threshhold);
        Graphics.Blit(source, destination, this._contrastCompositeMaterial);
        RenderTexture.ReleaseTemporary(halfRezColor);
        RenderTexture.ReleaseTemporary(quarterRezColor);
        RenderTexture.ReleaseTemporary(secondQuarterRezColor);
    }

    public float intensity;
    public float threshhold;
    private Material _separableBlurMaterial;
    private Material _contrastCompositeMaterial;
    public float sepBlurSpread;
    public Shader separableBlurShader;
    public Shader contrastCompositeShader;
    public ContrastEnhance()
    {
        this.intensity = 0.5f;
        this.sepBlurSpread = 1f;
    }

}