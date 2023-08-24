using UnityEngine;
using System.Collections;

public enum DofResolutionSetting
{
    Low = 0,
    Normal = 1,
    High = 2
}

public enum DofQualitySetting
{
    Low = 0,
    Medium = 1,
    High = 2
}

[System.Serializable]
[UnityEngine.ExecuteInEditMode]
[UnityEngine.RequireComponent(typeof(Camera))]
[UnityEngine.AddComponentMenu("Image Effects/Depth of Field")]
public partial class DepthOfField : PostEffectsBase
{
    public bool available;
    public Shader weightedBlurShader;
    private Material _weightedBlurMaterial;
    public Shader preDofShader;
    private Material _preDofMaterial;
    public Shader preDofZReadShader;
    private Material _preDofZReadMaterial;
    public Shader dofShader;
    private Material _dofMaterial;
    public Shader blurShader;
    private Material _blurMaterial;
    public Shader foregroundShader;
    private Material _foregroundBlurMaterial;
    public Shader depthConvertShader;
    private Material _depthConvertMaterial;
    public Shader depthBlurShader;
    private Material _depthBlurMaterial;
    public Shader recordCenterDepthShader;
    private Material _recordCenterDepthMaterial;
    private RenderTexture _onePixel;
    public virtual void CreateMaterials()
    {
        if (!this._onePixel)
        {
            this._onePixel = new RenderTexture(1, 1, 0);
            this._onePixel.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._recordCenterDepthMaterial)
        {
            if (!this.CheckShader(this.recordCenterDepthShader))
            {
                this.enabled = false;
                return;
            }
            this._recordCenterDepthMaterial = new Material(this.recordCenterDepthShader);
            this._recordCenterDepthMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._weightedBlurMaterial)
        {
            if (!this.CheckShader(this.blurShader))
            {
                this.enabled = false;
                return;
            }
            this._weightedBlurMaterial = new Material(this.weightedBlurShader);
            this._weightedBlurMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
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
        if (!this._dofMaterial)
        {
            if (!this.CheckShader(this.dofShader))
            {
                this.enabled = false;
                return;
            }
            this._dofMaterial = new Material(this.dofShader);
            this._dofMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._preDofMaterial)
        {
            if (!this.CheckShader(this.preDofShader))
            {
                this.enabled = false;
                return;
            }
            this._preDofMaterial = new Material(this.preDofShader);
            this._preDofMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._preDofZReadMaterial)
        {
            if (!this.CheckShader(this.preDofZReadShader))
            {
                this.enabled = false;
                return;
            }
            this._preDofZReadMaterial = new Material(this.preDofZReadShader);
            this._preDofZReadMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._foregroundBlurMaterial)
        {
            if (!this.CheckShader(this.foregroundShader))
            {
                this.enabled = false;
                return;
            }
            this._foregroundBlurMaterial = new Material(this.foregroundShader);
            this._foregroundBlurMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._depthConvertMaterial)
        {
            if (!this.CheckShader(this.depthConvertShader))
            {
                this.enabled = false;
                return;
            }
            this._depthConvertMaterial = new Material(this.depthConvertShader);
            this._depthConvertMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._depthBlurMaterial)
        {
            if (!this.CheckShader(this.depthBlurShader))
            {
                this.enabled = false;
                return;
            }
            this._depthBlurMaterial = new Material(this.depthBlurShader);
            this._depthBlurMaterial.hideFlags = HideFlags.HideAndDontSave;
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
        this.GetComponent<Camera>().depthTextureMode = this.GetComponent<Camera>().depthTextureMode | DepthTextureMode.Depth;
    }

    public virtual void OnEnable()
    {
        if (!this.CheckSupport())
        {
            this.enabled = false;
            return;
        }
        if (!this.available)
        {
            this.enabled = false;
            return;
        }
    }

    public override void OnRenderImage2(RenderTexture source, RenderTexture destination)
    {
        this.CreateMaterials();
        if (this.foregroundBlurIterations < 1)
        {
            this.foregroundBlurIterations = 1;
        }
        if (this.blurIterations < 1)
        {
            this.blurIterations = 1;
        }
        // quality and resolution settings
        float divider = 4f;
        if (this.resolution == DofResolutionSetting.Normal)
        {
            divider = 2f;
        }
        else
        {
            if (this.resolution == DofResolutionSetting.High)
            {
                divider = 1f;
            }
        }
        // support for a game object / transform defining the focal distance      
        Material preDofMaterial = this._preDofMaterial;
        if (this.focusOnThis)
        {
            Vector3 vpPoint = this.GetComponent<Camera>().WorldToViewportPoint(this.focusOnThis.position);
            vpPoint.z = vpPoint.z / this.GetComponent<Camera>().farClipPlane;
            this._focalDistance01 = vpPoint.z;
        }
        else
        {
            if (this.focusOnScreenCenterDepth)
            {
                preDofMaterial = this._preDofZReadMaterial;
                // OK, here the zBuffer based "raycast" mode, let's record the blurred
                // depth buffer center area and add some smoothing
                this._recordCenterDepthMaterial.SetFloat("deltaTime", Time.deltaTime * this.focalChangeSpeed);
                Graphics.Blit(this._onePixel, this._onePixel, this._recordCenterDepthMaterial);
            }
            else
            {
                this._focalDistance01 = this.GetComponent<Camera>().WorldToViewportPoint((this.focalZDistance * this.GetComponent<Camera>().transform.forward) + this.GetComponent<Camera>().transform.position).z / this.GetComponent<Camera>().farClipPlane;
            }
        }
        if (this.focalZEnd > this.GetComponent<Camera>().farClipPlane)
        {
            this.focalZEnd = this.GetComponent<Camera>().farClipPlane;
        }
        this._focalStart01 = this.GetComponent<Camera>().WorldToViewportPoint((this.focalZStart * this.GetComponent<Camera>().transform.forward) + this.GetComponent<Camera>().transform.position).z / this.GetComponent<Camera>().farClipPlane;
        this._focalEnd01 = this.GetComponent<Camera>().WorldToViewportPoint((this.focalZEnd * this.GetComponent<Camera>().transform.forward) + this.GetComponent<Camera>().transform.position).z / this.GetComponent<Camera>().farClipPlane;
        if (this._focalEnd01 < this._focalStart01)
        {
            this._focalEnd01 = this._focalStart01 + Mathf.Epsilon;
        }
        // we use the alpha channel for storing the COC
        // which also means, that unfortunately, alpha based
        // image effects such as sun shafts, bloom or glow
        // won't work if placed *after* the DOF image effect
        // simple COC calculation based on distance to focal point     
        preDofMaterial.SetFloat("focalDistance01", this._focalDistance01);
        preDofMaterial.SetFloat("focalFalloff", this.focalFalloff);
        preDofMaterial.SetFloat("focalStart01", this._focalStart01);
        preDofMaterial.SetFloat("focalEnd01", this._focalEnd01);
        preDofMaterial.SetFloat("focalSize", this.focalSize * 0.5f);
        preDofMaterial.SetTexture("_onePixelTex", this._onePixel);
        Graphics.Blit(source, source, preDofMaterial); // ColorMask == ALPHA
        RenderTexture lrTex0 = RenderTexture.GetTemporary((int) (source.width / divider), (int) (source.height / divider), 0);
        RenderTexture lrTex1 = RenderTexture.GetTemporary((int) (source.width / divider), (int) (source.height / divider), 0);
        RenderTexture lrTex2 = RenderTexture.GetTemporary((int) (source.width / divider), (int) (source.height / divider), 0);
        // high quality DOF should blur the forground       
        if (this.quality >= DofQualitySetting.High)
        {
            lrTex0.filterMode = FilterMode.Point;
            lrTex1.filterMode = FilterMode.Point;
            lrTex2.filterMode = FilterMode.Point;
            source.filterMode = FilterMode.Point;
            // downsample depth (and encoding in RGBA)
            Graphics.Blit(source, lrTex1, this._depthConvertMaterial);
            source.filterMode = FilterMode.Bilinear;
            //
            // depth blur 	(result in lrTex1)          	       
            int it = 0;
            while (it < this.foregroundBlurIterations)
            {
                this._depthBlurMaterial.SetVector("offsets", new Vector4(0f, this.foregroundBlurSpread / lrTex1.height, 0f, this._focalDistance01));
                Graphics.Blit(lrTex1, lrTex0, this._depthBlurMaterial);
                this._depthBlurMaterial.SetVector("offsets", new Vector4(this.foregroundBlurSpread / lrTex1.width, 0f, 0f, this._focalDistance01));
                Graphics.Blit(lrTex0, lrTex1, this._depthBlurMaterial);
                it++;
            }
            //
            // simple blur 	(result in lrTex0)
            lrTex0.filterMode = FilterMode.Bilinear;
            lrTex1.filterMode = FilterMode.Bilinear;
            lrTex2.filterMode = FilterMode.Bilinear;
            int tempBlurIterations = this.blurIterations;
            if (this.resolution != DofResolutionSetting.High)
            {
                Graphics.Blit(source, lrTex0);
            }
            else
            {
                 // high resolution, divider is 1
                this._blurMaterial.SetVector("offsets", new Vector4(0f, this.blurSpread / lrTex1.height, 0f, 0f));
                Graphics.Blit(source, lrTex2, this._blurMaterial);
                this._blurMaterial.SetVector("offsets", new Vector4(this.blurSpread / lrTex1.width, 0f, 0f, 0f));
                Graphics.Blit(lrTex2, lrTex0, this._blurMaterial);
                tempBlurIterations = tempBlurIterations - 1;
            }
            it = 0;
            while (it < tempBlurIterations)
            {
                this._blurMaterial.SetVector("offsets", new Vector4(0f, this.blurSpread / lrTex1.height, 0f, 0f));
                Graphics.Blit(lrTex0, lrTex2, this._blurMaterial);
                this._blurMaterial.SetVector("offsets", new Vector4(this.blurSpread / lrTex1.width, 0f, 0f, 0f));
                Graphics.Blit(lrTex2, lrTex0, this._blurMaterial);
                it++;
            }
            // calculate the foreground blur factor and add to our COC
            lrTex1.filterMode = FilterMode.Point;
            this._foregroundBlurMaterial.SetFloat("focalFalloff", this.focalFalloff);
            this._foregroundBlurMaterial.SetFloat("focalDistance01", this._focalDistance01);
            this._foregroundBlurMaterial.SetFloat("focalStart01", this._focalStart01);
            this._foregroundBlurMaterial.SetFloat("focalEnd01", this._focalEnd01);
            this._foregroundBlurMaterial.SetFloat("foregroundBlurStrength", this.foregroundBlurStrength);
            this._foregroundBlurMaterial.SetFloat("foregroundBlurThreshhold", this.foregroundBlurThreshhold);
            this._foregroundBlurMaterial.SetTexture("_SourceTex", source);
            this._foregroundBlurMaterial.SetTexture("_BlurredCoc", lrTex0);
            Graphics.Blit(lrTex1, source, this._foregroundBlurMaterial);
            lrTex0.filterMode = FilterMode.Bilinear;
            lrTex1.filterMode = FilterMode.Bilinear;
            lrTex2.filterMode = FilterMode.Bilinear;
        }
         // high quality
        // weighted by COC blur
        int tempStdBlurIterations = this.blurIterations;
        if (this.resolution != DofResolutionSetting.High)
        {
            Graphics.Blit(source, lrTex1);
        }
        else
        {
             // high resolution and divider == 1
            if (this.quality >= DofQualitySetting.Medium)
            {
                this._weightedBlurMaterial.SetVector("offsets", new Vector4(0f, this.blurSpread / lrTex1.height, 0f, this._focalDistance01));
                Graphics.Blit(source, lrTex2, this._weightedBlurMaterial);
                this._weightedBlurMaterial.SetVector("offsets", new Vector4(this.blurSpread / lrTex1.width, 0f, 0f, this._focalDistance01));
                Graphics.Blit(lrTex2, lrTex1, this._weightedBlurMaterial);
            }
            else
            {
                this._blurMaterial.SetVector("offsets", new Vector4(0f, this.blurSpread / lrTex1.height, 0f, 0f));
                Graphics.Blit(source, lrTex2, this._blurMaterial);
                this._blurMaterial.SetVector("offsets", new Vector4(this.blurSpread / lrTex1.width, 0f, 0f, 0f));
                Graphics.Blit(lrTex2, lrTex1, this._blurMaterial);
            }
            tempStdBlurIterations = tempStdBlurIterations - 1;
        }
        it = 0;
        while (it < tempStdBlurIterations)
        {
            if (this.quality >= DofQualitySetting.Medium)
            {
                this._weightedBlurMaterial.SetVector("offsets", new Vector4(0f, this.blurSpread / lrTex1.height, 0f, this._focalDistance01));
                Graphics.Blit(lrTex1, lrTex2, this._weightedBlurMaterial);
                this._weightedBlurMaterial.SetVector("offsets", new Vector4(this.blurSpread / lrTex1.width, 0f, 0f, this._focalDistance01));
                Graphics.Blit(lrTex2, lrTex1, this._weightedBlurMaterial);
            }
            else
            {
                this._blurMaterial.SetVector("offsets", new Vector4(0f, this.blurSpread / lrTex1.height, 0f, 0f));
                Graphics.Blit(lrTex1, lrTex2, this._blurMaterial);
                this._blurMaterial.SetVector("offsets", new Vector4(this.blurSpread / lrTex1.width, 0f, 0f, 0f));
                Graphics.Blit(lrTex2, lrTex1, this._blurMaterial);
            }
            it++;
        }
        //
        // composite the final image, depending on COC, source and low resolution color buffers
        this._dofMaterial.SetFloat("focalDistance01", this._focalDistance01);
        this._dofMaterial.SetTexture("_LowRez", lrTex1);
        Graphics.Blit(source, destination, this._dofMaterial); // also writes ALPHA = 1
        RenderTexture.ReleaseTemporary(lrTex0);
        RenderTexture.ReleaseTemporary(lrTex1);
        RenderTexture.ReleaseTemporary(lrTex2);
    }

    public DofResolutionSetting resolution;
    public DofQualitySetting quality;
    public float focalZDistance;
    public float focalZStart;
    public float focalZEnd;
    private float _focalDistance01;
    private float _focalStart01;
    private float _focalEnd01;
    public float focalFalloff;
    public Transform focusOnThis;
    public bool focusOnScreenCenterDepth;
    public float focalSize;
    public float focalChangeSpeed;
    public int blurIterations;
    public int foregroundBlurIterations;
    public float blurSpread;
    public float foregroundBlurSpread;
    public float foregroundBlurStrength;
    public float foregroundBlurThreshhold;
    public DepthOfField()
    {
        this.available = true;
        this.resolution = DofResolutionSetting.Normal;
        this.quality = DofQualitySetting.High;
        this.focalZEnd = 10000f;
        this._focalEnd01 = 1f;
        this.focalFalloff = 1f;
        this.focalSize = 0.075f;
        this.focalChangeSpeed = 2.275f;
        this.blurIterations = 2;
        this.foregroundBlurIterations = 2;
        this.blurSpread = 1.25f;
        this.foregroundBlurSpread = 1.75f;
        this.foregroundBlurStrength = 1.15f;
        this.foregroundBlurThreshhold = 0.002f;
    }

}