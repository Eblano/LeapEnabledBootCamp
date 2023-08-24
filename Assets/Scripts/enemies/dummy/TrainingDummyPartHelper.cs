using UnityEngine;
using System.Collections;

[System.Serializable]
public class TrainingDummyPartHelper : MonoBehaviour
{
    public bool attached;
    public int index;
    public TrainingDummy dummy;
    public virtual void Hit(RaycastHit hit)
    {
        if (this.dummy != null)
        {
            this.dummy.Hit(hit, this.index);
        }
    }

    public virtual void Destruct()
    {
        if (this.dummy != null)
        {
            this.dummy.Destruct(this.index);
        }
    }

}