using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.ExecuteInEditMode]
[UnityEngine.RequireComponent(typeof(Camera))]
[UnityEngine.AddComponentMenu("Image Effects/Edge Detection (Depth-Normals)")]
public partial class EdgeDetectEffectNormals : PostEffectsBase
{
    public Shader edgeDetectHqShader;
    private Material _edgeDetectHqMaterial;
    public Shader edgeDetectShader;
    private Material _edgeDetectMaterial;
    public Shader sepBlurShader;
    private Material _sepBlurMaterial;
    public Shader edgeApplyShader;
    private Material _edgeApplyMaterial;
    public virtual void CreateMaterials()
    {
        if (!this._edgeDetectHqMaterial)
        {
            if (!this.CheckShader(this.edgeDetectHqShader))
            {
                this.enabled = false;
                return;
            }
            this._edgeDetectHqMaterial = new Material(this.edgeDetectHqShader);
            this._edgeDetectHqMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._edgeDetectMaterial)
        {
            if (!this.CheckShader(this.edgeDetectShader))
            {
                this.enabled = false;
                return;
            }
            this._edgeDetectMaterial = new Material(this.edgeDetectShader);
            this._edgeDetectMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._sepBlurMaterial)
        {
            if (!this.CheckShader(this.sepBlurShader))
            {
                this.enabled = false;
                return;
            }
            this._sepBlurMaterial = new Material(this.sepBlurShader);
            this._sepBlurMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._edgeApplyMaterial)
        {
            if (!this.CheckShader(this.edgeApplyShader))
            {
                this.enabled = false;
                return;
            }
            this._edgeApplyMaterial = new Material(this.edgeApplyShader);
            this._edgeApplyMaterial.hideFlags = HideFlags.HideAndDontSave;
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
        this.GetComponent<Camera>().depthTextureMode = this.GetComponent<Camera>().depthTextureMode | DepthTextureMode.DepthNormals;
    }

    public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Vector2 sensitivity = default(Vector2);
        this.CreateMaterials();
        sensitivity.x = this.sensitivityDepth;
        sensitivity.y = this.sensitivityNormals;
        if (this.highQuality)
        {
            RenderTexture lrTex1 = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0);
            RenderTexture lrTex2 = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0);
            this._edgeDetectHqMaterial.SetVector("sensitivity", new Vector4(sensitivity.x, sensitivity.y, Mathf.Max(0.1f, this.spread), sensitivity.y));
            this._edgeDetectHqMaterial.SetFloat("edgesOnly", this.edgesOnly);
            Vector4 vecCol = this.edgesOnlyBgColor;
            this._edgeDetectHqMaterial.SetVector("edgesOnlyBgColor", vecCol);
            Graphics.Blit(source, source, this._edgeDetectHqMaterial); // writes edges into .a
            if (this.edgeBlur)
            {
                Graphics.Blit(source, lrTex1);
                int i = 0;
                while (i < this.blurIterations)
                {
                    this._sepBlurMaterial.SetVector("offsets", new Vector4(0f, this.blurSpread / lrTex1.height, 0f, 0f));
                    Graphics.Blit(lrTex1, lrTex2, this._sepBlurMaterial);
                    this._sepBlurMaterial.SetVector("offsets", new Vector4(this.blurSpread / lrTex1.width, 0f, 0f, 0f));
                    Graphics.Blit(lrTex2, lrTex1, this._sepBlurMaterial);
                    i++;
                }
                this._edgeApplyMaterial.SetTexture("_EdgeTex", lrTex1);
                this._edgeApplyMaterial.SetFloat("edgesIntensity", this.edgesIntensity);
                Graphics.Blit(source, destination, this._edgeApplyMaterial);
            }
            else
            {
                this._edgeApplyMaterial.SetTexture("_EdgeTex", source);
                this._edgeApplyMaterial.SetFloat("edgesIntensity", this.edgesIntensity);
                Graphics.Blit(source, destination, this._edgeApplyMaterial);
            }
            RenderTexture.ReleaseTemporary(lrTex1);
            RenderTexture.ReleaseTemporary(lrTex2);
        }
        else
        {
            Graphics.Blit(source, destination, this._edgeDetectMaterial);
        }
    }

    public bool highQuality;
    public float sensitivityDepth;
    public float sensitivityNormals;
    public float spread;
    public float edgesIntensity;
    public float edgesOnly;
    public Color edgesOnlyBgColor;
    public bool edgeBlur;
    public float blurSpread;
    public int blurIterations;
    public EdgeDetectEffectNormals()
    {
        this.sensitivityDepth = 1f;
        this.sensitivityNormals = 1f;
        this.spread = 1f;
        this.edgesIntensity = 1f;
        this.edgesOnlyBgColor = Color.white;
        this.blurSpread = 0.75f;
        this.blurIterations = 1;
    }

}