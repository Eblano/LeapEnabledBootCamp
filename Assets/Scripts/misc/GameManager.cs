using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameManager : MonoBehaviour
{
    public GameObject gamePlaySoldier;
    public ParticleSystem soldierSmoke;
    public SargeManager sarge;
    public static bool receiveDamage;
    public static bool pause;
    public static bool scores;
    public static float time;
    public static object running;
    public MainMenuScreen menu;
    public Camera[] PauseEffectCameras;
    private bool _paused;
    public virtual void Start()
    {
        TrainingStatistics.ResetStatistics();
        Screen.lockCursor = true;
        GameManager.running = false;
        GameManager.pause = false;
        GameManager.scores = false;
        this._paused = false;
        GameManager.time = 0f;
        Transform auxT = null;
        bool hasCutscene = false;
        int i = 0;
        while (i < this.transform.childCount)
        {
            auxT = this.transform.GetChild(i);
            if (auxT.name == "Cutscene")
            {
                if (auxT.gameObject.active)
                {
                    hasCutscene = true;
                    break;
                }
            }
            i++;
        }
        if (!hasCutscene)
        {
            this.StartGame();
        }
    }

    public virtual void CutsceneStart()
    {
        this.gamePlaySoldier.SetActiveRecursively(false);
    }

    public virtual void Update()
    {
        if (!GameManager.pause && (GameManager.running != null))
        {
            GameManager.time = GameManager.time + Time.deltaTime;
        }
        if ((Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Escape)) || Input.GetKeyDown(KeyCode.P))
        {
            GameManager.pause = !GameManager.pause;
            this.menu.visible = GameManager.pause;
            if (GameManager.pause)
            {
                Time.timeScale = 1E-05f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
        if (this._paused != GameManager.pause)
        {
            this._paused = GameManager.pause;
            this.CameraBlur(GameManager.pause);
            int i = 0;
            while (i < this.PauseEffectCameras.Length)
            {
                Camera cam = this.PauseEffectCameras[i];
                if (cam == null)
                {
                    goto Label_for_55;
                }
                if (cam.name != "radar_camera")
                {
                    goto Label_for_55;
                }
                cam.enabled = !GameManager.pause;
                Label_for_55:
                i++;
            }
        }
        Screen.lockCursor = !GameManager.pause && !GameManager.scores;
    }

    public virtual void StartGame()
    {
        GameManager.running = true;
        if (this.gamePlaySoldier != null)
        {
            if (!this.gamePlaySoldier.active)
            {
                this.gamePlaySoldier.SetActiveRecursively(true);
            }
        }
        if (this.soldierSmoke != null)
        {
            if (GameQualitySettings.ambientParticles)
            {
                this.soldierSmoke.enableEmission = true;
            }
        }
        if ((this.sarge != null) && (Application.loadedLevelName == "demo_forest"))
        {
            this.sarge.ShowInstruction("instructions");
            this.sarge.ShowInstruction("instructions2");
            this.sarge.ShowInstruction("instructions3");
            this.sarge.ShowInstruction("instructions4");
            this.sarge.ShowInstruction("instructions5");
            this.sarge.ShowInstruction("additional_instructions");
        }
    }

    public virtual void CameraBlur(bool state)
    {
        if (this.PauseEffectCameras == null)
        {
            return;
        }
        if (this.PauseEffectCameras.Length <= 0)
        {
            return;
        }
        BlurEffect blurEffect = null;
        int i = 0;
        while (i < this.PauseEffectCameras.Length)
        {
            Camera cam = this.PauseEffectCameras[i];
            if (cam == null)
            {
                goto Label_for_56;
            }
            blurEffect = cam.GetComponent("BlurEffect") as BlurEffect;
            if (blurEffect == null)
            {
                blurEffect = cam.gameObject.AddComponent<BlurEffect>() as BlurEffect;
                blurEffect.iterations = cam.gameObject.name.IndexOf("radar") != -1 ? 1 : 2;
                blurEffect.blurSpread = 0.4f;
            }
            blurEffect.enabled = state;
            Label_for_56:
            i++;
        }
    }

}