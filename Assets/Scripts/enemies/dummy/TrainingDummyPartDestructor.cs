using UnityEngine;
using System.Collections;

[System.Serializable]
public class TrainingDummyPartDestructor : MonoBehaviour
{
    private float alpha;
    private float timer;
    private Color color;
    private MeshRenderer r;
    private string colorName;
    public virtual void Start()
    {
        this.r = this.GetComponent<Renderer>();
        if (this.r.material.HasProperty("_MainColor"))
        {
            this.colorName = "_MainColor";
        }
        else
        {
            if (this.r.material.HasProperty("_Color"))
            {
                this.colorName = "_Color";
            }
            else
            {
                UnityEngine.Object.Destroy(this.gameObject);
                return;
            }
        }
        this.color = this.r.material.GetColor(this.colorName);
        this.alpha = 1f;
        this.timer = 3f;
    }

    public virtual void Update()
    {
        if (this.timer > 0f)
        {
            this.timer = this.timer - Time.deltaTime;
        }
        else
        {
            if (this.alpha > 0f)
            {
                this.alpha = this.alpha - Time.deltaTime;
                this.color.a = this.alpha;
                this.r.material.SetColor(this.colorName, this.color);
            }
            else
            {
                UnityEngine.Object.Destroy(this.gameObject);
            }
        }
    }

}