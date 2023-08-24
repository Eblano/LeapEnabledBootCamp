using UnityEngine;
using System.Collections;

[System.Serializable]
public class EaglesEye : MonoBehaviour
{
    private bool registered;
    public virtual void Start()
    {
        this.registered = false;
    }

    public virtual void Update()
    {
        if (!this.registered)
        {
            TrainingStatistics.totalEaglesEye++;
            this.registered = true;
        }
    }

    public virtual void Hit(RaycastHit hit)
    {
        TrainingStatistics.eaglesEye++;
        UnityEngine.Object.Destroy(this);
    }

}