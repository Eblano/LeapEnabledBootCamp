using UnityEngine;
using System.Collections;

[System.Serializable]
public class CullLayer : object
{
    public int layer;
    public float distance;
}
[System.Serializable]
public class CullLayers : MonoBehaviour
{
    public CullLayer[] layers;
    public Camera[] cameras;
    public virtual void Start()
    {
        float[] distances = new float[32];
        int i = 0;
        while (i < this.layers.Length)
        {
            if (this.layers[i].layer < 32)
            {
                distances[this.layers[i].layer] = Mathf.Max(0f, this.layers[i].distance);
            }
            i++;
        }
        if (!(this.cameras == null))
        {
            int j = 0;
            while (j < this.cameras.Length)
            {
                if (this.cameras[j] != null)
                {
                    this.cameras[j].layerCullDistances = distances;
                }
                j++;
            }
        }
    }

}