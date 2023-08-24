using UnityEngine;
using System.Collections;

[System.Serializable]
public class SimpleRotation : MonoBehaviour
{
    public DesiredAxis desiredAxis;
    public float visualSlowSpeed;
    public int framePerVisualRotation;
    private Transform t;
    private Vector3 axis;
    public virtual void Start()
    {
        this.t = this.transform;
        this.axis = new Vector3(1, 0, 0);
        switch (this.desiredAxis)
        {
            case DesiredAxis.Y:
                this.axis = new Vector3(0, 1, 0);
                break;
            case DesiredAxis.Z:
                this.axis = new Vector3(0, 0, 1);
                break;
        }
    }

    public virtual void Update()
    {
        if (GameManager.pause)
        {
            return;
        }
        this.t.Rotate(this.axis * (((this.visualSlowSpeed * 360f) * Time.deltaTime) + (360f / this.framePerVisualRotation)));
    }

    public SimpleRotation()
    {
        this.visualSlowSpeed = 0.5f;
        this.framePerVisualRotation = 4;
    }

}
public enum DesiredAxis
{
    X = 0,
    Y = 1,
    Z = 2
}