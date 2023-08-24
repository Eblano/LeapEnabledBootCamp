using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class UVScroller_simple : MonoBehaviour
{
    // Scroll main texture based on time
    public float scrollSpeed;
    public virtual void Update()
    {
        float offset = Time.time * this.scrollSpeed;
        //renderer.material.SetTextureOffset ("_LightMap", Vector2(offset/20, offset));
        this.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(offset / 3, offset / -3));
    }

    public UVScroller_simple()
    {
        this.scrollSpeed = 0.1f;
    }

}