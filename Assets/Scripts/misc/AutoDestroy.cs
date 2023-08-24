using UnityEngine;
using System.Collections;

[System.Serializable]
public class AutoDestroy : MonoBehaviour
{
    public float time;
    public virtual void Update()
    {
        this.time = this.time - Time.deltaTime;
        if (this.time < 0f)
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }

}