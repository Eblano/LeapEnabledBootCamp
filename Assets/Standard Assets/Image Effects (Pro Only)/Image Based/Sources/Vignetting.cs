using UnityEngine;
using System.Collections;

// NEEDED SHADERS
//  NOTE:
//  usually hidden in the inspector by proper editor scripts
[System.Serializable]
[UnityEngine.ExecuteInEditMode]
[UnityEngine.RequireComponent(typeof(Camera))]
[UnityEngine.AddComponentMenu("Image Effects/Vignette")]
public partial class Vignetting : PostEffectsBase
{
    // needed shaders & materials
    public Shader vignetteShader;
    private Material _vignetteMaterial;
    public Shader separableBlurShader;
    private Material _separableBlurMaterial;
    public Shader chromAberrationShader;
    private Material _chromAberrationMaterial;
    public override void Start()
    {
        this.CreateMaterials();
    }

    public virtual void CreateMaterials()
    {
        if (!this._vignetteMaterial)
        {
            if (!this.CheckShader(this.vignetteShader))
            {
                this.enabled = false;
                return;
            }
            this._vignetteMaterial = new Material(this.vignetteShader);
            this._vignetteMaterial.hideFlags = HideFlags.HideAndDontSave;
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
        if (!this._chromAberrationMaterial)
        {
            if (!this.CheckShader(this.chromAberrationShader))
            {
                this.enabled = false;
                return;
            }
            this._chromAberrationMaterial = new Material(this.chromAberrationShader);
            this._chromAberrationMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    public virtual void OnEnable()
    {
    }

    public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
         // needed for most of the new and improved image FX
        this.CreateMaterials();
        // get render targets	
        RenderTexture color = RenderTexture.GetTemporary(source.width, source.height, 0);
        RenderTexture halfRezColor = RenderTexture.GetTemporary((int) (source.width / 2f), (int) (source.height / 2f), 0);
        RenderTexture quarterRezColor = RenderTexture.GetTemporary((int) (source.width / 4f), (int) (source.height / 4f), 0);
        RenderTexture secondQuarterRezColor = RenderTexture.GetTemporary((int) (source.width / 4f), (int) (source.height / 4f), 0);
        // do the downsample and blur
        Graphics.Blit(source, halfRezColor);
        Graphics.Blit(halfRezColor, quarterRezColor);
        // blur the result to get a nicer bloom radius
        int it = 0;
        while (it < 2)
        {
            this._separableBlurMaterial.SetVector("offsets", new Vector4(0f, (1.5f * 1f) / quarterRezColor.height, 0f, 0f));
            Graphics.Blit(quarterRezColor, secondQuarterRezColor, this._separableBlurMaterial);
            this._separableBlurMaterial.SetVector("offsets", new Vector4((1.5f * 1f) / quarterRezColor.width, 0f, 0f, 0f));
            Graphics.Blit(secondQuarterRezColor, quarterRezColor, this._separableBlurMaterial);
            it++;
        }
        this._vignetteMaterial.SetFloat("vignetteIntensity", this.vignetteIntensity);
        this._vignetteMaterial.SetFloat("blurVignette", this.blurVignette);
        this._vignetteMaterial.SetTexture("_VignetteTex", quarterRezColor);
        Graphics.Blit(source, color, this._vignetteMaterial);
        this._chromAberrationMaterial.SetFloat("chromaticAberrationIntensity", this.chromaticAberrationIntensity);
        Graphics.Blit(color, destination, this._chromAberrationMaterial);
        RenderTexture.ReleaseTemporary(color);
        RenderTexture.ReleaseTemporary(halfRezColor);
        RenderTexture.ReleaseTemporary(quarterRezColor);
        RenderTexture.ReleaseTemporary(secondQuarterRezColor);
    }

    public float vignetteIntensity;
    public float chromaticAberrationIntensity;
    public float blurVignette;
    public Vignetting()
    {
        this.vignetteIntensity = 0.375f;
    }

}