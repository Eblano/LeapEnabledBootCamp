using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class AssignSkybox : MonoBehaviour
{
    public Material skybox;
    public virtual void Awake()// RenderSettings.skybox = skybox;
    {
    }

    public virtual void DoJoachimsSkyboxThing()
    {
        RenderSettings.skybox = this.skybox;
    }

}