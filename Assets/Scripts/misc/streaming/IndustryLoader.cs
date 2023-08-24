using UnityEngine;
using System.Collections;

[System.Serializable]
public class IndustryLoader : MonoBehaviour
{
    public SargeManager sarge;
    public GameObject endSceneTrigger;
    private bool playing;
    private AssetBundle auxBundle;
    private WWW con;
    public static float industryProgress;
    public virtual void Start()
    {
        if (this.endSceneTrigger != null)
        {
            UnityEngine.Object.Destroy(this.endSceneTrigger);
        }
        this.playing = false;
        this.con = new WWW(StreamingController.baseAddress + "industry.unity3d");
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (!this.playing)
        {
            if (other.name.ToLower() == "soldier")
            {
                this.playing = true;
                this.StartCoroutine("LoadIndustry");
            }
        }
    }

    public virtual IEnumerator LoadIndustry()
    {
         //var progress : float = Application.GetStreamProgressForLevel("demo_industry");
        if (!(this.con == null) && this.con.isDone)//progress >= 1.0)
        {
            this.auxBundle = this.con.assetBundle;
            IndustryLoader.industryProgress = 1f;
            MainMenuScreen.goingToGame = true;
            this.con.Dispose();
            this.con = null;
        }
        else
        {
            MainMenuScreen.showProgress = true;
            this.sarge.ShowInstruction("preparing_bots");
            while (!this.con.isDone)//progress < 1.0)
            {
                IndustryLoader.industryProgress = this.con.progress;
                //progress = Application.GetStreamProgressForLevel("demo_industry");
                yield return null;
            }
            this.auxBundle = this.con.assetBundle;
            MainMenuScreen.goingToGame = true;
            this.con.Dispose();
            this.con = null;
        }
    }

}