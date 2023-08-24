using UnityEngine;
using System.Collections;

[System.Serializable]
public class DistanceFadeObject : object
{
    public Renderer renderer;
    public Transform transform;
    public Material[] materials;
    public Color[] colors;
    public string[] colorName;
    public Color fadeColor;
    public bool enabled;
    //@HideInInspector
    public float minDistance;
    //@HideInInspector
    public float maxDistance;
    public Shader fadeShader;
    //@HideInInspector
    public bool alphaMaterial;
    //@HideInInspector
    public int count;
    //@HideInInspector
    public float normalDistance;
    public virtual void Initialize(Renderer r)
    {
        this.renderer = r;
        this.alphaMaterial = false;
        this.enabled = this.renderer.enabled;
        this.transform = this.renderer.transform;
        this.count = this.renderer.sharedMaterials.Length;
        this.normalDistance = 1f / 10f;
        this.materials = new Material[this.count];
        //fadeMaterials = new Material[count];
        this.colors = new Color[this.count];
        //fadeColors = new Color[count];
        this.colorName = new string[this.count];
        int i = 0;
        while (i < this.count)
        {
            this.materials[i] = this.renderer.sharedMaterials[i];
            if (this.materials[i].HasProperty("_Color"))
            {
                this.colorName[i] = "_Color";
            }
            else
            {
                if (this.materials[i].HasProperty("_MainColor"))
                {
                    this.colorName[i] = "_MainColor";
                }
            }
            this.colors[i] = this.materials[i].GetColor(this.colorName[i]);
            i++;
        }
        this.fadeColor = this.colors[0];
    }

    public virtual void SetMaxDistance(float d)
    {
        this.maxDistance = d;
        this.minDistance = this.maxDistance - 5f;
        this.normalDistance = 1f / 5f;//(maxDistance - minDistance);
    }

    public virtual void Disable()
    {
        this.renderer.enabled = false;
        this.enabled = false;
    }

    public virtual void StartFade()
    {
        if (!this.enabled)
        {
            this.renderer.enabled = true;
            this.enabled = true;
        }
        if (!this.alphaMaterial)
        {
            this.alphaMaterial = true;
            int i = 0;
            while (i < this.count)
            {
                this.renderer.materials[i].shader = this.fadeShader;
                i++;
            }
        }
        this.fadeColor.a = 0f;
        int k = 0;
        while (k < this.count)
        {
            this.renderer.materials[k].SetColor("_Color", this.fadeColor);
            k++;
        }
    }

    public virtual bool DoFade(float deltaTime)
    {
        this.fadeColor.a = this.fadeColor.a + (0.5f * deltaTime);
        int k = 0;
        while (k < this.count)
        {
            this.renderer.materials[k].SetColor("_Color", this.fadeColor);
            k++;
        }
        if (this.fadeColor.a >= 1f)
        {
            if (this.alphaMaterial)
            {
                this.alphaMaterial = false;
                int j = 0;
                while (j < this.count)
                {
                    this.renderer.materials[j].shader = this.materials[j].shader;
                    this.renderer.materials[j].SetColor(this.colorName[j], this.colors[j]);
                    j++;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void DistanceBased(float d)
    {
        if (d > this.maxDistance)
        {
            if (this.enabled)
            {
                this.renderer.enabled = false;
                this.enabled = false;
            }
        }
        else
        {
            if (d > this.minDistance)
            {
                if (!this.enabled)
                {
                    this.renderer.enabled = true;
                    this.enabled = true;
                }
                if (!this.alphaMaterial)
                {
                    this.alphaMaterial = true;
                    int i = 0;
                    while (i < this.count)
                    {
                        this.renderer.materials[i].shader = this.fadeShader;
                        i++;
                    }
                }
                this.fadeColor.a = 1f - ((d - this.minDistance) * this.normalDistance);
                int k = 0;
                while (k < this.count)
                {
                    this.renderer.materials[k].SetColor("_Color", this.fadeColor);
                    k++;
                }
            }
            else
            {
                if (!this.enabled)
                {
                    this.renderer.enabled = true;
                    this.enabled = true;
                }
                if (this.alphaMaterial)
                {
                    this.alphaMaterial = false;
                    int j = 0;
                    while (j < this.count)
                    {
                        this.renderer.materials[j].shader = this.materials[j].shader;
                        this.renderer.materials[j].SetColor(this.colorName[j], this.colors[j]);
                        j++;
                    }
                }
            }
        }
    }

}