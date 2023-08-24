using UnityEngine;
using System.Collections;

[System.Serializable]
public class ShotLight : MonoBehaviour
{
    public float time;
    private float timer;
    public virtual void OnEnable()
    {
        if (this.GetComponent<Light>() == null)
        {
            UnityEngine.Object.Destroy(this);
        }
        else
        {
            this.timer = this.time;
            this.GetComponent<Light>().enabled = false;
        }
    }

    public virtual void OnDisable()
    {
        if (this.GetComponent<Light>() == null)
        {
            UnityEngine.Object.Destroy(this);
        }
        else
        {
            this.timer = this.time;
            this.GetComponent<Light>().enabled = false;
        }
    }

    public virtual void LateUpdate()
    {
        this.timer = this.timer - Time.deltaTime;
        if (this.timer <= 0f)
        {
            this.timer = this.time;
            this.GetComponent<Light>().enabled = !this.GetComponent<Light>().enabled;
        }
    }

    public ShotLight()
    {
        this.time = 0.02f;
    }

}