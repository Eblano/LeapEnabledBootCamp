using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Turret : MonoBehaviour
{
    public Transform[] parts;
    public bool dead;
    public Transform target;
    public Transform vertical;
    public Transform horizontal;
    public Vector2 speed;
    public float minAngle;
    public float maxAngle;
    public LayerMask hitLayer;
    private float timerToCreateDecal;
    public float shootDelay;
    private float _shootDelay;
    public int shootBurst;
    private int _shootBurst;
    public GameObject weapon;
    public GameObject woodParticle;
    public GameObject metalParticle;
    public GameObject sandParticle;
    public GameObject concreteParticle;
    public GameObject bulletMark;
    public float shootRange;
    public float activeDistance;
    public float disableDistance;
    public float pushPower;
    public GunParticles shotingEmitter;
    public ShotLight shotLight;
    private bool shooting;
    private float shootTime;
    private bool running;
    public Shader fadeShader;
    public virtual void Start()
    {
        this.running = false;
        this.activeDistance = this.activeDistance * this.activeDistance;
        this.dead = false;
        if (this.target == null)
        {
            this.target = SoldierController.enemiesReference;
        }
        this.GunEffects(false);
    }

    public virtual void Update()
    {
        Vector3 hDir = default(Vector3);
        if (GameManager.pause || GameManager.scores)
        {
            return;
        }
        if (this.dead)
        {
            UnityEngine.Object.Destroy(this);
        }
        if (this.target == null)
        {
            this.target = SoldierController.enemiesReference;
        }
        if (this.target == null)
        {
            return;
        }
        if (!this.running)
        {
            if ((this.transform.position - this.target.position).sqrMagnitude < this.activeDistance)
            {
                this.running = true;
            }
            else
            {
                return;
            }
        }
        if ((this.transform.position - this.target.position).magnitude > this.disableDistance)
        {
            this.GunEffects(false);
            return;
        }
        this.timerToCreateDecal = this.timerToCreateDecal - Time.deltaTime;
        if (!this.shooting)
        {
            this._shootDelay = this._shootDelay - Time.deltaTime;
            if (this._shootDelay <= 0f)
            {
                this.shooting = true;
            }
        }
        if (this.horizontal != null)
        {
            hDir = (new Vector3(this.target.position.x, this.horizontal.position.y, this.target.position.z) - this.horizontal.position).normalized;
            this.horizontal.rotation = Quaternion.Slerp(this.horizontal.rotation, Quaternion.FromToRotation(Vector3.right, hDir), this.speed.x * Time.deltaTime);
        }
        if (this.vertical != null)
        {
            hDir = (new Vector3(this.target.position.x, this.vertical.position.y, this.target.position.z) - this.vertical.position).normalized;
            Vector3 vDir = (this.target.position - this.vertical.position).normalized;
            float angle = (vDir.y < 0f ? -1f : 1f) * Vector3.Angle(hDir, vDir);
            if (angle < this.minAngle)
            {
                angle = this.minAngle;
            }
            else
            {
                if (angle > this.maxAngle)
                {
                    angle = this.maxAngle;
                }
            }
            this.vertical.localRotation = Quaternion.Slerp(this.vertical.localRotation, Quaternion.Euler(new Vector3(0, 0, angle)), this.speed.y * Time.deltaTime);
        }
        this.ShootTheTarget();
    }

    public virtual void ShootTheTarget()
    {
        RaycastHit hit = default(RaycastHit);
         //if(GameManager.pause || GameManager.scores) return;
        if (!this.shooting)
        {
            return;
        }
        if (this.weapon == null)
        {
            return;
        }
        if (this.shootTime < Time.time)
        {
            this._shootBurst = this._shootBurst + 1;
            this.GunEffects(true);
            if (Physics.Raycast(this.weapon.transform.position, this.weapon.transform.forward, out hit, this.shootRange, (int) this.hitLayer))
            {
                if (hit.collider.tag == "glass")
                {
                    hit.collider.gameObject.SendMessage("Hit", hit, SendMessageOptions.DontRequireReceiver);
                    if (Physics.Raycast(hit.point + (this.weapon.transform.forward * 0.05f), this.weapon.transform.forward, out hit, this.shootRange - hit.distance, (int) this.hitLayer))
                    {
                        hit.collider.gameObject.SendMessage("HitSoldier", "Turret", SendMessageOptions.DontRequireReceiver);
                        this.GenerateGraphicStuff(hit);
                    }
                }
                else
                {
                    hit.collider.gameObject.SendMessage("HitSoldier", "Turret", SendMessageOptions.DontRequireReceiver);
                    this.GenerateGraphicStuff(hit);
                }
            }
            this.GetComponent<AudioSource>().Play();
            this.shootTime = Time.time + 0.1f;
            if (this._shootBurst >= this.shootBurst)
            {
                this.GunEffects(false);
                this._shootBurst = 0;
                this.shooting = false;
                this._shootDelay = this.shootDelay;
            }
        }
    }

    public virtual void GunEffects(bool b)
    {
        if (this.shotingEmitter != null)
        {
            this.shotingEmitter.ChangeState(b);
        }
        if (this.shotLight != null)
        {
            this.shotLight.enabled = b;
        }
    }

    public virtual void GenerateGraphicStuff(RaycastHit hit)
    {
        HitType hitType = default(HitType);
        if (this.timerToCreateDecal < 0f)
        {
            this.timerToCreateDecal = 0.1f;
            Rigidbody body = hit.collider.GetComponent<Rigidbody>();
            if (body == null)
            {
                if (hit.collider.transform.parent != null)
                {
                    body = hit.collider.transform.parent.GetComponent<Rigidbody>();
                }
            }
            if (body != null)
            {
                if ((body.gameObject.layer != 10) && !body.gameObject.name.ToLower().Contains("door"))
                {
                    body.isKinematic = false;
                }
                if (!body.isKinematic)
                {
                    Vector3 direction = hit.collider.transform.position - this.weapon.transform.position;
                    body.AddForceAtPosition(direction.normalized * this.pushPower, hit.point, ForceMode.Impulse);
                }
            }
            GameObject go = null;
            switch (hit.collider.tag)
            {
                case "wood":
                    hitType = HitType.WOOD;
                    go = GameObject.Instantiate(this.woodParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                    break;
                case "metal":
                    hitType = HitType.METAL;
                    go = GameObject.Instantiate(this.metalParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                    break;
                case "car":
                    hitType = HitType.METAL;
                    go = GameObject.Instantiate(this.metalParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                    break;
                case "concrete":
                    hitType = HitType.CONCRETE;
                    go = GameObject.Instantiate(this.concreteParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                    break;
                case "dirt":
                    hitType = HitType.CONCRETE;
                    go = GameObject.Instantiate(this.sandParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                    break;
                case "sand":
                    hitType = HitType.CONCRETE;
                    go = GameObject.Instantiate(this.sandParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                    break;
                default:
                    return;
            }
            go.layer = hit.collider.gameObject.layer;
            if (hit.collider.GetComponent<Renderer>() == null)
            {
                return;
            }
            go = GameObject.Instantiate(this.bulletMark, hit.point, Quaternion.FromToRotation(Vector3.forward, -hit.normal));
            BulletMarks bm = (BulletMarks) go.GetComponent("BulletMarks");
            bm.GenerateDecal(hitType, hit.collider.gameObject);
        }
    }

    public virtual void Destruct()
    {
        if (this.dead)
        {
            return;
        }
        this.dead = true;
        TrainingStatistics.turrets++;
        this.GunEffects(false);
        if (!(this.parts == null))
        {
            int length = this.parts.Length;
            Transform t = null;
            int i = 0;
            while (i < length)
            {
                t = this.parts[i] as Transform;
                t.parent = null;
                Material mat = new Material(this.fadeShader);
                Renderer r = t.gameObject.GetComponent<Renderer>();
                mat.mainTexture = r.material.mainTexture;
                r.material = mat;
                if (t.gameObject.GetComponent<TrainingDummyPartDestructor>() == null)
                {
                    t.gameObject.AddComponent<TrainingDummyPartDestructor>();
                }
                if (t.GetComponent<Rigidbody>() != null)
                {
                    t.GetComponent<Rigidbody>().isKinematic = false;
                }
                i++;
            }

            List<Transform> tempList = new List<Transform>(); // Здесь используем другое имя переменной
            int j = 0; // Используем другое имя для переменной цикла
            while (j < length)
            {
                Transform childT = this.parts[j] as Transform;
                if (childT.childCount > 0)
                {
                    int k = 0;
                    while (k < childT.childCount)
                    {
                        tempList.Add(childT.GetChild(k));
                        k++;
                    }
                    foreach (Transform a in tempList)
                    {
                        UnityEngine.Object.Destroy(a.gameObject);
                    }
                    tempList.Clear();
                }
                j++;
            }
        }
        UnityEngine.Object.Destroy(this);
    }

    public virtual void Hit()
    {
        if (!this.running)
        {
            this.running = true;
        }
    }

    public Turret()
    {
        this.shootDelay = 1f;
        this.shootRange = 40f;
        this.activeDistance = 25f;
        this.disableDistance = 30f;
        this.pushPower = 5f;
    }

}