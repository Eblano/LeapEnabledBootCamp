using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SceneSettings : object
{
     //SCENE SETTINGS
    public bool sceneInitialized;
    public float detailObjectDistance;
    public float detailObjectDensity;
    public float treeDistance;
    public float nearTerrainPixelError;
    public float terrainTreesBillboardStart;
    public int maxMeshTrees;
    public int heightmapMaximumLOD;
}
[System.Serializable]
public class AmbientParticleSettings : object
{
    public float minSize;
    public float maxSize;
    public float minEmission;
    public float maxEmission;
}
[System.Serializable]
public class GameQualitySettings : MonoBehaviour
{
     //HAVE WE TAKE CURRENT GAME SETTINGS
    private static bool initializedGameSettings;
    private static float _dynamicObjectsFarClip;
    public static SceneSettings[] scenes;
    //GAME SETTINGS
    public static int overallQuality;
    public static float shadowDistance;
    public static int masterTextureLimit;
    public static bool anisotropicFiltering;
    public static float particleQualityMultiplier;
    private static float _particleQualityMultiplier;
    //FULLSCREEN EFFECTS
    public static bool colorCorrection;
    private static bool _colorCorrection;
    public static bool bloomAndFlares;
    private static bool _bloomAndFlares;
    public static bool sunShafts;
    private static bool _sunShafts;
    public static bool depthOfField;
    private static bool _depthOfField;
    public static bool depthOfFieldAvailable;
    private static bool _depthOfFieldAvailable;
    public static bool ssao;
    private static bool _ssao;
    public static bool clouds;
    private static bool _clouds;
    public static bool underwater;
    private static bool _underwater;
    //END FULLSCREEN EFFECTS
    // WATER
    private static int _water;
    public static int water;
    public static bool ambientParticles;
    private static bool _ambientParticles;
    //RESET PER SCENE (MULTIPLY BASE DISTANCE EVERY TIME SCENE START)
    public static float dynamicObjectsFarClip;
    public static Vector2 _dynamicLayersRange;
    public static Vector2 _staticLayersRange;
    public static RenderingPath currentRenderingPath;
    public static DepthTextureMode currentDepthTextureMode;
    //LOCAL, PER SCENE, PROPERTIES
    public Camera[] cameras;
    public int[] dynamicLayers;
    public Vector2 dynamicLayersRange;
    public Vector2 staticLayersRange;
    public Terrain nearTerrain;
    public GameObject[] ambientParticleObjects;
    private List<object> _ambientParticleObjectSettings = new List<object>();
    public Light[] lights;
    public bool mainMenu;
    public virtual void Start()
    {
        GameQualitySettings._dynamicLayersRange = this.dynamicLayersRange;
        GameQualitySettings._staticLayersRange = this.staticLayersRange;
        GameQualitySettings._ambientParticles = GameQualitySettings.ambientParticles;
        this._ambientParticleObjectSettings = new List<object>();
        foreach (GameObject go in this.ambientParticleObjects)
        {
            AmbientParticleSettings setting = new AmbientParticleSettings();
            if (go)
            {
                ParticleSystem particleSystem = go.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    ParticleSystem.MainModule mainModule = particleSystem.main;
                    setting.minSize = mainModule.startSizeMultiplier;
                    setting.maxSize = mainModule.startSizeMultiplier;
                    setting.minEmission = mainModule.startLifetimeMultiplier;
                    setting.maxEmission = mainModule.startLifetimeMultiplier;
                }
            }
            this._ambientParticleObjectSettings.Add(setting);
        }
        this.InitializeGameSettings();
        this.InitializeSceneSettings();
        this.InitializeQualitySettings((int) QualitySettings.currentLevel);
        this.InitializeCameraSettings();
        this.AutoChooseQualityLevel();
    }

    public virtual void AutoChooseQualityLevel()
    {
        int shaderLevel = SystemInfo.graphicsShaderLevel;
        int fillrate = SystemInfo.graphicsPixelFillrate;
        int vram = SystemInfo.graphicsMemorySize;
        int cpus = SystemInfo.processorCount;
        if (fillrate < 0)
        {
            if (shaderLevel < 10)
            {
                fillrate = 1000;
            }
            else
            {
                if (shaderLevel < 20)
                {
                    fillrate = 1300;
                }
                else
                {
                    if (shaderLevel < 30)
                    {
                        fillrate = 2000;
                    }
                    else
                    {
                        fillrate = 3000;
                    }
                }
            }
            if (cpus >= 6)
            {
                fillrate = fillrate * 3;
            }
            else
            {
                if (cpus >= 3)
                {
                    fillrate = fillrate * 2;
                }
            }
            if (vram >= 512)
            {
                fillrate = fillrate * 2;
            }
            else
            {
                if (vram <= 128)
                {
                    fillrate = fillrate / 2;
                }
            }
        }
        int resx = Screen.width;
        int resy = Screen.height;
        float fillneed = ((resx * resy) + (400 * 300)) * (30f / 1000000f);
        float[] levelmult = new float[] {5f, 30f, 80f, 130f, 200f, 320f};
        int level = 0;
        while ((((QualityLevel) level) < QualityLevel.Fantastic) && (fillrate > (fillneed * levelmult[level + 1])))
        {
            ++level;
        }
        //print (String.Format("{0}x{1} need {2} has {3} = {4} level", resx, resy, fillneed, fillrate, level));
        GameQualitySettings.overallQuality = level;
        this.UpdateAllSettings();
    }

    public virtual void InitializeQualitySettings(int qualityLevel)
    {
        this.ApplyCustomQualityLevel(qualityLevel);
        GameQualitySettings._ambientParticles = GameQualitySettings.ambientParticles;
        if (!(this.ambientParticleObjects == null))
        {
            int k = 0;
            while (k < this.ambientParticleObjects.Length)
            {
                if (this.ambientParticleObjects[k] == null)
                {
                    goto Label_for_23;
                }
                if ((this.ambientParticleObjects[k].name == "dust") || (this.ambientParticleObjects[k].name == "leaves"))
                {
                    goto Label_for_23;
                }
                this.ambientParticleObjects[k].SetActiveRecursively(GameQualitySettings.ambientParticles);
                Label_for_23:
                k++;
            }
        }
        if (GameQualitySettings._particleQualityMultiplier != GameQualitySettings.particleQualityMultiplier)
        {
            this.UpdateAmbientParticleQuality();
        }
    }

    public virtual void UpdateAmbientParticleQuality()
    {
        if (GameQualitySettings._particleQualityMultiplier != GameQualitySettings.particleQualityMultiplier)
        {
            GameQualitySettings._particleQualityMultiplier = GameQualitySettings.particleQualityMultiplier;
            int k = 0;
            while (k < this.ambientParticleObjects.Length)
            {
                AmbientParticleSettings setting = this._ambientParticleObjectSettings[k] as AmbientParticleSettings;
                if (this.ambientParticleObjects[k] == null)
                {
                    goto Label_for_24;
                }
                if (!this.ambientParticleObjects[k].active)
                {
                    goto Label_for_24;
                }
                ParticleSystem particleSystem = this.ambientParticleObjects[k].GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    ParticleSystem.MainModule mainModule = particleSystem.main;
                    mainModule.startSizeMultiplier = setting.minSize * GameQualitySettings._particleQualityMultiplier;
                    mainModule.startSizeMultiplier = setting.maxSize * GameQualitySettings._particleQualityMultiplier;
                    mainModule.startLifetimeMultiplier = setting.minEmission * GameQualitySettings._particleQualityMultiplier;
                    mainModule.startLifetimeMultiplier = setting.maxEmission * GameQualitySettings._particleQualityMultiplier;
                }
                Label_for_24:
                k++;
            }
        }
    }

    public virtual void InitializeGameSettings()
    {
        if (GameQualitySettings.initializedGameSettings)
        {
            return;
        }
        //If we are running the game first time, we need to take the current game quality settings
        GameQualitySettings.initializedGameSettings = true;
        GameQualitySettings.overallQuality = (int) QualitySettings.currentLevel;
        GameQualitySettings.shadowDistance = QualitySettings.shadowDistance;
        GameQualitySettings.masterTextureLimit = QualitySettings.masterTextureLimit;
        GameQualitySettings.anisotropicFiltering = QualitySettings.anisotropicFiltering == AnisotropicFiltering.ForceEnable;
    }

    public virtual void InitializeSceneSettings()
    {
        int currentScene = 0;
        if (GameQualitySettings.scenes == null)
        {
            GameQualitySettings.scenes = new SceneSettings[3];
            int i = 0;
            while (i < 3)
            {
                GameQualitySettings.scenes[i] = new SceneSettings();
                i++;
            }
        }
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
        SceneSettings cur = GameQualitySettings.scenes[currentScene];
        if (!cur.sceneInitialized)
        {
             //If this is the first time we entered this level, we need to take the current settings
             //into account...
             //cur.fogDensity = RenderSettings.fogDensity;
            if (this.nearTerrain != null)
            {
                cur.detailObjectDistance = Mathf.Clamp(this.nearTerrain.detailObjectDistance, 0f, 50f);
                cur.detailObjectDensity = Mathf.Clamp(this.nearTerrain.detailObjectDensity, 0f, 1f);
                cur.treeDistance = Mathf.Clamp(this.nearTerrain.treeDistance, 200f, 400f);
                cur.nearTerrainPixelError = Mathf.Clamp(this.nearTerrain.heightmapPixelError, 5f, 50f);
                cur.terrainTreesBillboardStart = Mathf.Clamp(this.nearTerrain.treeBillboardDistance, 10f, 70f);
                cur.maxMeshTrees = Mathf.Clamp(this.nearTerrain.treeMaximumFullLODCount, 5, 60);
                cur.heightmapMaximumLOD = this.nearTerrain.heightmapMaximumLOD;
            }
        }
        else
        {
             //If we have already entered the level, we overwrite current scene settings as 
             //the user may have changed some settings...
             //RenderSettings.fogDensity = cur.fogDensity;
            if (this.nearTerrain != null)
            {
                this.nearTerrain.detailObjectDistance = cur.detailObjectDistance;
                this.nearTerrain.detailObjectDensity = cur.detailObjectDensity;
                this.nearTerrain.treeDistance = cur.treeDistance;
                this.nearTerrain.heightmapPixelError = cur.nearTerrainPixelError;
                this.nearTerrain.treeBillboardDistance = cur.terrainTreesBillboardStart;
                this.nearTerrain.treeMaximumFullLODCount = cur.maxMeshTrees;
                this.nearTerrain.heightmapMaximumLOD = cur.heightmapMaximumLOD;
            }
        }
    }

    public virtual void InitializeCameraSettings()
    {
        GameQualitySettings._dynamicObjectsFarClip = GameQualitySettings.dynamicObjectsFarClip;
        GameQualitySettings._colorCorrection = GameQualitySettings.colorCorrection;
        GameQualitySettings._bloomAndFlares = GameQualitySettings.bloomAndFlares;
        GameQualitySettings._sunShafts = GameQualitySettings.sunShafts;
        GameQualitySettings._depthOfField = GameQualitySettings.depthOfField;
        GameQualitySettings._ssao = GameQualitySettings.ssao;
        GameQualitySettings._clouds = GameQualitySettings.clouds;
        GameQualitySettings._underwater = GameQualitySettings.underwater;
        GameQualitySettings._depthOfFieldAvailable = GameQualitySettings.depthOfFieldAvailable;
        GameQualitySettings._water = GameQualitySettings.water;
        if (GameQualitySettings._particleQualityMultiplier != GameQualitySettings.particleQualityMultiplier)
        {
            this.UpdateAmbientParticleQuality();
        }
        if (!(this.cameras == null))
        {
            float[] distances = new float[32];
            if (!(this.dynamicLayers == null))
            {
                float dynamicDistance = (GameQualitySettings.dynamicObjectsFarClip * (this.dynamicLayersRange.y - this.dynamicLayersRange.x)) + this.dynamicLayersRange.x;
                int d = 0;
                while (d < this.dynamicLayers.Length)
                {
                    if ((this.dynamicLayers[d] >= 0) && (this.dynamicLayers[d] < 32))
                    {
                        distances[this.dynamicLayers[d]] = dynamicDistance;
                    }
                    d++;
                }
            }
            ColorCorrectionCurves cCorrection = null;
            BloomAndFlares bloomFlares = null;
            SunShafts shafts = null;
            DepthOfField depth = null;
            SSAOEffect screenSpaceAO = null;
            //var cloud : CloudEffects;
            //var water : ???
            if (this.cameras.Length > 0)
            {
                int c = 0;
                while (c < this.cameras.Length)
                {
                    if (this.cameras[c] == null)
                    {
                        goto Label_for_27;
                    }
                    this.cameras[c].layerCullDistances = distances;
                    this.cameras[c].renderingPath = GameQualitySettings.currentRenderingPath;
                    this.cameras[c].depthTextureMode = GameQualitySettings.currentDepthTextureMode;
                    cCorrection = (ColorCorrectionCurves) this.cameras[c].GetComponent("ColorCorrectionCurves");
                    if (cCorrection != null)
                    {
                        cCorrection.enabled = GameQualitySettings.colorCorrection;
                    }
                    bloomFlares = (BloomAndFlares) this.cameras[c].GetComponent("BloomAndFlares");
                    if (bloomFlares != null)
                    {
                        bloomFlares.enabled = GameQualitySettings.bloomAndFlares;
                    }
                    shafts = (SunShafts) this.cameras[c].GetComponent("SunShafts");
                    if (shafts != null)
                    {
                        shafts.enabled = GameQualitySettings.sunShafts;
                    }
                    if (shafts && (GameQualitySettings.currentDepthTextureMode == DepthTextureMode.None))
                    {
                        (shafts as SunShafts).useDepthTexture = false;
                    }
                    else
                    {
                        if (shafts)
                        {
                            (shafts as SunShafts).useDepthTexture = true;
                        }
                    }
                    depth = (DepthOfField) this.cameras[c].GetComponent("DepthOfField");
                    if (depth != null)
                    {
                        depth.available = GameQualitySettings.depthOfFieldAvailable;
                    }
                    screenSpaceAO = (SSAOEffect) this.cameras[c].GetComponent("SSAOEffect");
                    if (screenSpaceAO != null)
                    {
                        screenSpaceAO.enabled = GameQualitySettings.ssao;
                    }
                    Label_for_27:
                    c++;
                }
            }
            //cloud = cameras[c].GetComponent("CloudEffects");
            //if(cloud != null) cloud.enabled = clouds;
            if (!(this.lights == null))
            {
                int l = 0;
                while (l < this.lights.Length)
                {
                    if (this.lights[l] == null)
                    {
                        goto Label_for_28;
                    }
                    this.lights[l].shadowStrength = GameQualitySettings.currentRenderingPath == RenderingPath.DeferredLighting ? 0.75f : 0.65f;
                    Label_for_28:
                    l++;
                }
            }
        }
    }

    public virtual void Update()
    {
        if (GameManager.pause || this.mainMenu)
        {
            this.UpdateAllSettings();
        }
    }

    public virtual void UpdateAllSettings()
    {
        this.UpdateGameQuality();
        this.UpdateSceneQuality();
        this.UpdateCameraSettings();
    }

    public virtual void ApplyCustomQualityLevel(int qualityLevel)
    {
        float dObjectDistance = 50f;
        float dObjectDensity = 1f;
        float nPError = 5f;
        float tDistance = 400f;
        int lod = 2;
        float billboards = 70f;
        int mTrees = 60;
        float fPError = 5f;
        switch (qualityLevel)
        {
            case 0:
                GameQualitySettings.ambientParticles = false;
                GameQualitySettings.particleQualityMultiplier = 0f;
                GameQualitySettings.dynamicObjectsFarClip = 0f;
                dObjectDistance = 0f;
                dObjectDensity = 0f;
                nPError = 50f;
                tDistance = 200f;
                lod = 2;
                billboards = 10f;
                mTrees = 5;
                fPError = 50f;
                GameQualitySettings.currentRenderingPath = RenderingPath.VertexLit;
                GameQualitySettings.currentDepthTextureMode = DepthTextureMode.None;
                GameQualitySettings.colorCorrection = false;
                GameQualitySettings.bloomAndFlares = false;
                GameQualitySettings.sunShafts = false;
                GameQualitySettings.depthOfField = false;
                GameQualitySettings.ssao = false;
                GameQualitySettings.clouds = false;
                GameQualitySettings.underwater = false;
                GameQualitySettings.water = 0;
                break;
            case 1:
                GameQualitySettings.ambientParticles = false;
                GameQualitySettings.particleQualityMultiplier = 0.2f;
                GameQualitySettings.dynamicObjectsFarClip = 0.2f;
                dObjectDistance = 10f;
                dObjectDensity = 0.1f;
                nPError = 41f;
                tDistance = 240f;
                lod = 2;
                billboards = 22f;
                mTrees = (int) 16f;
                fPError = 41f;
                GameQualitySettings.currentRenderingPath = RenderingPath.Forward;
                GameQualitySettings.currentDepthTextureMode = DepthTextureMode.None;
                GameQualitySettings.colorCorrection = true;
                GameQualitySettings.bloomAndFlares = false;
                GameQualitySettings.sunShafts = false;
                GameQualitySettings.depthOfField = false;
                GameQualitySettings.ssao = false;
                GameQualitySettings.clouds = false;
                GameQualitySettings.underwater = false;
                GameQualitySettings.water = 0;
                break;
            case 2:
                GameQualitySettings.ambientParticles = false;
                GameQualitySettings.particleQualityMultiplier = 0.3f;
                GameQualitySettings.dynamicObjectsFarClip = 0.4f;
                dObjectDistance = 20f;
                dObjectDensity = 0.3f;
                nPError = 32f;
                tDistance = 280f;
                lod = 1;
                billboards = 34f;
                mTrees = (int) 27f;
                fPError = 32f;
                GameQualitySettings.currentRenderingPath = RenderingPath.Forward;
                GameQualitySettings.currentDepthTextureMode = DepthTextureMode.None;
                GameQualitySettings.colorCorrection = true;
                GameQualitySettings.bloomAndFlares = false;
                GameQualitySettings.sunShafts = false;
                GameQualitySettings.depthOfField = false;
                GameQualitySettings.ssao = false;
                GameQualitySettings.clouds = false;
                GameQualitySettings.underwater = false;
                GameQualitySettings.water = 0;
                break;
            case 3:
                GameQualitySettings.ambientParticles = false;
                GameQualitySettings.particleQualityMultiplier = 0.4f;
                GameQualitySettings.dynamicObjectsFarClip = 0.6f;
                dObjectDistance = 35f;
                dObjectDensity = 0.4f;
                nPError = 23f;
                tDistance = 320f;
                lod = 1;
                billboards = 46f;
                mTrees = (int) 38f;
                fPError = 23f;
                GameQualitySettings.currentRenderingPath = RenderingPath.Forward;
                GameQualitySettings.currentDepthTextureMode = DepthTextureMode.None;
                GameQualitySettings.colorCorrection = true;
                GameQualitySettings.bloomAndFlares = true;
                GameQualitySettings.sunShafts = true;
                GameQualitySettings.depthOfField = false;
                GameQualitySettings.ssao = false;
                GameQualitySettings.clouds = false;
                GameQualitySettings.underwater = false;
                GameQualitySettings.water = 0;
                break;
            case 4:
                GameQualitySettings.ambientParticles = true;
                GameQualitySettings.particleQualityMultiplier = 0.5f;
                GameQualitySettings.dynamicObjectsFarClip = 0.8f;
                dObjectDistance = 40f;
                dObjectDensity = 0.6f;
                nPError = 14f;
                tDistance = 360f;
                lod = 0;
                billboards = 58f;
                mTrees = (int) 49f;
                fPError = 14f;
                GameQualitySettings.currentRenderingPath = RenderingPath.DeferredLighting;
                GameQualitySettings.currentDepthTextureMode = DepthTextureMode.Depth;
                GameQualitySettings.colorCorrection = true;
                GameQualitySettings.bloomAndFlares = true;
                GameQualitySettings.sunShafts = true;
                GameQualitySettings.depthOfField = true;
                GameQualitySettings.ssao = false;
                GameQualitySettings.clouds = true;
                GameQualitySettings.underwater = true;
                GameQualitySettings.water = 1;
                break;
            case 5:
                GameQualitySettings.ambientParticles = true;
                GameQualitySettings.particleQualityMultiplier = 1f;
                GameQualitySettings.dynamicObjectsFarClip = 1f;
                dObjectDistance = 50f;
                dObjectDensity = 1f;
                nPError = 5f;
                tDistance = 400f;
                lod = 0;
                billboards = 70f;
                mTrees = 60;
                fPError = 5f;
                GameQualitySettings.currentRenderingPath = RenderingPath.DeferredLighting;
                GameQualitySettings.currentDepthTextureMode = DepthTextureMode.Depth;
                GameQualitySettings.colorCorrection = true;
                GameQualitySettings.bloomAndFlares = true;
                GameQualitySettings.sunShafts = true;
                GameQualitySettings.depthOfField = true;
                GameQualitySettings.ssao = false;
                GameQualitySettings.clouds = true;
                GameQualitySettings.underwater = true;
                GameQualitySettings.water = 1;
                break;
        }
        if (!(GameQualitySettings.scenes == null))
        {
            int i = 0;
            while (i < GameQualitySettings.scenes.Length)
            {
                GameQualitySettings.scenes[i].sceneInitialized = true;
                GameQualitySettings.scenes[i].detailObjectDistance = dObjectDistance;
                GameQualitySettings.scenes[i].detailObjectDensity = dObjectDensity;
                GameQualitySettings.scenes[i].nearTerrainPixelError = nPError;
                GameQualitySettings.scenes[i].treeDistance = tDistance;
                GameQualitySettings.scenes[i].heightmapMaximumLOD = lod;
                GameQualitySettings.scenes[i].terrainTreesBillboardStart = billboards;
                GameQualitySettings.scenes[i].maxMeshTrees = mTrees;
                i++;
            }
        }
    }

    private void UpdateGameQuality()
    {
        if (QualitySettings.currentLevel != (QualityLevel) GameQualitySettings.overallQuality)
        {
            QualitySettings.currentLevel = (QualityLevel) GameQualitySettings.overallQuality;
            GameQualitySettings.initializedGameSettings = false;
            this.ApplyCustomQualityLevel(GameQualitySettings.overallQuality);
            this.InitializeGameSettings();
        }
        else
        {
            if (QualitySettings.shadowDistance != GameQualitySettings.shadowDistance)
            {
                QualitySettings.shadowDistance = GameQualitySettings.shadowDistance;
            }
            if (QualitySettings.masterTextureLimit != GameQualitySettings.masterTextureLimit)
            {
                QualitySettings.masterTextureLimit = GameQualitySettings.masterTextureLimit;
            }
            if ((QualitySettings.anisotropicFiltering == AnisotropicFiltering.ForceEnable) != GameQualitySettings.anisotropicFiltering)
            {
                if (GameQualitySettings.anisotropicFiltering)
                {
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                }
                else
                {
                    if (GameQualitySettings.overallQuality < 2)
                    {
                        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                    }
                    else
                    {
                        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                    }
                }
            }
        }
    }

    private void UpdateSceneQuality()
    {
        int currentScene = 0;
        if (GameQualitySettings.scenes == null)
        {
            return;
        }
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
        if ((currentScene < 0) || (currentScene >= GameQualitySettings.scenes.Length))
        {
            return;
        }
        SceneSettings cur = GameQualitySettings.scenes[currentScene];
        if (cur == null)
        {
            return;
        }
        if (this.nearTerrain != null)
        {
            if (this.nearTerrain.detailObjectDistance != cur.detailObjectDistance)
            {
                this.nearTerrain.detailObjectDistance = cur.detailObjectDistance;
            }
            if (this.nearTerrain.detailObjectDensity != cur.detailObjectDensity)
            {
                this.nearTerrain.detailObjectDensity = cur.detailObjectDensity;
            }
            if (this.nearTerrain.treeDistance != cur.treeDistance)
            {
                this.nearTerrain.treeDistance = cur.treeDistance;
            }
            if (this.nearTerrain.heightmapPixelError != cur.nearTerrainPixelError)
            {
                this.nearTerrain.heightmapPixelError = cur.nearTerrainPixelError;
            }
            if (this.nearTerrain.treeBillboardDistance != cur.terrainTreesBillboardStart)
            {
                this.nearTerrain.treeBillboardDistance = cur.terrainTreesBillboardStart;
            }
            if (this.nearTerrain.treeMaximumFullLODCount != cur.maxMeshTrees)
            {
                this.nearTerrain.treeMaximumFullLODCount = cur.maxMeshTrees;
            }
            if (this.nearTerrain.heightmapMaximumLOD != cur.heightmapMaximumLOD)
            {
                this.nearTerrain.heightmapMaximumLOD = cur.heightmapMaximumLOD;
            }
        }
        if (GameQualitySettings._ambientParticles != GameQualitySettings.ambientParticles)
        {
            GameQualitySettings._ambientParticles = GameQualitySettings.ambientParticles;
            if (!(this.ambientParticleObjects == null))
            {
                int k = 0;
                while (k < this.ambientParticleObjects.Length)
                {
                    if (this.ambientParticleObjects[k] == null)
                    {
                        goto Label_for_30;
                    }
                    if ((this.ambientParticleObjects[k].name == "dust") || (this.ambientParticleObjects[k].name == "leaves"))
                    {
                        goto Label_for_30;
                    }
                    this.ambientParticleObjects[k].SetActiveRecursively(GameQualitySettings.ambientParticles);
                    Label_for_30:
                    k++;
                }
            }
        }
    }

    private void UpdateCameraSettings()
    {
        if (((((((((((GameQualitySettings._dynamicObjectsFarClip != GameQualitySettings.dynamicObjectsFarClip) || (GameQualitySettings._colorCorrection != GameQualitySettings.colorCorrection)) || (GameQualitySettings._bloomAndFlares != GameQualitySettings.bloomAndFlares)) || (GameQualitySettings._sunShafts != GameQualitySettings.sunShafts)) || (GameQualitySettings._depthOfField != GameQualitySettings.depthOfField)) || (GameQualitySettings._depthOfFieldAvailable != GameQualitySettings.depthOfFieldAvailable)) || (GameQualitySettings._ssao != GameQualitySettings.ssao)) || (GameQualitySettings._clouds != GameQualitySettings.clouds)) || (GameQualitySettings._underwater != GameQualitySettings.underwater)) || (GameQualitySettings._water != GameQualitySettings.water)) || (GameQualitySettings._particleQualityMultiplier != GameQualitySettings.particleQualityMultiplier))
        {
            this.InitializeCameraSettings();
        }
    }

    static GameQualitySettings()
    {
        GameQualitySettings.particleQualityMultiplier = 1f;
        GameQualitySettings._particleQualityMultiplier = 1f;
        GameQualitySettings.colorCorrection = true;
        GameQualitySettings._colorCorrection = true;
        GameQualitySettings.bloomAndFlares = true;
        GameQualitySettings._bloomAndFlares = true;
        GameQualitySettings.sunShafts = true;
        GameQualitySettings._sunShafts = true;
        GameQualitySettings.depthOfField = true;
        GameQualitySettings._depthOfField = true;
        GameQualitySettings.depthOfFieldAvailable = true;
        GameQualitySettings._depthOfFieldAvailable = true;
        GameQualitySettings.clouds = true;
        GameQualitySettings._clouds = true;
        GameQualitySettings.underwater = true;
        GameQualitySettings._underwater = true;
        GameQualitySettings._water = 1;
        GameQualitySettings.water = 1;
        GameQualitySettings.ambientParticles = true;
        GameQualitySettings._ambientParticles = true;
        GameQualitySettings.dynamicObjectsFarClip = 0.55f;
    }

}