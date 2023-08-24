using UnityEngine;
using System.Collections;

[System.Serializable]
public class SetChildrenTag : MonoBehaviour
{
    public string desiredTag;
    public virtual void Start()
    {
        if (string.IsNullOrEmpty(this.desiredTag))
        {
            return;
        }
        this.gameObject.tag = this.desiredTag;
        int i = 0;
        while (i < this.transform.childCount)
        {
            this.SetTag(this.transform.GetChild(i));
            i++;
        }
    }

    public virtual void SetTag(Transform t)
    {
        t.tag = this.desiredTag;
        int i = 0;
        while (i < t.childCount)
        {
            this.SetTag(t.GetChild(i));
            i++;
        }
    }

}