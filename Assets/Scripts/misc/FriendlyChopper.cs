using UnityEngine;
using System.Collections;

[System.Serializable]
public class FriendlyChopper : MonoBehaviour
{
    public SargeManager sarge;
    public virtual void Start()
    {
        GameObject sargeObject = GameObject.Find("SargeManager") as GameObject;
        if (sargeObject != null)
        {
            this.sarge = sargeObject.GetComponent("SargeManager") as SargeManager;
        }
    }

    public virtual void Hit(RaycastHit hitInfo)
    {
        if (this.sarge == null)
        {
            return;
        }
        this.sarge.FriendlyFire();
    }

}