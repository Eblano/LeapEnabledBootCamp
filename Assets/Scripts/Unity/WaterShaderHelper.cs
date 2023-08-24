using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class WaterShaderHelper : MonoBehaviour
{
    public Transform lightDir;

    void Update()
    {
        Material mat = GetComponent<Renderer>().material;
        mat.shader.maximumLOD = GameQualitySettings.water == 1 ? 600 : 300;

        if (lightDir != null)
            mat.SetVector("_WorldLightDir", lightDir.forward);
        else
            mat.SetVector("_WorldLightDir", new Vector3(0.7f, 0.7f, 0.0f));
    }
}