using UnityEngine;
using System.Collections;

// general settings
// bloom settings
// lens flare settings
public enum LensflareStyle
{
    Ghosting = 0,
    Hollywood = 1,
    Combined = 2
}

public enum TweakMode
{
    Simple = 0,
    Advanced = 1
}

[System.Serializable]
[UnityEngine.ExecuteInEditMode]
[UnityEngine.RequireComponent(typeof(Camera))]
[UnityEngine.AddComponentMenu("Image Effects/Bloom and Flares")]
public partial class BloomAndFlares : PostEffectsBase
{
     // needed shaders & materials ...
    public Shader addAlphaHackShader;
    private Material _alphaAddMaterial;
    public Shader lensFlareShader;
    private Material _lensFlareMaterial;
    public Shader vignetteShader;
    private Material _vignetteMaterial;
    public Shader separableBlurShader;
    private Material _separableBlurMaterial;
    public Shader addBrightStuffOneOneShader;
    private Material _addBrightStuffBlendOneOneMaterial;
    public Shader hollywoodFlareBlurShader;
    private Material _hollywoodFlareBlurMaterial;
    public Shader hollywoodFlareStretchShader;
    private Material _hollywoodFlareStretchMaterial;
    public Shader brightPassFilterShader;
    private Material _brightPassFilterMaterial;
    public override void Start()
    {
        this.CreateMaterials();
    }

    public virtual void CreateMaterials()
    {
        if (!this._lensFlareMaterial)
        {
            if (!this.CheckShader(this.lensFlareShader))
            {
                this.enabled = false;
                return;
            }
            this._lensFlareMaterial = new Material(this.lensFlareShader);
            this._lensFlareMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
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
        if (!this._addBrightStuffBlendOneOneMaterial)
        {
            if (!this.CheckShader(this.addBrightStuffOneOneShader))
            {
                this.enabled = false;
                return;
            }
            this._addBrightStuffBlendOneOneMaterial = new Material(this.addBrightStuffOneOneShader);
            this._addBrightStuffBlendOneOneMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._hollywoodFlareBlurMaterial)
        {
            if (!this.CheckShader(this.hollywoodFlareBlurShader))
            {
                this.enabled = false;
                return;
            }
            this._hollywoodFlareBlurMaterial = new Material(this.hollywoodFlareBlurShader);
            this._hollywoodFlareBlurMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._hollywoodFlareStretchMaterial)
        {
            if (!this.CheckShader(this.hollywoodFlareStretchShader))
            {
                this.enabled = false;
                return;
            }
            this._hollywoodFlareStretchMaterial = new Material(this.hollywoodFlareStretchShader);
            this._hollywoodFlareStretchMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._brightPassFilterMaterial)
        {
            if (!this.CheckShader(this.brightPassFilterShader))
            {
                this.enabled = false;
                return;
            }
            this._brightPassFilterMaterial = new Material(this.brightPassFilterShader);
            this._brightPassFilterMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._alphaAddMaterial)
        {
            if (!this.CheckShader(this.addAlphaHackShader))
            {
                this.enabled = false;
                return;
            }
            this._alphaAddMaterial = new Material(this.addAlphaHackShader);
            this._alphaAddMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    public virtual void OnEnable()
    {
        if (!this.CheckSupport())
        {
            this.enabled = false;
            return;
        }
    }

    public virtual void OnPreCull()
    {
        if (!string.IsNullOrEmpty(this.bloomThisTag) && (this.bloomThisTag != "Untagged"))
        {
            GameObject[] gos = GameObject.FindGameObjectsWithTag(this.bloomThisTag);
            foreach (GameObject go in gos)
            {
                if ((MeshFilter) go.GetComponent(typeof(MeshFilter)))
                {
                    Mesh mesh = (((MeshFilter) go.GetComponent(typeof(MeshFilter))) as MeshFilter).sharedMesh;
                    Graphics.DrawMesh(mesh, go.transform.localToWorldMatrix, this._alphaAddMaterial, 0, this.GetComponent<Camera>());
                }
            }
        }
    }

    public override void OnRenderImage2(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination);
        this.OnRenderImage3(destination, null);
    }

    public override bool PreferRenderImage3()
    {
        return true;
    }

    public override void OnRenderImage3(RenderTexture sourceAndDestination, RenderTexture temp)
    {
        RenderTexture source = sourceAndDestination;
        RenderTexture destination = sourceAndDestination;
        this.CreateMaterials();
        // needed render targets
        RenderTexture halfRezColor = RenderTexture.GetTemporary((int) (source.width / 2f), (int) (source.height / 2f), 0);
        RenderTexture quarterRezColor = RenderTexture.GetTemporary((int) (source.width / 4f), (int) (source.height / 4f), 0);
        RenderTexture secondQuarterRezColor = RenderTexture.GetTemporary((int) (source.width / 4f), (int) (source.height / 4f), 0);
        RenderTexture thirdQuarterRezColor = RenderTexture.GetTemporary((int) (source.width / 4f), (int) (source.height / 4f), 0);
        // at this point, we have massaged the alpha channel enough to start downsampling process for bloom	
        Graphics.Blit(source, halfRezColor);
        Graphics.Blit(halfRezColor, quarterRezColor);
        // cut colors (threshholding)			
        this._brightPassFilterMaterial.SetVector("threshhold", new Vector4(this.bloomThreshhold, 1f / (1f - this.bloomThreshhold), 0f, 0f));
        this._brightPassFilterMaterial.SetFloat("useSrcAlphaAsMask", this.useSrcAlphaAsMask);
        Graphics.Blit(quarterRezColor, secondQuarterRezColor, this._brightPassFilterMaterial);
        // blurring
        if (this.bloomBlurIterations < 1)
        {
            this.bloomBlurIterations = 1;
        }
        // blur the result to get a nicer bloom radius
        Graphics.Blit(secondQuarterRezColor, quarterRezColor);
        int iter = 0;
        while (iter < this.bloomBlurIterations)
        {
            this._separableBlurMaterial.SetVector("offsets", new Vector4(0f, (this.sepBlurSpread * 1f) / quarterRezColor.height, 0f, 0f));
            Graphics.Blit(quarterRezColor, thirdQuarterRezColor, this._separableBlurMaterial);
            this._separableBlurMaterial.SetVector("offsets", new Vector4((this.sepBlurSpread * 1f) / quarterRezColor.width, 0f, 0f, 0f));
            Graphics.Blit(thirdQuarterRezColor, quarterRezColor, this._separableBlurMaterial);
            iter++;
        }
        // operate on lens flares now
        if (this.lensflares)
        {
            if (this.lensflareMode == (LensflareStyle) 0) // ghosting
            {
                 // lens flare fun: cut some additional values and normalize
                this._brightPassFilterMaterial.SetVector("threshhold", new Vector4(this.lensflareThreshhold, 1f / (1f - this.lensflareThreshhold), 0f, 0f));
                this._brightPassFilterMaterial.SetFloat("useSrcAlphaAsMask", 0f);
                Graphics.Blit(quarterRezColor, thirdQuarterRezColor, this._brightPassFilterMaterial);
                this._separableBlurMaterial.SetVector("offsets", new Vector4(0f, (this.sepBlurSpread * 1f) / quarterRezColor.height, 0f, 0f));
                Graphics.Blit(thirdQuarterRezColor, secondQuarterRezColor, this._separableBlurMaterial);
                this._separableBlurMaterial.SetVector("offsets", new Vector4((this.sepBlurSpread * 1f) / quarterRezColor.width, 0f, 0f, 0f));
                Graphics.Blit(secondQuarterRezColor, thirdQuarterRezColor, this._separableBlurMaterial);
                // vignette for lens flares
                this._vignetteMaterial.SetFloat("vignetteIntensity", 1f);
                Graphics.Blit(thirdQuarterRezColor, secondQuarterRezColor, this._vignetteMaterial);
                // creating the flares
                // _lensFlareMaterial has One One Blend
                this._lensFlareMaterial.SetVector("color0", new Vector4(0f, 0f, 0f, 0f) * this.lensflareIntensity);
                this._lensFlareMaterial.SetVector("colorA", new Vector4(this.flareColorA.r, this.flareColorA.g, this.flareColorA.b, this.flareColorA.a) * this.lensflareIntensity);
                this._lensFlareMaterial.SetVector("colorB", new Vector4(this.flareColorB.r, this.flareColorB.g, this.flareColorB.b, this.flareColorB.a) * this.lensflareIntensity);
                this._lensFlareMaterial.SetVector("colorC", new Vector4(this.flareColorC.r, this.flareColorC.g, this.flareColorC.b, this.flareColorC.a) * this.lensflareIntensity);
                this._lensFlareMaterial.SetVector("colorD", new Vector4(this.flareColorD.r, this.flareColorD.g, this.flareColorD.b, this.flareColorD.a) * this.lensflareIntensity);
                Graphics.Blit(secondQuarterRezColor, thirdQuarterRezColor, this._lensFlareMaterial);
                this._addBrightStuffBlendOneOneMaterial.SetFloat("intensity", 1f);
                Graphics.Blit(thirdQuarterRezColor, quarterRezColor, this._addBrightStuffBlendOneOneMaterial);
            }
            else
            {
                if (this.lensflareMode == (LensflareStyle) 1) // hollywood flares
                {
                     // lens flare fun: cut some additional values 
                    this._brightPassFilterMaterial.SetVector("threshhold", new Vector4(this.lensflareThreshhold, 1f / (1f - this.lensflareThreshhold), 0f, 0f));
                    this._brightPassFilterMaterial.SetFloat("useSrcAlphaAsMask", 0f);
                    Graphics.Blit(secondQuarterRezColor, thirdQuarterRezColor, this._brightPassFilterMaterial);
                    // ole: NEW and AWESOME new feature for hollyflares
                    // narrow down the size that creates on of these lines
                    this._hollywoodFlareBlurMaterial.SetVector("offsets", new Vector4(0f, (this.sepBlurSpread * 1f) / quarterRezColor.height, 0f, 0f));
                    this._hollywoodFlareBlurMaterial.SetTexture("_NonBlurredTex", quarterRezColor);
                    this._hollywoodFlareBlurMaterial.SetVector("tintColor", (new Vector4(this.flareColorA.r, this.flareColorA.g, this.flareColorA.b, this.flareColorA.a) * this.flareColorA.a) * this.lensflareIntensity);
                    Graphics.Blit(thirdQuarterRezColor, secondQuarterRezColor, this._hollywoodFlareBlurMaterial);
                    this._hollywoodFlareStretchMaterial.SetVector("offsets", new Vector4((this.sepBlurSpread * 1f) / quarterRezColor.width, 0f, 0f, 0f));
                    this._hollywoodFlareStretchMaterial.SetFloat("stretchWidth", this.hollyStretchWidth);
                    Graphics.Blit(secondQuarterRezColor, thirdQuarterRezColor, this._hollywoodFlareStretchMaterial);
                    int itera = 0;
                    while (itera < this.hollywoodFlareBlurIterations)
                    {
                        this._separableBlurMaterial.SetVector("offsets", new Vector4((this.sepBlurSpread * 1f) / quarterRezColor.width, 0f, 0f, 0f));
                        Graphics.Blit(thirdQuarterRezColor, secondQuarterRezColor, this._separableBlurMaterial);
                        this._separableBlurMaterial.SetVector("offsets", new Vector4((this.sepBlurSpread * 1f) / quarterRezColor.width, 0f, 0f, 0f));
                        Graphics.Blit(secondQuarterRezColor, thirdQuarterRezColor, this._separableBlurMaterial);
                        itera++;
                    }
                    this._addBrightStuffBlendOneOneMaterial.SetFloat("intensity", 1f);
                    Graphics.Blit(thirdQuarterRezColor, quarterRezColor, this._addBrightStuffBlendOneOneMaterial);
                }
                else
                {
                     // 'both' flares :)
                     // lens flare fun: cut some additional values 
                    this._brightPassFilterMaterial.SetVector("threshhold", new Vector4(this.lensflareThreshhold, 1f / (1f - this.lensflareThreshhold), 0f, 0f));
                    this._brightPassFilterMaterial.SetFloat("useSrcAlphaAsMask", 0f);
                    Graphics.Blit(secondQuarterRezColor, thirdQuarterRezColor, this._brightPassFilterMaterial);
                    // ole: NEW and AWESOME new feature for hollyflares
                    // narrow down the size that creates on of these lines
                    this._hollywoodFlareBlurMaterial.SetVector("offsets", new Vector4(0f, (this.sepBlurSpread * 1f) / quarterRezColor.height, 0f, 0f));
                    this._hollywoodFlareBlurMaterial.SetTexture("_NonBlurredTex", quarterRezColor);
                    this._hollywoodFlareBlurMaterial.SetVector("tintColor", (new Vector4(this.flareColorA.r, this.flareColorA.g, this.flareColorA.b, this.flareColorA.a) * this.flareColorA.a) * this.lensflareIntensity);
                    Graphics.Blit(thirdQuarterRezColor, secondQuarterRezColor, this._hollywoodFlareBlurMaterial);
                    this._hollywoodFlareStretchMaterial.SetVector("offsets", new Vector4((this.sepBlurSpread * 1f) / quarterRezColor.width, 0f, 0f, 0f));
                    this._hollywoodFlareStretchMaterial.SetFloat("stretchWidth", this.hollyStretchWidth);
                    Graphics.Blit(secondQuarterRezColor, thirdQuarterRezColor, this._hollywoodFlareStretchMaterial);
                    int ix = 0;
                    while (ix < this.hollywoodFlareBlurIterations)
                    {
                        this._separableBlurMaterial.SetVector("offsets", new Vector4((this.sepBlurSpread * 1f) / quarterRezColor.width, 0f, 0f, 0f));
                        Graphics.Blit(thirdQuarterRezColor, secondQuarterRezColor, this._separableBlurMaterial);
                        this._separableBlurMaterial.SetVector("offsets", new Vector4((this.sepBlurSpread * 1f) / quarterRezColor.width, 0f, 0f, 0f));
                        Graphics.Blit(secondQuarterRezColor, thirdQuarterRezColor, this._separableBlurMaterial);
                        ix++;
                    }
                    // vignette for lens flares
                    this._vignetteMaterial.SetFloat("vignetteIntensity", 1f);
                    Graphics.Blit(thirdQuarterRezColor, secondQuarterRezColor, this._vignetteMaterial);
                    // creating the flares
                    // _lensFlareMaterial has One One Blend
                    this._lensFlareMaterial.SetVector("color0", new Vector4(0f, 0f, 0f, 0f) * this.lensflareIntensity);
                    this._lensFlareMaterial.SetVector("colorA", (new Vector4(this.flareColorA.r, this.flareColorA.g, this.flareColorA.b, this.flareColorA.a) * this.flareColorA.a) * this.lensflareIntensity);
                    this._lensFlareMaterial.SetVector("colorB", (new Vector4(this.flareColorB.r, this.flareColorB.g, this.flareColorB.b, this.flareColorB.a) * this.flareColorB.a) * this.lensflareIntensity);
                    this._lensFlareMaterial.SetVector("colorC", (new Vector4(this.flareColorC.r, this.flareColorC.g, this.flareColorC.b, this.flareColorC.a) * this.flareColorC.a) * this.lensflareIntensity);
                    this._lensFlareMaterial.SetVector("colorD", (new Vector4(this.flareColorD.r, this.flareColorD.g, this.flareColorD.b, this.flareColorD.a) * this.flareColorD.a) * this.lensflareIntensity);
                    Graphics.Blit(secondQuarterRezColor, thirdQuarterRezColor, this._lensFlareMaterial);
                    this._addBrightStuffBlendOneOneMaterial.SetFloat("intensity", 1f);
                    Graphics.Blit(thirdQuarterRezColor, quarterRezColor, this._addBrightStuffBlendOneOneMaterial);
                }
            }
        }
        this._addBrightStuffBlendOneOneMaterial.SetFloat("intensity", this.bloomIntensity);
        Graphics.Blit(quarterRezColor, destination, this._addBrightStuffBlendOneOneMaterial);
        RenderTexture.ReleaseTemporary(halfRezColor);
        RenderTexture.ReleaseTemporary(quarterRezColor);
        RenderTexture.ReleaseTemporary(secondQuarterRezColor);
        RenderTexture.ReleaseTemporary(thirdQuarterRezColor);
    }

    public string bloomThisTag;
    public float sepBlurSpread;
    public float useSrcAlphaAsMask;
    public float bloomIntensity;
    public float bloomThreshhold;
    public int bloomBlurIterations;
    public TweakMode tweakMode;
    public bool lensflares;
    public int hollywoodFlareBlurIterations;
    public LensflareStyle lensflareMode;
    public float hollyStretchWidth;
    public float lensflareIntensity;
    public float lensflareThreshhold;
    public Color flareColorA;
    public Color flareColorB;
    public Color flareColorC;
    public Color flareColorD;
    public float blurWidth;
    public BloomAndFlares()
    {
        this.sepBlurSpread = 1.5f;
        this.useSrcAlphaAsMask = 0.5f;
        this.bloomIntensity = 1f;
        this.bloomThreshhold = 0.4f;
        this.bloomBlurIterations = 3;
        this.tweakMode = (TweakMode) 1;
        this.lensflares = true;
        this.hollywoodFlareBlurIterations = 4;
        this.hollyStretchWidth = 2.5f;
        this.lensflareIntensity = 0.75f;
        this.lensflareThreshhold = 0.5f;
        this.flareColorA = new Color(0.4f, 0.4f, 0.8f, 0.75f);
        this.flareColorB = new Color(0.4f, 0.8f, 0.8f, 0.75f);
        this.flareColorC = new Color(0.8f, 0.4f, 0.8f, 0.75f);
        this.flareColorD = new Color(0.8f, 0.4f, 0f, 0.75f);
        this.blurWidth = 1f;
    }

}