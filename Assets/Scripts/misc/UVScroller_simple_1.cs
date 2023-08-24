using UnityEngine;
using System.Collections;


public partial class UVScroller_simple_1 : MonoBehaviour
{
    public float scrollSpeed = 0.1f;

    private void Update()
    {
        float offset = Time.time * scrollSpeed;
        //renderer.material.SetTextureOffset ("_LightMap", new Vector2(offset / 20, offset));

        GetComponent<Renderer>().material.SetTextureOffset("_BumpMap", new Vector2(offset / 3, offset / -3));
    }
}