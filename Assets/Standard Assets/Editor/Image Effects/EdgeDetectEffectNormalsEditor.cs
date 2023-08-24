using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
[UnityEditor.CustomEditor(typeof(EdgeDetectEffectNormals))]
[UnityEngine.ExecuteInEditMode]
public partial class EdgeDetectEffectNormalsEditor : Editor
{
    public SerializedObject serObj;
    public SerializedProperty highQuality;
    public SerializedProperty sensitivityDepth;
    public SerializedProperty sensitivityNormals;
    public SerializedProperty spread;
    public SerializedProperty edgesIntensity;
    public SerializedProperty edgesOnly;
    public SerializedProperty edgesOnlyBgColor;
    public SerializedProperty edgeBlur;
    public SerializedProperty blurSpread;
    public SerializedProperty blurIterations;
    public bool showShaders;
    public virtual void OnEnable()
    {
        this.serObj = new SerializedObject(this.target);
        this.highQuality = this.serObj.FindProperty("highQuality");
        this.sensitivityDepth = this.serObj.FindProperty("sensitivityDepth");
        this.sensitivityNormals = this.serObj.FindProperty("sensitivityNormals");
        this.spread = this.serObj.FindProperty("spread");
        this.edgesIntensity = this.serObj.FindProperty("edgesIntensity");
        this.edgesOnly = this.serObj.FindProperty("edgesOnly");
        this.edgesOnlyBgColor = this.serObj.FindProperty("edgesOnlyBgColor");
        this.edgeBlur = this.serObj.FindProperty("edgeBlur");
        this.blurSpread = this.serObj.FindProperty("blurSpread");
        this.blurIterations = this.serObj.FindProperty("blurIterations");
    }

    /*
    	// some genral tweak needs
    	EditorGUILayout.PropertyField (bloomIntensity, new GUIContent("bloomIntensity"));	
    	bloomBlurIterations.intValue = EditorGUILayout.IntSlider ("Blur iterations", bloomBlurIterations.intValue, 1, 10);
    	if(1==tweakMode)
    		sepBlurSpread.floatValue = EditorGUILayout.Slider ("Blur spread", sepBlurSpread.floatValue, 0.1, 2.0);
    	else
    		sepBlurSpread.floatValue = 1.0;    	
    	bloomThreshhold.floatValue = EditorGUILayout.Slider ("Threshhold", bloomThreshhold.floatValue, 0.1, 2.0);
    	
    	if(1==tweakMode)
    		useSrcAlphaAsMask.floatValue = EditorGUILayout.Slider (new  GUIContent("Use image alpha as mask","How much should the image alpha values (deifned by all materials, colors and textures alpha values define the bright (blooming/glowing) areas of the image"), useSrcAlphaAsMask.floatValue, 0.0, 1.0);
    	else
    		useSrcAlphaAsMask.floatValue = 1.0;
    	
    	EditorGUILayout.Separator ();
    	
    	EditorGUILayout.PropertyField (lensflares, new GUIContent("Cast lens flares"));
    	if(lensflares.boolValue) {
    		
    		EditorGUILayout.PropertyField (lensflareIntensity, new GUIContent("Intensity"));
    		lensflareThreshhold.floatValue = EditorGUILayout.Slider ("Threshhold", lensflareThreshhold.floatValue, 0.0, 1.0);
    		
    		EditorGUILayout.Separator ();
    		
    		// further lens flare tweakings
    		EditorGUILayout.PropertyField (lensflareMode, new GUIContent(" Mode"));
    		
    		if (lensflareMode.intValue == 0) {
    			// ghosting	
    			EditorGUILayout.PropertyField (flareColorA, new GUIContent(" Color"));
    			EditorGUILayout.PropertyField (flareColorB, new GUIContent(" Color"));
    			EditorGUILayout.PropertyField (flareColorC, new GUIContent(" Color"));
    			EditorGUILayout.PropertyField (flareColorD, new GUIContent(" Color"));
    			
    		} else if (lensflareMode.intValue == 1) {
    			// hollywood
    			EditorGUILayout.PropertyField (hollyStretchWidth, new GUIContent(" Stretch width"));
    			hollywoodFlareBlurIterations.intValue = EditorGUILayout.IntSlider (" Blur iterations", hollywoodFlareBlurIterations.intValue, 1, 10);
    			
    			EditorGUILayout.PropertyField (flareColorA, new GUIContent(" Color"));
    			
    		} else if (lensflareMode.intValue == 2) {
    			// both
    			EditorGUILayout.PropertyField (hollyStretchWidth, new GUIContent(" Stretch width"));
    			hollywoodFlareBlurIterations.intValue = EditorGUILayout.IntSlider (" Blur iterations", hollywoodFlareBlurIterations.intValue, 1, 10);
    			
    			EditorGUILayout.PropertyField (flareColorA, new GUIContent(" Color"));
    			EditorGUILayout.PropertyField (flareColorB, new GUIContent(" Color"));
    			EditorGUILayout.PropertyField (flareColorC, new GUIContent(" Color"));
    			EditorGUILayout.PropertyField (flareColorD, new GUIContent(" Color"));    			
    		} 
    	}
    	
    	EditorGUILayout.Separator ();
    	
    	if(0==tweakMode) {
    		
    	} else if (1==tweakMode) {
    		EditorGUILayout.PropertyField (enableAddToBloomLayer, new GUIContent("Bloom specific layers?","If you want to always have objects in specific layers to be glowing, chose an appropriate layer mask here. These objects will be glowing/blooming no matter what their material writes to alpha. Make sure to specify the layer mask as precise as possible for maximum performance."));
    		if (enableAddToBloomLayer.boolValue)
    			EditorGUILayout.PropertyField (addToBloomLayers, new GUIContent(" Choose mask","If you want to always have objects in specific layers to be glowing, chose an appropriate layer mask here. These objects will be glowing/blooming no matter what their material writes to alpha.")); 

    		EditorGUILayout.PropertyField (enableRemoveFromBloomLayer, new GUIContent("Don't bloom specific layers?"));
    		if (enableRemoveFromBloomLayer.boolValue)
    			EditorGUILayout.PropertyField (removeFromBloomLayers, new GUIContent(" Choose mask"));     		
    		
		
			EditorGUILayout.Separator ();
    	}
    	
    	// maybe show the fucking shaders
    	showShaders = EditorGUILayout.Toggle ("Show assigned shaders", showShaders);
    	if (showShaders) {
 	    	target.addAlphaHackShader = EditorGUILayout.ObjectField(" shader",target.addAlphaHackShader,Shader as System.Type);
 			target.vignetteShader = EditorGUILayout.ObjectField(" shader",target.vignetteShader,Shader as System.Type);
 			target.lensFlareShader = EditorGUILayout.ObjectField(" shader",target.lensFlareShader,Shader as System.Type);
 			target.separableBlurShader = EditorGUILayout.ObjectField(" shader",target.separableBlurShader,Shader as System.Type);
 			
 			target.addBrightStuffShader = EditorGUILayout.ObjectField(" shader",target.addBrightStuffShader,Shader as System.Type);
    		target.addBrightStuffOneOneShader = EditorGUILayout.ObjectField(" shader",target.addBrightStuffOneOneShader,Shader as System.Type);
    		target.hollywoodFlareBlurShader = EditorGUILayout.ObjectField(" shader",target.hollywoodFlareBlurShader,Shader as System.Type);
    		target.hollywoodFlareStretchShader = EditorGUILayout.ObjectField(" shader",target.hollywoodFlareStretchShader,Shader as System.Type);
    		target.brightPassFilterShader = EditorGUILayout.ObjectField(" shader",target.brightPassFilterShader,Shader as System.Type);
    	}
    	
    	serObj.ApplyModifiedProperties();
       */    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(this.highQuality, new GUIContent("Advanced"), new GUILayoutOption[] {});
        if (this.highQuality.boolValue)
        {
            EditorGUILayout.PropertyField(this.sensitivityDepth, new GUIContent("Depth sensitivity"), new GUILayoutOption[] {});
            EditorGUILayout.PropertyField(this.sensitivityNormals, new GUIContent("Normals sensitivity"), new GUILayoutOption[] {});
            this.spread.floatValue = EditorGUILayout.Slider("Spread", this.spread.floatValue, 0.1f, 2f, new GUILayoutOption[] {});
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(this.edgesIntensity, new GUIContent("Edge intensity"), new GUILayoutOption[] {});
            EditorGUILayout.Separator();
            this.edgesOnly.floatValue = EditorGUILayout.Slider("Draw edges only", this.edgesOnly.floatValue, 0f, 1f, new GUILayoutOption[] {});
            EditorGUILayout.PropertyField(this.edgesOnlyBgColor, new GUIContent("Background color"), new GUILayoutOption[] {});
            EditorGUILayout.PropertyField(this.edgeBlur, new GUIContent("Blur edges"), new GUILayoutOption[] {});
            if (this.edgeBlur.boolValue)
            {
                EditorGUILayout.Separator();
                this.blurSpread.floatValue = EditorGUILayout.Slider("Blur spread", this.blurSpread.floatValue, 0.1f, 10f, new GUILayoutOption[] {});
                this.blurIterations.intValue = EditorGUILayout.IntSlider("Blur iterations", this.blurIterations.intValue, 1, 10, new GUILayoutOption[] {});
            }
        }
        this.serObj.ApplyModifiedProperties();
        this.showShaders = EditorGUILayout.Toggle("Show assigned shaders", this.showShaders, new GUILayoutOption[] {});
        if (this.showShaders)
        {
            this.target.edgeDetectHqShader = EditorGUILayout.ObjectField(" shader", this.target.edgeDetectHqShader, Shader as System.Type, new GUILayoutOption[] {});
            this.target.edgeDetectShader = EditorGUILayout.ObjectField(" shader", this.target.edgeDetectShader, Shader as System.Type, new GUILayoutOption[] {});
            this.target.sepBlurShader = EditorGUILayout.ObjectField(" shader", this.target.sepBlurShader, Shader as System.Type, new GUILayoutOption[] {});
            this.target.edgeApplyShader = EditorGUILayout.ObjectField(" shader", this.target.edgeApplyShader, Shader as System.Type, new GUILayoutOption[] {});
        }
    }

}