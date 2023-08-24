using UnityEngine;
using System.Collections;

[System.Serializable]
public class TrainingDummy : MonoBehaviour
{
    public GameObject weapon;
    public int shootsToDestroyPart;
    public Shader fadeShader;
    [UnityEngine.HideInInspector]
    public TrainingDummyPart[] dummyParts;
    private bool enableFailEffect;
    private float failEffectTimer;
    public GameObject explosionEffectPrefab;
    public float explosionUpwardsModifier;
    public float explosionPower;
    public float explosionRadius;
    public Vector2 animTime;
    public Vector2 animSpeed;
    public float animTimeVariation;
    public GameObject failEffect;
    public float failEffectTime;
    public float failEffectTimeVariation;
    private bool dead;
    private bool canShoot;
    private float timer;
    private int state;
    public GunParticles shotingEmitter;
    public ShotLight shotLight;
    public Transform target;
    public Transform rotatingPart;
    public AudioClip riseSound;
    public AudioClip downSound;
    public AudioClip dieSound;
    public AudioClip shootSound;
    public float moveSpeed;
    public int shootBurst;
    private int _shootBurst;
    public float shootDelay;
    private float _shootDelay;
    private bool shooting;
    private float shootTime;
    public LayerMask hitLayer;
    public float shootRange;
    private Transform shootRef;
    public GameObject metalParticle;
    public GameObject concreteParticle;
    public GameObject sandParticle;
    public GameObject woodParticle;
    public GameObject bulletMark;
    private float timerToCreateDecal;
    public float pushPower;
    public float activeDistance;
    public float disableDistance;
    private bool running;
    public Material mat;
    public virtual void Start()
    {
        this.GetComponent<Animation>()["dum_crouch"].speed = this.animSpeed.x;
        this.GetComponent<Animation>()["dum_stand"].speed = this.animSpeed.y;
        this.GetComponent<Animation>()["dum_death"].speed = 2f;
        this.running = false;
        this.shootTime = 0f;
        this.activeDistance = this.activeDistance * this.activeDistance;
        if (this.target == null)
        {
            this.target = SoldierController.enemiesReference;
        }
        this.shootsToDestroyPart = Mathf.Max(this.shootsToDestroyPart, 1);
        this.failEffect.SetActiveRecursively(true);
        this.enableFailEffect = false;
        this.dead = false;
        this.timer = 0f;
        this.state = 2;
        this.canShoot = true;
        this.animTimeVariation = this.animTimeVariation * 0.5f;
        this.failEffectTimeVariation = this.failEffectTimeVariation * 0.5f;
        this.failEffectTimer = Mathf.Max(Random.Range(this.failEffectTime - this.failEffectTimeVariation, this.failEffectTime + this.failEffectTimeVariation), 0.5f);
        this.GetComponent<Animation>().Play("dum_crouch_idle");
        this.GunEffects(false);
    }

    public virtual void Update()
    {
        if (GameManager.pause || GameManager.scores)
        {
            this.GunEffects(false);
            return;
        }
        if (this.dead)
        {
            return;
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
        this.HandleFailEffects();
        this.HandleAnimations();
        this.timerToCreateDecal = this.timerToCreateDecal - Time.deltaTime;
        if (!this.shooting)
        {
            this._shootDelay = this._shootDelay - Time.deltaTime;
            if (this._shootDelay <= 0f)
            {
                this.shooting = true;
            }
        }
        if (this.target == null)
        {
            this.target = SoldierController.enemiesReference;
        }
        if (this.shootRef == null)
        {
            this.shootRef = SoldierController.enemiesShootReference;
        }
        if (((this.target != null) && (this.rotatingPart != null)) && (this.state == 0))
        {
            this.rotatingPart.rotation = Quaternion.Lerp(this.rotatingPart.rotation, Quaternion.LookRotation(this.target.position - this.rotatingPart.position), this.moveSpeed * Time.deltaTime);
        }
        this.ShootTheTarget();
    }

    public virtual void ShootTheTarget()
    {
        RaycastHit hit = default(RaycastHit);
        if (!this.shooting)
        {
            return;
        }
        if (this.weapon == null)
        {
            return;
        }
        if ((this.shootTime < Time.time) && (this.state == 0))
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
                        hit.collider.gameObject.SendMessage("HitSoldier", "Dummy", SendMessageOptions.DontRequireReceiver);
                        this.GenerateGraphicStuff(hit);
                    }
                }
                else
                {
                    hit.collider.gameObject.SendMessage("HitSoldier", "Dummy", SendMessageOptions.DontRequireReceiver);
                    this.GenerateGraphicStuff(hit);
                }
            }
            this.GetComponent<AudioSource>().PlayOneShot(this.shootSound);
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

    public virtual void HandleFailEffects()
    {
        if (this.enableFailEffect)
        {
            this.failEffectTimer = this.failEffectTimer - Time.deltaTime;
            if (this.failEffectTimer <= 0f)
            {
                this.failEffectTimer = Mathf.Max(Random.Range(this.failEffectTime - this.failEffectTimeVariation, this.failEffectTime + this.failEffectTimeVariation), 0.5f);
                (this.failEffect.GetComponent("ParticleSystem") as ParticleSystem).Play();
            }
        }
    }

    public virtual void HandleAnimations()
    {
        this.timer = this.timer - Time.deltaTime;
        if (this.timer <= 0f)
        {
            if (this.GetComponent<Animation>() == null)
            {
                return;
            }
            switch (this.state)
            {
                case 0:
                    if (this.GetComponent<Animation>()["dum_crouch"] == null)
                    {
                        return;
                    }
                    this.timer = this.GetComponent<Animation>()["dum_crouch"].length / this.animSpeed.x;
                    this.GetComponent<Animation>().CrossFade("dum_crouch");
                    this.GetComponent<AudioSource>().PlayOneShot(this.downSound);
                    if (this.canShoot)
                    {
                        this.GunEffects(false);
                    }
                    break;
                case 1:
                    if (this.GetComponent<Animation>()["dum_crouch_idle"] == null)
                    {
                        return;
                    }
                    this.timer = Mathf.Max(Random.Range(this.animTime.x - this.animTimeVariation, this.animTime.x + this.animTimeVariation), 0.5f);
                    this.GetComponent<Animation>()["dum_crouch_idle"].time = 0f;
                    this.GetComponent<Animation>().Play("dum_crouch_idle");
                    break;
                case 2:
                    if (this.GetComponent<Animation>()["dum_stand"] == null)
                    {
                        return;
                    }
                    this.timer = this.GetComponent<Animation>()["dum_stand"].length / this.animSpeed.y;
                    this.GetComponent<AudioSource>().PlayOneShot(this.riseSound);
                    this.GetComponent<Animation>().CrossFade("dum_stand");
                    break;
                case 3:
                    if (this.GetComponent<Animation>()["dum_stand_idle"] == null)
                    {
                        return;
                    }
                    this.timer = Mathf.Max(Random.Range(this.animTime.y - this.animTimeVariation, this.animTime.y + this.animTimeVariation), 0.5f);
                    this.GetComponent<Animation>()["dum_stand_idle"].time = 0f;
                    this.GetComponent<Animation>().Play("dum_stand_idle");
                    if (this.canShoot)
                    {
                        this.GunEffects(true);
                    }
                    break;
            }
            this.state++;
            if (this.state > 3)
            {
                this.state = 0;
            }
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

    public virtual void Hit(RaycastHit hit, int target)
    {
        this.Hit(hit, target, 1);
    }

    public virtual void Destruct(int target)
    {
        TrainingDummyPart dp = this.dummyParts[target];
        if (dp.gameObject == null)
        {
            return;
        }
        dp.shootsTaken = this.shootsToDestroyPart;
        GameObject go = null;
        if (dp.gameObject.transform.parent != null)
        {
            if (dp.gameObject.name.Contains("head") || dp.gameObject.name.Contains("neck"))
            {
                if (!this.dead)
                {
                    this.dead = true;
                    TrainingStatistics.dummies++;
                    this.GunEffects(false);
                    this.GetComponent<AudioSource>().PlayOneShot(this.dieSound);
                    this.Destruct(4);
                    this.GetComponent<Animation>().CrossFade("dum_death");
                }
            }
            if (dp.gameObject != this.weapon)
            {
                dp.gameObject.transform.parent = null;
                dp.gameObject.active = false;
                int i = 0;
                while (i < dp.brokeParts.Length)
                {
                    go = dp.brokeParts[i];
                    if (go.transform.parent == null)
                    {
                        goto Label_for_8;
                    }
                    this.InitializeMeshCollider(go);
                    this.DestroyPart(go, false);
                    Label_for_8:
                    i++;
                }
                int j = 0;
                while (j < dp.siblings.Length)
                {
                    go = dp.siblings[j];
                    if (go == null)
                    {
                        goto Label_for_9;
                    }
                    if (go.transform.parent == null)
                    {
                        goto Label_for_9;
                    }
                    if (go.name.Contains("head"))
                    {
                        this.dead = true;
                        TrainingStatistics.dummies++;
                        this.GetComponent<AudioSource>().PlayOneShot(this.dieSound);
                        this.GunEffects(false);
                    }
                    if (go == this.weapon)
                    {
                        this.canShoot = false;
                        this.GunEffects(false);
                    }
                    this.DestroyPart(go, false);
                    Label_for_9:
                    j++;
                }
                UnityEngine.Object.Destroy(dp.gameObject);
            }
            else
            {
                this.canShoot = false;
                this.GunEffects(false);
                if (this.weapon.transform.parent != null)
                {
                    this.DestroyPart(this.weapon, false);
                }
            }
        }
        else
        {
            if (dp.gameObject != this.weapon)
            {
                dp.gameObject.active = false;
                var i = 0;
                while (i < dp.brokeParts.Length)
                {
                    go = dp.brokeParts[i];
                    this.InitializeMeshCollider(go);
                    this.DestroyPart(go, false);
                    i++;
                }
                UnityEngine.Object.Destroy(dp.gameObject);
            }
        }
    }

    public virtual void Hit(RaycastHit hit, int target, int damage)
    {
        if (!this.running)
        {
            this.running = true;
        }
        TrainingDummyPart dp = this.dummyParts[target];
        if (dp.gameObject == null)
        {
            return;
        }
        dp.shootsTaken = dp.shootsTaken + damage;
        GameObject go = null;
        if (dp.gameObject.transform.parent != null)
        {
            if (!this.dead)
            {
                if (dp.dummyPart == DummyPart.CHEST)
                {
                    Vector3 dir = hit.point - this.transform.position;
                    if (Vector3.Dot(dir, this.transform.right) > 0)
                    {
                        TrainingStatistics.Register(dp.dummyPart);
                    }
                    else
                    {
                        TrainingStatistics.Register(DummyPart.HEART);
                    }
                }
                else
                {
                    TrainingStatistics.Register(dp.dummyPart);
                }
            }
            if (dp.shootsTaken == (this.shootsToDestroyPart - 1))
            {
                if (dp.gameObject != this.weapon)
                {
                    this.enableFailEffect = true;
                }
            }
            else
            {
                if (dp.shootsTaken >= this.shootsToDestroyPart)
                {
                    if (dp.gameObject.name.Contains("head") || dp.gameObject.name.Contains("neck"))
                    {
                        if (!this.dead)
                        {
                            this.dead = true;
                            TrainingStatistics.headShoot++;
                            TrainingStatistics.dummies++;
                            this.GetComponent<AudioSource>().PlayOneShot(this.dieSound);
                            this.GunEffects(false);
                            if (this.state != 0)
                            {
                                this.Hit(hit, 4, this.shootsToDestroyPart);
                            }
                            this.GetComponent<Animation>().CrossFade("dum_death");
                        }
                    }
                    if (dp.gameObject != this.weapon)
                    {
                        dp.gameObject.transform.parent = null;
                        dp.gameObject.active = false;
                        int i = 0;
                        while (i < dp.brokeParts.Length)
                        {
                            go = dp.brokeParts[i];
                            if (go.transform.parent == null)
                            {
                                goto Label_for_11;
                            }
                            this.InitializeMeshCollider(go);
                            this.DestroyPart(go, true);
                            Label_for_11:
                            i++;
                        }
                        int j = 0;
                        while (j < dp.siblings.Length)
                        {
                            go = dp.siblings[j];
                            if (go == null)
                            {
                                goto Label_for_12;
                            }
                            if (go.transform.parent == null)
                            {
                                goto Label_for_12;
                            }
                            if (go.name.Contains("head"))
                            {
                                this.dead = true;
                                TrainingStatistics.dummies++;
                                this.GetComponent<AudioSource>().PlayOneShot(this.dieSound);
                                this.GunEffects(false);
                            }
                            if (go == this.weapon)
                            {
                                this.canShoot = false;
                                this.GunEffects(false);
                            }
                            this.DestroyPart(go, true);
                            Label_for_12:
                            j++;
                        }
                        GameObject.Instantiate(this.explosionEffectPrefab, hit.point, Quaternion.identity);
                        UnityEngine.Object.Destroy(dp.gameObject);
                    }
                    else
                    {
                        this.canShoot = false;
                        this.GunEffects(false);
                        if (this.weapon.transform.parent != null)
                        {
                            this.DestroyPart(this.weapon, true);
                        }
                    }
                }
            }
        }
        else
        {
            if (dp.gameObject != this.weapon)
            {
                dp.gameObject.active = false;
                var i = 0;
                while (i < dp.brokeParts.Length)
                {
                    go = dp.brokeParts[i];
                    this.InitializeMeshCollider(go);
                    this.DestroyPart(go, true);
                    i++;
                }
                UnityEngine.Object.Destroy(dp.gameObject);
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

    public virtual void DestroyPart(GameObject go, bool applyForce)
    {
        go.transform.parent = null;
        this.mat = new Material(this.fadeShader);
        Renderer r = go.GetComponent<Renderer>();
        this.mat.mainTexture = r.material.mainTexture;
        r.material = this.mat;
        if (go.GetComponent("TrainingDummyPartDestructor") == null)
        {
            go.AddComponent<TrainingDummyPartDestructor>();
        }
        Rigidbody rb = this.InitializeRigidbody(go);
        if (applyForce)
        {
            rb.AddForceAtPosition((go.transform.position - this.transform.position).normalized * this.explosionPower, this.transform.position);
        }
    }

    public virtual Rigidbody InitializeRigidbody(GameObject go)
    {
        Rigidbody rb = go.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        return rb;
    }

    public virtual void InitializeMeshCollider(GameObject go)
    {
        go.active = true;
    }

    public TrainingDummy()
    {
        this.animSpeed = new Vector2(1f, 1f);
        this.shootRange = 25f;
        this.pushPower = 3f;
        this.activeDistance = 15f;
        this.disableDistance = 30f;
    }

}