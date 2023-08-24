using UnityEngine;
using System.Collections;

[System.Serializable]
public class DownChoperCutscene : MonoBehaviour
{
    public GameObject soldierWeapon;
    public GameObject weaponAnimation;
    public GameObject rope;
    public GameObject soldier;
    public GameObject cutsceneCamera;
    public SargeManager sarge;
    public float timer;
    private bool destroy;
    private bool endCutscene;
    private Transform currentWeaponParent;
    private Vector3 currentWeaponPosition;
    private Quaternion currentWeaponRotation;
    public AudioSource[] audioSources;
    private bool audioStarted;
    public GameObject pilot;
    public GameObject wingman;
    public ParticleSystem[] particles;
    public GameObject windZone;
    public GameObject soldierCamera;
    public virtual void Start()
    {
        GameObject sargeObject = GameObject.Find("SargeManager") as GameObject;
        if (sargeObject != null)
        {
            this.sarge = sargeObject.GetComponent("SargeManager") as SargeManager;
        }
        this.audioStarted = false;
        this.destroy = false;
        this.SendMessageUpwards("CutsceneStart");
        this.endCutscene = false;
        this.rope.GetComponent<Animation>().Play("RopeAnimation");
        this.rope.GetComponent<Animation>()["RopeAnimation"].enabled = true;
        this.rope.GetComponent<Animation>()["RopeAnimation"].time = 0.05f;
        this.rope.GetComponent<Animation>().Sample();
        this.rope.GetComponent<Animation>()["RopeAnimation"].enabled = false;
        this.currentWeaponParent = this.soldierWeapon.transform.parent;
        this.currentWeaponPosition = this.soldierWeapon.transform.localPosition;
        this.currentWeaponRotation = this.soldierWeapon.transform.localRotation;
        this.GetComponent<Animation>().Play("heli_rapel_cutscene");
    }

    public virtual void StartAudios()
    {
        if (!this.audioStarted)
        {
            this.audioStarted = true;
        }
    }

    public virtual void Update()
    {
        int p = 0;
        if (GameManager.pause)
        {
            if (!(this.audioSources == null))
            {
                int i = 0;
                while (i < this.audioSources.Length)
                {
                    if (this.audioSources[i].isPlaying)
                    {
                        this.audioSources[i].Pause();
                    }
                    i++;
                }
            }
            if (this.windZone.active)
            {
                this.windZone.active = false;
            }
            this.GetComponent<Animation>()["heli_rapel_cutscene"].speed = 0f;
            this.rope.GetComponent<Animation>()["RopeAnimation"].speed = 0f;
            this.soldier.GetComponent<Animation>()["CS_Rope"].speed = 0f;
            this.weaponAnimation.GetComponent<Animation>()["Take 001"].speed = 0f;
            this.pilot.GetComponent<Animation>()["CS_Pilot1"].speed = 0f;
            this.wingman.GetComponent<Animation>()["CS_Pilot2"].speed = 0f;
            if (!(this.particles == null))
            {
                p = 0;
                while (p < this.particles.Length)
                {
                    if (this.particles[p] == null)
                    {
                        goto Label_for_4;
                    }
                    if (this.particles[p].emission.enabled)
                    {
                        this.particles[p].emission.enabled = false;
                    }
                    Label_for_4:
                    p++;
                }
            }
            if (!this.endCutscene)
            {
                if (Time.timeScale > 0f)
                {
                    Time.timeScale = 0f;
                }
            }
        }
        else
        {
            if (!this.windZone.active)
            {
                this.windZone.active = true;
            }
            if (this.GetComponent<Animation>()["heli_rapel_cutscene"].speed < 1f)
            {
                this.GetComponent<Animation>()["heli_rapel_cutscene"].speed = 1f;
                this.rope.GetComponent<Animation>()["RopeAnimation"].speed = 1f;
                this.soldier.GetComponent<Animation>()["CS_Rope"].speed = 1f;
                this.weaponAnimation.GetComponent<Animation>()["Take 001"].speed = 1f;
                this.pilot.GetComponent<Animation>()["CS_Pilot1"].speed = 1f;
                this.wingman.GetComponent<Animation>()["CS_Pilot2"].speed = 1f;
                if (!(this.particles == null))
                {
                    p = 0;
                    while (p < this.particles.Length)
                    {
                        if (this.particles[p] == null)
                        {
                            goto Label_for_5;
                        }
                        if (!this.particles[p].emission.enabled)
                        {
                            this.particles[p].emission.enabled = true;
                        }
                        Label_for_5:
                        p++;
                    }
                }
            }
            if (!this.endCutscene)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    this.GetComponent<Animation>().Stop();
                    this.GetComponent<AudioSource>().Stop();
                    this.GetComponent<Animation>()["heli_rapel_cutscene"].enabled = true;
                    this.GetComponent<Animation>()["heli_rapel_cutscene"].time = 25f;
                    this.GetComponent<Animation>().Play("heli_rapel_cutscene");
                    if (this.sarge != null)
                    {
                        this.sarge.StopInstructions();
                    }
                    this.EndCutscene();
                }
                if (this.rope.GetComponent<Animation>()["RopeAnimation"].normalizedTime > 0.6f)
                {
                    this.soldierWeapon.transform.parent = this.currentWeaponParent;
                    this.soldierWeapon.transform.localPosition = this.currentWeaponPosition;
                    this.soldierWeapon.transform.localRotation = this.currentWeaponRotation;
                }
                if (!(this.audioSources == null) && this.audioStarted)
                {
                    int j = 0;
                    while (j < this.audioSources.Length)
                    {
                        if (!this.audioSources[j].isPlaying)
                        {
                            this.audioSources[j].Play();
                        }
                        j++;
                    }
                }
            }
            else
            {
                 //Handle object destruction
                if (this.destroy)
                {
                    this.timer = this.timer - Time.deltaTime;
                    if (this.timer <= 0f)
                    {
                        UnityEngine.Object.Destroy(this.gameObject);
                    }
                }
                else
                {
                    if (this.GetComponent<Animation>()["heli_rapel_cutscene"].normalizedTime > 0.99f)
                    {
                        this.destroy = true;
                    }
                }
            }
        }
    }

    public virtual void StartRapelAnimation()
    {
        if (this.endCutscene)
        {
            return;
        }
        this.soldierWeapon.transform.parent = this.weaponAnimation.transform.GetChild(0);
        this.soldierWeapon.transform.localPosition = Vector3.zero;
        this.weaponAnimation.GetComponent<Animation>().Play("Take 001");
        this.rope.GetComponent<Animation>().Play("RopeAnimation");
        this.soldier.GetComponent<Animation>().Play("CS_Rope");
    }

    public virtual void EndCutscene()
    {
        if (this.endCutscene)
        {
            return;
        }
        this.audioStarted = false;
        if (!(this.audioSources == null))
        {
            int j = 0;
            while (j < this.audioSources.Length)
            {
                this.audioSources[j].Stop();
                j++;
            }
        }
        this.soldier.SetActiveRecursively(false);
        this.soldierWeapon.SetActiveRecursively(false);
        this.cutsceneCamera.SetActiveRecursively(false);
        this.SendMessageUpwards("StartGame");
        Camera soldCam = ((Camera) this.soldierCamera.GetComponentInChildren(typeof(Camera))) as Camera;
        if (soldCam)
        {
            soldCam.enabled = true;
        }
        this.endCutscene = true;
    }

    public virtual void PlayInstruction(int i)
    {
        switch (i)
        {
            case 0:
                this.sarge.ShowInstruction("helicopter_base");
                break;
            case 1:
                this.sarge.ShowInstruction("wait_zone");
                break;
            case 2:
                this.sarge.ShowInstruction("ropes");
                break;
        }
    }

    public DownChoperCutscene()
    {
        this.timer = 2f;
    }

}