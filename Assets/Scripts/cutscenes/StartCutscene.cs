using UnityEngine;
using System.Collections;

[System.Serializable]
public class StartCutscene : MonoBehaviour
{
    public GameObject cutsceneCamera1;
    public GameObject cutsceneCamera2;
    public GameObject thirdPersonCamera;
    public SkinnedMeshRenderer soldierRenderer;
    public Material[] cutsceneMaterials;
    public Material[] thirdPersonMaterials;
    public Transform soldierT;
    private bool loopFinished;
    private bool loading;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    private float currentState;
    private MouseLook cameraController;
    private bool playedPoint;
    public Animation coleague;
    public Transform heliRef;
    public AudioSource heliSound;
    public Transform blurRef;
    public Transform blurRefBack;
    private AssetBundle auxBundle;
    private WWW con;
    public static float forestProgress;
    public SargeManager sarge;
    private bool playedClearing;
    private float timeToPlayRandom;
    private float waitToPlayRandom;
    public virtual void OnEnable()
    {
        this.playedClearing = false;
        this.timeToPlayRandom = this.waitToPlayRandom;
        GameObject go = GameObject.Find("start_terrain");
        Terrain terrain = go.GetComponent("Terrain") as Terrain;
        terrain.treeMaximumFullLODCount = 15;
        GameObject auxCam = GameObject.Find("StartCamera");
        if (auxCam != null)
        {
            this.sarge = auxCam.GetComponent("SargeManager") as SargeManager;
        }
        this.GetComponent<Animation>().Play("intro_cutscene_1");
        this.thirdPersonCamera.active = false;
        this.cutsceneCamera1.active = true;
        this.cutsceneCamera1.GetComponent<Camera>().enabled = true;
        this.cutsceneCamera2.active = true;
        this.loopFinished = false;
        this.loading = false;
        this.playedPoint = false;
        this.currentState = 0f;
        this.targetRotation = Quaternion.Euler(3.931946f, -86.54218f, 0f);
        this.cameraController = this.thirdPersonCamera.GetComponent("MouseLook") as MouseLook;
        if (this.soldierRenderer != null)
        {
            this.soldierRenderer.materials = this.cutsceneMaterials;
        }
        if (this.soldierT != null)
        {
            this.soldierT.localScale = Vector3.one;
            this.soldierT.localPosition = Vector3.zero;
        }
        this.con = new WWW(StreamingController.baseAddress + "forest.unity3d");
        this.sarge.ShowInstruction("good_morning");
        this.sarge.ShowInstruction("menu");
    }

    public virtual void ChangeToThirdPersonCamera()
    {
        this.GetComponent<Animation>()["intro_cutscene_2"].wrapMode = WrapMode.Loop;
        this.GetComponent<Animation>().Play("intro_cutscene_2");
        this.thirdPersonCamera.active = true;
        this.thirdPersonCamera.GetComponent<Camera>().enabled = true;
        this.cutsceneCamera1.active = false;
        this.cutsceneCamera2.active = false;
        GameObject go = GameObject.Find("start_terrain");
        Terrain terrain = go.GetComponent("Terrain") as Terrain;
        terrain.treeMaximumFullLODCount = 0;
        if (this.soldierRenderer != null)
        {
            this.soldierRenderer.materials = this.thirdPersonMaterials;
        }
        if (this.soldierT != null)
        {
            this.soldierT.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            this.soldierT.localPosition = new Vector3(0.1788807f, -0.1774399f, 0.3102949f);
        }
        if (this.sarge != null)
        {
            this.sarge.ShowInstruction("mouse_look");
            this.sarge.ShowInstruction("lz_woods");
        }
    }

    public virtual void LoadingSceneLoop()
    {
        this.loopFinished = true;
    }

    public virtual void Update()
    {
        if (StreamingController.loadForest)
        {
            this.thirdPersonCamera.transform.localRotation = this.targetRotation;
            return;
        }
        if (!this.loading)
        {
            if (GameManager.pause)
            {
                if (this.cameraController.enabled)
                {
                    this.cameraController.enabled = false;
                }
            }
            else
            {
                if (!this.cameraController.enabled)
                {
                    this.cameraController.enabled = true;
                }
            }
        }
        if (StreamingController.loadForest)
        {
            if (this.heliSound.volume > 0f)
            {
                this.heliSound.volume = this.heliSound.volume - Time.deltaTime;
            }
            return;
        }
        if (!(this.con == null) && !this.con.isDone)
        {
            StartCutscene.forestProgress = this.con.progress;
        }
        if (!this.loading && this.thirdPersonCamera.active)
        {
            if (this.sarge != null)
            {
                this.timeToPlayRandom = this.timeToPlayRandom - Time.deltaTime;
                if (this.timeToPlayRandom <= 0f)
                {
                    this.timeToPlayRandom = this.waitToPlayRandom;
                    int aux = Random.Range(0, 2);
                    if (aux == 0)
                    {
                        this.sarge.ShowInstruction("wait1");
                    }
                    else
                    {
                        this.sarge.ShowInstruction("wait2");
                    }
                }
            }
        }
        if (this.loopFinished && !this.loading)
        {
            if (!(this.con == null))
            {
                if (this.con.isDone)
                {
                    this.auxBundle = this.con.assetBundle;
                    StartCutscene.forestProgress = 1f;
                    this.loading = true;
                    this.playedPoint = false;
                    this.cameraController.enabled = false;
                    this.currentState = 0f;
                    this.startRotation = this.thirdPersonCamera.transform.localRotation;
                    this.con.Dispose();
                    this.con = null;
                }
            }
        }
        else
        {
            if (this.loading)
            {
                if (!this.playedPoint)
                {
                    this.currentState = this.currentState + Time.deltaTime;
                    this.thirdPersonCamera.transform.localRotation = Quaternion.Slerp(this.startRotation, this.targetRotation, this.currentState);
                    this.thirdPersonCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(60, 45, this.currentState);
                    this.cameraController.enabled = false;
                    if (this.currentState >= 1f)
                    {
                        this.playedPoint = true;
                        this.coleague.Play("CS_Pointing");
                    }
                }
                else
                {
                    if (!this.playedClearing && (this.coleague["CS_Pointing"].normalizedTime > 0.4f))
                    {
                        this.playedClearing = true;
                        if (this.sarge != null)
                        {
                            this.sarge.ShowInstruction("see_clearing");
                        }
                    }
                    if (this.coleague["CS_Pointing"].normalizedTime > 0.99f)
                    {
                        this.loopFinished = false;
                        this.loading = false;
                        StreamingController.loadForest = true;
                    }
                }
            }
        }
    }

    public StartCutscene()
    {
        this.waitToPlayRandom = 20f;
    }

}