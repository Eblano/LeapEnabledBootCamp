using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class WaterShaderHelper : MonoBehaviour
{
    public Transform lightDir;
    public virtual void Update()
    {
        Material mat = this.GetComponent<Renderer>().material;
        mat.shader.maximumLOD = GameQualitySettings.water ? 600 : 300;
        if (this.lightDir)
        {
            mat.SetVector("_WorldLightDir", this.lightDir.forward);
        }
        else
        {
            mat.SetVector("_WorldLightDir", new Vector3(0.7f, 0.7f, 0f));
        }
    }

}