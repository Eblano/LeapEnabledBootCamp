using UnityEngine;
using System.Collections;

[System.Serializable]
public class Grenade : MonoBehaviour
{
    private Transform thisTransform;
    public float minY;
    public GameObject smoke;
    public GameObject explosionEmitter;
    public float explosionTime;
    public float explosionRadius;
    public float power;
    private float timer;
    public SoldierCamera soldierCamera;
    public AudioClip[] nearSounds;
    public AudioClip[] farSounds;
    public float farSoundDistance;
    private bool exploded;
    private RaycastHit hit;
    public virtual void Start()
    {
        this.exploded = false;
        this.timer = 0f;
        this.thisTransform = this.transform;
    }

    public virtual void Detonate()
    {
        if (this.exploded)
        {
            return;
        }
        this.exploded = true;
        if (this.GetComponent<Renderer>() != null)
        {
            this.GetComponent<Renderer>().enabled = false;
            if (this.smoke != null)
            {
                UnityEngine.Object.Destroy(this.smoke);
            }
        }
        else
        {
            Component[] renderers = this.GetComponentsInChildren(typeof(Renderer));
            foreach (Renderer r in renderers)
            {
                r.enabled = false;
            }
        }
        Vector3 _explosionPosition = this.thisTransform.position;
        Collider[] col = Physics.OverlapSphere(_explosionPosition, this.explosionRadius);
        float distance = Vector3.Distance(this.soldierCamera.transform.position, _explosionPosition);
        this.soldierCamera.StartShake(distance);
        Rigidbody body = null;
        if (!(col == null))
        {
            int c = 0;
            while (c < col.Length)
            {
                col[c].gameObject.SendMessage("Destruct", SendMessageOptions.DontRequireReceiver);
                body = null;
                body = col[c].gameObject.GetComponent<Rigidbody>();
                if (body != null)
                {
                    body.isKinematic = false;
                }
                else
                {
                    if (col[c].gameObject.transform.parent != null)
                    {
                        body = col[c].gameObject.transform.parent.GetComponent<Rigidbody>();
                        if (body != null)
                        {
                            body.isKinematic = false;
                        }
                    }
                }
                if (body != null)
                {
                    body.AddExplosionForce(this.power, _explosionPosition, this.explosionRadius, 3f);
                }
                if (col[c].GetComponent<Collider>().tag == "glass")
                {
                    col[c].gameObject.SendMessage("BreakAll", SendMessageOptions.DontRequireReceiver);
                }
                c++;
            }
        }
        this.gameObject.SendMessage("Explode", SendMessageOptions.DontRequireReceiver);
        this.PlaySound(distance);
        if (this.explosionEmitter != null)
        {
            GameObject.Instantiate(this.explosionEmitter, this.transform.position, Quaternion.identity);
        }
    }

    public virtual void PlaySound(float distance)
    {
        int sIndex = 0;
        if (distance < this.farSoundDistance)
        {
            sIndex = Random.Range(0, this.nearSounds.Length);
            this.GetComponent<AudioSource>().PlayOneShot(this.nearSounds[sIndex]);
            this.timer = this.nearSounds[sIndex].length + 1f;
        }
        else
        {
            sIndex = Random.Range(0, this.farSounds.Length);
            this.GetComponent<AudioSource>().PlayOneShot(this.farSounds[sIndex]);
            this.timer = this.farSounds[sIndex].length + 1f;
        }
    }

    public virtual void Update()
    {
        if (this.thisTransform.position.y < this.minY)
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }
        if (this.exploded)
        {
            if (this.timer > 0f)
            {
                this.timer = this.timer - Time.deltaTime;
                if (this.timer <= 0f)
                {
                    UnityEngine.Object.Destroy(this.gameObject);
                }
            }
        }
    }

    public virtual void OnCollisionEnter(Collision c)
    {
        if (this.exploded)
        {
            return;
        }
        this.Detonate();
    }

    public virtual void OnCollisionStay(Collision c)
    {
        if (this.exploded)
        {
            return;
        }
        this.Detonate();
    }

    public virtual void OnCollisionExit(Collision c)
    {
        if (this.exploded)
        {
            return;
        }
        this.Detonate();
    }

    public Grenade()
    {
        this.minY = -10f;
        this.power = 3200;
        this.farSoundDistance = 25f;
    }

}