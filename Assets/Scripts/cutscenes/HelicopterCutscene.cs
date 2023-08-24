using UnityEngine;
using System.Collections;

[System.Serializable]
public class HelicopterCutscene : MonoBehaviour
{
    public GameObject inChopperCamera;
    public GameObject cutsceneCamera;
    public GameObject soldier;
    public GameObject coleague;
    public GameObject soldierWeapon;
    public GameObject weaponAnimation;
    public GameObject rope;
    public bool childActive;
    private int currentPlaying;
    public virtual void Start()
    {
        this.currentPlaying = -1;
        this.rope.GetComponent<Animation>().Play("RopeAnimation");
        this.rope.GetComponent<Animation>()["RopeAnimation"].enabled = true;
        this.rope.GetComponent<Animation>()["RopeAnimation"].time = 0.05f;
        this.rope.GetComponent<Animation>().Sample();
        this.rope.GetComponent<Animation>()["RopeAnimation"].enabled = false;
        foreach (Transform t in this.transform)
        {
            t.gameObject.SetActiveRecursively(false);
            this.childActive = false;
        }
        this.GetComponent<Animation>()["heli_load_animation"].wrapMode = WrapMode.Loop;
    }

    public virtual void Update()
    {
        switch (this.currentPlaying)
        {
            case 0:
                if (this.GetComponent<Animation>()["heli_start_animation"].normalizedTime > 0.99f)
                {
                    this.currentPlaying = -1;
                    this.SendMessageUpwards("HeliCutsceneEnd", 0);
                }
                break;
            case 1:
                break;
            case 2:
                if (this.coleague.GetComponent<Animation>()["CS_Pointing"].normalizedTime > 0.99f)
                {
                    this.currentPlaying = -1;
                    this.SendMessageUpwards("HeliCutsceneEnd", 2);
                }
                break;
            case 3:
                if (this.rope.GetComponent<Animation>()["RopeAnimation"].normalizedTime > 0.99f)
                {
                    this.currentPlaying = -1;
                    this.SendMessageUpwards("HeliCutsceneEnd", 3);
                }
                break;
        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            this.SendMessageUpwards("HeliCutsceneEnd", 3);
        }
    }

    public virtual void Play(int step)
    {
        if ((step > 3) || (step < 0))
        {
            return;
        }
        this.currentPlaying = step;
        if (!this.childActive)
        {
            foreach (Transform t in this.transform)
            {
                t.gameObject.SetActiveRecursively(true);
                this.childActive = true;
            }
        }
        switch (step)
        {
            case 0:
                this.inChopperCamera.SetActiveRecursively(false);
                this.GetComponent<Animation>().Play("heli_start_animation");
                this.coleague.GetComponent<Animation>().CrossFade("CS_ColeagueIdle");
                break;
            case 1:
                this.inChopperCamera.SetActiveRecursively(true);
                this.cutsceneCamera.SetActiveRecursively(false);
                this.soldier.SetActiveRecursively(false);
                this.GetComponent<Animation>().Play("heli_load_animation");
                this.coleague.GetComponent<Animation>().CrossFade("CS_ColeagueIdle");
                break;
            case 2:
                this.inChopperCamera.SetActiveRecursively(true);
                this.cutsceneCamera.SetActiveRecursively(false);
                this.soldier.SetActiveRecursively(false);
                this.coleague.GetComponent<Animation>().CrossFade("CS_Pointing");
                break;
            case 3:
                this.inChopperCamera.SetActiveRecursively(false);
                this.cutsceneCamera.SetActiveRecursively(true);
                this.soldier.SetActiveRecursively(true);
                this.coleague.GetComponent<Animation>().CrossFade("CS_ColeagueIdle");
                this.soldierWeapon.transform.parent = this.weaponAnimation.transform.GetChild(0);
                this.soldierWeapon.transform.localPosition = Vector3.zero;
                this.weaponAnimation.GetComponent<Animation>().Play("Take 001");
                this.rope.GetComponent<Animation>().Play("RopeAnimation");
                this.soldier.GetComponent<Animation>().Play("CS_Rope");
                break;
        }
    }

    public virtual void DestroyScene()
    {
        UnityEngine.Object.Destroy(this.gameObject);
    }

}