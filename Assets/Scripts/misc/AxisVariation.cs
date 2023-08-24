using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class AxisVariation : MonoBehaviour
{
    public Vector3 amplitude;
    public float speed;
    private Vector3 startPosition;
    private float runningTime;
    public virtual void OnEnable()
    {
        this.runningTime = Mathf.PI / this.speed;// * 0.5;
        this.startPosition = this.transform.localPosition;
    }

    public virtual void LateUpdate()
    {
        if (GameManager.pause)
        {
            return;
        }
        this.transform.localPosition = this.startPosition + (Mathf.Sin(this.runningTime * this.speed) * this.amplitude);
        this.runningTime = this.runningTime + Time.deltaTime;
    }

}