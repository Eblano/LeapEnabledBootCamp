using UnityEngine;
using System.Collections;

[System.Serializable]
public class Destructable : MonoBehaviour
{
    public virtual void Destruct()
    {
        if (this.transform.childCount > 0)
        {
            Transform t = null;
            int i = 0;
            while (i < this.transform.childCount)
            {
                t = this.transform.GetChild(i);
                t.parent = null;
                t.gameObject.active = true;
                if (t.GetComponent<Renderer>() != null)
                {
                    t.GetComponent<Renderer>().enabled = true;
                }
                if (t.GetComponent<Rigidbody>() != null)
                {
                    t.GetComponent<Rigidbody>().isKinematic = false;
                }
                if (t.gameObject.GetComponent("TrainingDummyPartDestructor") == null)
                {
                    t.gameObject.AddComponent<TrainingDummyPartDestructor>();
                }
                i++;
            }
            if (this.transform.parent != null)
            {
                UnityEngine.Object.Destroy(this.transform.parent.gameObject);
            }
            else
            {
                UnityEngine.Object.Destroy(this.gameObject);
            }
        }
    }

}