using UnityEngine;
using System.Collections;

[System.Serializable]
public class TurretPart : MonoBehaviour
{
    public Turret turret;
    public virtual void Destruct()
    {
        if (this.turret != null)
        {
            this.turret.Destruct();
        }
        UnityEngine.Object.Destroy(this);
    }

    public virtual void Hit(RaycastHit hit)
    {
        if (this.turret != null)
        {
            this.turret.Hit();
        }
    }

}