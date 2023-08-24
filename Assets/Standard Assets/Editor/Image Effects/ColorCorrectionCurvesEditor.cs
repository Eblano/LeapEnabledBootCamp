using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
[UnityEditor.CustomEditor(typeof(ColorCorrectionCurves))]
[UnityEngine.ExecuteInEditMode]
public partial class ColorCorrectionCurvesEditor : Editor
{
    public bool showShaders;
    public virtual void Awake()
    {
    }

    public virtual void OnEnable()
    {
        if (this.target.redChannel == null)
        {
            this.target.redChannel = new AnimationCurve(new Keyframe[] {new Keyframe(0, 0f, 1f, 1f), new Keyframe(1, 1f, 1f, 1f)});
        }
        if (this.target.greenChannel == null)
        {
            this.target.greenChannel = new AnimationCurve(new Keyframe[] {new Keyframe(0, 0f, 1f, 1f), new Keyframe(1, 1f, 1f, 1f)});
        }
        if (this.target.blueChannel == null)
        {
            this.target.blueChannel = new AnimationCurve(new Keyframe[] {new Keyframe(0, 0f, 1f, 1f), new Keyframe(1, 1f, 1f, 1f)});
        }
        if (this.target.depthRedChannel == null)
        {
            this.target.depthRedChannel = new AnimationCurve(new Keyframe[] {new Keyframe(0, 0f, 1f, 1f), new Keyframe(1, 1f, 1f, 1f)});
        }
        if (this.target.depthGreenChannel == null)
        {
            this.target.depthGreenChannel = new AnimationCurve(new Keyframe[] {new Keyframe(0, 0f, 1f, 1f), new Keyframe(1, 1f, 1f, 1f)});
        }
        if (this.target.depthBlueChannel == null)
        {
            this.target.depthBlueChannel = new AnimationCurve(new Keyframe[] {new Keyframe(0, 0f, 1f, 1f), new Keyframe(1, 1f, 1f, 1f)});
        }
        if (this.target.zCurve == null)
        {
            this.target.zCurve = new AnimationCurve(new Keyframe[] {new Keyframe(0, 0f, 1f, 1f), new Keyframe(1, 1f, 1f, 1f)});
        }
        EditorUtility.SetDirty(this.target);
    }

    public override void OnInspectorGUI()
    {
        this.target.mode = EditorGUILayout.EnumPopup("Mode", this.target.mode, EditorStyles.popup, new GUILayoutOption[] {});
        EditorGUILayout.Separator();
        EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {});
        this.target.redChannel = EditorGUILayout.CurveField(new GUIContent("Red"), this.target.redChannel, Color.red, new Rect(0f, 0f, 1f, 1f), new GUILayoutOption[] {});
        //EditorGUILayout.CurveField (GUIContent("Red"), property, settings );
        if (GUILayout.Button("Reset", new GUILayoutOption[] {}))
        {
            this.target.redChannel = EditorGUILayout.CurveField(new GUIContent("Red"), new AnimationCurve(new Keyframe[] {new Keyframe(0, 0f, 1f, 1f), new Keyframe(1, 1f, 1f, 1f)}), Color.red, new Rect(0f, 0f, 1f, 1f), new GUILayoutOption[] {});
            this.target.updateTextures = true;
            EditorUtility.SetDirty(this.target);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {});
        this.target.greenChannel = EditorGUILayout.CurveField(new GUIContent("Green"), this.target.greenChannel, Color.green, new Rect(0f, 0f, 1f, 1f), new GUILayoutOption[] {});
        if (GUILayout.Button("Reset", new GUILayoutOption[] {}))
        {
            this.target.greenChannel = EditorGUILayout.CurveField(new GUIContent("Green"), new AnimationCurve(new Keyframe[] {new Keyframe(0, 0f, 1f, 1f), new Keyframe(1, 1f, 1f, 1f)}), Color.green, new Rect(0f, 0f, 1f, 1f), new GUILayoutOption[] {});
            this.target.updateTextures = true;
            EditorUtility.SetDirty(this.target);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {});
        this.target.blueChannel = EditorGUILayout.CurveField(new GUIContent("Blue"), this.target.blueChannel, Color.blue, new Rect(0f, 0f, 1f, 1f), new GUILayoutOption[] {});
        if (GUILayout.Button("Reset", new GUILayoutOption[] {}))
        {
            this.target.blueChannel = EditorGUILayout.CurveField(new GUIContent("Blue"), new AnimationCurve(new Keyframe[] {new Keyframe(0, 0f, 1f, 1f), new Keyframe(1, 1f, 1f, 1f)}), Color.blue, new Rect(0f, 0f, 1f, 1f), new GUILayoutOption[] {});
            this.target.updateTextures = true;
            EditorUtility.SetDirty(this.target);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();
        //target.useDepthCorrection = EditorGUILayout.Toggle ("Depth Correction", target.useDepthCorrection);
        if (this.target.mode > 0)
        {
            this.target.useDepthCorrection = true;
        }
        else
        {
            this.target.useDepthCorrection = false;
        }
        if (this.target.useDepthCorrection != null)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {});
            this.target.depthRedChannel = EditorGUILayout.CurveField(new GUIContent("Red (Depth)"), this.target.depthRedChannel, Color.red, new Rect(0f, 0f, 1f, 1f), new GUILayoutOption[] {});
            if (GUILayout.Button("Reset", new GUILayoutOption[] {}))
            {
                this.target.depthRedChannel = EditorGUILayout.CurveField(new GUIContent("Red (Depth)"), new AnimationCurve(new Keyframe[] {new Keyframe(0, 0f, 1f, 1f), new Keyframe(1, 1f, 1f, 1f)}), Color.red, new Rect(0f, 0f, 1f, 1f), new GUILayoutOption[] {});
                this.target.updateTextures = true;
                EditorUtility.SetDirty(this.target);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {});
            this.target.depthGreenChannel = EditorGUILayout.CurveField(new GUIContent("Green (Depth)"), this.target.depthGreenChannel, Color.green, new Rect(0f, 0f, 1f, 1f), new GUILayoutOption[] {});
            if (GUILayout.Button("Reset", new GUILayoutOption[] {}))
            {
                this.target.depthGreenChannel = EditorGUILayout.CurveField(new GUIContent("Green (Depth)"), new AnimationCurve(new Keyframe[] {new Keyframe(0, 0f, 1f, 1f), new Keyframe(1, 1f, 1f, 1f)}), Color.green, new Rect(0f, 0f, 1f, 1f), new GUILayoutOption[] {});
                this.target.updateTextures = true;
                EditorUtility.SetDirty(this.target);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {});
            this.target.depthBlueChannel = EditorGUILayout.CurveField(new GUIContent("Blue (Depth)"), this.target.depthBlueChannel, Color.blue, new Rect(0f, 0f, 1f, 1f), new GUILayoutOption[] {});
            if (GUILayout.Button("Reset", new GUILayoutOption[] {}))
            {
                this.target.depthBlueChannel = EditorGUILayout.CurveField(new GUIContent("Blue (Depth)"), new AnimationCurve(new Keyframe[] {new Keyframe(0, 0f, 1f, 1f), new Keyframe(1, 1f, 1f, 1f)}), Color.blue, new Rect(0f, 0f, 1f, 1f), new GUILayoutOption[] {});
                this.target.updateTextures = true;
                EditorUtility.SetDirty(this.target);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {});
            this.target.zCurve = EditorGUILayout.CurveField(new GUIContent("z Curve"), this.target.zCurve, Color.white, new Rect(0f, 0f, 1f, 1f), new GUILayoutOption[] {});
            if (GUILayout.Button("Reset", new GUILayoutOption[] {}))
            {
                this.target.zCurve = EditorGUILayout.CurveField(new GUIContent("z Curve"), new AnimationCurve(new Keyframe[] {new Keyframe(0, 0f, 1f, 1f), new Keyframe(1, 1f, 1f, 1f)}), Color.white, new Rect(0f, 0f, 1f, 1f), new GUILayoutOption[] {});
                this.target.updateTextures = true;
                EditorUtility.SetDirty(this.target);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.Separator();
        this.target.selectiveCc = EditorGUILayout.Toggle("Selective", this.target.selectiveCc, new GUILayoutOption[] {});
        if (this.target.selectiveCc != null)
        {
            this.target.selectiveFromColor = EditorGUILayout.ColorField("Key color", this.target.selectiveFromColor, new GUILayoutOption[] {});
            this.target.selectiveToColor = EditorGUILayout.ColorField("Target color", this.target.selectiveToColor, new GUILayoutOption[] {});
        }
        EditorGUILayout.Separator();
        object oldCurveResolution = this.target.curveResolution;
        this.target.curveResolution = EditorGUILayout.IntSlider("Curve Resolution", (int) this.target.curveResolution, 8, 256, new GUILayoutOption[] {});
        this.target.curveResolution = Mathf.ClosestPowerOfTwo((int) this.target.curveResolution);
        if (!(oldCurveResolution == this.target.curveResolution))
        {
        }
        this.target.recreateTextures = true;
        if (GUI.changed)
        {
            this.target.updateTextures = true;
            EditorUtility.SetDirty(this.target);
        }
    }

}