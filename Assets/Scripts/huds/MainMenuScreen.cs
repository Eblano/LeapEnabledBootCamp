using UnityEngine;
using System.Collections;

public enum MainMenuState
{
    IDLE = 0,
    OPTIONS = 1,
    GRAPHICS = 2,
    ABOUT = 3
}

[System.Serializable]
public class MainMenuScreen : MonoBehaviour
{
    public Texture2D menuBackground;
    private Rect menuBackgroundRect;
    public Texture2D windowBackground;
    private Rect windowBackgroundRect;
    public Texture2D playGame;
    public Texture2D playGameOver;
    private Rect playGameRect;
    public Texture2D resume;
    public Texture2D resumeOver;
    private Rect resumeRect;
    public Texture2D options;
    public Texture2D optionsOver;
    private Rect optionsRect;
    public Texture2D graphics;
    public Texture2D graphicsOver;
    private Rect graphicsRect;
    public Texture2D about;
    public Texture2D aboutOver;
    private Rect aboutRect;
    public GUISkin hudSkin;
    private GUIStyle panelLeft;
    private Rect panelLeftRect;
    private GUIStyle panelRight;
    private Rect panelRightRect;
    private GUIStyle descriptionStyle;
    private GUIStyle titleStyle;
    private GUIStyle customBox;
    private Vector2 mousePos;
    private Vector2 screenSize;
    private Event evt;
    private MainMenuState state;
    private float lastMouseTime;
    public Texture2D receiveDamageOn;
    public Texture2D receiveDamageOff;
    public Texture2D dontReceiveDamageOn;
    public Texture2D dontReceiveDamageOff;
    private Rect damageRect;
    private Rect scrollPosition;
    private Rect scrollView;
    private Vector2 scroll;
    public Texture2D black;
    private float alpha;
    public static bool goingToGame;
    public static bool showProgress;
    private Vector2 aboutScroll;
    private Vector2 graphicsScroll;
    private GUIStyle aboutStyle;
    private bool resumeGame;
    public bool visible;
    private GUIStyle sliderBackground;
    private GUIStyle sliderButton;
    public Texture2D greenBar;
    public Texture2D checkOn;
    public Texture2D checkOff;
    public Texture2D whiteMarker;
    private float margin;
    private Rect questionRect;
    private Rect greenRect;
    private GUIStyle tooltipStyle;
    private GUIStyle questionButtonStyle;
    private GUIStyle aquirisLogo;
    private GUIStyle unityLogo;
    public AudioClip overSound;
    public float overVolume;
    public AudioClip clickSound;
    public float clickVolume;
    private bool over;
    private bool hideOptions;
    private bool loadingIndustry;
    public GUIStyle textStyle;
    private float angle;
    public Texture2D loadingBackground;
    public virtual void Start()
    {
        this.angle = 0f;
        this.over = false;
        this.loadingIndustry = false;
        MainMenuScreen.showProgress = false;
        this.hideOptions = Application.loadedLevelName != "demo_industry";
        this.questionButtonStyle = this.hudSkin.GetStyle("QuestionButton");
        this.tooltipStyle = this.hudSkin.GetStyle("TooltipStyle");
        this.aquirisLogo = this.hudSkin.GetStyle("AquirisLogo");
        this.unityLogo = this.hudSkin.GetStyle("UnityLogo");
        this.questionRect = new Rect(0, 0, 11, 11);
        this.alpha = 1f;
        MainMenuScreen.goingToGame = false;
        this.resumeGame = false;
        this.state = MainMenuState.IDLE;
        this.descriptionStyle = this.hudSkin.GetStyle("Description");
        this.titleStyle = this.hudSkin.GetStyle("Titles");
        this.customBox = this.hudSkin.GetStyle("CustomBox");
        this.panelLeft = this.hudSkin.GetStyle("PanelLeft");
        this.panelRight = this.hudSkin.GetStyle("PanelRight");
        this.aboutStyle = this.hudSkin.GetStyle("About");
        this.menuBackgroundRect = new Rect(0, 0, this.menuBackground.width, this.menuBackground.height);
        this.windowBackgroundRect = new Rect(0, 0, this.windowBackground.width, this.windowBackground.height);
        this.panelLeftRect = new Rect(0, 0, 235, 240);
        this.panelRightRect = new Rect(0, 0, 235, 240);
        this.playGameRect = new Rect(0, 0, this.playGame.width * 0.75f, this.playGame.height * 0.75f);
        this.optionsRect = new Rect(0, 0, this.options.width * 0.75f, this.options.height * 0.75f);
        this.graphicsRect = new Rect(0, 0, this.graphics.width * 0.75f, this.graphics.height * 0.75f);
        this.aboutRect = new Rect(0, 0, this.about.width * 0.75f, this.about.height * 0.75f);
        this.resumeRect = new Rect(0, 0, this.resume.width * 0.75f, this.resume.height * 0.75f);
        this.damageRect = new Rect(0, 0, this.receiveDamageOn.width * 0.7f, this.receiveDamageOn.height * 0.7f);
    }

    public virtual void Update()
    {
        if (MainMenuScreen.goingToGame)
        {
            this.alpha = this.alpha + Time.deltaTime;
            if (this.alpha >= 1f)
            {
                if (!this.loadingIndustry)
                {
                    this.loadingIndustry = true;
                    Application.LoadLevelAsync("demo_industry");
                }
            }
        }
        else
        {
            if (this.alpha > 0)
            {
                this.alpha = this.alpha - (Time.deltaTime * 0.5f);
            }
        }
        if ((Time.timeScale == 0f) || GameManager.pause)
        {
            this.lastMouseTime = this.lastMouseTime - 0.01f;
        }
        if (MainMenuScreen.showProgress)
        {
            this.angle = this.angle - (Time.deltaTime * 360);
            if (this.angle < -360f)
            {
                this.angle = this.angle + 360f;
            }
        }
    }

    public virtual void DrawGUI(Event @event)
    {
        this.evt = @event;
        this.screenSize = new Vector2(Screen.width, Screen.height);
        this.mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        if (this.visible)
        {
            if (this.state != MainMenuState.IDLE)
            {
                this.windowBackgroundRect.x = this.menuBackgroundRect.x + this.menuBackgroundRect.width;
                this.windowBackgroundRect.y = (this.screenSize.y - this.windowBackgroundRect.height) * 0.5f;
                GUI.DrawTexture(this.windowBackgroundRect, this.windowBackground);
                if ((this.state == MainMenuState.GRAPHICS) || (this.state == MainMenuState.ABOUT))
                {
                    this.panelLeftRect.width = 475;
                    this.panelLeftRect.x = this.windowBackgroundRect.x + 15;
                    this.panelLeftRect.y = this.windowBackgroundRect.y + 55;
                    GUI.Box(this.panelLeftRect, "", this.panelLeft);
                }
            }
            if (this.state == MainMenuState.OPTIONS)
            {
                this.DrawGameOptions();
            }
            else
            {
                if (this.state == MainMenuState.GRAPHICS)
                {
                    this.DrawGraphicOptions();
                }
                else
                {
                    if (this.state == MainMenuState.ABOUT)
                    {
                        this.DrawAbout();
                    }
                }
            }
            this.DrawMenu();
        }
        if (MainMenuScreen.showProgress)
        {
            float currentProgress = IndustryLoader.industryProgress;
            currentProgress = currentProgress * 100f;//Application.GetStreamProgressForLevel("demo_industry");
            int aux = (int) currentProgress;
            currentProgress = aux;
            if (currentProgress < 1f)
            {
                GUIUtility.RotateAroundPivot(this.angle, new Vector2(Screen.width - 28, Screen.height - 28));
                GUI.DrawTexture(new Rect(Screen.width - 56, Screen.height - 56, 56, 56), this.loadingBackground, ScaleMode.ScaleToFit, true, 0);
                GUI.matrix = Matrix4x4.identity;
                GUI.Label(new Rect(Screen.width - 52, Screen.height - 36, 50, 20), currentProgress.ToString(), this.textStyle);
            }
        }
        if (this.alpha > 0f)
        {
            Color c = GUI.color;
            Color d = c;
            d.a = this.alpha;
            GUI.color = d;
            GUI.DrawTexture(new Rect(0, 0, this.screenSize.x, this.screenSize.y), this.black);
            if (MainMenuScreen.goingToGame)
            {
                GUI.Label(new Rect(Screen.width - 120, Screen.height - 40, 90, 20), "Loading...", this.textStyle);
            }
            GUI.color = c;
        }
    }

    public virtual void DrawGameOptions()
    {
        this.panelLeftRect.width = 235;
        this.panelLeftRect.x = this.windowBackgroundRect.x + 15;
        this.panelLeftRect.y = this.windowBackgroundRect.y + 55;
        this.panelRightRect.x = (this.panelLeftRect.x + 5) + this.panelLeftRect.width;
        this.panelRightRect.y = this.panelLeftRect.y;
        this.damageRect.x = this.panelLeftRect.x + ((this.panelLeftRect.width - this.damageRect.width) * 0.5f);
        this.damageRect.y = this.panelLeftRect.y + ((this.panelLeftRect.height - this.damageRect.height) * 0.5f);
        Rect dRect = this.damageRect;
        dRect.x = this.panelRightRect.x + ((this.panelRightRect.width - this.damageRect.width) * 0.5f);
        if (((Event.current.type == EventType.MouseUp) && (this.evt.button == 0)) && (Time.time > this.lastMouseTime))
        {
            if (this.damageRect.Contains(this.mousePos))
            {
                if (!GameManager.receiveDamage)
                {
                    this.GetComponent<AudioSource>().volume = this.clickVolume;
                    this.GetComponent<AudioSource>().PlayOneShot(this.clickSound);
                    GameManager.receiveDamage = true;
                    this.lastMouseTime = Time.time;
                }
            }
            else
            {
                if (dRect.Contains(this.mousePos))
                {
                    if (GameManager.receiveDamage)
                    {
                        this.GetComponent<AudioSource>().volume = this.clickVolume;
                        this.GetComponent<AudioSource>().PlayOneShot(this.clickSound);
                        GameManager.receiveDamage = false;
                        this.lastMouseTime = Time.time;
                    }
                }
            }
        }
        if (GameManager.receiveDamage)
        {
            GUI.DrawTexture(this.damageRect, this.receiveDamageOn);
            GUI.DrawTexture(dRect, this.dontReceiveDamageOff);
        }
        else
        {
            GUI.DrawTexture(this.damageRect, this.receiveDamageOff);
            GUI.DrawTexture(dRect, this.dontReceiveDamageOn);
        }
        GUI.Label(new Rect(this.windowBackgroundRect.x + 20, this.windowBackgroundRect.y + 15, 200, 20), "GAME OPTIONS", this.titleStyle);
    }

    private SceneSettings sceneConf;
    public virtual void GetSceneRef()
    {
        int currentScene = 0;
         //var currentScene : int = Application.loadedLevel;
        if (Application.loadedLevelName == "demo_start_cutscene")
        {
            currentScene = 0;
        }
        else
        {
            if (Application.loadedLevelName == "demo_forest")
            {
                currentScene = 1;
            }
            else
            {
                if (Application.loadedLevelName == "demo_industry")
                {
                    currentScene = 2;
                }
            }
        }
        if (!(GameQualitySettings.scenes == null))
        {
            if ((currentScene >= 0) && (currentScene < GameQualitySettings.scenes.Length))
            {
                this.sceneConf = GameQualitySettings.scenes[currentScene];
            }
            else
            {
                currentScene = -1;
            }
        }
        else
        {
            currentScene = -1;
        }
    }

    private void DrawSliderOverlay(Rect rect, float val)
    {
        rect.width = Mathf.Clamp(val * 199f, 0f, 199f);
        GUI.DrawTexture(rect, this.greenBar);
    }

    private float SettingsSlider(string name, int nameLen, string tooltip, float v, float vmin, float vmax, string dispname, float dispmul, float dispadd)
    {
        GUILayout.BeginHorizontal(new GUILayoutOption[] {});
        GUILayout.Space(this.margin);
        GUILayout.BeginVertical(new GUILayoutOption[] {});
        GUILayout.Label(name, new GUILayoutOption[] {});
        this.questionRect.x = this.margin + nameLen;
        this.questionRect.y = this.questionRect.y + 39;
        GUI.Button(this.questionRect, new GUIContent(string.Empty, tooltip), this.questionButtonStyle);
        v = GUILayout.HorizontalSlider(v, vmin, vmax, new GUILayoutOption[] {GUILayout.Width(210)});
        this.greenRect.y = this.greenRect.y + 39;
        this.DrawSliderOverlay(this.greenRect, Mathf.InverseLerp(vmin, vmax, v));
        float disp = (v * dispmul) + dispadd;
        GUI.Label(new Rect(this.greenRect.x + 220, this.greenRect.y - 7, 200, 20), dispname + disp.ToString("0.00"));
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        return v;
    }

    private int SettingsIntSlider(string name, int nameLen, string tooltip, int v, int vmin, int vmax, string[] dispnames)
    {
        GUILayout.BeginHorizontal(new GUILayoutOption[] {});
        GUILayout.Space(this.margin);
        GUILayout.BeginVertical(new GUILayoutOption[] {});
        GUILayout.Label(name, new GUILayoutOption[] {});
        this.questionRect.x = this.margin + nameLen;
        this.questionRect.y = this.questionRect.y + 39;
        GUI.Button(this.questionRect, new GUIContent(string.Empty, tooltip), this.questionButtonStyle);
        v = (int) GUILayout.HorizontalSlider(v, vmin, vmax, new GUILayoutOption[] {GUILayout.Width(210)});
        this.greenRect.y = this.greenRect.y + 39;
        this.DrawSliderOverlay(this.greenRect, Mathf.InverseLerp(vmin, vmax, v));
        GUI.Label(new Rect(this.greenRect.x + 220, this.greenRect.y - 7, 200, 20), dispnames == null ? v.ToString() : dispnames[v]);
        if (Mathf.Abs(vmin - vmax) < 8)
        {
            this.DrawMarker(this.greenRect.y + 5, Mathf.Abs(vmin - vmax));
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        return v;
    }

    private void SettingsSpace(int pix)
    {
        GUILayout.Space(pix);
        this.questionRect.y = this.questionRect.y + pix;
        this.greenRect.y = this.greenRect.y + pix;
    }

    private bool SettingsToggle(bool v, string name, int nameLen, string tooltip)
    {
        GUILayout.BeginVertical(new GUILayoutOption[] {});
        GUILayout.Space(7);
        v = GUILayout.Toggle(v, v ? this.checkOn : this.checkOff, new GUILayoutOption[] {GUILayout.Width(14), GUILayout.Height(14)});
        GUILayout.EndVertical();
        GUILayout.Space(5);
        GUILayout.Label(name, new GUILayoutOption[] {});
        this.questionRect.x = this.margin + nameLen;
        GUI.Button(this.questionRect, new GUIContent(string.Empty, tooltip), this.questionButtonStyle);
        return v;
    }

    private void BeginToggleRow()
    {
        GUILayout.BeginHorizontal(new GUILayoutOption[] {});
        GUILayout.Space(this.margin);
        GUILayout.BeginVertical(new GUILayoutOption[] {});
        GUILayout.BeginHorizontal(new GUILayoutOption[] {GUILayout.Width(400)});
        this.questionRect.y = this.questionRect.y + 21;
    }

    private void EndToggleRow(int pix)
    {
        GUILayout.Space(pix);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    public virtual void DrawGraphicOptions()
    {
        this.GetSceneRef();
        int currentQualityLevel = (int) QualitySettings.currentLevel;
        Color originalColor = GUI.color;
        if (this.sceneConf == null)
        {
            return;
        }
        GUI.Label(new Rect(this.windowBackgroundRect.x + 20, this.windowBackgroundRect.y + 15, 200, 20), "GRAPHICS OPTIONS", this.titleStyle);
        Rect graphicRect = new Rect(this.panelLeftRect.x + 7, this.panelLeftRect.y + 30, this.panelLeftRect.width - 9, this.panelLeftRect.height - 35);
        GUISkin cSkin = GUI.skin;
        GUI.skin = this.hudSkin;
        this.greenRect = new Rect(this.margin, 0, 210, 5);
        GUILayout.BeginArea(graphicRect);
        this.graphicsScroll = GUILayout.BeginScrollView(this.graphicsScroll, new GUILayoutOption[] {GUILayout.Width(graphicRect.width)});
        Rect boxRect = new Rect(17, 0, 430, 0);
        // overall level
        boxRect.height = 18 + 39;
        GUI.Box(boxRect, "", this.customBox);
        // post-fx
        boxRect.y = boxRect.y + (10 + boxRect.height);
        boxRect.height = 93;
        GUI.Box(boxRect, "", this.customBox);
        // distances
        boxRect.y = boxRect.y + (10 + boxRect.height);
        boxRect.height = 18 + 39;
        GUI.Box(boxRect, "", this.customBox);
        // shadow distance
        boxRect.y = boxRect.y + (10 + boxRect.height);
        boxRect.height = 18 + 39;
        GUI.Box(boxRect, "", this.customBox);
        // texture limit
        boxRect.y = boxRect.y + (10 + boxRect.height);
        boxRect.height = 18 + 39;
        GUI.Box(boxRect, "", this.customBox);
        // terrain
        boxRect.y = boxRect.y + (10 + boxRect.height);
        boxRect.height = 18 + (39 * 7);
        GUI.Box(boxRect, "", this.customBox);
        GUILayout.BeginVertical(new GUILayoutOption[] {});
        this.questionRect.y = -31;
        this.greenRect.y = -9;
        GameQualitySettings.overallQuality = this.SettingsIntSlider("Overall Quality Level", 125, "Overall quality level of the game.", GameQualitySettings.overallQuality, 0, 5, new string[] {"QUALITY: FASTEST", "QUALITY: FAST", "QUALITY: SIMPLE", "QUALITY: GOOD", "QUALITY: BEAUTIFUL", "QUALITY: FANTASTIC"});
        GUILayout.Space(29);
        this.questionRect.y = this.questionRect.y + 47;
        this.BeginToggleRow();
        GameQualitySettings.anisotropicFiltering = this.SettingsToggle(GameQualitySettings.anisotropicFiltering, "Anisotropic Filtering", 153, "Anisotropic filtering for the textures.");
        GUILayout.FlexibleSpace();
        GameQualitySettings.ambientParticles = this.SettingsToggle(GameQualitySettings.ambientParticles, "Ambient Particles", 355, "Smoke & dust particles.");
        this.EndToggleRow(50);
        this.BeginToggleRow();
        GameQualitySettings.colorCorrection = this.SettingsToggle(GameQualitySettings.colorCorrection, "Color Correction", 128, "Color correction image effect.");
        GUILayout.FlexibleSpace();
        GameQualitySettings.bloomAndFlares = this.SettingsToggle(GameQualitySettings.bloomAndFlares, "Bloom & Flares", 336, "Bloom & Lens Flares image effect.");
        this.EndToggleRow(71);
        this.BeginToggleRow();
        GameQualitySettings.sunShafts = this.SettingsToggle(GameQualitySettings.sunShafts, "Sun Shafts", 100, "Sun Shafts image effect.");
        GUILayout.FlexibleSpace();
        GameQualitySettings.depthOfFieldAvailable = this.SettingsToggle(GameQualitySettings.depthOfFieldAvailable, "Depth Of Field", 336, "Depth Of Field image effect while aiming.");
        this.EndToggleRow(73);
        this.BeginToggleRow();
        bool ssaoEnable = this.SettingsToggle(GameQualitySettings.ssao, "SSAO", 60, "Screen Space Ambient Ccclusion image effect.");
        if (GameQualitySettings.overallQuality > 3)
        {
            GameQualitySettings.ssao = ssaoEnable;
        }
        GUILayout.FlexibleSpace();
        this.EndToggleRow(0);
        this.greenRect.y = this.greenRect.y + 113;
        this.questionRect.y = this.questionRect.y - 18;
        this.SettingsSpace(25);
        GameQualitySettings.dynamicObjectsFarClip = this.SettingsSlider("Dynamic Objects Far Distance", 180, "Drawing distance of dynamic objects.", GameQualitySettings.dynamicObjectsFarClip, 0f, 1f, "DYNAMIC: ", GameQualitySettings._dynamicLayersRange.y - GameQualitySettings._dynamicLayersRange.x, GameQualitySettings._dynamicLayersRange.x);
        this.SettingsSpace(27);
        GameQualitySettings.shadowDistance = this.SettingsSlider("Shadow Distance", 108, "Realtime shadows drawing distance.", GameQualitySettings.shadowDistance, 0f, 30f, "", 1f, 0f);
        this.SettingsSpace(28);
        GameQualitySettings.masterTextureLimit = this.SettingsIntSlider("Texture Quality", 100, "Overall texture detail.", GameQualitySettings.masterTextureLimit, 3, 0, new string[] {"FULL RESOLUTION", "HALF RESOLUTION", "QUARTER RESOLUTION", "1/8 RESOLUTION"});
        this.SettingsSpace(27);
        this.sceneConf.detailObjectDensity = this.SettingsSlider("Terrain Grass Density", 136, "Grass density.", this.sceneConf.detailObjectDensity, 0f, 1f, "", 1f, 0f);
        this.sceneConf.detailObjectDistance = this.SettingsSlider("Terrain Grass Distance", 141, "Grass drawing distance.", this.sceneConf.detailObjectDistance, 0f, 50f, "", 1f, 0f);
        this.sceneConf.nearTerrainPixelError = this.SettingsSlider("Terrain Pixel Error", 146, "Set terrain pixel error.", this.sceneConf.nearTerrainPixelError, 50f, 5f, "", 1f, 0f);
        this.sceneConf.treeDistance = this.SettingsSlider("Terrain Tree Distance", 137, "Tree drawing distance.", this.sceneConf.treeDistance, 200f, 400f, "", 1f, 0f);
        this.sceneConf.heightmapMaximumLOD = this.SettingsIntSlider("Terrain Level of Detail", 139, "Overall terrain Level of Detail.", this.sceneConf.heightmapMaximumLOD, 2, 0, new string[] {"FULL RESOLUTION", "QUARTER RESOLUTION", "1/16 RESOLUTION"});
        this.sceneConf.terrainTreesBillboardStart = this.SettingsSlider("Terrain Billboard Start", 140, "Distance from the camera where trees will be rendered as billboards.", this.sceneConf.terrainTreesBillboardStart, 10f, 70f, "", 1f, 0f);
        this.sceneConf.maxMeshTrees = this.SettingsIntSlider("Max Mesh Trees", 100, "Set the maximum number of trees rendered at full LOD.", this.sceneConf.maxMeshTrees, 5, 60, null);
        GUILayout.Space(20);
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
        GUILayout.EndArea();
        if (GUI.tooltip != "")
        {
            GUI.Label(new Rect(this.mousePos.x + 15, this.mousePos.y - 60, 150, 70), GUI.tooltip, this.tooltipStyle);
        }
        GUI.skin = cSkin;
    }

    public virtual void DrawMarker(float y, int steps)
    {
        float aux = 0.0f;
        Rect markerRect = new Rect(this.margin, y + 2, 1, 5);
        float s = steps;
        int i = 0;
        while (i <= steps)
        {
            aux = i;
            aux = aux / s;
            markerRect.x = (this.margin + 5) + (aux * 196);
            GUI.DrawTexture(markerRect, this.whiteMarker);
            i++;
        }
    }

    public virtual void DrawAbout()
    {
        GUI.Label(new Rect(this.windowBackgroundRect.x + 20, this.windowBackgroundRect.y + 15, 200, 20), "ABOUT", this.titleStyle);
        Rect abRect = new Rect(this.panelLeftRect.x + 7, this.panelLeftRect.y + 30, this.panelLeftRect.width - 12, this.panelLeftRect.height - 40);
        GUISkin cSkin = GUI.skin;
        GUI.skin = this.hudSkin;
        GUILayout.BeginArea(abRect);
        this.aboutScroll = GUILayout.BeginScrollView(this.aboutScroll, new GUILayoutOption[] {GUILayout.Width(abRect.width)});
        GUILayout.BeginHorizontal(new GUILayoutOption[] {});
        GUILayout.Space(17);
        GUILayout.BeginVertical(new GUILayoutOption[] {});
        GUILayout.Label("Developed by Aquiris Game Experience and Unity Technologies ApS.", this.aboutStyle, new GUILayoutOption[] {GUILayout.Width(423)});
        GUILayout.Space(5);
        string names = null;
        names = "Alessandro Peixoto Lima, ";
        names = names + "Amilton Diesel, ";
        names = names + "Andre Schaan, ";
        names = names + "Aras Pranckevicius, ";
        names = names + "Bret Church, ";
        names = names + "Ethan Vosburgh, ";
        names = names + "Gustavo Allebrandt, ";
        names = names + "Israel Mendes, ";
        names = names + "Henrique Geremia Nievinski, ";
        names = names + "Jakub Cupisz, ";
        names = names + "Joe Robins, ";
        names = names + "Marcelo Ferranti, ";
        names = names + "Mauricio Longoni, ";
        names = names + "Ole Ciliox, ";
        names = names + "Rafael Rodrigues, ";
        names = names + "Raphael Lopes Baldi, ";
        names = names + "Robert Cupisz, ";
        names = names + "Rodrigo Peixoto Lima, ";
        names = names + "Rune Skovbo Johansen, ";
        names = names + "Wagner Monticelli.";
        GUILayout.Label(names, new GUILayoutOption[] {GUILayout.Width(400)});
        GUILayout.Space(20);
        GUILayout.Label("Special thanks to:", this.aboutStyle, new GUILayoutOption[] {GUILayout.Width(423)});
        GUILayout.Space(5);
        names = "Cristiano Bartel, ";
        names = names + "Daniel Merkel, ";
        names = names + "Felipe Lahti, ";
        names = names + "Kely Cunha, ";
        names = names + "Otto Lopes, ";
        names = names + "Rory Jennings.";
        GUILayout.Label(names, new GUILayoutOption[] {GUILayout.Width(400)});
        GUILayout.Space(70);
        GUI.DrawTexture(new Rect(170, 180, 339 * 0.75f, 94 * 0.75f), this.aquirisLogo.normal.background);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();
        GUILayout.EndArea();
        GUI.skin = cSkin;
    }

    public virtual void DrawMenu()
    {
        this.menuBackgroundRect.x = 0;
        this.menuBackgroundRect.y = ((this.screenSize.y - this.menuBackgroundRect.height) * 0.5f) - 50;
        this.playGameRect.x = this.menuBackgroundRect.x + 93;
        if (this.hideOptions)
        {
            this.playGameRect.y = this.menuBackgroundRect.y + 80;
        }
        else
        {
            this.playGameRect.y = this.menuBackgroundRect.y + 62;
        }
        this.resumeRect.x = this.playGameRect.x;
        this.resumeRect.y = this.playGameRect.y;
        this.optionsRect.x = this.playGameRect.x;
        this.optionsRect.y = (this.playGameRect.y + this.playGameRect.height) + 15;
        this.graphicsRect.x = this.playGameRect.x;
        if (this.hideOptions)
        {
            this.graphicsRect.y = (this.playGameRect.y + this.playGameRect.height) + 15;
        }
        else
        {
            this.graphicsRect.y = (this.optionsRect.y + this.optionsRect.height) + 15;
        }
        this.aboutRect.x = this.playGameRect.x;
        this.aboutRect.y = (this.graphicsRect.y + this.graphicsRect.height) + 15;
        GUI.DrawTexture(this.menuBackgroundRect, this.menuBackground);
        bool overButton = false;
        //		if(Application.loadedLevelName == "main_menu")
        //		{
        //			if(playGameRect.Contains(mousePos))
        //			{
        //				overButton = true;
        //						
        //				if(!over)
        //				{
        //					over = true;
        //					audio.volume = overVolume;
        //					audio.PlayOneShot(overSound);
        //				}
        //				
        //				GUI.DrawTexture(playGameRect, playGameOver);
        //				
        //				if(alpha <= 0.0 && !goingToGame)
        //				{
        //					if(evt.type == EventType.MouseUp && evt.button == 0 && Time.time > lastMouseTime)
        //					{
        //						audio.volume = clickVolume;
        //						audio.PlayOneShot(clickSound);
        //						
        //						goingToGame = true;
        //						
        //						lastMouseTime = Time.time;
        //					}
        //				}
        //			}
        //			else
        //			{
        //				GUI.DrawTexture(playGameRect, playGame);
        //			}
        //		}
        //		else
        //		{
        if (this.resumeRect.Contains(this.mousePos))
        {
            overButton = true;
            if (!this.over)
            {
                this.over = true;
                this.GetComponent<AudioSource>().volume = this.overVolume;
                this.GetComponent<AudioSource>().PlayOneShot(this.overSound);
            }
            GUI.DrawTexture(this.resumeRect, this.resumeOver);
            if ((this.alpha <= 0f) && GameManager.pause)
            {
                if (((this.evt.type == EventType.MouseUp) && (this.evt.button == 0)) && (Time.time > this.lastMouseTime))
                {
                    this.GetComponent<AudioSource>().volume = this.clickVolume;
                    this.GetComponent<AudioSource>().PlayOneShot(this.clickSound);
                    GameManager.pause = false;
                    Time.timeScale = 1f;
                    //Time.timeScale = 1.0;
                    this.visible = false;
                    this.lastMouseTime = Time.time;
                }
            }
        }
        else
        {
            GUI.DrawTexture(this.resumeRect, this.resume);
        }
        //}
        if (!this.hideOptions)
        {
            if (this.optionsRect.Contains(this.mousePos))
            {
                overButton = true;
                if (!this.over)
                {
                    this.over = true;
                    this.GetComponent<AudioSource>().volume = this.overVolume;
                    this.GetComponent<AudioSource>().PlayOneShot(this.overSound);
                }
                GUI.DrawTexture(this.optionsRect, this.optionsOver);
                if ((this.alpha <= 0f) && !MainMenuScreen.goingToGame)
                {
                    if (((this.evt.type == EventType.MouseUp) && (this.evt.button == 0)) && (Time.time > this.lastMouseTime))
                    {
                        this.GetComponent<AudioSource>().volume = this.clickVolume;
                        this.GetComponent<AudioSource>().PlayOneShot(this.clickSound);
                        if (this.state != MainMenuState.OPTIONS)
                        {
                            this.state = MainMenuState.OPTIONS;
                        }
                        else
                        {
                            this.state = MainMenuState.IDLE;
                        }
                        this.lastMouseTime = Time.time;
                    }
                }
            }
            else
            {
                GUI.DrawTexture(this.optionsRect, this.options);
            }
        }
        if (this.graphicsRect.Contains(this.mousePos))
        {
            overButton = true;
            if (!this.over)
            {
                this.over = true;
                this.GetComponent<AudioSource>().volume = this.overVolume;
                this.GetComponent<AudioSource>().PlayOneShot(this.overSound);
            }
            GUI.DrawTexture(this.graphicsRect, this.graphicsOver);
            if ((this.alpha <= 0f) && !MainMenuScreen.goingToGame)
            {
                if (((this.evt.type == EventType.MouseUp) && (this.evt.button == 0)) && (Time.time > this.lastMouseTime))
                {
                    this.GetComponent<AudioSource>().volume = this.clickVolume;
                    this.GetComponent<AudioSource>().PlayOneShot(this.clickSound);
                    if (this.state != MainMenuState.GRAPHICS)
                    {
                        this.state = MainMenuState.GRAPHICS;
                    }
                    else
                    {
                        this.state = MainMenuState.IDLE;
                    }
                    this.lastMouseTime = Time.time;
                }
            }
        }
        else
        {
            GUI.DrawTexture(this.graphicsRect, this.graphics);
        }
        if (this.aboutRect.Contains(this.mousePos))
        {
            overButton = true;
            if (!this.over)
            {
                this.over = true;
                this.GetComponent<AudioSource>().volume = this.overVolume;
                this.GetComponent<AudioSource>().PlayOneShot(this.overSound);
            }
            GUI.DrawTexture(this.aboutRect, this.aboutOver);
            if ((this.alpha <= 0f) && !MainMenuScreen.goingToGame)
            {
                if (((this.evt.type == EventType.MouseUp) && (this.evt.button == 0)) && (Time.time > this.lastMouseTime))
                {
                    this.GetComponent<AudioSource>().volume = this.clickVolume;
                    this.GetComponent<AudioSource>().PlayOneShot(this.clickSound);
                    if (this.state != MainMenuState.ABOUT)
                    {
                        this.state = MainMenuState.ABOUT;
                    }
                    else
                    {
                        this.state = MainMenuState.IDLE;
                    }
                    this.lastMouseTime = Time.time;
                }
            }
        }
        else
        {
            GUI.DrawTexture(this.aboutRect, this.about);
        }
        if (!overButton)
        {
            this.over = false;
        }
    }

    public MainMenuScreen()
    {
        this.margin = 30;
        this.overVolume = 0.4f;
        this.clickVolume = 0.7f;
    }

}