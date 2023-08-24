using UnityEngine;
using System.Collections;

// GENERAL stuff
public enum ColorCorrectionMode
{
    Simple = 0,
    Advanced = 1
}

// SHADERS
[System.Serializable]
[UnityEngine.ExecuteInEditMode]
[UnityEngine.AddComponentMenu("Image Effects/Color Correction (Curves)")]
public partial class ColorCorrectionCurves : PostEffectsBase
{
    public override void Start()
    {
        this.updateTextures = true;
        this.CreateMaterials();
    }

    public virtual void CreateMaterials()
    {
        if (this.recreateTextures)
        {
            this._rgbChannelTex = null;
            this._rgbDepthChannelTex = null;
            this._zCurve = null;
            this.recreateTextures = false;
        }
        if (!this._ccMaterial)
        {
            if (!this.CheckShader(this.simpleColorCorrectionCurvesShader))
            {
                this.enabled = false;
                return;
            }
            this._ccMaterial = new Material(this.simpleColorCorrectionCurvesShader);
            this._ccMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._ccDepthMaterial)
        {
            if (!this.CheckShader(this.colorCorrectionCurvesShader))
            {
                this.enabled = false;
                return;
            }
            this._ccDepthMaterial = new Material(this.colorCorrectionCurvesShader);
            this._ccDepthMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._selectiveCcMaterial)
        {
            if (!this.CheckShader(this.colorCorrectionSelectiveShader))
            {
                this.enabled = false;
                return;
            }
            this._selectiveCcMaterial = new Material(this.colorCorrectionSelectiveShader);
            this._selectiveCcMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
        {
            this.enabled = false;
            return;
        }
        // sample all curves, create textures
        if (!this._rgbChannelTex)
        {
            this._rgbChannelTex = new Texture2D(this.curveResolution, 4, TextureFormat.ARGB32, false);
            this._rgbChannelTex.hideFlags = HideFlags.HideAndDontSave;
            this.updateTextures = true;
        }
        if (!this._rgbDepthChannelTex)
        {
            this._rgbDepthChannelTex = new Texture2D(this.curveResolution, 4, TextureFormat.ARGB32, false);
            this._rgbDepthChannelTex.hideFlags = HideFlags.HideAndDontSave;
            this.updateTextures = true;
        }
        if (!this._zCurve)
        {
            this._zCurve = new Texture2D(this.curveResolution, 1, TextureFormat.ARGB32, false);
            this._zCurve.hideFlags = HideFlags.HideAndDontSave;
            this.updateTextures = true;
        }
        this._rgbChannelTex.wrapMode = TextureWrapMode.Clamp;
        this._rgbDepthChannelTex.wrapMode = TextureWrapMode.Clamp;
        this._zCurve.wrapMode = TextureWrapMode.Clamp;
    }

    public virtual void OnEnable()
    {
        if (!this.CheckSupport())
        {
            this.enabled = false;
            return;
        }
        if (this.useDepthCorrection)
        {
            this.GetComponent<Camera>().depthTextureMode = this.GetComponent<Camera>().depthTextureMode | DepthTextureMode.Depth;
        }
    }

    public virtual void OnDisable()
    {
    }

    public virtual void UpdateParameters()
    {
        if (((this.updateTextures && (this.redChannel != null)) && (this.greenChannel != null)) && (this.blueChannel != null))
        {
            Vector3 rgbC = new Vector3(0, 0, 0);
            Vector3 rgbDC = new Vector3(0, 0, 0);
            float zC = 0;
            float curveResolutionAsFloat = this.curveResolution;
            float curveStep = 1f / curveResolutionAsFloat;
            float step = 1f / 256f;
            float subStep = 0f;
            int texelIndex = 0;
            float i = 0f;
            while (i <= 1f)
            {
                rgbC.x = rgbC.x + Mathf.Clamp01(this.redChannel.Evaluate(i));
                rgbC.y = rgbC.y + Mathf.Clamp01(this.greenChannel.Evaluate(i));
                rgbC.z = rgbC.z + Mathf.Clamp01(this.blueChannel.Evaluate(i));
                zC = zC + Mathf.Clamp01(this.zCurve.Evaluate(i));
                rgbDC.x = rgbDC.x + Mathf.Clamp01(this.depthRedChannel.Evaluate(i));
                rgbDC.y = rgbDC.y + Mathf.Clamp01(this.depthGreenChannel.Evaluate(i));
                rgbDC.z = rgbDC.z + Mathf.Clamp01(this.depthBlueChannel.Evaluate(i));
                subStep = subStep + step;
                if (subStep >= curveStep)
                {
                    rgbC = rgbC * (step / curveStep);
                    zC = zC * (step / curveStep);
                    rgbDC = rgbDC * (step / curveStep);
                    this._rgbChannelTex.SetPixel(texelIndex, 0, new Color(rgbC.x, rgbC.x, rgbC.x));
                    this._rgbChannelTex.SetPixel(texelIndex, 1, new Color(rgbC.y, rgbC.y, rgbC.y));
                    this._rgbChannelTex.SetPixel(texelIndex, 2, new Color(rgbC.z, rgbC.z, rgbC.z));
                    this._zCurve.SetPixel(texelIndex, 0, new Color(zC, zC, zC));
                    this._rgbDepthChannelTex.SetPixel(texelIndex, 0, new Color(rgbDC.x, rgbDC.x, rgbDC.x));
                    this._rgbDepthChannelTex.SetPixel(texelIndex, 1, new Color(rgbDC.y, rgbDC.y, rgbDC.y));
                    this._rgbDepthChannelTex.SetPixel(texelIndex, 2, new Color(rgbDC.z, rgbDC.z, rgbDC.z));
                    rgbC = new Vector3(0, 0, 0);
                    rgbDC = new Vector3(0, 0, 0);
                    zC = 0;
                    texelIndex++;
                    subStep = 0f;
                }
                i = i + step;
            }
            this._rgbChannelTex.Apply();
            this._rgbDepthChannelTex.Apply();
            this._zCurve.Apply();
            this.updateTextures = false;
        }
    }

    public override void OnRenderImage2(RenderTexture source, RenderTexture destination)
    {
        this.CreateMaterials();
        this.UpdateParameters();
        // force disable anisotropic filtering
        if (this._rgbChannelTex)
        {
            this._rgbChannelTex.anisoLevel = 0;
        }
        if (this._rgbDepthChannelTex)
        {
            this._rgbDepthChannelTex.anisoLevel = 0;
        }
        RenderTexture renderTarget2Use = destination;
        if (this.selectiveCc)
        {
            // we need an extra render target
            renderTarget2Use = RenderTexture.GetTemporary(source.width, source.height);
        }
        if (this.useDepthCorrection)
        {
            this._ccDepthMaterial.SetTexture("_RgbTex", this._rgbChannelTex);
            this._ccDepthMaterial.SetTexture("_ZCurve", this._zCurve);
            this._ccDepthMaterial.SetTexture("_RgbDepthTex", this._rgbDepthChannelTex);
            Graphics.Blit(source, renderTarget2Use, this._ccDepthMaterial);
        }
        else
        {
            this._ccMaterial.SetTexture("_RgbTex", this._rgbChannelTex);
            Graphics.Blit(source, renderTarget2Use, this._ccMaterial);
        }
        if (this.selectiveCc)
        {
            this._selectiveCcMaterial.SetVector("selColor", new Vector4(this.selectiveFromColor.r, this.selectiveFromColor.g, this.selectiveFromColor.b, this.selectiveFromColor.a));
            this._selectiveCcMaterial.SetVector("targetColor", new Vector4(this.selectiveToColor.r, this.selectiveToColor.g, this.selectiveToColor.b, this.selectiveToColor.a));
            Graphics.Blit(renderTarget2Use, destination, this._selectiveCcMaterial);
            RenderTexture.ReleaseTemporary(renderTarget2Use);
        }
    }

    public AnimationCurve redChannel;
    public AnimationCurve greenChannel;
    public AnimationCurve blueChannel;
    public bool useDepthCorrection;
    public AnimationCurve zCurve;
    public AnimationCurve depthRedChannel;
    public AnimationCurve depthGreenChannel;
    public AnimationCurve depthBlueChannel;
    private Material _ccMaterial;
    private Material _ccDepthMaterial;
    private Material _selectiveCcMaterial;
    private Texture2D _rgbChannelTex;
    private Texture2D _rgbDepthChannelTex;
    private Texture2D _zCurve;
    public bool selectiveCc;
    public Color selectiveFromColor;
    public Color selectiveToColor;
    public bool updateTextures;
    public bool recreateTextures;
    public int curveResolution;
    public ColorCorrectionMode mode;
    public Shader colorCorrectionCurvesShader;
    public Shader simpleColorCorrectionCurvesShader;
    public Shader colorCorrectionSelectiveShader;
    public ColorCorrectionCurves()
    {
        this.selectiveFromColor = Color.white;
        this.selectiveToColor = Color.white;
        this.updateTextures = true;
        this.curveResolution = 256;
    }

}