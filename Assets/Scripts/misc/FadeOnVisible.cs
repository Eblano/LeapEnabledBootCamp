using UnityEngine;
using System.Collections;

[System.Serializable]
public class FadeOnVisible : MonoBehaviour
{
    public float alpha;
    public Material[] originalMaterials;
    public Material[] fadeMaterials;
    public Color[] colors;
    public MeshRenderer r;
    public Shader fadeShader;
    private int mLength;
    private int i;
    public virtual void Start()
    {
        this.fadeShader = Shader.Find("Transparent/VertexLit");
        if (this.GetComponent<Renderer>() == null)
        {
            UnityEngine.Object.Destroy(this);
            return;
        }
        this.alpha = 0f;
        if (this.r == null)
        {
            this.r = this.GetComponent<MeshRenderer>();
            this.mLength = this.r.materials.Length;
            this.colors = new Color[this.mLength];
            this.originalMaterials = new Material[this.mLength];
            this.fadeMaterials = new Material[this.mLength];
            this.i = 0;
            while (this.i < this.mLength)
            {
                this.originalMaterials[this.i] = this.r.materials[this.i];
                this.colors[this.i] = this.r.materials[this.i].color;
                this.fadeMaterials[this.i] = new Material(this.fadeShader);
                this.fadeMaterials[this.i].color = this.colors[this.i];
                this.i++;
            }
            this.i = 0;
            while (this.i < this.mLength)
            {
                this.r.materials[this.i] = this.fadeMaterials[this.i];
                this.colors[this.i].a = 0f;
                this.i++;
            }
        }
    }

    public virtual void OnBecameVisible()
    {
        if ((!this.enabled && (this.alpha == 0f)) && (Time.time > 0.2f))
        {
            this.enabled = true;
        }
    }

    public virtual void OnBecameInvisible()
    {
        this.alpha = 0f;
        this.enabled = false;
        this.i = 0;
        while (this.i < this.mLength)
        {
            this.r.materials[this.i] = this.fadeMaterials[this.i];
            this.colors[this.i].a = 0f;
            this.i++;
        }
    }

    public virtual void Update()
    {
        this.alpha = this.alpha + Time.deltaTime;
        this.i = 0;
        while (this.i < this.mLength)
        {
            this.colors[this.i].a = this.alpha;
            this.fadeMaterials[this.i].color = this.colors[this.i];
            this.i++;
        }
        if (this.alpha >= 1f)
        {
            this.i = 0;
            while (this.i < this.mLength)
            {
                this.colors[this.i].a = 1f;
                this.r.materials[this.i] = this.originalMaterials[this.i];
                this.r.materials[this.i].color = this.colors[this.i];
                this.i++;
            }
            this.alpha = 1f;
            this.enabled = false;
        }
    }

}