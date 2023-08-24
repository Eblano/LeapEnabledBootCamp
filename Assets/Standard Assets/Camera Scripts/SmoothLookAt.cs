using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class SmoothLookAt : MonoBehaviour
{
    public Transform target;
    public float damping;
    public bool smooth;
    private float alpha;
    public float minDistance;
    private Color color;
    public string property;
    //@script AddComponentMenu("Camera-Control/Smooth Look At")
    public virtual void LateUpdate()
    {
        if (this.target)
        {
            if (this.smooth)
            {
                 // Look at and dampen the rotation
                Quaternion rotation = Quaternion.LookRotation(this.target.position - this.transform.position);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, Time.deltaTime * this.damping);
            }
            else
            {
                 // Just lookat
                this.transform.rotation = Quaternion.FromToRotation(-Vector3.forward, (new Vector3(this.target.position.x, this.transform.position.y, this.target.position.z) - this.transform.position).normalized);
                float distance = (this.target.position - this.transform.position).magnitude;
                if (distance < this.minDistance)
                {
                    this.alpha = Mathf.Lerp(this.alpha, 0f, Time.deltaTime * 2f);
                }
                else
                {
                    this.alpha = Mathf.Lerp(this.alpha, 1f, Time.deltaTime * 2f);
                }
                if (!string.IsNullOrEmpty(this.property))
                {
                    this.color.a = Mathf.Clamp(this.alpha, 0f, 1f);
                    this.GetComponent<Renderer>().material.SetColor(this.property, this.color);
                }
            }
        }
    }

    public virtual void Start()
    {
        if (this.GetComponent<Renderer>().material.HasProperty(this.property))
        {
            this.color = this.GetComponent<Renderer>().material.GetColor(this.property);
        }
        else
        {
            this.property = "";
        }
        // Make the rigid body not change rotation
        if (this.GetComponent<Rigidbody>())
        {
            this.GetComponent<Rigidbody>().freezeRotation = true;
        }
    }

    public SmoothLookAt()
    {
        this.damping = 6f;
        this.smooth = true;
        this.alpha = 1f;
        this.minDistance = 10f;
    }

}