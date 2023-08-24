using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class CutsceneFoot : MonoBehaviour
{
    private float timer;
    public Collider[] cols;
    public virtual void Start()
    {
        int i = 0;
        while (i < this.cols.Length)
        {
            Physics.IgnoreCollision(this.cols[i], this.GetComponent<Collider>());
            i++;
        }
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (this.timer <= 0f)
        {
            this.timer = 0.2f;
            this.SendMessageUpwards("OnFootStrike", SendMessageOptions.DontRequireReceiver);
        }
    }

    public virtual void Update()
    {
        this.timer = this.timer - Time.deltaTime;
    }

}