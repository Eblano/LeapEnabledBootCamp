using UnityEngine;
using System.Collections;

public enum CutsceneBehaviourType
{
    PLAY_ANIM = 0,
    SET_POSITION = 1
}

[System.Serializable]
public class ForestCutsceneBehaviour : object
{
    public CutsceneBehaviourType action;
    public float time;
    public string anim;
    public Vector3 position;
    public Vector3 rotation;
}
[System.Serializable]
public class ForestCutscene : MonoBehaviour
{
    private bool played;
    private bool playing;
    public GameObject soldier;
    public Transform soldierFinalRef;
    public Camera soldierCam;
    public Transform cam;
    private Animation camAnimation;
    public ForestCutsceneBehaviour[] anims;
    public float totalTime;
    public float timer;
    private int cStep;
    private float nextAnimTime;
    private bool part1;
    private bool part2;
    public virtual void Start()
    {
        this.part1 = this.part2 = false;
        this.cStep = 0;
        this.nextAnimTime = this.anims[0].time;
        this.camAnimation = this.cam.GetComponent<Animation>();
        this.played = false;
        this.playing = false;
        this.timer = 0f;
        this.Play();
    }

    public virtual void Update()
    {
        this.timer = this.timer + Time.deltaTime;
        if (GameManager.pause)
        {
            this.soldier.GetComponent<Animation>()["CS_2_Part1"].speed = 0f;
            this.soldier.GetComponent<Animation>()["CS_2_Part2"].speed = 0f;
            this.cam.GetComponent<Animation>()["industryCutsceneCamera_entire"].speed = 0f;
        }
        else
        {
            if (this.soldier.GetComponent<Animation>()["CS_2_Part1"].speed < 1f)
            {
                this.soldier.GetComponent<Animation>()["CS_2_Part1"].speed = 1f;
                this.soldier.GetComponent<Animation>()["CS_2_Part2"].speed = 1f;
                this.cam.GetComponent<Animation>()["industryCutsceneCamera_entire"].speed = 1f;
            }
        }
        if ((this.soldier.GetComponent<Animation>()["CS_2_Part1"].normalizedTime > 0.965f) && !this.part1)
        {
            this.part1 = true;
            this.soldier.GetComponent<Animation>()["CS_2_Part2"].speed = 0.7f;
            this.soldier.GetComponent<Animation>().Play("CS_2_Part2");
            this.soldier.transform.localPosition = new Vector3(3.256119f, this.soldier.transform.localPosition.y, this.soldier.transform.localPosition.z);
        }
        if ((this.soldier.GetComponent<Animation>()["CS_2_Part2"].normalizedTime > 0.27f) && !this.part2)
        {
            this.part2 = true;
            this.soldier.transform.localPosition = new Vector3(0.6527214f, 1.321428f, -1.147861f);
            this.soldier.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        if (!GameManager.pause)
        {
            this.totalTime = this.totalTime - Time.deltaTime;
            if (this.totalTime <= 0f)
            {
                this.StartCoroutine(this.WaitAndDestroy());
            }
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                this.StartCoroutine(this.WaitAndDestroy());
            }
        }
    }

    public virtual IEnumerator WaitAndDestroy()
    {
        this.enabled = false;
        UnityEngine.Object.Destroy(this.cam.GetComponent("AudioListener"));
        this.SendMessageUpwards("StartGame");
        this.soldier.GetComponent<Animation>().Stop();
        if (this.soldierCam)
        {
            this.soldierCam.enabled = true;
        }
        yield return null;
        UnityEngine.Object.Destroy(this.gameObject);
    }

    public virtual void Play()
    {
        if (this.played)
        {
            return;
        }
        this.played = true;
        this.gameObject.SetActiveRecursively(true);
        this.SendMessageUpwards("CutsceneStart", SendMessageOptions.DontRequireReceiver);
        this.soldier.GetComponent<Animation>().CrossFade("CS_2_Part1");
    }

}