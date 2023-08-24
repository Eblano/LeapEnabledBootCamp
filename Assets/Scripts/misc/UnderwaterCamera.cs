using UnityEngine;
using System.Collections;

[System.Serializable]
public class UnderwaterCamera : MonoBehaviour
{
    public MonoBehaviour[] effectComponents;
    public LayerMask waterLayer;
    private Transform thisT;
    private bool effectState;
    private RaycastHit hitInfo;
    public virtual void OnEnable()
    {
        this.effectState = false;
        if (this.effectComponents == null)
        {
            UnityEngine.Object.Destroy(this);
            return;
        }
        if (this.effectComponents.Length <= 0)
        {
            UnityEngine.Object.Destroy(this);
            return;
        }
        int i = 0;
        while (i < this.effectComponents.Length)
        {
            this.effectComponents[i].enabled = false;
            i++;
        }
        this.thisT = this.transform;
    }

    public virtual void OnDisable()
    {
        this.SwitchEffect(false);
    }

    public virtual void Update()
    {
        if (this.thisT == null)
        {
            return;
        }
        if (!GameQualitySettings.underwater)
        {
            this.SwitchEffect(false);
            return;
        }
        if (Physics.Raycast(this.thisT.position + new Vector3(0, 4, 0), -Vector3.up, out this.hitInfo, 4f, (int) this.waterLayer))
        {
            if (this.hitInfo.collider.tag == "water")
            {
                this.SwitchEffect(true);
            }
            else
            {
                this.SwitchEffect(false);
            }
        }
        else
        {
            this.SwitchEffect(false);
        }
    }

    public virtual void SwitchEffect(bool b)
    {
        if (b == this.effectState)
        {
            return;
        }
        this.effectState = b;
        int i = 0;
        while (i < this.effectComponents.Length)
        {
            this.effectComponents[i].enabled = b;
            i++;
        }
    }

}