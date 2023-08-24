using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
[UnityEditor.CustomEditor(typeof(DepthOfField))]
[UnityEngine.ExecuteInEditMode]
public partial class DepthOfFieldEditor : Editor
{
    public SerializedObject serObj;
    public SerializedProperty resolution; // = DofResolutionSetting.Normal;
    public SerializedProperty quality; // = DofQualitySetting.High;
    public SerializedProperty focalZDistance;//float = 0.0;
    public SerializedProperty focalZStart;//float = 0.0;
    public SerializedProperty focalZEnd;//float = 10000.0;
    public SerializedProperty focalFalloff;//float = 1.0;
    public SerializedProperty focusOnThis;//Transform = null;
    public SerializedProperty focusOnScreenCenterDepth;//boolean = false;
    public SerializedProperty focalSize;//float = 0.0375;
    public SerializedProperty focalChangeSpeed;//float = 2.275;
    public SerializedProperty blurIterations;//int = 2;
    public SerializedProperty foregroundBlurIterations;//int = 2;
    public SerializedProperty blurSpread;//float = 1.5;
    public SerializedProperty foregroundBlurSpread;//float = 1.5;
    public SerializedProperty foregroundBlurStrength;//float = 1.5;
    public SerializedProperty foregroundBlurThreshhold;//float = 0.001;
    public virtual void OnEnable()
    {
        this.serObj = new SerializedObject(this.target);
        this.resolution = this.serObj.FindProperty("resolution");
        this.quality = this.serObj.FindProperty("quality");
        this.focalZDistance = this.serObj.FindProperty("focalZDistance");
        this.focalZStart = this.serObj.FindProperty("focalZStart");
        this.focalZEnd = this.serObj.FindProperty("focalZEnd");
        this.focalFalloff = this.serObj.FindProperty("focalFalloff");
        this.focusOnThis = this.serObj.FindProperty("focusOnThis");
        this.focusOnScreenCenterDepth = this.serObj.FindProperty("focusOnScreenCenterDepth");
        this.focalSize = this.serObj.FindProperty("focalSize");
        this.focalChangeSpeed = this.serObj.FindProperty("focalChangeSpeed");
        this.blurIterations = this.serObj.FindProperty("blurIterations");
        this.foregroundBlurIterations = this.serObj.FindProperty("foregroundBlurIterations");
        this.blurSpread = this.serObj.FindProperty("blurSpread");
        this.foregroundBlurSpread = this.serObj.FindProperty("foregroundBlurSpread");
        this.foregroundBlurStrength = this.serObj.FindProperty("foregroundBlurStrength");
        this.foregroundBlurThreshhold = this.serObj.FindProperty("foregroundBlurThreshhold");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(this.resolution, new GUIContent("Resolution"), new GUILayoutOption[] {});
        EditorGUILayout.PropertyField(this.quality, new GUIContent("Quality"), new GUILayoutOption[] {});
        EditorGUILayout.Separator();
        this.focalZDistance.floatValue = EditorGUILayout.FloatField("Focal Distance", this.focalZDistance.floatValue, new GUILayoutOption[] {});
        this.focalZStart.floatValue = EditorGUILayout.FloatField("Focal Start", this.focalZStart.floatValue, new GUILayoutOption[] {});
        this.focalZEnd.floatValue = EditorGUILayout.FloatField("Focal End", this.focalZEnd.floatValue, new GUILayoutOption[] {});
        this.focalFalloff.floatValue = EditorGUILayout.FloatField("Focal Falloff", this.focalFalloff.floatValue, new GUILayoutOption[] {});
        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(this.focusOnScreenCenterDepth, new GUIContent("Focus On Center", "This will enable automatic depth buffer read to focus on the area centered around a raycast throught the center of the screen."), new GUILayoutOption[] {});
        if (this.focusOnScreenCenterDepth.boolValue)
        {
            EditorGUILayout.PropertyField(this.focalSize, new GUIContent("Focal Size"), new GUILayoutOption[] {});
            EditorGUILayout.PropertyField(this.focalChangeSpeed, new GUIContent("Adjust Speed"), new GUILayoutOption[] {});
        }
        else
        {
            EditorGUILayout.PropertyField(this.focusOnThis, new GUIContent("Focus on transform"), new GUILayoutOption[] {});
        }
        EditorGUILayout.Separator();
        this.blurIterations.intValue = EditorGUILayout.IntSlider("Blur Iterations", this.blurIterations.intValue, 1, 10, new GUILayoutOption[] {});
        this.blurSpread.floatValue = EditorGUILayout.Slider("Blur Spread", this.blurSpread.floatValue, 0f, 5f, new GUILayoutOption[] {});
        EditorGUILayout.Separator();
        if (this.quality.intValue > 1)
        {
            GUILayout.Label("Foreground Blur Settings", new GUILayoutOption[] {});
            this.foregroundBlurIterations.intValue = EditorGUILayout.IntSlider("Iterations", this.foregroundBlurIterations.intValue, 1, 5, new GUILayoutOption[] {});
            this.foregroundBlurSpread.floatValue = EditorGUILayout.Slider("Spread", this.foregroundBlurSpread.floatValue, 0f, 5f, new GUILayoutOption[] {});
            this.foregroundBlurStrength.floatValue = EditorGUILayout.FloatField("Strength", this.foregroundBlurStrength.floatValue, new GUILayoutOption[] {});
            this.foregroundBlurThreshhold.floatValue = EditorGUILayout.FloatField("Threshhold", this.foregroundBlurThreshhold.floatValue, new GUILayoutOption[] {});
        }
        this.serObj.ApplyModifiedProperties();
    }

}