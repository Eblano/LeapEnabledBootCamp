using UnityEngine;
using System.Collections;

//Class responsible for processing all soldier's animations
[System.Serializable]
public class SoldierAnimations : MonoBehaviour
{
    public Transform aimPivot;
    public Transform aimTarget;
    public HeadLookController headLookController;
    public float jumpAnimStretch;
    public float jumpLandCrouchAmount;
    private SoldierController soldier;
    private CharacterMotor motor;
    private float lastNonRelaxedTime;
    private float aimAngleY;
    private bool aim;
    private bool fire;
    private bool walk;
    private bool crouch;
    private Vector3 moveDir;
    private bool reloading;
    private int currentWeapon;
    private bool inAir;
    private float groundedWeight;
    private float crouchWeight;
    private float relaxedWeight;
    private float aimWeight;
    private float fireWeight;
    public virtual void OnEnable()
    {
        this.soldier = (SoldierController) this.gameObject.GetComponent("SoldierController");
        this.motor = (CharacterMotor) this.gameObject.GetComponent("CharacterMotor");
        this.SetAnimationProperties();
    }

    public virtual void Update()
    {
        this.CheckSoldierState();
        if (this.crouch)
        {
            this.crouchWeight = this.CrossFadeUp(this.crouchWeight, 0.4f);
        }
        else
        {
            if (this.inAir && (this.jumpLandCrouchAmount > 0))
            {
                this.crouchWeight = this.CrossFadeUp(this.crouchWeight, 1 / this.jumpLandCrouchAmount);
            }
            else
            {
                this.crouchWeight = this.CrossFadeDown(this.crouchWeight, 0.45f);
            }
        }
        float uprightWeight = 1 - this.crouchWeight;
        if (this.fire)
        {
            this.aimWeight = this.CrossFadeUp(this.aimWeight, 0.2f);
            this.fireWeight = this.CrossFadeUp(this.fireWeight, 0.2f);
        }
        else
        {
            if (this.aim)
            {
                this.aimWeight = this.CrossFadeUp(this.aimWeight, 0.3f);
                this.fireWeight = this.CrossFadeDown(this.fireWeight, 0.3f);
            }
            else
            {
                this.aimWeight = this.CrossFadeDown(this.aimWeight, 0.5f);
                this.fireWeight = this.CrossFadeDown(this.fireWeight, 0.5f);
            }
        }
        float nonAimWeight = 1 - this.aimWeight;
        float aimButNotFireWeight = this.aimWeight - this.fireWeight;
        if (this.inAir)
        {
            this.groundedWeight = this.CrossFadeDown(this.groundedWeight, 0.1f);
        }
        else
        {
            this.groundedWeight = this.CrossFadeUp(this.groundedWeight, 0.2f);
        }
        // Method that computes the idle timer to control IDLE and RELAXEDWALK animations
        if ((((this.aim || this.fire) || this.crouch) || !this.walk) || ((this.moveDir != Vector3.zero) && (this.moveDir.normalized.z < 0.8f)))
        {
            this.lastNonRelaxedTime = Time.time;
        }
        if (Time.time > (this.lastNonRelaxedTime + 2))
        {
            this.relaxedWeight = this.CrossFadeUp(this.relaxedWeight, 1f);
        }
        else
        {
            this.relaxedWeight = this.CrossFadeDown(this.relaxedWeight, 0.3f);
        }
        float nonRelaxedWeight = 1 - this.relaxedWeight;
        this.GetComponent<Animation>()["NormalGroup"].weight = ((uprightWeight * nonAimWeight) * this.groundedWeight) * nonRelaxedWeight;
        this.GetComponent<Animation>()["RelaxedGroup"].weight = ((uprightWeight * nonAimWeight) * this.groundedWeight) * this.relaxedWeight;
        this.GetComponent<Animation>()["CrouchGroup"].weight = (this.crouchWeight * nonAimWeight) * this.groundedWeight;
        this.GetComponent<Animation>()["NormalAimGroup"].weight = (uprightWeight * aimButNotFireWeight) * this.groundedWeight;
        this.GetComponent<Animation>()["CrouchAimGroup"].weight = (this.crouchWeight * aimButNotFireWeight) * this.groundedWeight;
        this.GetComponent<Animation>()["NormalFireGroup"].weight = (uprightWeight * this.fireWeight) * this.groundedWeight;
        this.GetComponent<Animation>()["CrouchFireGroup"].weight = (this.crouchWeight * this.fireWeight) * this.groundedWeight;
        float runningJump = Mathf.Clamp01(Vector3.Dot(this.motor.movement.velocity, this.transform.forward) / 2f);
        this.GetComponent<Animation>()["StandingJump"].weight = (1 - this.groundedWeight) * (1 - runningJump);
        this.GetComponent<Animation>()["RunJump"].weight = (1 - this.groundedWeight) * runningJump;
        if (this.inAir)
        {
            //var normalizedTime = Mathf.Lerp(0.15, 0.65, Mathf.InverseLerp(jumpAnimStretch, -jumpAnimStretch, motor.movement.velocity.y));
            float normalizedTime = Mathf.InverseLerp(this.jumpAnimStretch, -this.jumpAnimStretch, this.motor.movement.velocity.y);
            this.GetComponent<Animation>()["StandingJump"].normalizedTime = normalizedTime;
            this.GetComponent<Animation>()["RunJump"].normalizedTime = normalizedTime;
        }
        //Debug.Log("motor.movement.velocity.y="+motor.movement.velocity.y+" - "+animation["StandingJump"].normalizedTime);
        float locomotionWeight = 1;
        locomotionWeight = locomotionWeight * (1 - this.GetComponent<Animation>()["Crouch"].weight);
        locomotionWeight = locomotionWeight * (1 - this.GetComponent<Animation>()["CrouchAim"].weight);
        locomotionWeight = locomotionWeight * (1 - this.GetComponent<Animation>()["CrouchFire"].weight);
        this.GetComponent<Animation>()["LocomotionSystem"].weight = locomotionWeight;
        // Aiming up/down
        Vector3 aimDir = (this.aimTarget.position - this.aimPivot.position).normalized;
        float targetAngle = Mathf.Asin(aimDir.y) * Mathf.Rad2Deg;
        this.aimAngleY = Mathf.Lerp(this.aimAngleY, targetAngle, Time.deltaTime * 8);
        // Use HeadLookController when not aiming/firing
        this.headLookController.effect = nonAimWeight;
        // Use additive animations for aiming when aiming and firing
        this.GetComponent<Animation>()["StandingAimUp"].weight = uprightWeight * this.aimWeight;
        this.GetComponent<Animation>()["StandingAimDown"].weight = uprightWeight * this.aimWeight;
        this.GetComponent<Animation>()["CrouchAimUp"].weight = this.crouchWeight * this.aimWeight;
        this.GetComponent<Animation>()["CrouchAimDown"].weight = this.crouchWeight * this.aimWeight;
        // Set time of animations according to current vertical aiming angle
        this.GetComponent<Animation>()["StandingAimUp"].time = Mathf.Clamp01(this.aimAngleY / 90);
        this.GetComponent<Animation>()["StandingAimDown"].time = Mathf.Clamp01(-this.aimAngleY / 90);
        this.GetComponent<Animation>()["CrouchAimUp"].time = Mathf.Clamp01(this.aimAngleY / 90);
        this.GetComponent<Animation>()["CrouchAimDown"].time = Mathf.Clamp01(-this.aimAngleY / 90);
        if (this.reloading)
        {
            this.GetComponent<Animation>().CrossFade("Reload" + this.soldier.currentWeaponName, 0.1f);
        }
        if ((this.currentWeapon > 0) && this.fire)
        {
            this.GetComponent<Animation>().CrossFade("FireM203");
        }
    }

    public virtual float CrossFadeUp(float weight, float fadeTime)
    {
        return Mathf.Clamp01(weight + (Time.deltaTime / fadeTime));
    }

    public virtual float CrossFadeDown(float weight, float fadeTime)
    {
        return Mathf.Clamp01(weight - (Time.deltaTime / fadeTime));
    }

    public virtual void CheckSoldierState()
    {
        this.aim = this.soldier.aim;
        this.fire = this.soldier.fire;
        this.walk = this.soldier.walk;
        this.crouch = this.soldier.crouch;
        this.reloading = this.soldier.reloading;
        this.currentWeapon = this.soldier.currentWeapon;
        this.moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        this.inAir = this.GetComponent("CharacterController").isGrounded == null;
    }

    //Method that initializes animations properties
    public virtual void SetAnimationProperties()
    {
        this.GetComponent<Animation>().AddClip(this.GetComponent<Animation>()["StandingReloadM4"].clip, "ReloadM4");
        this.GetComponent<Animation>()["ReloadM4"].AddMixingTransform(this.transform.Find("Pelvis/Spine1/Spine2"));
        this.GetComponent<Animation>()["ReloadM4"].wrapMode = WrapMode.Clamp;
        this.GetComponent<Animation>()["ReloadM4"].layer = 3;
        this.GetComponent<Animation>()["ReloadM4"].time = 0;
        this.GetComponent<Animation>()["ReloadM4"].speed = 1f;
        this.GetComponent<Animation>().AddClip(this.GetComponent<Animation>()["StandingReloadRPG1"].clip, "ReloadM203");
        this.GetComponent<Animation>()["ReloadM203"].AddMixingTransform(this.transform.Find("Pelvis/Spine1/Spine2"));
        this.GetComponent<Animation>()["ReloadM203"].wrapMode = WrapMode.Clamp;
        this.GetComponent<Animation>()["ReloadM203"].layer = 3;
        this.GetComponent<Animation>()["ReloadM203"].time = 0;
        this.GetComponent<Animation>()["ReloadM203"].speed = 1f;
        this.GetComponent<Animation>().AddClip(this.GetComponent<Animation>()["StandingFireRPG"].clip, "FireM203");
        this.GetComponent<Animation>()["FireM203"].AddMixingTransform(this.transform.Find("Pelvis/Spine1/Spine2"));
        this.GetComponent<Animation>()["FireM203"].wrapMode = WrapMode.Clamp;
        this.GetComponent<Animation>()["FireM203"].layer = 3;
        this.GetComponent<Animation>()["FireM203"].time = 0;
        this.GetComponent<Animation>()["FireM203"].speed = 1f;
        this.GetComponent<Animation>()["StandingJump"].layer = 2;
        this.GetComponent<Animation>()["StandingJump"].weight = 0;
        this.GetComponent<Animation>()["StandingJump"].speed = 0;
        this.GetComponent<Animation>()["StandingJump"].enabled = true;
        this.GetComponent<Animation>()["RunJump"].layer = 2;
        this.GetComponent<Animation>()["RunJump"].weight = 0;
        this.GetComponent<Animation>()["RunJump"].speed = 0;
        this.GetComponent<Animation>()["RunJump"].enabled = true;
        this.GetComponent<Animation>().SyncLayer(2);
        this.SetupAdditiveAiming("StandingAimUp");
        this.SetupAdditiveAiming("StandingAimDown");
        this.SetupAdditiveAiming("CrouchAimUp");
        this.SetupAdditiveAiming("CrouchAimDown");
    }

    public virtual void SetupAdditiveAiming(string anim)
    {
        this.GetComponent<Animation>()[anim].blendMode = AnimationBlendMode.Additive;
        this.GetComponent<Animation>()[anim].enabled = true;
        this.GetComponent<Animation>()[anim].weight = 1;
        this.GetComponent<Animation>()[anim].layer = 4;
        this.GetComponent<Animation>()[anim].time = 0;
        this.GetComponent<Animation>()[anim].speed = 0;
    }

    public SoldierAnimations()
    {
        this.jumpAnimStretch = 5;
        this.jumpLandCrouchAmount = 1.6f;
        this.groundedWeight = 1;
        this.relaxedWeight = 1;
    }

}