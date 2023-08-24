using UnityEngine;
using System.Collections;

[System.Serializable]
public class DestroyOnHit : MonoBehaviour
{
    public int hitsToDestroy;
    public GameObject destructionParticles;
    public bool destroyOnExplosion;
    public bool destroyParent;
    public virtual void Start()
    {
        this.gameObject.layer = 11;
    }

    public virtual void Destruct()
    {
        if (this.destroyOnExplosion)
        {
            this.DestroyObject();
        }
    }

    public virtual void Hit(RaycastHit hit)
    {
        this.hitsToDestroy--;
        if (this.hitsToDestroy <= 0)
        {
            this.DestroyObject();
        }
    }

    public virtual void DestroyObject()
    {
        if (this.destructionParticles != null)
        {
            GameObject.Instantiate(this.destructionParticles, this.transform.position, Quaternion.identity);
        }
        if (this.destroyParent)
        {
            if (this.transform.parent != null)
            {
                UnityEngine.Object.Destroy(this.transform.parent.gameObject);
            }
            else
            {
                UnityEngine.Object.Destroy(this.gameObject);
            }
        }
        else
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }

    public DestroyOnHit()
    {
        this.hitsToDestroy = 1;
        this.destroyOnExplosion = true;
        this.destroyParent = true;
    }

}