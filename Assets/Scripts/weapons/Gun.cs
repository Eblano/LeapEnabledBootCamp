using UnityEngine;
using System.Collections;

public enum FireType
{
    RAYCAST = 0,
    PHYSIC_PROJECTILE = 1
}

public enum FireMode
{
    SEMI_AUTO = 0,
    FULL_AUTO = 1,
    BURST = 2
}

[System.Serializable]
public class Gun : MonoBehaviour
{
    public string gunName;
    public GameObject bulletMark;
    public GameObject projectilePrefab;
    public Transform weaponTransformReference;
    public LayerMask hitLayer;
    public GameObject woodParticle;
    public GameObject metalParticle;
    public GameObject concreteParticle;
    public GameObject sandParticle;
    public GameObject waterParticle;
    //How many shots the gun can take in one second
    public float fireRate;
    public bool useGravity;
    private FireType fireType;
    public FireMode fireMode;
    //Number of shoots to fire when on burst mode
    public int burstRate;
    //Range of fire in meters
    public float fireRange;
    //Speed of the projectile in m/s
    public float projectileSpeed;
    public int clipSize;
    public int totalClips;
    //Time to reload the weapon in seconds
    public float reloadTime;
    public bool autoReload;
    public int currentRounds;
    public float shootVolume;
    public AudioClip shootSound;
    private AudioSource shootSoundSource;
    public AudioClip reloadSound;
    private AudioSource reloadSoundSource;
    public AudioClip outOfAmmoSound;
    private AudioSource outOfAmmoSoundSource;
    private float reloadTimer;
    [UnityEngine.HideInInspector]
    public bool freeToShoot;
    [UnityEngine.HideInInspector]
    public bool reloading;
    private float lastShootTime;
    private float shootDelay;
    private int cBurst;
    [UnityEngine.HideInInspector]
    public bool fire;
    public GameObject hitParticles;
    public GunParticles shotingEmitter;
    private Transform shottingParticles;
    public ParticleSystem[] capsuleEmitter;
    public ShotLight shotLight;
    public bool unlimited;
    private float timerToCreateDecal;
    public float pushPower;
    public SoldierCamera soldierCamera;
    private Camera cam;
    public virtual void OnDisable()
    {
        if (this.shotingEmitter != null)
        {
            this.shotingEmitter.ChangeState(false);
        }
        if (!(this.capsuleEmitter == null))
        {
            int i = 0;
            while (i < this.capsuleEmitter.Length)
            {
                if (this.capsuleEmitter[i] != null)
                {
                    this.capsuleEmitter[i].enableEmission = false;
                }
                i++;
            }
        }
        if (this.shotLight != null)
        {
            this.shotLight.enabled = false;
        }
    }

    public virtual void OnEnable()
    {
        this.cam = this.soldierCamera.GetComponent<Camera>();
        this.reloadTimer = 0f;
        this.reloading = false;
        this.freeToShoot = true;
        this.shootDelay = 1f / this.fireRate;
        this.cBurst = this.burstRate;
        this.totalClips--;
        this.currentRounds = this.clipSize;
        if (this.projectilePrefab != null)
        {
            this.fireType = FireType.PHYSIC_PROJECTILE;
        }
        if (this.shotLight != null)
        {
            this.shotLight.enabled = false;
        }
        this.shottingParticles = null;
        if (this.shotingEmitter != null)
        {
            int i = 0;
            while (i < this.shotingEmitter.transform.childCount)
            {
                if (this.shotingEmitter.transform.GetChild(i).name == "bullet_trace")
                {
                    this.shottingParticles = this.shotingEmitter.transform.GetChild(i);
                    break;
                }
                i++;
            }
        }
    }

    public virtual void ShotTheTarget()
    {
        if (this.fire && !this.reloading)
        {
            if (this.currentRounds > 0)
            {
                if (((Time.time > this.lastShootTime) && this.freeToShoot) && (this.cBurst > 0))
                {
                    this.lastShootTime = Time.time + this.shootDelay;
                    switch (this.fireMode)
                    {
                        case FireMode.SEMI_AUTO:
                            this.freeToShoot = false;
                            break;
                        case FireMode.BURST:
                            this.cBurst--;
                            break;
                    }
                    if (!(this.capsuleEmitter == null))
                    {
                        int i = 0;
                        while (i < this.capsuleEmitter.Length)
                        {
                            this.capsuleEmitter[i].Play();
                            i++;
                        }
                    }
                    this.PlayShootSound();
                    if (this.shotingEmitter != null)
                    {
                        this.shotingEmitter.ChangeState(true);
                    }
                    if (this.shotLight != null)
                    {
                        this.shotLight.enabled = true;
                    }
                    switch (this.fireType)
                    {
                        case FireType.RAYCAST:
                            TrainingStatistics.shootsFired++;
                            this.CheckRaycastHit();
                            break;
                        case FireType.PHYSIC_PROJECTILE:
                            TrainingStatistics.grenadeFired++;
                            this.LaunchProjectile();
                            break;
                    }
                    this.currentRounds--;
                    if (this.currentRounds <= 0)
                    {
                        this.Reload();
                    }
                }
            }
            else
            {
                if (this.autoReload && this.freeToShoot)
                {
                    if (this.shotingEmitter != null)
                    {
                        this.shotingEmitter.ChangeState(false);
                    }
                    if (this.shotLight != null)
                    {
                        this.shotLight.enabled = false;
                    }
                    if (!this.reloading)
                    {
                        this.Reload();
                    }
                }
            }
        }
        else
        {
            if (this.shotingEmitter != null)
            {
                this.shotingEmitter.ChangeState(false);
            }
            if (this.shotLight != null)
            {
                this.shotLight.enabled = false;
            }
        }
    }

    public virtual void LaunchProjectile()
    {
        Vector3 startPosition = default(Vector3);
        RaycastHit hit = default(RaycastHit);
         //Get the launch position (weapon related)
        Ray camRay = this.cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.6f, 0));
        if (this.weaponTransformReference != null)
        {
            startPosition = this.weaponTransformReference.position;
        }
        else
        {
            startPosition = this.cam.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.5f));
        }
        GameObject projectile = GameObject.Instantiate(this.projectilePrefab, startPosition, Quaternion.identity);
        Grenade grenadeObj = projectile.GetComponent("Grenade") as Grenade;
        grenadeObj.soldierCamera = this.soldierCamera;
        projectile.transform.rotation = Quaternion.LookRotation(camRay.direction);
        Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
        if (projectile.GetComponent<Rigidbody>() == null)
        {
            projectileRigidbody = projectile.AddComponent<Rigidbody>();
        }
        projectileRigidbody.useGravity = this.useGravity;
        Ray camRay2 = this.cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.55f, 0));
        if (Physics.Raycast(camRay2.origin, camRay2.direction, out hit, this.fireRange, (int) this.hitLayer))
        {
            projectileRigidbody.velocity = (hit.point - this.weaponTransformReference.position).normalized * this.projectileSpeed;
        }
        else
        {
            projectileRigidbody.velocity = (this.cam.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.55f, 40)) - this.weaponTransformReference.position).normalized * this.projectileSpeed;
        }
    }

    public virtual void CheckRaycastHit()
    {
        RaycastHit hit = default(RaycastHit);
        RaycastHit glassHit = default(RaycastHit);
        Ray camRay = default(Ray);
        Vector3 origin = default(Vector3);
        Vector3 glassOrigin = default(Vector3);
        Vector3 dir = default(Vector3);
        Vector3 glassDir = default(Vector3);
        if (this.weaponTransformReference == null)
        {
            camRay = this.cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
            origin = camRay.origin;
            dir = camRay.direction;
            origin = origin + (dir * 0.1f);
        }
        else
        {
            camRay = this.cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
            origin = this.weaponTransformReference.position + (this.weaponTransformReference.right * 0.2f);
            if (Physics.Raycast(camRay.origin + (camRay.direction * 0.1f), camRay.direction, out hit, this.fireRange, (int) this.hitLayer))
            {
                dir = (hit.point - origin).normalized;
                if (hit.collider.tag == "glass")
                {
                    glassOrigin = hit.point + (dir * 0.05f);
                    if (Physics.Raycast(glassOrigin, camRay.direction, out glassHit, this.fireRange - hit.distance, (int) this.hitLayer))
                    {
                        glassDir = glassHit.point - glassOrigin;
                    }
                }
            }
            else
            {
                dir = this.weaponTransformReference.forward;
            }
        }
        if (this.shottingParticles != null)
        {
            this.shottingParticles.rotation = Quaternion.FromToRotation(Vector3.forward, (this.cam.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, this.cam.farClipPlane)) - this.weaponTransformReference.position).normalized);
        }
        if (Physics.Raycast(origin, dir, out hit, this.fireRange, (int) this.hitLayer))
        {
            hit.collider.gameObject.SendMessage("Hit", hit, SendMessageOptions.DontRequireReceiver);
            this.GenerateGraphicStuff(hit);
            if (hit.collider.tag == "glass")
            {
                if (Physics.Raycast(glassOrigin, glassDir, out glassHit, this.fireRange - hit.distance, (int) this.hitLayer))
                {
                    glassHit.collider.gameObject.SendMessage("Hit", glassHit, SendMessageOptions.DontRequireReceiver);
                    this.GenerateGraphicStuff(glassHit);
                }
            }
        }
    }

    public virtual void GenerateGraphicStuff(RaycastHit hit)
    {
        HitType hitType = default(HitType);
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
                Vector3 direction = hit.collider.transform.position - this.weaponTransformReference.position;
                body.AddForceAtPosition(direction.normalized * this.pushPower, hit.point, ForceMode.Impulse);
            }
        }
        GameObject go = null;
        float delta = -0.02f;
        Vector3 hitUpDir = hit.normal;
        Vector3 hitPoint = hit.point + (hit.normal * delta);
        switch (hit.collider.tag)
        {
            case "wood":
                hitType = HitType.WOOD;
                go = GameObject.Instantiate(this.woodParticle, hitPoint, Quaternion.FromToRotation(Vector3.up, hitUpDir)) as GameObject;
            case "metal":
                hitType = HitType.METAL;
                go = GameObject.Instantiate(this.metalParticle, hitPoint, Quaternion.FromToRotation(Vector3.up, hitUpDir)) as GameObject;
            case "car":
                hitType = HitType.METAL;
                go = GameObject.Instantiate(this.metalParticle, hitPoint, Quaternion.FromToRotation(Vector3.up, hitUpDir)) as GameObject;
            case "concrete":
                hitType = HitType.CONCRETE;
                go = GameObject.Instantiate(this.concreteParticle, hitPoint, Quaternion.FromToRotation(Vector3.up, hitUpDir)) as GameObject;
            case "dirt":
                hitType = HitType.CONCRETE;
                go = GameObject.Instantiate(this.sandParticle, hitPoint, Quaternion.FromToRotation(Vector3.up, hitUpDir)) as GameObject;
            case "sand":
                hitType = HitType.CONCRETE;
                go = GameObject.Instantiate(this.sandParticle, hitPoint, Quaternion.FromToRotation(Vector3.up, hitUpDir)) as GameObject;
            case "water":
                go = GameObject.Instantiate(this.waterParticle, hitPoint, Quaternion.FromToRotation(Vector3.up, hitUpDir)) as GameObject;
            default:
                return;
                break;
        }
        go.layer = hit.collider.gameObject.layer;
        if (hit.collider.GetComponent<Renderer>() == null)
        {
            return;
        }
        if ((this.timerToCreateDecal < 0f) && (hit.collider.tag != "water"))
        {
            go = GameObject.Instantiate(this.bulletMark, hit.point, Quaternion.FromToRotation(Vector3.forward, -hit.normal));
            BulletMarks bm = (BulletMarks) go.GetComponent("BulletMarks");
            bm.GenerateDecal(hitType, hit.collider.gameObject);
            this.timerToCreateDecal = 0.02f;
        }
    }

    public virtual void Update()
    {
        this.timerToCreateDecal = this.timerToCreateDecal - Time.deltaTime;
        if (((Input.GetButtonDown("Fire1") && (this.currentRounds == 0)) && !this.reloading) && this.freeToShoot)
        {
            this.PlayOutOfAmmoSound();
        }
        if (Input.GetButtonUp("Fire1"))
        {
            this.freeToShoot = true;
            this.cBurst = this.burstRate;
        }
        this.HandleReloading();
        this.ShotTheTarget();
    }

    public virtual void HandleReloading()
    {
        if (Input.GetKeyDown(KeyCode.R) && !this.reloading)
        {
            this.Reload();
        }
        if (this.reloading)
        {
            this.reloadTimer = this.reloadTimer - Time.deltaTime;
            if (this.reloadTimer <= 0f)
            {
                this.reloading = false;
                if (!this.unlimited)
                {
                    this.totalClips--;
                }
                this.currentRounds = this.clipSize;
            }
        }
    }

    public virtual void Reload()
    {
        if ((this.totalClips > 0) && (this.currentRounds < this.clipSize))
        {
            this.PlayReloadSound();
            this.reloading = true;
            this.reloadTimer = this.reloadTime;
        }
    }

    //---------------AUDIO METHODS--------
    public virtual void PlayOutOfAmmoSound()
    {
        this.GetComponent<AudioSource>().PlayOneShot(this.outOfAmmoSound, 1.5f);
    }

    public virtual void PlayReloadSound()
    {
        this.GetComponent<AudioSource>().PlayOneShot(this.reloadSound, 1.5f);
    }

    public virtual void PlayShootSound()
    {
        this.GetComponent<AudioSource>().PlayOneShot(this.shootSound);
    }

    public Gun()
    {
        this.shootVolume = 0.4f;
        this.unlimited = true;
        this.pushPower = 3f;
    }

}