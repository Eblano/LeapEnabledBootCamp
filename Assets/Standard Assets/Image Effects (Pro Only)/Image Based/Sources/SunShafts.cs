using UnityEngine;
using System.Collections;

public enum SunShaftsResolution
{
    Low = 0,
    Normal = 1,
    High = 2
}

[System.Serializable]
[UnityEngine.ExecuteInEditMode]
[UnityEngine.RequireComponent(typeof(Camera))]
[UnityEngine.AddComponentMenu("Image Effects/Sun Shafts")]
public partial class SunShafts : PostEffectsBase
{
     // needed shaders & materials
    public Shader clearShader;
    private Material _clearMaterial;
    public Shader depthDecodeShader;
    private Material _encodeDepthRGBA8Material;
    public Shader depthBlurShader;
    private Material _radialDepthBlurMaterial;
    public Shader sunShaftsShader;
    private Material _sunShaftsMaterial;
    public Shader simpleClearShader;
    private Material _simpleClearMaterial;
    public Shader compShader;
    private Material _compMaterial;
    public override void Start()
    {
        this.order = 1; // hacked in higher order for sun shafts
        this.CreateMaterials();
    }

    public virtual void CreateMaterials()
    {
        if (!this._clearMaterial)
        {
            if (!this.CheckShader(this.clearShader))
            {
                this.enabled = false;
                return;
            }
            this._clearMaterial = new Material(this.clearShader);
            this._clearMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._sunShaftsMaterial)
        {
            if (!this.CheckShader(this.sunShaftsShader))
            {
                this.enabled = false;
                return;
            }
            this._sunShaftsMaterial = new Material(this.sunShaftsShader);
            this._sunShaftsMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._encodeDepthRGBA8Material)
        {
            if (!this.CheckShader(this.depthDecodeShader))
            {
                this.enabled = false;
                return;
            }
            this._encodeDepthRGBA8Material = new Material(this.depthDecodeShader);
            this._encodeDepthRGBA8Material.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._radialDepthBlurMaterial)
        {
            if (!this.CheckShader(this.depthBlurShader))
            {
                this.enabled = false;
                return;
            }
            this._radialDepthBlurMaterial = new Material(this.depthBlurShader);
            this._radialDepthBlurMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._simpleClearMaterial)
        {
            if (!this.CheckShader(this.simpleClearShader))
            {
                this.enabled = false;
                return;
            }
            this._simpleClearMaterial = new Material(this.simpleClearShader);
            this._simpleClearMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        //_compMaterial
        if (!this._compMaterial)
        {
            if (!this.CheckShader(this.compShader))
            {
                this.enabled = false;
                return;
            }
            this._compMaterial = new Material(this.compShader);
            this._compMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    public virtual void OnEnable()
    {
        if (!this.CheckSupport())
        {
            this.enabled = false;
            return;
        }
        if (this.useDepthTexture)
        {
            this.GetComponent<Camera>().depthTextureMode = this.GetComponent<Camera>().depthTextureMode | DepthTextureMode.Depth;
        }
    }

    public override void OnRenderImage2(RenderTexture source, RenderTexture destination)
    {
         // save the color buffer
        Graphics.Blit(source, destination);
        this.OnRenderImage3(destination, source);
    }

    public override bool PreferRenderImage3()
    {
        return true;
    }

    /*
			tmpBuffer  = RenderTexture.GetTemporary(source.width, source.height, 0);	
			RenderTexture.active = tmpBuffer;
			GL.ClearWithSkybox(false, camera);
			
			RenderTexture.active = source;
			_compMaterial.SetTexture("_Skybox", tmpBuffer);
			Graphics.Blit (source, destination, _compMaterial);
			
			RenderTexture.ReleaseTemporary(tmpBuffer);	
		*/    public override void OnRenderImage3(RenderTexture sourceAndDestination, RenderTexture temp)
    {
         // needed for most of the new and improved image FX
        this.CreateMaterials();
        float divider = 4f;
        if (this.resolution == SunShaftsResolution.Normal)
        {
            divider = 2f;
        }
        if (this.resolution == SunShaftsResolution.High)
        {
            divider = 1f;
        }
        // get render targets		
        RenderTexture secondQuarterRezColor = RenderTexture.GetTemporary((int) (sourceAndDestination.width / divider), (int) (sourceAndDestination.height / divider), 0);
        RenderTexture lrDepthBuffer = RenderTexture.GetTemporary((int) (sourceAndDestination.width / divider), (int) (sourceAndDestination.height / divider), 0);
        RenderTexture destination = sourceAndDestination;
        // mask skybox (some pixels are clip()'ped, others are kept ...)
        if (!this.useDepthTexture)
        {
            RenderTexture tmpBuffer = RenderTexture.GetTemporary(sourceAndDestination.width, sourceAndDestination.height, 0);
            RenderTexture.active = tmpBuffer;
            GL.ClearWithSkybox(false, this.GetComponent<Camera>());
            RenderTexture.active = temp;
            this._compMaterial.SetTexture("_Skybox", tmpBuffer);
            Graphics.Blit(sourceAndDestination, temp, this._compMaterial);
            RenderTexture.ReleaseTemporary(tmpBuffer);
        }
        else
        {
            // =>
            /*Graphics.Blit(source, destination);
			RenderTexture.ReleaseTemporary (lrDepthBuffer);	
			RenderTexture.ReleaseTemporary (secondQuarterRezColor);	
					
			return;
			*/
            Graphics.Blit(sourceAndDestination, temp, this._clearMaterial);
        }
        // get depth values
        this._encodeDepthRGBA8Material.SetFloat("noSkyBoxMask", 1f - this.useSkyBoxAlpha);
        this._encodeDepthRGBA8Material.SetFloat("dontUseSkyboxBrightness", 0f);
        Graphics.Blit(temp, lrDepthBuffer, this._encodeDepthRGBA8Material);
        // black small pixel border to get rid of clamping annoyances
        // we don't need to care about this for bootcamp
        //DrawBorder(lrDepthBuffer,_simpleClearMaterial);
        Vector3 v = Vector3.one * 0.5f;
        if (this.sunTransform)
        {
            v = this.GetComponent<Camera>().WorldToViewportPoint(this.sunTransform.position);
        }
        else
        {
            v = new Vector3(0.5f, 0.5f, 0f);
        }
        // radial depth blur now
        this._radialDepthBlurMaterial.SetVector("blurRadius4", new Vector4(1f, 1f, 0f, 0f) * this.sunShaftBlurRadius);
        this._radialDepthBlurMaterial.SetVector("sunPosition", new Vector4(v.x, v.y, v.z, this.maxRadius));
        if (this.radialBlurIterations < 1)
        {
            this.radialBlurIterations = 1;
        }
        int it2 = 0;
        while (it2 < this.radialBlurIterations)
        {
            Graphics.Blit(lrDepthBuffer, secondQuarterRezColor, this._radialDepthBlurMaterial);
            Graphics.Blit(secondQuarterRezColor, lrDepthBuffer, this._radialDepthBlurMaterial);
            it2++;
        }
        // composite now			
        this._sunShaftsMaterial.SetFloat("sunShaftIntensity", this.sunShaftIntensity);
        if (v.z >= 0f)
        {
            this._sunShaftsMaterial.SetVector("sunColor", new Vector4(this.sunColor.r, this.sunColor.g, this.sunColor.b, this.sunColor.a));
        }
        else
        {
            this._sunShaftsMaterial.SetVector("sunColor", new Vector4(0f, 0f, 0f, 0f)); // no backprojection !
        }
        Graphics.Blit(lrDepthBuffer, destination, this._sunShaftsMaterial);
        RenderTexture.ReleaseTemporary(lrDepthBuffer);
        RenderTexture.ReleaseTemporary(secondQuarterRezColor);
    }

    public SunShaftsResolution resolution;
    public Transform sunTransform;
    public int radialBlurIterations;
    public Color sunColor;
    public float sunShaftBlurRadius;
    public float sunShaftIntensity;
    public float useSkyBoxAlpha;
    public float maxRadius;
    public bool useDepthTexture;
    public SunShafts()
    {
        this.radialBlurIterations = 2;
        this.sunColor = Color.white;
        this.sunShaftBlurRadius = 0.0164f;
        this.sunShaftIntensity = 1.25f;
        this.useSkyBoxAlpha = 0.75f;
        this.maxRadius = 1.25f;
        this.useDepthTexture = true;
    }

}