using UnityEngine;
using System.Collections;

[System.Serializable]
public class WaterInteractions : MonoBehaviour
{
    public Transform soldier;
    private SoldierController controller;
    private bool emitMovement;
    public Transform movementContainer;
    public ParticleSystem[] movementEmitters;
    private bool emitStand;
    public Transform standingContainer;
    public ParticleSystem[] standingEmitters;
    public float jumpHitDistance;
    public GameObject jumpParticle;
    //public var jumpEmitters : ParticleEmitter[];
    private Transform thisT;
    public LayerMask affectedLayers;
    private RaycastHit hitInfo;
    private bool jumped;
    private bool emittedHit;
    private float jumpTimer;
    private float runSpeed;
    private float runStrafeSpeed;
    private float walkSpeed;
    private float walkStrafeSpeed;
    private float crouchRunSpeed;
    private float crouchRunStrafeSpeed;
    private float crouchWalkSpeed;
    private float crouchWalkStrafeSpeed;
    private float currentAmount;
    public float depthToReduceSpeed;
    public float speedUnderWater;
    public AudioClip waterImpactSound;
    public AudioClip waterJumpingSound;
    public float fadeSpeed;
    private Vector3 lastPositon;
    private Vector3 currentPosition;
    public virtual void Start()
    {
        int i = 0;
        this.controller = (SoldierController) this.soldier.GetComponent("SoldierController");
        this.currentAmount = 1f;
        this.runSpeed = this.controller.runSpeed;
        this.runStrafeSpeed = this.controller.runStrafeSpeed;
        this.walkSpeed = this.controller.walkSpeed;
        this.walkStrafeSpeed = this.controller.walkStrafeSpeed;
        this.crouchRunSpeed = this.controller.crouchRunSpeed;
        this.crouchRunStrafeSpeed = this.controller.crouchRunStrafeSpeed;
        this.crouchWalkSpeed = this.controller.crouchWalkSpeed;
        this.crouchWalkStrafeSpeed = this.controller.crouchWalkStrafeSpeed;
        this.jumpTimer = 0f;
        this.emitMovement = false;
        this.jumped = false;
        this.movementContainer.parent = null;
        this.movementContainer.GetComponent<AudioSource>().volume = 0f;
        i = 0;
        while (i < this.movementEmitters.Length)
        {
            this.movementEmitters[i].enableEmission = false;
            i++;
        }
        this.emitStand = false;
        this.standingContainer.parent = null;
        i = 0;
        while (i < this.standingEmitters.Length)
        {
            this.standingEmitters[i].enableEmission = false;
            i++;
        }
        this.thisT = this.transform;
    }

    public virtual void Update()
    {
        if (!this.soldier.gameObject.active)
        {
            return;
        }
        this.lastPositon = this.currentPosition;
        this.currentPosition = new Vector3(this.soldier.position.x, 0f, this.soldier.position.z);
        Vector3 dir = (this.currentPosition - this.lastPositon).normalized;
        this.thisT.position = this.soldier.position + new Vector3(0, 1.8f, 0);
        if (!GameManager.pause)
        {
            this.jumped = Input.GetButtonDown("Jump");
        }
        if (!this.controller.inAir)
        {
            this.jumpTimer = 0f;
            this.emittedHit = false;
        }
        else
        {
            this.jumpTimer = this.jumpTimer + Time.deltaTime;
        }
        if (Physics.Raycast(this.thisT.position, -Vector3.up, out this.hitInfo, Mathf.Infinity, (int) this.affectedLayers))
        {
            if (this.hitInfo.collider.tag == "water")
            {
                if (this.hitInfo.distance < this.depthToReduceSpeed)
                {
                    this.ChangeSpeed(this.speedUnderWater);
                }
                else
                {
                    this.ChangeSpeed(1f);
                }
                if (this.controller.inAir)
                {
                    if (((this.hitInfo.distance < this.jumpHitDistance) && !this.emittedHit) && (this.jumpTimer > 0.5f))
                    {
                        this.emittedHit = true;
                        this.EmitJumpParticles(true, this.hitInfo);
                        this.ChangeMovementState(false);
                        this.ChangeStandingState(false);
                    }
                }
                else
                {
                    if (this.jumped)
                    {
                        this.EmitJumpParticles(false, this.hitInfo);
                        this.ChangeMovementState(false);
                        this.ChangeStandingState(false);
                    }
                    else
                    {
                        if (!this.controller.inAir)
                        {
                            if (dir.magnitude > 0.2f)
                            {
                                this.movementContainer.position = this.hitInfo.point;
                                this.ChangeMovementState(true);
                                this.ChangeStandingState(false);
                            }
                            else
                            {
                                this.standingContainer.position = this.hitInfo.point;
                                this.ChangeMovementState(false);
                                this.ChangeStandingState(true);
                            }
                        }
                    }
                }
            }
            else
            {
                this.ChangeSpeed(1f);
                this.ChangeMovementState(false);
                this.ChangeStandingState(false);
            }
        }
        else
        {
            this.ChangeSpeed(1f);
            this.ChangeMovementState(false);
            this.ChangeStandingState(false);
        }
        if (this.emitMovement)
        {
            if (this.movementContainer.GetComponent<AudioSource>().volume < 0.65f)
            {
                if (!this.movementContainer.GetComponent<AudioSource>().isPlaying)
                {
                    this.movementContainer.GetComponent<AudioSource>().Play();
                }
                this.movementContainer.GetComponent<AudioSource>().volume = this.movementContainer.GetComponent<AudioSource>().volume + (Time.deltaTime * this.fadeSpeed);
            }
            else
            {
                this.movementContainer.GetComponent<AudioSource>().volume = 0.65f;
            }
        }
        else
        {
            if (this.movementContainer.GetComponent<AudioSource>().isPlaying)
            {
                if (this.movementContainer.GetComponent<AudioSource>().volume > 0f)
                {
                    this.movementContainer.GetComponent<AudioSource>().volume = this.movementContainer.GetComponent<AudioSource>().volume - ((Time.deltaTime * this.fadeSpeed) * 2f);
                }
                else
                {
                    this.movementContainer.GetComponent<AudioSource>().Pause();
                }
            }
        }
    }

    public virtual void ChangeSpeed(float amount)
    {
        if (this.currentAmount == amount)
        {
            return;
        }
        this.currentAmount = amount;
        this.controller.runSpeed = this.runSpeed * amount;
        this.controller.runStrafeSpeed = this.runStrafeSpeed * amount;
        this.controller.walkSpeed = this.walkSpeed * amount;
        this.controller.walkStrafeSpeed = this.walkStrafeSpeed * amount;
        this.controller.crouchRunSpeed = this.crouchRunSpeed * amount;
        this.controller.crouchRunStrafeSpeed = this.crouchRunStrafeSpeed * amount;
        this.controller.crouchWalkSpeed = this.crouchWalkSpeed * amount;
        this.controller.crouchWalkStrafeSpeed = this.crouchWalkStrafeSpeed * amount;
    }

    public virtual void EmitJumpParticles(bool b, RaycastHit hitInfo)
    {
        GameObject go = GameObject.Instantiate(this.jumpParticle, hitInfo.point, Quaternion.identity) as GameObject;
        if (go.GetComponent<AudioSource>() != null)
        {
            if (b)
            {
                go.GetComponent<AudioSource>().PlayOneShot(this.waterImpactSound, 0.5f);
            }
            else
            {
                go.GetComponent<AudioSource>().PlayOneShot(this.waterJumpingSound, 1);
            }
        }
        ParticleSystem emitter = null;
        int i = 0;
        while (i < go.transform.childCount)
        {
            emitter = go.transform.GetChild(i).GetComponent("ParticleSystem") as ParticleSystem;
            if (emitter == null)
            {
                goto Label_for_72;
            }
            emitter.enableEmission = false;
            emitter.Play();
            Label_for_72:
            i++;
        }
        AutoDestroy aux = go.AddComponent<AutoDestroy>() as AutoDestroy;
        aux.time = 2;
    }

    public virtual void ChangeMovementState(bool b)
    {
        if (b == this.emitMovement)
        {
            return;
        }
        this.emitMovement = b;
        int i = 0;
        while (i < this.movementEmitters.Length)
        {
            this.movementEmitters[i].enableEmission = b;
            i++;
        }
    }

    public virtual void ChangeStandingState(bool b)
    {
        if (b == this.emitStand)
        {
            return;
        }
        this.emitStand = b;
        int i = 0;
        while (i < this.standingEmitters.Length)
        {
            this.standingEmitters[i].enableEmission = b;
            i++;
        }
    }

    public WaterInteractions()
    {
        this.jumpHitDistance = 1.4f;
        this.depthToReduceSpeed = 0.9f;
        this.speedUnderWater = 0.8f;
        this.fadeSpeed = 0.6f;
    }

}