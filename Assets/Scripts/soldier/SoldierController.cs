using UnityEngine;
using System.Collections;

[System.Serializable]
public class SoldierController : MonoBehaviour
{
     // Public variables shown in inspector
    public float runSpeed;
    public float runStrafeSpeed;
    public float walkSpeed;
    public float walkStrafeSpeed;
    public float crouchRunSpeed;
    public float crouchRunStrafeSpeed;
    public float crouchWalkSpeed;
    public float crouchWalkStrafeSpeed;
    public GameObject radarObject;
    public float maxRotationSpeed;
    public GunManager weaponSystem;
    public float minCarDistance;
    // pxs Leap modifictaion 
    public bool leapEnabledVerticalAxis;
    public bool leapEnabledHorizontalAxis;
    public bool leapEnabledFire;
    public bool hideMouse;
    public static bool dead;
    // Public variables hidden in inspector
    [UnityEngine.HideInInspector]
    public bool walk;
    [UnityEngine.HideInInspector]
    public bool crouch;
    [UnityEngine.HideInInspector]
    public bool inAir;
    [UnityEngine.HideInInspector]
    public bool fire;
    [UnityEngine.HideInInspector]
    public bool aim;
    [UnityEngine.HideInInspector]
    public bool reloading;
    [UnityEngine.HideInInspector]
    public string currentWeaponName;
    [UnityEngine.HideInInspector]
    public int currentWeapon;
    [UnityEngine.HideInInspector]
    public bool grounded;
    [UnityEngine.HideInInspector]
    public float targetYRotation;
    // Private variables
    private Transform soldierTransform;
    private CharacterController controller;
    private HeadLookController headLookController;
    private CharacterMotor motor;
    private bool firing;
    private float firingTimer;
    public float idleTimer;
    public Transform enemiesRef;
    public Transform enemiesShootRef;
    public static Transform enemiesReference;
    public static Transform enemiesShootReference;
    [UnityEngine.HideInInspector]
    public Vector3 moveDir;
    private bool _useIK;
    public virtual void Awake()
    {
        if (this.enemiesRef != null)
        {
            SoldierController.enemiesReference = this.enemiesRef;
        }
        if (this.enemiesShootRef != null)
        {
            SoldierController.enemiesShootReference = this.enemiesShootRef;
        }
    }

    public virtual void Start()
    {
        this.idleTimer = 0f;
        this.soldierTransform = this.transform;
        this.walk = true;
        this.aim = false;
        this.reloading = false;
        this.controller = (CharacterController) this.gameObject.GetComponent("CharacterController");
        this.motor = (CharacterMotor) this.gameObject.GetComponent("CharacterMotor");
        if (this.hideMouse == true)
        {
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }
    }

    public virtual void OnEnable()
    {
        if (this.radarObject != null)
        {
            this.radarObject.SetActiveRecursively(true);
        }
        this.moveDir = Vector3.zero;
        this.headLookController = (HeadLookController) this.gameObject.GetComponent("HeadLookController");
        this.headLookController.enabled = true;
        this.walk = true;
        this.aim = false;
        this.reloading = false;
    }

    public virtual void OnDisable()
    {
        if (this.radarObject != null)
        {
            this.radarObject.SetActiveRecursively(false);
        }
        this.moveDir = Vector3.zero;
        this.headLookController.enabled = false;
        this.walk = true;
        this.aim = false;
        this.reloading = false;
    }

    public virtual void Update()
    {
        if (GameManager.pause || GameManager.scores)
        {
            this.moveDir = Vector3.zero;
            this.motor.canControl = false;
        }
        else
        {
            this.GetUserInputs();
            if (!this.motor.canControl)
            {
                this.motor.canControl = true;
            }
            if (!SoldierController.dead)
            {
                 // moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                 // pxsLeap
                float x = 0;
                float z = 0;
                if (this.leapEnabledHorizontalAxis == true)
                {
                    x = pxsLeapInput.GetHandAxisStep("Horizontal");
                }
                else
                {
                    x = Input.GetAxis("Horizontal");
                }
                if (this.leapEnabledVerticalAxis == true)
                {
                    z = pxsLeapInput.GetHandAxisStep("Depth");
                }
                else
                {
                    z = Input.GetAxis("Vertical");
                }
                // moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                this.moveDir = new Vector3(x, 0, z);
            }
            else
            {
                this.moveDir = Vector3.zero;
                this.motor.canControl = false;
            }
        }
        //Check the soldier move direction
        if (this.moveDir.sqrMagnitude > 1)
        {
            this.moveDir = this.moveDir.normalized;
        }
        this.motor.inputMoveDirection = this.transform.TransformDirection(this.moveDir);
        this.motor.inputJump = Input.GetButton("Jump") && !this.crouch;
        this.motor.movement.maxForwardSpeed = this.walk ? (this.crouch ? this.crouchWalkSpeed : this.walkSpeed) : (this.crouch ? this.crouchRunSpeed : this.runSpeed);
        this.motor.movement.maxBackwardsSpeed = this.motor.movement.maxForwardSpeed;
        this.motor.movement.maxSidewaysSpeed = this.walk ? (this.crouch ? this.crouchWalkStrafeSpeed : this.walkStrafeSpeed) : (this.crouch ? this.crouchRunStrafeSpeed : this.runStrafeSpeed);
        if (this.moveDir != Vector3.zero)
        {
            this.idleTimer = 0f;
        }
        this.inAir = !this.motor.grounded;
        float currentAngle = this.soldierTransform.localRotation.eulerAngles.y;
        float delta = Mathf.Repeat(this.targetYRotation - currentAngle, 360);
        if (delta > 180)
        {
            delta = delta - 360;
        }

        {
            float _150 = Mathf.MoveTowards(currentAngle, currentAngle + delta, Time.deltaTime * this.maxRotationSpeed);
            Quaternion _151 = this.soldierTransform.localRotation;
            Vector3 _152 = _151.eulerAngles;
            _152.y = _150;
            _151.eulerAngles = _152;
            this.soldierTransform.localRotation = _151;
        }
    }

    public virtual void GetUserInputs()
    {
         // pxs Leap Mod
         // if leapenabled, check fire from leap
        bool fireLeap = false;
        bool aimLeap = false;
        if (this.leapEnabledFire == true)
        {
            fireLeap = ((pxsLeapInput.GetHandGesture("Fire1") && this.weaponSystem.currentGun.freeToShoot) && !SoldierController.dead) && !this.inAir;
        }
        // aimLeap = pxsLeapInput.GetHandGesture("RotationFire2") && !dead;
        // print ("Aim Leap: " + aimLeap.ToString());
        //Check if the user if firing the weapon
        this.fire = fireLeap || (((Input.GetButton("Fire1") && this.weaponSystem.currentGun.freeToShoot) && !SoldierController.dead) && !this.inAir);
        //Check if the user is aiming the weapon
        this.aim = aimLeap || (Input.GetButton("Fire2") && !SoldierController.dead);
        this.idleTimer = this.idleTimer + Time.deltaTime;
        if (this.aim || this.fire)
        {
            this.firingTimer = this.firingTimer - Time.deltaTime;
            this.idleTimer = 0f;
        }
        else
        {
            this.firingTimer = 0.3f;
        }
        this.firing = (this.firingTimer <= 0f) && this.fire;
        if (this.weaponSystem.currentGun != null)
        {
            this.weaponSystem.currentGun.fire = this.firing;
            this.reloading = this.weaponSystem.currentGun.reloading;
            this.currentWeaponName = this.weaponSystem.currentGun.gunName;
            this.currentWeapon = this.weaponSystem.currentWeapon;
        }
        //Check if the user wants the soldier to crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            this.crouch = !this.crouch;
            this.idleTimer = 0f;
        }
        // crouch |= dead;
        //Check if the user wants the soldier to walk
        this.walk = ((!Input.GetKey(KeyCode.LeftShift) && !SoldierController.dead) || (this.moveDir == Vector3.zero)) || this.crouch;
    }

    public SoldierController()
    {
        this.runSpeed = 4.6f;
        this.runStrafeSpeed = 3.07f;
        this.walkSpeed = 1.22f;
        this.walkStrafeSpeed = 1.22f;
        this.crouchRunSpeed = 5;
        this.crouchRunStrafeSpeed = 5;
        this.crouchWalkSpeed = 1.8f;
        this.crouchWalkStrafeSpeed = 1.8f;
        this.maxRotationSpeed = 540;
    }

}