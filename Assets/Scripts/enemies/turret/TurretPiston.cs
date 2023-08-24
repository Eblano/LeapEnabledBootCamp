using UnityEngine;
using System.Collections;

[System.Serializable]
public class TurretPiston : MonoBehaviour
{
    public Transform objectToFollow;
    private Transform t;
    public Vector3 axis;
    public virtual void Start()
    {
        if (this.objectToFollow == null)
        {
            UnityEngine.Object.Destroy(this.gameObject);
            return;
        }
        this.t = this.transform;
    }

    public virtual void Update()
    {
        this.t.rotation = Quaternion.FromToRotation(this.axis, (this.objectToFollow.position - this.t.position).normalized);
    }

    public TurretPiston()
    {
        this.axis = new Vector3(1, 0, 0);
    }

}