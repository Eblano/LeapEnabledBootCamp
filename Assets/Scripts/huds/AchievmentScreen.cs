using UnityEngine;
using System.Collections;

[System.Serializable]
public class AchievmentScreen : MonoBehaviour
{
    public Texture2D menuBackground;
    private Rect menuBackgroundRect;
    public Texture2D windowBackground;
    private Rect windowBackgroundRect;
    public Texture2D dummyHead;
    private Rect dummyHeadRect;
    public Texture2D dummyLArm;
    private Rect dummyLArmRect;
    public Texture2D dummyRArm;
    private Rect dummyRArmRect;
    public Texture2D dummyHeart;
    private Rect dummyHeartRect;
    public Texture2D dummyTorso;
    private Rect dummyTorsoRect;
    public Texture2D dummyChest;
    private Rect dummyChestRect;
    private Rect dummyRect;
    public Texture2D exploreMap;
    public Texture2D exploreMapOver;
    private Rect exploreMapRect;
    public Texture2D restartTraining;
    public Texture2D restartTrainingOver;
    private Rect restartTrainingRect;
    private Rect scrollPosition;
    private Rect scrollView;
    private Vector2 scroll;
    public Texture2D achievmentDone;
    public Texture2D achievmentUndone;
    private Rect achievmentIconBackgroundRect;
    private Rect achievmentBackgroundRect;
    public GUISkin hudSkin;
    public GUIStyle panelLeft;
    private Rect panelLeftRect;
    public GUIStyle panelRight;
    private Rect panelRightRect;
    public Achievment[] achievments;
    private int activeAchievments;
    private GUIStyle descriptionStyle;
    private GUIStyle titleStyle;
    private GUIStyle dummyInfoStyle;
    private GUIStyle customBox;
    private int head;
    private int chest;
    private int lArm;
    private int rArm;
    private int heart;
    private int torso;
    private GUIStyle titleBackground;
    private Rect titleRect;
    public bool visible;
    private Event evt;
    private float lastMouseTime;
    public static bool returningToTraining;
    private float overallAlpha;
    private float alpha;
    public Texture2D black;
    public virtual void Start()
    {
        this.lastMouseTime = 0f;
        this.visible = false;
        AchievmentScreen.returningToTraining = false;
        this.alpha = 0f;
        this.descriptionStyle = this.hudSkin.GetStyle("Description");
        this.titleStyle = this.hudSkin.GetStyle("Titles");
        this.dummyInfoStyle = this.hudSkin.GetStyle("DummyInfo");
        this.customBox = this.hudSkin.GetStyle("CustomBox");
        this.titleBackground = this.hudSkin.GetStyle("TitleBackground");
        this.menuBackgroundRect = new Rect(0, 0, this.menuBackground.width, this.menuBackground.height);
        this.windowBackgroundRect = new Rect(0, 0, this.windowBackground.width, this.windowBackground.height);
        this.titleRect = new Rect(0, 0, 478, 25);
        this.panelLeftRect = new Rect(0, 0, 220, 220);
        this.panelRightRect = new Rect(0, 0, 250, 220);
        this.dummyRect = new Rect(0, 0, 128, 178);
        this.exploreMapRect = new Rect(0, 0, this.exploreMap.width * 0.75f, this.exploreMap.height * 0.75f);
        this.restartTrainingRect = new Rect(0, 0, this.restartTraining.width * 0.75f, this.restartTraining.height * 0.75f);
        this.scrollPosition = new Rect(0, 0, 240, 190);
        this.achievmentBackgroundRect = new Rect(0, 0, 222, 60);
        this.dummyHeadRect = new Rect(0, 0, this.dummyHead.width, this.dummyHead.height);
        this.dummyLArmRect = new Rect(0, 0, this.dummyLArm.width, this.dummyLArm.height);
        this.dummyRArmRect = new Rect(0, 0, this.dummyRArm.width, this.dummyRArm.height);
        this.dummyTorsoRect = new Rect(0, 0, this.dummyTorso.width, this.dummyTorso.height);
        this.dummyChestRect = new Rect(0, 0, this.dummyChest.width, this.dummyChest.height);
        this.dummyHeartRect = new Rect(0, 0, this.dummyHeart.width, this.dummyHeart.height);
        this.activeAchievments = 0;
        int i = 0;
        while (i < this.achievments.Length)
        {
            if (this.achievments[i].enabled)
            {
                this.activeAchievments++;
            }
            i++;
        }
        this.scrollView = new Rect(0, 0, 223, this.activeAchievments * 65f);
        this.achievmentIconBackgroundRect = new Rect(0, 0, this.achievmentDone.width, this.achievmentDone.height);
    }

    public virtual void Update()
    {
        if (!this.visible || GameManager.pause)
        {
            if (!this.visible)
            {
                this.overallAlpha = 0f;
            }
            return;
        }
        this.CheckAchievments();
        this.CheckAccuracy();
        if (AchievmentScreen.returningToTraining)
        {
            this.alpha = this.alpha + Time.deltaTime;
            if (this.alpha >= 1f)
            {
                Application.LoadLevel("demo_industry");
            }
        }
        if (this.visible)
        {
            if (this.overallAlpha < 1f)
            {
                this.overallAlpha = this.overallAlpha + Time.deltaTime;
            }
        }
    }

    public virtual void DrawGUI(Event @event)
    {
        if (!this.visible || GameManager.pause)
        {
            return;
        }
        GUI.color = new Color(1f, 1f, 1f, this.overallAlpha);
        this.evt = @event;//Event.current;
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        this.menuBackgroundRect.x = 0;
        this.menuBackgroundRect.y = ((screenSize.y - this.menuBackgroundRect.height) * 0.5f) - 50;
        //playGameRect.x = menuBackgroundRect.x + 93;// + ((menuBackgroundRect.width - playGameRect.width) * 0.5);
        //playGameRect.y = menuBackgroundRect.y + 62
        this.exploreMapRect.x = this.menuBackgroundRect.x + 93;//((menuBackgroundRect.width - exploreMapRect.width) * 0.5) - 10;
        this.exploreMapRect.y = this.menuBackgroundRect.y + 62;//70;
        this.restartTrainingRect.x = this.exploreMapRect.x;
        this.restartTrainingRect.y = (this.exploreMapRect.y + this.exploreMapRect.height) + 30;
        this.windowBackgroundRect.x = this.menuBackgroundRect.x + this.menuBackground.width;
        this.windowBackgroundRect.y = (screenSize.y - this.windowBackgroundRect.height) * 0.5f;
        this.panelLeftRect.x = this.windowBackgroundRect.x + 15;
        this.panelLeftRect.y = this.windowBackgroundRect.y + 80;
        this.panelRightRect.x = (this.panelLeftRect.x + 10) + this.panelLeftRect.width;
        this.panelRightRect.y = this.panelLeftRect.y;
        this.scrollPosition.x = this.panelRightRect.x + 5;
        this.scrollPosition.y = this.panelRightRect.y + 25;
        this.dummyRect.x = this.panelLeftRect.x + ((this.panelLeftRect.width - this.dummyRect.width) * 0.5f);
        this.dummyRect.y = this.panelLeftRect.y + ((this.panelLeftRect.height - this.dummyRect.height) * 0.5f);
        //Draw GUI stuff
        GUI.DrawTexture(this.windowBackgroundRect, this.windowBackground);
        GUI.Box(this.panelLeftRect, "", this.panelLeft);
        GUI.Box(this.panelRightRect, "", this.panelRight);
        Color c = GUI.color;
        Color d = c;
        float auxV = this.head;
        auxV = auxV * 0.01f;
        d.a = 0.2f + (0.8f * auxV);
        GUI.color = d;
        this.dummyHeadRect.x = this.dummyRect.x + 45;
        this.dummyHeadRect.y = this.dummyRect.y + 0;
        GUI.DrawTexture(this.dummyHeadRect, this.dummyHead);
        auxV = this.lArm;
        auxV = auxV * 0.01f;
        d.a = 0.2f + (0.8f * auxV);
        GUI.color = d;
        this.dummyLArmRect.x = this.dummyRect.x + 0;
        this.dummyLArmRect.y = this.dummyRect.y + 58;
        GUI.DrawTexture(this.dummyLArmRect, this.dummyLArm);
        auxV = this.rArm;
        auxV = auxV * 0.01f;
        d.a = 0.2f + (0.8f * auxV);
        GUI.color = d;
        this.dummyRArmRect.x = this.dummyRect.x + 101;
        this.dummyRArmRect.y = this.dummyRect.y + 58;
        GUI.DrawTexture(this.dummyRArmRect, this.dummyRArm);
        auxV = this.heart;
        auxV = auxV * 0.01f;
        d.a = 0.2f + (0.8f * auxV);
        GUI.color = d;
        this.dummyHeartRect.x = this.dummyRect.x + 60;
        this.dummyHeartRect.y = this.dummyRect.y + 59;
        GUI.DrawTexture(this.dummyHeartRect, this.dummyHeart);
        auxV = this.chest;
        auxV = auxV * 0.01f;
        d.a = 0.2f + (0.8f * auxV);
        GUI.color = d;
        this.dummyChestRect.x = this.dummyRect.x + 20;
        this.dummyChestRect.y = this.dummyRect.y + 55;
        GUI.DrawTexture(this.dummyChestRect, this.dummyChest);
        auxV = this.torso;
        auxV = auxV * 0.01f;
        d.a = 0.2f + (0.8f * auxV);
        GUI.color = d;
        this.dummyTorsoRect.x = this.dummyRect.x + 30;
        this.dummyTorsoRect.y = this.dummyRect.y + 98;
        GUI.DrawTexture(this.dummyTorsoRect, this.dummyTorso);
        GUI.color = c;
        //*/
        GUISkin cSkin = GUI.skin;
        GUI.skin = this.hudSkin;
        this.achievmentBackgroundRect.x = 1;
        this.achievmentBackgroundRect.y = 0;
        this.achievmentIconBackgroundRect.x = 7;
        this.achievmentIconBackgroundRect.y = 14;
        GUI.Label(new Rect(this.dummyRect.x + 1, this.dummyRect.y + 14, this.dummyRect.width, 20), ("HEAD\n" + this.head.ToString()) + "%", this.dummyInfoStyle);
        GUI.Label(new Rect(this.dummyRect.x + 1, this.dummyRect.y + 115, this.dummyRect.width, 20), ("TORSO\n" + this.torso.ToString()) + "%", this.dummyInfoStyle);
        GUI.Label(new Rect(this.dummyRect.x - 25, this.dummyRect.y + 55, 30, 20), ("L.ARM\n" + this.lArm.ToString()) + "%", this.dummyInfoStyle);
        GUI.Label(new Rect((this.dummyRect.x + this.dummyRect.width) - 4, this.dummyRect.y + 55, 30, 20), ("R.ARM\n" + this.rArm.ToString()) + "%", this.dummyInfoStyle);
        GUI.Label(new Rect(this.dummyRect.x + 28, this.dummyRect.y + 65, 35, 20), ("CHEST\n" + this.chest.ToString()) + "%", this.dummyInfoStyle);
        GUI.Label(new Rect(this.dummyRect.x + 63, this.dummyRect.y + 69, 35, 20), ("HEART\n" + this.heart.ToString()) + "%", this.dummyInfoStyle);
        GUI.Label(new Rect(this.windowBackgroundRect.x + 20, this.windowBackgroundRect.y + 15, 200, 20), "TRAINING SCORES", this.titleStyle);
        GUI.Label(new Rect(this.panelLeftRect.x + 10, this.panelLeftRect.y + 5, 200, 20), "ACCURACY", this.titleStyle);
        GUI.Label(new Rect(this.scrollPosition.x + 20, this.scrollPosition.y - 20, 200, 20), "REWARDS", this.titleStyle);
        this.titleRect.x = this.windowBackgroundRect.x + 16;
        this.titleRect.y = this.windowBackgroundRect.y + 50;
        GUI.Box(this.titleRect, "", this.titleBackground);
        Rect timeRect = new Rect(this.panelLeftRect.x + 10, this.titleRect.y + 4, 200, 20);
        string time = "";
        int hours = (int) GameManager.time;
        hours = hours / 3600;
        int minutes = (int) GameManager.time;
        minutes = (minutes / 60) - (hours * 60);
        int seconds = (int) GameManager.time;
        seconds = seconds % 60;
        time = time + ((((((hours < 10 ? "0" : "") + hours.ToString()) + ":") + ((minutes < 10 ? "0" : "") + minutes.ToString())) + ":") + ((seconds < 10 ? "0" : "") + seconds.ToString()));
        GUI.Label(timeRect, "TOTAL TIME: " + time, this.titleStyle);
        this.scroll = GUI.BeginScrollView(this.scrollPosition, this.scroll, this.scrollView);
        int i = 0;
        while (i < this.achievments.Length)
        {
            if (this.achievments[i].enabled)
            {
                GUI.Box(this.achievmentBackgroundRect, "", this.customBox);
                if (this.achievments[i].done)
                {
                    GUI.DrawTexture(this.achievmentIconBackgroundRect, this.achievmentDone);
                    this.hudSkin.label.normal.textColor = new Color(1f, 1f, 1f, 1f);
                    this.descriptionStyle.normal.textColor = new Color(1f, 1f, 1f, 1f);
                }
                else
                {
                    GUI.DrawTexture(this.achievmentIconBackgroundRect, this.achievmentUndone);
                    if (this.achievments[i].showProgress || ((i == 5) && this.achievments[i].done))
                    {
                        Rect dr = this.achievmentIconBackgroundRect;
                        float p1 = this.achievments[i].progress;
                        p1 = p1 / this.achievments[i].maxProgress;
                        dr.height = this.achievmentIconBackgroundRect.height * p1;
                        dr.y = (this.achievmentIconBackgroundRect.y + 35) - dr.height;
                        GUI.DrawTexture(dr, this.achievmentDone);
                    }
                    this.hudSkin.label.normal.textColor = new Color(0.3f, 0.3f, 0.3f, 1f);
                    this.descriptionStyle.normal.textColor = new Color(0.3f, 0.3f, 0.3f, 1f);
                }
                GUI.Label(new Rect(this.achievmentIconBackgroundRect.width + 12, this.achievmentIconBackgroundRect.y - 4, 200, 20), this.achievments[i].name.ToUpper());
                GUI.Label(new Rect(this.achievmentIconBackgroundRect.width + 12, this.achievmentIconBackgroundRect.y + 12, 170, 50), this.achievments[i].description, this.descriptionStyle);
                GUI.DrawTexture(new Rect((this.achievmentIconBackgroundRect.x + ((this.achievmentIconBackgroundRect.width - this.achievments[i].icon.width) * 0.5f)) - 1, (this.achievmentIconBackgroundRect.y + ((this.achievmentIconBackgroundRect.height - this.achievments[i].icon.height) * 0.5f)) - 1, this.achievments[i].icon.width, this.achievments[i].icon.height), this.achievments[i].icon);
                if (this.achievments[i].showProgress)
                {
                    Rect progressRect = new Rect(this.achievmentIconBackgroundRect.width + 10, this.achievmentBackgroundRect.y + 45, 140, 3);
                    GUI.Label(new Rect((progressRect.x + progressRect.width) + 7, progressRect.y - 3, 50, 20), (this.achievments[i].progress.ToString() + "/") + this.achievments[i].maxProgress.ToString(), this.descriptionStyle);
                }
                this.achievmentIconBackgroundRect.y = this.achievmentIconBackgroundRect.y + 65;
                this.achievmentBackgroundRect.y = this.achievmentBackgroundRect.y + 65;
            }
            i++;
        }
        GUI.EndScrollView();
        GUI.skin = cSkin;
        GUI.DrawTexture(this.menuBackgroundRect, this.menuBackground);
        if (this.exploreMapRect.Contains(mousePos))
        {
            GUI.DrawTexture(this.exploreMapRect, this.exploreMapOver);
            if (this.visible && (this.overallAlpha >= 1f))
            {
                if (((this.evt.type == EventType.MouseUp) && (this.evt.button == 0)) && (Time.time > this.lastMouseTime))
                {
                    this.visible = false;
                    GameManager.scores = false;
                    this.lastMouseTime = Time.time;
                }
            }
        }
        else
        {
            GUI.DrawTexture(this.exploreMapRect, this.exploreMap);
        }
        if (this.restartTrainingRect.Contains(mousePos))
        {
            GUI.DrawTexture(this.restartTrainingRect, this.restartTrainingOver);
            if (this.visible && (this.overallAlpha >= 1f))
            {
                if (((this.evt.type == EventType.MouseUp) && (this.evt.button == 0)) && (Time.time > this.lastMouseTime))
                {
                    AchievmentScreen.returningToTraining = true;
                    this.lastMouseTime = Time.time;
                }
            }
        }
        else
        {
            GUI.DrawTexture(this.restartTrainingRect, this.restartTraining);
        }
        if (AchievmentScreen.returningToTraining)
        {
            GUI.color = new Color(1f, 1f, 1f, this.alpha);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.black);
        }
    }

    public virtual void CheckAchievments()
    {
        this.achievments[0].progress = TrainingStatistics.headShoot;
        this.achievments[0].done = this.achievments[0].progress >= this.achievments[0].maxProgress;
        this.achievments[1].done = GameManager.time < 180f;
        this.achievments[4].done = (TrainingStatistics.turretsHit == 0) && (TrainingStatistics.dummiesHit == 0);
        this.achievments[5].progress = TrainingStatistics.turrets;
        this.achievments[5].done = (TrainingStatistics.turretsHit == 0) && (this.achievments[5].progress >= this.achievments[5].maxProgress);
        this.achievments[8].progress = TrainingStatistics.eaglesEye;
        this.achievments[8].done = this.achievments[8].progress >= this.achievments[8].maxProgress;
        this.achievments[9].progress = TrainingStatistics.blueLeaf;
        this.achievments[9].done = this.achievments[9].progress >= this.achievments[9].maxProgress;
    }

    public virtual void CheckAccuracy()
    {
        if (TrainingStatistics.totalHits <= 0)
        {
            return;
        }
        float aux = 0f;
        float tShoots = 100f / TrainingStatistics.totalHits;
        aux = TrainingStatistics.head;
        aux = aux * tShoots;
        this.head = (int) aux;
        aux = TrainingStatistics.lArm;
        aux = aux * tShoots;
        this.lArm = (int) aux;
        aux = TrainingStatistics.rArm;
        aux = aux * tShoots;
        this.rArm = (int) aux;
        aux = TrainingStatistics.heart;
        aux = aux * tShoots;
        this.heart = (int) aux;
        aux = TrainingStatistics.chest;
        aux = aux * tShoots;
        this.chest = (int) aux;
        aux = TrainingStatistics.torso;
        aux = aux * tShoots;
        this.torso = (int) aux;
    }

}