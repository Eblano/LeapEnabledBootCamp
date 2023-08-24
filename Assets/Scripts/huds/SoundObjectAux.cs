using UnityEngine;
using System.Collections;

[System.Serializable]
public class SoundObjectAux : MonoBehaviour
{
    public SoundObject soundGenerator;
    public virtual void Awake()
    {
        if (this.GetComponent<Rigidbody>() != null)
        {
            this.GetComponent<Rigidbody>().Sleep();
        }
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (this.soundGenerator == null)
        {
            UnityEngine.Object.Destroy(this);
        }
        else
        {
            this.soundGenerator.OnCollisionEnter(collision);
        }
    }

}