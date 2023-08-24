using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
[UnityEditor.CustomEditor(typeof(SunShafts))]
[UnityEngine.ExecuteInEditMode]
public partial class SunShaftsEditor : Editor
{
    public SerializedObject serObj;
    public SerializedProperty sunTransform;
    public SerializedProperty radialBlurIterations;
    public SerializedProperty sunColor;
    public SerializedProperty sunShaftBlurRadius;
    public SerializedProperty sunShaftIntensity;
    public SerializedProperty useSkyBoxAlpha;
    public SerializedProperty useDepthTexture;
    public SerializedProperty resolution;
    public SerializedProperty maxRadius;
    public virtual void OnEnable()
    {
        this.serObj = new SerializedObject(this.target);
        this.sunTransform = this.serObj.FindProperty("sunTransform");
        this.sunColor = this.serObj.FindProperty("sunColor");
        this.sunShaftBlurRadius = this.serObj.FindProperty("sunShaftBlurRadius");
        this.radialBlurIterations = this.serObj.FindProperty("radialBlurIterations");
        this.sunShaftIntensity = this.serObj.FindProperty("sunShaftIntensity");
        this.useSkyBoxAlpha = this.serObj.FindProperty("useSkyBoxAlpha");
        this.resolution = this.serObj.FindProperty("resolution");
        this.maxRadius = this.serObj.FindProperty("maxRadius");
        this.useDepthTexture = this.serObj.FindProperty("useDepthTexture");
    }

    public override void OnInspectorGUI()
    {
        bool oldVal = this.useDepthTexture.boolValue;
        EditorGUILayout.PropertyField(this.useDepthTexture, new GUIContent("Use Depth Texture"), new GUILayoutOption[] {});
        GUILayout.Label(((object) " Camera depth texture mode: ") + this.target.camera.depthTextureMode);
        bool newVal = this.useDepthTexture.boolValue;
        if (newVal != oldVal)
        {
            if (newVal)
            {

                {
                    object _5 = this.target.camera.depthTextureMode | DepthTextureMode.Depth;
                    object _6 = this.target.camera;
                    _6.depthTextureMode = _5;
                }
            }
            else
            {

                {
                    object _7 = this.target.camera.depthTextureMode & ~DepthTextureMode.Depth;
                    object _8 = this.target.camera;
                    _8.depthTextureMode = _7;
                }
            }
        }
        EditorGUILayout.PropertyField(this.resolution, new GUIContent("Resolution"), new GUILayoutOption[] {});
        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(this.sunTransform, new GUIContent("Sun caster", "Chose a transform that acts as a root point for the produced sun shafts"), new GUILayoutOption[] {});
        if ((this.target.sunTransform != null) && (this.target.camera != null))
        {
            if (GUILayout.Button("Align to viewport center", new GUILayoutOption[] {}))
            {
                Ray ray = this.target.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

                {
                    Vector3 _9 = ray.origin + (ray.direction * 500f);
                    object _10 = this.target.sunTransform;
                    _10.position = _9;
                }
                this.target.sunTransform.LookAt(this.target.transform);
            }
        }
        EditorGUILayout.PropertyField(this.sunColor, new GUIContent("Sun color"), new GUILayoutOption[] {});
        EditorGUILayout.PropertyField(this.maxRadius, new GUIContent("Radius"), new GUILayoutOption[] {});
        EditorGUILayout.Separator();
        this.sunShaftBlurRadius.floatValue = EditorGUILayout.Slider("Blur offset", this.sunShaftBlurRadius.floatValue, 0f, 0.1f, new GUILayoutOption[] {});
        this.radialBlurIterations.intValue = EditorGUILayout.IntSlider("Blur iterations", this.radialBlurIterations.intValue, 0, 6, new GUILayoutOption[] {});
        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(this.sunShaftIntensity, new GUIContent("Intensity"), new GUILayoutOption[] {});
        this.useSkyBoxAlpha.floatValue = EditorGUILayout.Slider("Use alpha mask", this.useSkyBoxAlpha.floatValue, 0f, 1f, new GUILayoutOption[] {});
        this.serObj.ApplyModifiedProperties();
    }

}