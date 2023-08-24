using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.AddComponentMenu("Aquiris/Graphics/UVScroller")]
public partial class UVScroller : MonoBehaviour
{
    public Vector2 scrollSpeed;
    public string[] texturesToScroll;
    private Vector2 offset;
    private Renderer cRenderer;
    public virtual void Start()
    {
        if (this.GetComponent<Renderer>() == null)
        {
            Debug.LogWarning(("UVScroller[\"" + this.gameObject.name) + "\"]: There is no renderer attached to the gameObject.");
            UnityEngine.Object.Destroy(this);
            return;
        }
        else
        {
            if (this.texturesToScroll == null)
            {
                Debug.LogWarning(("UVScroller[\"" + this.gameObject.name) + "\"]: You need to specify at least one texture to scroll.");
                UnityEngine.Object.Destroy(this);
                return;
            }
            else
            {
                if (this.texturesToScroll.Length <= 0)
                {
                    Debug.LogWarning(("UVScroller[\"" + this.gameObject.name) + "\"]: You need to specify at least one texture to scroll.");
                    UnityEngine.Object.Destroy(this);
                    return;
                }
            }
        }
        this.offset = Vector2.zero;
        this.cRenderer = this.GetComponent<Renderer>();
    }

    public virtual void Update()
    {
        this.offset = this.offset + ((this.scrollSpeed * 0.1f) * Time.deltaTime);
        foreach (string textureName in this.texturesToScroll)
        {
            this.cRenderer.material.SetTextureOffset(textureName, new Vector2(this.offset.x, this.offset.y));
        }
    }

}