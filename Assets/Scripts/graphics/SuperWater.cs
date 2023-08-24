using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.ExecuteInEditMode]
public partial class SuperWater : MonoBehaviour
{
    public Transform theLight;
    public virtual void Update()
    {
        this.GetComponent<Renderer>().sharedMaterial.SetVector("_WorldLightDir", -this.theLight.forward);
    }

}