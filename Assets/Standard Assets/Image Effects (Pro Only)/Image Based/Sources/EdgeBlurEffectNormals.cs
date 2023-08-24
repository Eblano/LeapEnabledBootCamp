using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.ExecuteInEditMode]
[UnityEngine.RequireComponent(typeof(Camera))]
[UnityEngine.AddComponentMenu("Image Effects/Edge Blur")]
public partial class EdgeBlurEffectNormals : PostEffectsBase
{
    public Shader edgeDetectHqShader;
    private Material _edgeDetectHqMaterial;
    public Shader edgeBlurApplyShader;
    private Material _edgeBlurApplyMaterial;
    public Shader showAlphaChannelShader;
    private Material _showAlphaChannelMaterial;
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
        if (!this._edgeBlurApplyMaterial)
        {
            if (!this.CheckShader(this.edgeBlurApplyShader))
            {
                this.enabled = false;
                return;
            }
            this._edgeBlurApplyMaterial = new Material(this.edgeBlurApplyShader);
            this._edgeBlurApplyMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!this._showAlphaChannelMaterial)
        {
            if (!this.CheckShader(this.showAlphaChannelShader))
            {
                this.enabled = false;
                return;
            }
            this._showAlphaChannelMaterial = new Material(this.showAlphaChannelShader);
            this._showAlphaChannelMaterial.hideFlags = HideFlags.HideAndDontSave;
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
        this._edgeDetectHqMaterial.SetVector("sensitivity", new Vector4(sensitivity.x, sensitivity.y, Mathf.Max(0.1f, this.edgeDetectSpread), sensitivity.y));
        this._edgeDetectHqMaterial.SetFloat("edgesOnly", 0f);
        this._edgeDetectHqMaterial.SetVector("edgesOnlyBgColor", Vector4.zero);
        Graphics.Blit(source, source, this._edgeDetectHqMaterial);
        if (this.showEdges)
        {
            Graphics.Blit(source, destination, this._showAlphaChannelMaterial);
        }
        else
        {
            this._edgeBlurApplyMaterial.SetTexture("_EdgeTex", source);
            this._edgeBlurApplyMaterial.SetFloat("filterRadius", this.filterRadius);
            Graphics.Blit(source, destination, this._edgeBlurApplyMaterial);
            int its = this.iterations - 1;
            if (its < 0)
            {
                its = 0;
            }
            if (its > 5)
            {
                its = 5;
            }
            while (its > 0)
            {
                Graphics.Blit(destination, source, this._edgeBlurApplyMaterial);
                this._edgeBlurApplyMaterial.SetTexture("_EdgeTex", source);
                this._edgeBlurApplyMaterial.SetFloat("filterRadius", this.filterRadius);
                Graphics.Blit(source, destination, this._edgeBlurApplyMaterial);
                its--;
            }
        }
    }

    public float sensitivityDepth;
    public float sensitivityNormals;
    public float edgeDetectSpread;
    public float filterRadius;
    public bool showEdges;
    public int iterations;
    public EdgeBlurEffectNormals()
    {
        this.sensitivityDepth = 1f;
        this.sensitivityNormals = 1f;
        this.edgeDetectSpread = 0.9f;
        this.filterRadius = 0.8f;
        this.iterations = 1;
    }

}