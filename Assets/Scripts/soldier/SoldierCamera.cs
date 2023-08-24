using UnityEngine;
using System.Collections;

[System.Serializable]
public class SoldierCamera : MonoBehaviour
{
    public Transform target;
    public Transform soldier;
    public Vector2 speed;
    public Vector2 aimSpeed;
    public Vector2 maxSpeed;
    public int yMinLimit;
    public int yMaxLimit;
    public int normalFOV;
    public int zoomFOV;
    public float lerpSpeed;
    private float distance;
    private float x;
    public float y;
    private Transform camTransform;
    private Quaternion rotation;
    private Vector3 position;
    private float deltaTime;
    private Quaternion originalSoldierRotation;
    private SoldierController soldierController;
    public bool orbit;
    public LayerMask hitLayer;
    private Vector3 cPos;
    public Vector3 normalDirection;
    public Vector3 aimDirection;
    public Vector3 crouchDirection;
    public Vector3 aimCrouchDirection;
    public float positionLerp;
    public float normalHeight;
    public float crouchHeight;
    public float normalAimHeight;
    public float crouchAimHeight;
    public float minHeight;
    public float maxHeight;
    public float normalDistance;
    public float crouchDistance;
    public float normalAimDistance;
    public float crouchAimDistance;
    public float minDistance;
    public float maxDistance;
    private float targetDistance;
    private Vector3 camDir;
    private float targetHeight;
    public float minShakeSpeed;
    public float maxShakeSpeed;
    public float minShake;
    public float maxShake;
    public int minShakeTimes;
    public int maxShakeTimes;
    public float maxShakeDistance;
    private bool shake;
    private float shakeSpeed;
    private float cShakePos;
    private int shakeTimes;
    private float cShake;
    private float cShakeSpeed;
    private int cShakeTimes;
    public Transform radar;
    public Transform radarCamera;
    private DepthOfField _depthOfFieldEffect;
    public virtual void Start()
    {
        this.cShakeTimes = 0;
        this.cShake = 0f;
        this.cShakeSpeed = this.shakeSpeed;
        this._depthOfFieldEffect = this.gameObject.GetComponent("DepthOfField") as DepthOfField;
        if ((this.target == null) || (this.soldier == null))
        {
            UnityEngine.Object.Destroy(this);
            return;
        }
        this.target.parent = null;
        this.camTransform = this.transform;
        Vector3 angles = this.camTransform.eulerAngles;
        this.x = angles.y;
        this.y = angles.x;
        this.originalSoldierRotation = this.soldier.rotation;
        this.soldierController = (SoldierController) this.soldier.GetComponent("SoldierController");
        this.targetDistance = this.normalDistance;
        this.cPos = this.soldier.position + new Vector3(0, this.normalHeight, 0);
    }

    public virtual void GoToOrbitMode(bool state)
    {
        this.orbit = state;
        this.soldierController.idleTimer = 0f;
    }

    public virtual void Update()
    {
        if (GameManager.pause || GameManager.scores)
        {
            return;
        }
        //if(GameManager.scores) return;
        if (this.orbit && ((((Input.GetKeyDown(KeyCode.O) || (Input.GetAxis("Horizontal") != 0f)) || (Input.GetAxis("Vertical") != 0f)) || this.soldierController.aim) || this.soldierController.fire))
        {
            this.GoToOrbitMode(false);
        }
        if (!this.orbit && (this.soldierController.idleTimer > 0.1f))
        {
            this.GoToOrbitMode(true);
        }
    }

    public virtual void LateUpdate()
    {
         //if(GameManager.pause || GameManager.scores) return;
        if (GameManager.scores)
        {
            return;
        }
        this.deltaTime = Time.deltaTime;
        this.GetInput();
        this.RotateSoldier();
        this.CameraMovement();
        this.DepthOfFieldControl();
    }

    public virtual void CameraMovement()
    {
        RaycastHit hit = default(RaycastHit);
        if (this.soldierController.aim)
        {
            (((DepthOfField) this.GetComponent<Camera>().GetComponent(typeof(DepthOfField))) as DepthOfField).enabled = true;
            this.GetComponent<Camera>().fieldOfView = Mathf.Lerp(this.GetComponent<Camera>().fieldOfView, this.zoomFOV, this.deltaTime * this.lerpSpeed);
            if (this.soldierController.crouch)
            {
                this.camDir = (this.aimCrouchDirection.x * this.target.forward) + (this.aimCrouchDirection.z * this.target.right);
                this.targetHeight = this.crouchAimHeight;
                this.targetDistance = this.crouchAimDistance;
            }
            else
            {
                this.camDir = (this.aimDirection.x * this.target.forward) + (this.aimDirection.z * this.target.right);
                this.targetHeight = this.normalAimHeight;
                this.targetDistance = this.normalAimDistance;
            }
        }
        else
        {
            (((DepthOfField) this.GetComponent<Camera>().GetComponent(typeof(DepthOfField))) as DepthOfField).enabled = false;
            this.GetComponent<Camera>().fieldOfView = Mathf.Lerp(this.GetComponent<Camera>().fieldOfView, this.normalFOV, this.deltaTime * this.lerpSpeed);
            if (this.soldierController.crouch)
            {
                this.camDir = (this.crouchDirection.x * this.target.forward) + (this.crouchDirection.z * this.target.right);
                this.targetHeight = this.crouchHeight;
                this.targetDistance = this.crouchDistance;
            }
            else
            {
                this.camDir = (this.normalDirection.x * this.target.forward) + (this.normalDirection.z * this.target.right);
                this.targetHeight = this.normalHeight;
                this.targetDistance = this.normalDistance;
            }
        }
        this.camDir = this.camDir.normalized;
        this.HandleCameraShake();
        this.cPos = this.soldier.position + new Vector3(0, this.targetHeight, 0);
        if (Physics.Raycast(this.cPos, this.camDir, out hit, this.targetDistance + 0.2f, (int) this.hitLayer))
        {
            float t = hit.distance - 0.1f;
            t = t - this.minDistance;
            t = t / (this.targetDistance - this.minDistance);
            this.targetHeight = Mathf.Lerp(this.maxHeight, this.targetHeight, Mathf.Clamp(t, 0f, 1f));
            this.cPos = this.soldier.position + new Vector3(0, this.targetHeight, 0);
        }
        if (Physics.Raycast(this.cPos, this.camDir, out hit, this.targetDistance + 0.2f, (int) this.hitLayer))
        {
            this.targetDistance = hit.distance - 0.1f;
        }
        if (this.radar != null)
        {
            this.radar.position = this.cPos;
            this.radarCamera.rotation = Quaternion.Euler(90, this.x, 0);
        }
        Vector3 lookPoint = this.cPos;
        lookPoint = lookPoint + (this.target.right * Vector3.Dot(this.camDir * this.targetDistance, this.target.right));
        this.camTransform.position = this.cPos + (this.camDir * this.targetDistance);
        this.camTransform.LookAt(lookPoint);
        this.target.position = this.cPos;
        this.target.rotation = Quaternion.Euler(this.y, this.x, 0);
    }

    public virtual void HandleCameraShake()
    {
        if (this.shake)
        {
            this.cShake = this.cShake + (this.cShakeSpeed * this.deltaTime);
            if (Mathf.Abs(this.cShake) > this.cShakePos)
            {
                this.cShakeSpeed = this.cShakeSpeed * -1f;
                this.cShakeTimes++;
                if (this.cShakeTimes >= this.shakeTimes)
                {
                    this.shake = false;
                }
                if (this.cShake > 0f)
                {
                    this.cShake = this.maxShake;
                }
                else
                {
                    this.cShake = -this.maxShake;
                }
            }
            this.targetHeight = this.targetHeight + this.cShake;
        }
    }

    public virtual void StartShake(float distance)
    {
        float proximity = distance / this.maxShakeDistance;
        if (proximity > 1f)
        {
            return;
        }
        proximity = Mathf.Clamp(proximity, 0f, 1f);
        proximity = 1f - proximity;
        this.cShakeSpeed = Mathf.Lerp(this.minShakeSpeed, this.maxShakeSpeed, proximity);
        this.shakeTimes = (int) Mathf.Lerp(this.minShakeTimes, this.maxShakeTimes, proximity);
        this.cShakeTimes = 0;
        this.cShakePos = Mathf.Lerp(this.minShake, this.maxShake, proximity);
        this.shake = true;
    }

    public virtual void GetInput()
    {
        Vector2 a = this.soldierController.aim ? this.aimSpeed : this.speed;
        this.x = this.x + (Mathf.Clamp(Input.GetAxis("Mouse X") * a.x, -this.maxSpeed.x, this.maxSpeed.x) * this.deltaTime);
        this.y = this.y - (Mathf.Clamp(Input.GetAxis("Mouse Y") * a.y, -this.maxSpeed.y, this.maxSpeed.y) * this.deltaTime);
        this.y = SoldierCamera.ClampAngle(this.y, this.yMinLimit, this.yMaxLimit);
    }

    public virtual void DepthOfFieldControl()
    {
        if (this._depthOfFieldEffect == null)
        {
            return;
        }
        if (this.soldierController == null)
        {
            return;
        }
        if (this.soldierController.aim && GameQualitySettings.depthOfField)
        {
            if (!this._depthOfFieldEffect.enabled)
            {
                this._depthOfFieldEffect.enabled = true;
            }
        }
        else
        {
            if (this._depthOfFieldEffect.enabled)
            {
                this._depthOfFieldEffect.enabled = false;
            }
        }
    }

    public virtual void RotateSoldier()
    {
        if (!this.orbit)
        {
            this.soldierController.targetYRotation = this.x;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
        {
            angle = angle + 360;
        }
        if (angle > 360)
        {
            angle = angle - 360;
        }
        return Mathf.Clamp(angle, min, max);
    }

    public SoldierCamera()
    {
        this.speed = new Vector2(135f, 135f);
        this.aimSpeed = new Vector2(70f, 70f);
        this.maxSpeed = new Vector2(100f, 100f);
        this.yMinLimit = -90;
        this.yMaxLimit = 90;
        this.normalFOV = 60;
        this.zoomFOV = 30;
        this.lerpSpeed = 8f;
        this.distance = 10f;
        this.maxShake = 2f;
        this.shakeSpeed = 2f;
        this.shakeTimes = 8;
    }

}