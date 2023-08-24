using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum StreamingStep
{
    None = 0,
    Helicopter = 1,
    Pilot = 2,
    Wingman = 3,
    Coleague = 4,
    Soldier = 5,
    Terrain = 6,
    Cutscene = 7
}

[System.Serializable]
public class StreamingController : MonoBehaviour
{
    private AsyncOperation currentOp;
    private StreamingStep streamingStep;
    private bool ready;
    private GameObject helicopterGO;
    private GameObject crewContainerGO;
    private StartCutscene cutsceneController;
    private GameObject auxGO;
    private bool readyToPlayCutscene;
    public Transform heliParent;
    public int cutIterations;
    public float[] fixedCamAnimationWeights;
    public List<Transform> fixedCamAngles = new List<Transform>();
    public GameObject cloudBed;
    public float theScale;
    public float lerpSpeed;
    private bool loadedSoldiers;
    private float currentProgress;
    public Transform heliStartPoint;
    public Transform heliEndPoint;
    public Transform heliFlyAwayPoint;
    public Transform heliHoverPoint;
    public float startFOV;
    public float endFOV;
    public GameObject fakeClouds;
    private bool readyToLoadTerrain;
    public GUIStyle textStyle;
    public Texture2D loadingBackground;
    private float angle;
    public static bool loadForest;
    public Texture2D blackTexture;
    private float alpha;
    private bool started;
    public MainMenuScreen mainMenu;
    public SargeManager sarge;
    public GameManager gameManager;
    public GameQualitySettings quality;
    private AudioSource heliSound;
    private WWW con;
    public static string baseAddress;
    private AssetBundle auxBundle;
    private float helicopterProgress;
    private float pilotProgress;
    private float wingmanProgress;
    private float coleagueProgress;
    private float soldierProgress;
    private float terrainProgress;
    private float forestProgress;
    public virtual void Start()
    {
        this.started = false;
        this.currentProgress = 0f;
        this.angle = 0f;
        this.loadedSoldiers = false;
        this.readyToPlayCutscene = false;
        this.streamingStep = StreamingStep.None;
        this.readyToLoadTerrain = false;
        this.GetComponent<Camera>().fieldOfView = this.startFOV;
        StreamingController.loadForest = false;
        this.alpha = 0f;
        Screen.lockCursor = true;
        if (Application.isEditor)
        {
            StreamingController.baseAddress = ("file://" + Application.dataPath) + "/../webplayer/";
        }
        else
        {
            if ((Application.platform == RuntimePlatform.OSXPlayer) && (Application.platform != RuntimePlatform.WindowsPlayer))
            {
                StreamingController.baseAddress = ("file://" + Application.dataPath) + "../../webplayer/";
            }
            else
            {
                StreamingController.baseAddress = "";
            }
        }
        this.con = new WWW(StreamingController.baseAddress + "helicopter.unity3d");
    }

    public float _hOfs;
    public float _vOfs;
    public virtual void Update()
    {
         // a little bit of control
        if (!GameManager.pause)
        {
            float h = Input.GetAxis("Mouse X") * 0.25f;
            float v = Input.GetAxis("Mouse Y") * 0.25f;
            this._hOfs = this._hOfs + h;
            this._vOfs = this._vOfs + v;
            this._hOfs = Mathf.Clamp(this._hOfs, -7.5f, 7.5f);
            this._vOfs = Mathf.Clamp(this._vOfs, -7.5f, 7.5f);
            if (this._mouseControl)
            {

                {
                    float _144 = this._hOfs;
                    Vector3 _145 = this.GetComponent<Camera>().transform.localEulerAngles;
                    _145.y = _144;
                    this.GetComponent<Camera>().transform.localEulerAngles = _145;
                }

                {
                    float _146 = this._vOfs;
                    Vector3 _147 = this.GetComponent<Camera>().transform.localEulerAngles;
                    _147.x = _146;
                    this.GetComponent<Camera>().transform.localEulerAngles = _147;
                }
            }
        }
        switch (this.streamingStep)
        {
            case StreamingStep.None:
                if (!(this.con == null)) //Application.GetStreamProgressForLevel("demo_start_cutscene_helicopter") >= 1.0)
                {
                    if (this.con.isDone)
                    {
                        this.auxBundle = this.con.assetBundle;
                        this.helicopterProgress = 1f;
                        this.currentOp = Application.LoadLevelAdditiveAsync("demo_start_cutscene_helicopter");
                        this.streamingStep = StreamingStep.Helicopter;
                        this.con.Dispose();
                        this.con = null;
                    }
                    else
                    {
                        this.helicopterProgress = this.con.progress;
                    }
                }
                break;
            case StreamingStep.Helicopter:
                this.ready = false;
                this.currentProgress = 1f;
                if (!(this.currentOp == null))
                {
                    if (this.currentOp.isDone)
                    {
                        this.ready = true;
                        this.crewContainerGO = GameObject.Find("Crew");
                        this.auxGO = GameObject.Find("Cutscene");
                        Component[] cameras = this.auxGO.GetComponentsInChildren(typeof(Camera)) as Component[];
                        int i = 0;
                        while (i < cameras.Length)
                        {
                            this.gameManager.PauseEffectCameras[i + 1] = cameras[i] as Camera;
                            this.quality.cameras[i + 1] = cameras[i] as Camera;
                            i++;
                        }
                        if (this.auxGO != null)
                        {
                            this.cutsceneController = this.auxGO.GetComponent("StartCutscene") as StartCutscene;
                            this.helicopterGO = this.cutsceneController.heliRef.gameObject;
                            if (this.cutsceneController.blurRefBack)
                            {
                                this.cutsceneController.blurRefBack.gameObject.active = false;
                            }
                            if (this.cutsceneController.blurRef)
                            {
                                this.cutsceneController.blurRef.gameObject.active = false;
                            }
                        }
                        if (!this.started)
                        {
                            this.StartCoroutine("GoToHeli");
                        }
                        this.con = new WWW(StreamingController.baseAddress + "pilot.unity3d");
                        this.currentOp = null;
                    }
                }
                else
                {
                    this.ready = true;
                }
                if (this.ready)
                {
                    if (!(this.con == null))//Application.GetStreamProgressForLevel("demo_start_cutscene_pilot") >= 1.0)
                    {
                        if (this.con.isDone)
                        {
                            this.auxBundle = this.con.assetBundle;
                            this.pilotProgress = 1f;
                            this.currentOp = Application.LoadLevelAdditiveAsync("demo_start_cutscene_pilot");
                            this.streamingStep = StreamingStep.Pilot;
                            this.con.Dispose();
                            this.con = null;
                        }
                        else
                        {
                            this.pilotProgress = this.con.progress;
                        }
                    }
                }
                break;
            case StreamingStep.Pilot:
                if (this.LoadChar("Pilot", "wingman"))
                {
                    this.wingmanProgress = 1f;
                    this.streamingStep = StreamingStep.Wingman;
                }
                else
                {
                    if (!(this.con == null))
                    {
                        this.wingmanProgress = this.con.progress;
                    }
                }
                break;
            case StreamingStep.Wingman:
                if (this.LoadChar("Wingman", "coleague"))
                {
                    this.coleagueProgress = 1f;
                    this.streamingStep = StreamingStep.Coleague;
                }
                else
                {
                    if (!(this.con == null))
                    {
                        this.coleagueProgress = this.con.progress;
                    }
                }
                break;
            case StreamingStep.Coleague:
                if (this.LoadChar("Coleague", "mainsoldier"))
                {
                    this.streamingStep = StreamingStep.Soldier;
                    this.soldierProgress = 1f;
                    this.auxGO = GameObject.Find("Coleague");
                    this.cutsceneController.coleague = (Animation) this.auxGO.GetComponentInChildren(typeof(Animation));
                }
                else
                {
                    if (!(this.con == null))
                    {
                        this.soldierProgress = this.con.progress;
                    }
                }
                break;
            case StreamingStep.Soldier:
                if (!this.readyToLoadTerrain)
                {
                    if (this.LoadChar("MainSoldier", null))
                    {
                        if (!this.loadedSoldiers)
                        {
                            this.loadedSoldiers = true;
                            this.auxGO = GameObject.Find("MainSoldier");
                            this.con = new WWW(StreamingController.baseAddress + "terrain.unity3d");
                            this.cutsceneController.soldierT = this.auxGO.transform;
                            this.cutsceneController.soldierRenderer = (SkinnedMeshRenderer) this.auxGO.GetComponentInChildren(typeof(SkinnedMeshRenderer));
                        }
                        if (!(this.con == null))//Application.GetStreamProgressForLevel("demo_start_cutscene_terrain") >= 1.0)
                        {
                            if (this.con.isDone)
                            {
                                this.auxBundle = this.con.assetBundle;
                                this.readyToLoadTerrain = true;
                                this.terrainProgress = 1f;
                                this.con.Dispose();
                                this.con = null;
                            }
                            else
                            {
                                this.terrainProgress = this.con.progress;
                            }
                        }
                    }
                }
                break;
            case StreamingStep.Terrain:
                this.ready = false;
                if (!(this.currentOp == null))
                {
                    if (this.currentOp.isDone)
                    {
                        this.ready = true;
                    }
                }
                else
                {
                    this.ready = true;
                }
                if (this.ready)
                {
                    this.streamingStep = StreamingStep.Cutscene;
                    this.readyToPlayCutscene = true;
                }
                break;
        }
        if (StreamingController.loadForest)
        {
            if (this.alpha < 1f)
            {
                this.alpha = this.alpha + Time.deltaTime;
                if (this.alpha >= 1f)
                {
                    this.alpha = 1.2f;
                    Application.LoadLevelAsync("demo_forest");
                }
            }
        }
        this.HandleProgress();
        if (this.heliSound != null)
        {
            if (this.heliSound.volume < 0.45f)
            {
                this.heliSound.volume = this.heliSound.volume + Time.deltaTime;
            }
            else
            {
                this.heliSound = null;
            }
        }
    }

    public virtual void HandleProgress()
    {
        this.currentProgress = 1f;
        this.angle = this.angle - (Time.deltaTime * 360);
        if (this.angle < -360f)
        {
            this.angle = this.angle + 360f;
        }
        if (this.streamingStep == StreamingStep.None)
        {
            this.currentProgress = this.helicopterProgress;//Application.GetStreamProgressForLevel("demo_start_cutscene_helicopter");
        }
        if ((((this.streamingStep == StreamingStep.Helicopter) || (this.streamingStep == StreamingStep.Pilot)) || (this.streamingStep == StreamingStep.Wingman)) || (this.streamingStep == StreamingStep.Coleague))
        {
            this.currentProgress = this.pilotProgress;//Application.GetStreamProgressForLevel("demo_start_cutscene_pilot");
            this.currentProgress = this.currentProgress + this.wingmanProgress;//Application.GetStreamProgressForLevel("demo_start_cutscene_wingman");
            this.currentProgress = this.currentProgress + this.coleagueProgress;//Application.GetStreamProgressForLevel("demo_start_cutscene_coleague");
            this.currentProgress = this.currentProgress + this.soldierProgress;//Application.GetStreamProgressForLevel("demo_start_cutscene_mainsoldier");
            this.currentProgress = this.currentProgress / 4f;
        }
        if (this.streamingStep == StreamingStep.Soldier)
        {
            this.currentProgress = this.terrainProgress;//Application.GetStreamProgressForLevel("demo_start_cutscene_terrain");
        }
        else
        {
            if (this.streamingStep == StreamingStep.Cutscene)
            {
                this.currentProgress = StartCutscene.forestProgress;//Application.GetStreamProgressForLevel("demo_forest");
            }
        }
        this.currentProgress = this.currentProgress * 100f;
        int aux = (int) this.currentProgress;
        this.currentProgress = aux;
    }

    public virtual void OnGUI()
    {
        Color c = default(Color);
        Color g = default(Color);
        Event evt = Event.current;
        if (this.sarge != null)
        {
            this.sarge.DrawGUI(evt);
        }
        if (this.mainMenu != null)
        {
            this.mainMenu.DrawGUI(evt);
        }
        if (evt.type != EventType.Repaint)
        {
            return;
        }
        if (StreamingController.loadForest)
        {
            c = g = GUI.color;
            c.a = this.alpha;
            GUI.color = c;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.blackTexture);
            GUI.Label(new Rect(Screen.width - 120, Screen.height - 40, 90, 20), "Loading...", this.textStyle);
            GUI.color = g;
            return;
        }
        if (this.currentProgress >= 100f)
        {
            return;
        }
        GUIUtility.RotateAroundPivot(this.angle, new Vector2(Screen.width - 28, Screen.height - 28));
        GUI.DrawTexture(new Rect(Screen.width - 56, Screen.height - 56, 56, 56), this.loadingBackground, ScaleMode.ScaleToFit, true, 0);
        GUI.matrix = Matrix4x4.identity;
        GUI.Label(new Rect(Screen.width - 52, Screen.height - 36, 50, 20), this.currentProgress.ToString(), this.textStyle);
    }

    private bool _mouseControl;
    public virtual void EnableMouseControl(bool enable)
    {
        this._mouseControl = enable;
    }

    public GameObject _startWater;
    public GameObject _startTerrain;
    public virtual void CheckTerrainHide()
    {
        if (this.readyToPlayCutscene)
        {
            this._startTerrain = GameObject.Find("start_terrain");
            if (this._startTerrain)
            {
                (((Terrain) this._startTerrain.GetComponent(typeof(Terrain))) as Terrain).enabled = false;
            }
            this._startWater = GameObject.Find("water");
            if (this._startWater)
            {
                (((MeshRenderer) this._startWater.GetComponent(typeof(MeshRenderer))) as MeshRenderer).enabled = false;
            }
        }
    }

    public virtual IEnumerator GoToHeli()
    {
        if (this.started)
        {
            yield break;
        }
        this.started = true;
        // we need to remember the cutscene to make animations happen after this segment
        Transform cutsceneAni = this.helicopterGO.transform.parent.parent;
        this.helicopterGO.transform.parent.parent = this.heliParent;
        this.helicopterGO.transform.parent.transform.localPosition = Vector3.zero;
        this.helicopterGO.transform.parent.transform.localRotation = Quaternion.identity;
        this.helicopterGO.transform.parent.localScale = this.helicopterGO.transform.parent.localScale * this.theScale;
        Animation ani = ((Animation) this.heliParent.GetComponent(typeof(Animation))) as Animation;
        // trigger the fly-in animation
        ani.enabled = true;
        ani["helicopterintro_start"].wrapMode = WrapMode.Once;
        float restWait = 2.75f;
        yield return new WaitForSeconds(0.25f);
        restWait = restWait + 0.25f;
        float time2Shake = 0.75f;
        restWait = restWait + time2Shake;
        while (time2Shake > 0f)
        {

            {
                float _148 = this.transform.localEulerAngles.x + (Random.Range(-1f, 1f) * Mathf.Clamp01(time2Shake));
                Vector3 _149 = this.transform.localEulerAngles;
                _149.x = _148;
                this.transform.localEulerAngles = _149;
            }
            yield return null;
            time2Shake = time2Shake - Time.deltaTime;
        }
        yield return new WaitForSeconds(ani["helicopterintro_Mid"].clip.length - restWait);
        ani["helicopterintro_Mid"].wrapMode = WrapMode.Loop;
        ani["helicopterintro_Mid"].speed = 1f;
        ani.CrossFade("helicopterintro_Mid", 0.45f);
        // transform.localEulerAngles.x = 0.0;
        yield return new WaitForSeconds(2.5f);
        // sarge.ShowInstruction("good_morning");
        yield return new WaitForSeconds(2.5f);
        // 3 iterations for now
        int camAnglesToShow = 0;
        // we are doing this shit as long as needed
        while (!this.readyToPlayCutscene)
        {
            if (this.readyToLoadTerrain && !GameManager.pause)
            {
                this.streamingStep = StreamingStep.Terrain;
                this.currentOp = Application.LoadLevelAdditiveAsync("demo_start_cutscene_terrain");
            }
            Vector3 oldPos = this.transform.position;
            Quaternion oldRot = this.transform.rotation;
            if (this.transform.parent && this.transform.parent.GetComponent<Animation>())
            {
                this.transform.parent.GetComponent<Animation>().Stop();
            }
            float time2Play = 3f + (Random.value * 2f);
            this.EnableMouseControl(false);
            this.cloudBed.SendMessage("SetCut", 1f);
            float aniWeight = 1f;
            int indexForSpeed = camAnglesToShow % this.fixedCamAngles.Count;
            if (indexForSpeed >= this.fixedCamAnimationWeights.Length)
            {
                indexForSpeed = this.fixedCamAnimationWeights.Length - 1;
            }
            if (indexForSpeed >= 0)
            {
                aniWeight = this.fixedCamAnimationWeights[indexForSpeed];
            }
            ani["helicopterintro_Mid"].speed = aniWeight;
            while (time2Play > 0f)
            {
                this.transform.position = this.fixedCamAngles[camAnglesToShow % this.fixedCamAngles.Count].position;
                this.transform.rotation = this.fixedCamAngles[camAnglesToShow % this.fixedCamAngles.Count].rotation;
                this.CheckTerrainHide();
                yield return null;
                time2Play = time2Play - Time.deltaTime;
            }
            this.transform.position = oldPos;
            this.transform.rotation = oldRot;
            if (this.transform.parent && this.transform.parent.GetComponent<Animation>())
            {
                this.transform.parent.GetComponent<Animation>().Play();
            }
            this.EnableMouseControl(true);
            this.cloudBed.SendMessage("SetCut", 0f);
            ani["helicopterintro_Mid"].speed = 1f;
            time2Play = Random.Range(3f, 4f);
            while (time2Play > 0f)
            {
                this.CheckTerrainHide();
                yield return null;
                time2Play = time2Play - Time.deltaTime;
            }
            camAnglesToShow++;
        }
        /*
        var mouseOrbit : MouseOrbit = gameObject.GetComponent("MouseOrbit") as MouseOrbit;

        var heliT : Transform = helicopterGO.transform.parent;
        heliT.position = heliStartPoint.position;
        heliT.rotation = heliStartPoint.rotation;

        yield WaitForSeconds(0.2);
		*/
        // sarge.ShowInstruction("mouse_look");
        // sarge.ShowInstruction("menu");
        this.heliSound = this.cutsceneController.heliSound;
        /*
        while(!readyToLoadTerrain || GameManager.pause)
        {
            yield;
        }

        streamingStep = StreamingStep.Terrain;
        currentOp = Application.LoadLevelAdditiveAsync("demo_start_cutscene_terrain");

        while(!readyToPlayCutscene)
        {
            yield;
        }
        */
        // trigger fly away
        // trigger fly away
        ani["helicopterintro_Mid"].normalizedTime = Mathf.Repeat(ani["helicopterintro_Mid"].normalizedTime, 1);
        while (ani["helicopterintro_Mid"].normalizedTime < 1)
        {
            yield return null;
        }
        ani.CrossFade("helicopterintro_End", 0.1f);
        yield return new WaitForSeconds(ani["helicopterintro_End"].clip.length);
        // set correct helicopter position
        ani.enabled = false;
        this.helicopterGO.transform.parent.parent = cutsceneAni;
        GameObject.Find("AssignSkybox").SendMessage("DoJoachimsSkyboxThing");
        (((Terrain) this._startTerrain.GetComponent(typeof(Terrain))) as Terrain).enabled = true;
        (((MeshRenderer) this._startWater.GetComponent(typeof(MeshRenderer))) as MeshRenderer).enabled = true;
        //var heliT : Transform = helicopterGO.transform.parent;
        //heliT.position = heliFlyAwayPoint.position;
        //heliT.rotation = heliFlyAwayPoint.rotation;        
        this.cutsceneController.enabled = true;
        // disable all the camera cloud effects shit
        (this.GetComponent<Camera>().GetComponent("CloudEffects") as MonoBehaviour).enabled = false;
        this.GetComponent<Camera>().enabled = false;
        // destroy clouds
        if (this.fakeClouds)
        {
            UnityEngine.Object.Destroy(this.fakeClouds);
        }
        AudioListener listener = this.gameObject.GetComponent("AudioListener") as AudioListener;
        if (listener != null)
        {
            listener.enabled = false;
        }
    }

    public virtual bool LoadChar(string current, string next)
    {
        this.ready = false;
        if (!(this.currentOp == null))
        {
            if (this.currentOp.isDone)
            {
                this.ready = true;
                this.auxGO = GameObject.Find(current);
                if (this.auxGO != null)
                {
                    this.auxGO.transform.parent = this.crewContainerGO.transform;
                    this.auxGO.transform.localPosition = Vector3.zero;
                    this.auxGO.transform.localScale = Vector3.one;
                    this.auxGO.transform.localRotation = Quaternion.identity;
                }
                if (next != null)
                {
                    this.con = new WWW((StreamingController.baseAddress + next) + ".unity3d");
                }
                this.currentOp = null;
            }
        }
        else
        {
            this.ready = true;
            this.auxGO = GameObject.Find(current);
            if (this.auxGO != null)
            {
                this.auxGO.transform.parent = this.crewContainerGO.transform;
            }
        }
        if (this.ready)
        {
            if (next != null)
            {
                if (!(this.con == null) && this.con.isDone)//Application.GetStreamProgressForLevel("demo_start_cutscene_" + next) >= 1.0)
                {
                    this.auxBundle = this.con.assetBundle;
                    this.currentOp = Application.LoadLevelAdditiveAsync("demo_start_cutscene_" + next);
                    this.con.Dispose();
                    this.con = null;
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    public StreamingController()
    {
        this.cutIterations = 1;
        this.theScale = 2f;
        this.lerpSpeed = 3f;
        this._mouseControl = true;
    }

}