using UnityEngine;
using System.Collections;

[System.Serializable]
public class BlueLeafSound : MonoBehaviour
{
    public float delay;
    public virtual void Update()
    {
        if (this.delay > 0f)
        {
            this.delay = this.delay - Time.deltaTime;
            if (this.delay < 0f)
            {
                this.GetComponent<AudioSource>().Play();
            }
        }
    }

}