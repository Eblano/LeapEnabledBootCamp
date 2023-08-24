using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class UVScroller_waterfall : MonoBehaviour
{
    // Scroll main texture based on time
    public float scrollSpeed;
    public virtual void Update()
    {
        float offset = Time.time * this.scrollSpeed;
        //renderer.material.SetTextureOffset ("_LightMap", Vector2(offset/20, offset));
        Material mat = this.GetComponent<Renderer>().material;
        mat.SetTextureOffset("_MainTex", new Vector2(offset / 3, offset));
        mat.SetTextureOffset("_BumpMap", new Vector2(offset / 3.5f, offset));
        mat.SetTextureOffset("_FoamTex", new Vector2(offset / 4, offset * 2));
    }

    public UVScroller_waterfall()
    {
        this.scrollSpeed = 0.1f;
    }

}