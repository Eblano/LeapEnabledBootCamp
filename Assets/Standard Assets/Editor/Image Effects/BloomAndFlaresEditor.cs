using UnityEngine;
using UnityEditor;
using System.Collections;

public enum TweakMode
{
    Simple = 0,
    Advanced = 1
}

[System.Serializable]
[UnityEngine.ExecuteInEditMode]
public partial class BloomAndFlaresEditor : Editor
{
    public SerializedProperty tweakMode;
    public SerializedObject serObj;
    public SerializedProperty bloomThisTag;
    public SerializedProperty sepBlurSpread;
    public SerializedProperty useSrcAlphaAsMask;
    public SerializedProperty bloomIntensity;
    public SerializedProperty bloomThreshhold;
    public SerializedProperty bloomBlurIterations;
    public SerializedProperty lensflares;
    public SerializedProperty hollywoodFlareBlurIterations;
    public SerializedProperty lensflareMode;
    public SerializedProperty hollyStretchWidth;
    public SerializedProperty lensflareIntensity;
    public SerializedProperty lensflareThreshhold;
    public SerializedProperty flareColorA;
    public SerializedProperty flareColorB;
    public SerializedProperty flareColorC;
    public SerializedProperty flareColorD;
    public SerializedProperty blurWidth;
    public virtual void OnEnable()
    {
        this.serObj = new SerializedObject(this.target);
        this.bloomThisTag = this.serObj.FindProperty("bloomThisTag");
        this.sepBlurSpread = this.serObj.FindProperty("sepBlurSpread");
        this.useSrcAlphaAsMask = this.serObj.FindProperty("useSrcAlphaAsMask");
        this.bloomIntensity = this.serObj.FindProperty("bloomIntensity");
        this.bloomThreshhold = this.serObj.FindProperty("bloomThreshhold");
        this.bloomBlurIterations = this.serObj.FindProperty("bloomBlurIterations");
        this.lensflares = this.serObj.FindProperty("lensflares");
        this.lensflareMode = this.serObj.FindProperty("lensflareMode");
        this.hollywoodFlareBlurIterations = this.serObj.FindProperty("hollywoodFlareBlurIterations");
        this.hollyStretchWidth = this.serObj.FindProperty("hollyStretchWidth");
        this.lensflareIntensity = this.serObj.FindProperty("lensflareIntensity");
        this.lensflareThreshhold = this.serObj.FindProperty("lensflareThreshhold");
        this.flareColorA = this.serObj.FindProperty("flareColorA");
        this.flareColorB = this.serObj.FindProperty("flareColorB");
        this.flareColorC = this.serObj.FindProperty("flareColorC");
        this.flareColorD = this.serObj.FindProperty("flareColorD");
        this.blurWidth = this.serObj.FindProperty("blurWidth");
        this.tweakMode = this.serObj.FindProperty("tweakMode");
    }

    /*
       if (GUI.changed) {
        	EditorUtility.SetDirty (target);     
        	//(target._dirty = true;
       }
       */    public override void OnInspectorGUI()
    {
         //tweakMode = EditorGUILayout.EnumPopup("Mode", tweakMode, EditorStyles.popup);
        EditorGUILayout.PropertyField(this.tweakMode, new GUIContent("Mode"), new GUILayoutOption[] {});
        EditorGUILayout.Separator();
        // some genral tweak needs
        EditorGUILayout.PropertyField(this.bloomIntensity, new GUIContent("Intensity"), new GUILayoutOption[] {});
        EditorGUILayout.Separator();
        this.bloomBlurIterations.intValue = EditorGUILayout.IntSlider("Blur iterations", this.bloomBlurIterations.intValue, 1, 10, new GUILayoutOption[] {});
        if (1 == this.tweakMode.intValue)
        {
            this.sepBlurSpread.floatValue = EditorGUILayout.Slider("Blur spread", this.sepBlurSpread.floatValue, 0.1f, 2f, new GUILayoutOption[] {});
        }
        else
        {
            this.sepBlurSpread.floatValue = 1f;
        }
        this.bloomThreshhold.floatValue = EditorGUILayout.Slider("Threshhold", this.bloomThreshhold.floatValue, -0.05f, 1f, new GUILayoutOption[] {});
        if (1 == this.tweakMode.intValue)
        {
            this.useSrcAlphaAsMask.floatValue = EditorGUILayout.Slider(new GUIContent("Use alpha mask", "How much should the image alpha values (deifned by all materials, colors and textures alpha values define the bright (blooming/glowing) areas of the image"), this.useSrcAlphaAsMask.floatValue, 0f, 1f, new GUILayoutOption[] {});
        }
        else
        {
            this.useSrcAlphaAsMask.floatValue = 1f;
        }
        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(this.lensflares, new GUIContent("Cast lens flares"), new GUILayoutOption[] {});
        if (this.lensflares.boolValue)
        {
            EditorGUILayout.PropertyField(this.lensflareIntensity, new GUIContent("Intensity"), new GUILayoutOption[] {});
            this.lensflareThreshhold.floatValue = EditorGUILayout.Slider("Threshhold", this.lensflareThreshhold.floatValue, 0f, 1f, new GUILayoutOption[] {});
            EditorGUILayout.Separator();
            // further lens flare tweakings
            EditorGUILayout.PropertyField(this.lensflareMode, new GUIContent(" Mode"), new GUILayoutOption[] {});
            if (this.lensflareMode.intValue == 0)
            {
                // ghosting	
                EditorGUILayout.PropertyField(this.flareColorA, new GUIContent(" Color"), new GUILayoutOption[] {});
                EditorGUILayout.PropertyField(this.flareColorB, new GUIContent(" Color"), new GUILayoutOption[] {});
                EditorGUILayout.PropertyField(this.flareColorC, new GUIContent(" Color"), new GUILayoutOption[] {});
                EditorGUILayout.PropertyField(this.flareColorD, new GUIContent(" Color"), new GUILayoutOption[] {});
            }
            else
            {
                if (this.lensflareMode.intValue == 1)
                {
                    // hollywood
                    EditorGUILayout.PropertyField(this.hollyStretchWidth, new GUIContent(" Stretch width"), new GUILayoutOption[] {});
                    this.hollywoodFlareBlurIterations.intValue = EditorGUILayout.IntSlider(" Blur iterations", this.hollywoodFlareBlurIterations.intValue, 1, 8, new GUILayoutOption[] {});
                    EditorGUILayout.PropertyField(this.flareColorA, new GUIContent(" Color"), new GUILayoutOption[] {});
                }
                else
                {
                    if (this.lensflareMode.intValue == 2)
                    {
                        // both
                        EditorGUILayout.PropertyField(this.hollyStretchWidth, new GUIContent(" Stretch width"), new GUILayoutOption[] {});
                        this.hollywoodFlareBlurIterations.intValue = EditorGUILayout.IntSlider(" Blur iterations", this.hollywoodFlareBlurIterations.intValue, 1, 8, new GUILayoutOption[] {});
                        EditorGUILayout.PropertyField(this.flareColorA, new GUIContent(" Color"), new GUILayoutOption[] {});
                        EditorGUILayout.PropertyField(this.flareColorB, new GUIContent(" Color"), new GUILayoutOption[] {});
                        EditorGUILayout.PropertyField(this.flareColorC, new GUIContent(" Color"), new GUILayoutOption[] {});
                        EditorGUILayout.PropertyField(this.flareColorD, new GUIContent(" Color"), new GUILayoutOption[] {});
                    }
                }
            }
        }
        EditorGUILayout.Separator();
        if (0 == this.tweakMode.intValue)
        {
        }
        else
        {
            if (1 == this.tweakMode.intValue)
            {
                 //EditorGUILayout.PropertyField (bloomThisTag, new GUIContent("Extra Bloom Tag","If you want to always have objects of a certain tag to be 'glowing', select the tag here and tag the game objects in question. These objects will start glowing/blooming no matter what their material writes to the alpha channel."));    
                GUILayout.Label("Add extra bloom on tagged objects?", new GUILayoutOption[] {});
                EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {});
                GUILayout.Label(" Tag", new GUILayoutOption[] {});
                this.bloomThisTag.stringValue = EditorGUILayout.TagField(this.bloomThisTag.stringValue, new GUILayoutOption[] {});
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
            }
        }
        this.serObj.ApplyModifiedProperties();
    }

}