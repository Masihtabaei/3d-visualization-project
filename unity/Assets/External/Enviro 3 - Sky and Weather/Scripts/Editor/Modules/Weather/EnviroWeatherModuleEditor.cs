using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Enviro
{
    [CustomEditor(typeof(EnviroWeatherModule))]
    public class EnviroWeatherModuleEditor : EnviroModuleEditor
    {  
        private EnviroWeatherModule myTarget; 


        //Properties
        private SerializedProperty cloudsTransitionSpeed,fogTransitionSpeed,skyTransitionSpeed, lightingTransitionSpeed,effectsTransitionSpeed,auroraTransitionSpeed,environmentTransitionSpeed,audioTransitionSpeed;  
        private SerializedProperty globalAutoWeatherChange;

        private int controlID = -100;
        //On Enable
        public override void OnEnable()
        { 
            if(!target)
                return; 

            myTarget = (EnviroWeatherModule)target;
            serializedObj = new SerializedObject(myTarget);
            preset = serializedObj.FindProperty("preset");
            cloudsTransitionSpeed = serializedObj.FindProperty("Settings.cloudsTransitionSpeed");
            fogTransitionSpeed = serializedObj.FindProperty("Settings.fogTransitionSpeed");
            lightingTransitionSpeed = serializedObj.FindProperty("Settings.lightingTransitionSpeed");
            skyTransitionSpeed = serializedObj.FindProperty("Settings.skyTransitionSpeed"); 
            effectsTransitionSpeed = serializedObj.FindProperty("Settings.effectsTransitionSpeed");
            auroraTransitionSpeed = serializedObj.FindProperty("Settings.auroraTransitionSpeed"); 
            audioTransitionSpeed = serializedObj.FindProperty("Settings.audioTransitionSpeed");
            environmentTransitionSpeed = serializedObj.FindProperty("Settings.environmentTransitionSpeed");
            globalAutoWeatherChange = serializedObj.FindProperty("globalAutoWeatherChange"); 
        } 
        public override void OnInspectorGUI()
        {
            if(!target)
                return;

            base.OnInspectorGUI();

            GUI.backgroundColor = baseModuleColor;
            GUILayout.BeginVertical("",boxStyleModified);
            GUI.backgroundColor = Color.white;
            EditorGUILayout.BeginHorizontal();
            myTarget.showModuleInspector = GUILayout.Toggle(myTarget.showModuleInspector, "Weather", headerFoldout);
            
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("x", EditorStyles.miniButtonRight,GUILayout.Width(18), GUILayout.Height(18)))
            {
                EnviroManager.instance.RemoveModule(EnviroManager.ModuleType.Weather);
                DestroyImmediate(this);
                return;
            } 
            
            EditorGUILayout.EndHorizontal();
            
            if(myTarget.showModuleInspector) 
            {
                serializedObj.UpdateIfRequiredOrScript ();
                EditorGUI.BeginChangeCheck();

                GUI.backgroundColor = categoryModuleColor;
                GUILayout.BeginVertical("",boxStyleModified);
                GUI.backgroundColor = Color.white;
                myTarget.showWeatherPresetsControls = GUILayout.Toggle(myTarget.showWeatherPresetsControls, "Weather Preset Controls", headerFoldout);               
                if(myTarget.showWeatherPresetsControls)
                {
                GUILayout.Space(5);
                
                Object selectedObject = null;

                if(GUILayout.Button("Add"))
                {
                   controlID = EditorGUIUtility.GetControlID (FocusType.Passive);
                   EditorGUIUtility.ShowObjectPicker<EnviroWeatherType>(null,false,"",controlID);
                }
 
                string commandName = Event.current.commandName;

                if (commandName == "ObjectSelectorClosed" && EditorGUIUtility.GetObjectPickerControlID() == controlID) 
                {
            
                        selectedObject = EditorGUIUtility.GetObjectPickerObject ();
                        
                        bool add = true;
                        
                        for (int i = 0; i < myTarget.Settings.weatherTypes.Count; i++)
                        {
                            if((EnviroWeatherType)selectedObject == myTarget.Settings.weatherTypes[i])
                            add = false;
                        }

                        if(add)
                        {
                        myTarget.Settings.weatherTypes.Add((EnviroWeatherType)selectedObject);
                        EditorUtility.SetDirty(myTarget);
                        }
                        
                    controlID = -100;
                }

                if(GUILayout.Button("Create New"))
                {
                   myTarget.CreateNewWeatherType();
                } 


                GUILayout.Space(15);
                //Make sure that we remove old empty entries where user deleted the scriptable object.
                myTarget.CleanupList();
 
                for (int i = 0; i < myTarget.Settings.weatherTypes.Count; i++) 
                    {      
                          EnviroWeatherType curWT = myTarget.Settings.weatherTypes[i];

                          if(curWT == myTarget.targetWeatherType)
                             GUI.backgroundColor = new Color(0.0f,0.5f,0.0f,1f);

                            GUILayout.BeginVertical ("", boxStyleModified);
                            GUI.backgroundColor = Color.white;

                            EditorGUILayout.BeginHorizontal();
                            curWT.showEditor = GUILayout.Toggle(curWT.showEditor, curWT.name, headerFoldout);
                            GUILayout.FlexibleSpace();
                            if(curWT != myTarget.targetWeatherType)
                            {
                                if(GUILayout.Button("Set Active", EditorStyles.miniButtonRight,GUILayout.Width(70), GUILayout.Height(18)))
                                {
                                    if(EnviroManager.instance != null)
                                    {
                                        myTarget.ChangeWeather(curWT);
                                        EditorUtility.SetDirty(myTarget);
                                    }
                                } 
                            } 

                            if(GUILayout.Button("x", EditorStyles.miniButtonRight,GUILayout.Width(18), GUILayout.Height(18)))
                            {
                                myTarget.RemoveWeatherType(curWT);
                                EditorUtility.SetDirty(myTarget);
                            } 
                            
                            EditorGUILayout.EndHorizontal();
                            //GUILayout.Space(15);
                            if(curWT.showEditor)
                            {
                                Undo.RecordObject(curWT, "WeatherPreset Changed");
                                curWT.name = EditorGUILayout.TextField ("Name", curWT.name);

                                //Lighting
                                if(EnviroManager.instance == null || EnviroManager.instance.Lighting != null)
                                {
                                GUILayout.BeginVertical ("", boxStyleModified);
                                curWT.showLightingControls = GUILayout.Toggle(curWT.showLightingControls, "Lighting", headerFoldout);
                                
                                if(curWT.showLightingControls)
                                {
                                    GUILayout.Space(5);
                                    curWT.lightingOverride.directLightIntensityModifier = EditorGUILayout.Slider("Direct Light Intensity", curWT.lightingOverride.directLightIntensityModifier,0f,2f);
                                    curWT.lightingOverride.ambientIntensityModifier = EditorGUILayout.Slider("Ambient Light Intensity", curWT.lightingOverride.ambientIntensityModifier,0f,2f);
                                    curWT.lightingOverride.shadowIntensity = EditorGUILayout.Slider("Shadow Intensity", curWT.lightingOverride.shadowIntensity,0f,1f); 
                               
                                }
                                GUILayout.EndVertical();
                                }

                                //Sky
                                if(EnviroManager.instance == null || EnviroManager.instance.Sky != null)
                                { 
                                GUILayout.BeginVertical ("", boxStyleModified);
                                curWT.showSkyControls = GUILayout.Toggle(curWT.showSkyControls, "Sky", headerFoldout);
                                
                                if(curWT.showSkyControls)
                                {
                                    GUILayout.Space(5);
                                    GUIContent colorTitle = new GUIContent();
                                    colorTitle.text = "Sky Color Tint"; 
                                    colorTitle.tooltip = "Sets a color tint for skybox";
                                    //curWT.skyOverride.intensity = EditorGUILayout.FloatField("Direct Light Intensity", curWT.lightingOverride.directLightIntensityModifier,0f,2f);
                                    curWT.skyOverride.skyColorTint = EditorGUILayout.ColorField(colorTitle, curWT.skyOverride.skyColorTint,true,false,true);
                                }
                                GUILayout.EndVertical();
                                }

                                //Volumetric Clouds
                                if(EnviroManager.instance == null || EnviroManager.instance.VolumetricClouds != null)
                                {     
                                GUILayout.BeginVertical ("", boxStyleModified);
                                curWT.showCloudControls = GUILayout.Toggle(curWT.showCloudControls, "Volumetric Clouds", headerFoldout);
                                if(curWT.showCloudControls)
                                {
                                    GUILayout.Space(5);
                                    GUILayout.BeginVertical ("", boxStyleModified);
                                    
                                    curWT.cloudsOverride.showLayer1 = GUILayout.Toggle(curWT.cloudsOverride.showLayer1, "Layer 1", headerFoldout);
                                    if(curWT.cloudsOverride.showLayer1)
                                    {
                                        curWT.cloudsOverride.coverageLayer1 = EditorGUILayout.Slider("Coverage", curWT.cloudsOverride.coverageLayer1,-1f,1f);
                                        curWT.cloudsOverride.dilateCoverageLayer1 = EditorGUILayout.Slider("Dilate Coverage", curWT.cloudsOverride.dilateCoverageLayer1,0f,1f);
                                        curWT.cloudsOverride.dilateTypeLayer1 = EditorGUILayout.Slider("Dilate Type", curWT.cloudsOverride.dilateTypeLayer1,0f,1f);
                                        curWT.cloudsOverride.typeModifierLayer1 = EditorGUILayout.Slider("Type Modifier", curWT.cloudsOverride.typeModifierLayer1,0f,1f);
                                        curWT.cloudsOverride.anvilBiasLayer1 = EditorGUILayout.Slider("Anvil Bias", curWT.cloudsOverride.anvilBiasLayer1,0f,1f);
                                        GUILayout.Space(10);
                                        curWT.cloudsOverride.scatteringIntensityLayer1 = EditorGUILayout.Slider("Scattering Intensity", curWT.cloudsOverride.scatteringIntensityLayer1,0f,2f);
                                        curWT.cloudsOverride.multiScatteringALayer1 = EditorGUILayout.Slider("Multi Scattering A", curWT.cloudsOverride.multiScatteringALayer1,0f,1f);
                                        curWT.cloudsOverride.multiScatteringBLayer1 = EditorGUILayout.Slider("Multi Scattering B", curWT.cloudsOverride.multiScatteringBLayer1,0f,1f);
                                        curWT.cloudsOverride.multiScatteringCLayer1 = EditorGUILayout.Slider("Multi Scattering C", curWT.cloudsOverride.multiScatteringCLayer1,0f,1f);
                                        curWT.cloudsOverride.powderIntensityLayer1 = EditorGUILayout.Slider("Powder Intensity", curWT.cloudsOverride.powderIntensityLayer1,0f,1f);
                                        curWT.cloudsOverride.silverLiningSpreadLayer1 = EditorGUILayout.Slider("Silver Lining Spread", curWT.cloudsOverride.silverLiningSpreadLayer1,0f,1f);
                                        curWT.cloudsOverride.ligthAbsorbtionLayer1 = EditorGUILayout.Slider("Light Absorbtion", curWT.cloudsOverride.ligthAbsorbtionLayer1,0f,2f);
                                        curWT.cloudsOverride.ambientLightIntensity = EditorGUILayout.Slider("Ambient Light Intensity", curWT.cloudsOverride.ambientLightIntensity,0f,2f);
                                        GUILayout.Space(10);
                                        curWT.cloudsOverride.densityLayer1 = EditorGUILayout.Slider("Density", curWT.cloudsOverride.densityLayer1,0f,2f);
                                        curWT.cloudsOverride.densitySmoothnessLayer1 = EditorGUILayout.Slider("Density Smoothness", curWT.cloudsOverride.densitySmoothnessLayer1,0f,2f);
                                        curWT.cloudsOverride.baseErosionIntensityLayer1 = EditorGUILayout.Slider("Base Erosion Intensity", curWT.cloudsOverride.baseErosionIntensityLayer1,0f,1f);
                                        curWT.cloudsOverride.detailErosionIntensityLayer1 = EditorGUILayout.Slider("Detail Erosion Intensity", curWT.cloudsOverride.detailErosionIntensityLayer1,0f,1f);
                                        curWT.cloudsOverride.curlIntensityLayer1 = EditorGUILayout.Slider("Curl Intensity", curWT.cloudsOverride.curlIntensityLayer1,0f,1f);
                                        GUILayout.Space(10);
                                    }
                                    GUILayout.EndVertical();

                                    if(EnviroManager.instance == null || EnviroManager.instance.VolumetricClouds.settingsGlobal.dualLayer)
                                    {
                                        GUILayout.BeginVertical ("", boxStyleModified);
                                        curWT.cloudsOverride.showLayer2 = GUILayout.Toggle(curWT.cloudsOverride.showLayer2, "Layer 2", headerFoldout);
                                        if(curWT.cloudsOverride.showLayer2)
                                        {
                                            curWT.cloudsOverride.coverageLayer2 = EditorGUILayout.Slider("Coverage", curWT.cloudsOverride.coverageLayer2,-1f,1f);
                                            curWT.cloudsOverride.dilateCoverageLayer2 = EditorGUILayout.Slider("Dilate Coverage", curWT.cloudsOverride.dilateCoverageLayer2,0f,1f);
                                            curWT.cloudsOverride.dilateTypeLayer2 = EditorGUILayout.Slider("Dilate Type", curWT.cloudsOverride.dilateTypeLayer2,0f,1f);
                                            curWT.cloudsOverride.typeModifierLayer2 = EditorGUILayout.Slider("Type Modifier", curWT.cloudsOverride.typeModifierLayer2,0f,1f);
                                            curWT.cloudsOverride.anvilBiasLayer2 = EditorGUILayout.Slider("Anvil Bias", curWT.cloudsOverride.anvilBiasLayer2,0f,1f);
                                            GUILayout.Space(10);
                                            curWT.cloudsOverride.scatteringIntensityLayer2 = EditorGUILayout.Slider("Scattering Intensity", curWT.cloudsOverride.scatteringIntensityLayer2,0f,2f);
                                            curWT.cloudsOverride.multiScatteringALayer2 = EditorGUILayout.Slider("Multi Scattering A", curWT.cloudsOverride.multiScatteringALayer2,0f,1f);
                                            curWT.cloudsOverride.multiScatteringBLayer2 = EditorGUILayout.Slider("Multi Scattering B", curWT.cloudsOverride.multiScatteringBLayer2,0f,1f);
                                            curWT.cloudsOverride.multiScatteringCLayer2 = EditorGUILayout.Slider("Multi Scattering C", curWT.cloudsOverride.multiScatteringCLayer2,0f,1f);
                                            curWT.cloudsOverride.powderIntensityLayer2 = EditorGUILayout.Slider("Powder Intensity", curWT.cloudsOverride.powderIntensityLayer2,0f,1f);
                                            curWT.cloudsOverride.silverLiningSpreadLayer2 = EditorGUILayout.Slider("Silver Lining Spread", curWT.cloudsOverride.silverLiningSpreadLayer2,0f,1f);
                                            curWT.cloudsOverride.ligthAbsorbtionLayer2 = EditorGUILayout.Slider("Light Absorbtion", curWT.cloudsOverride.ligthAbsorbtionLayer2,0f,2f);
                                            curWT.cloudsOverride.ambientLightIntensity = EditorGUILayout.Slider("Ambient Light Intensity", curWT.cloudsOverride.ambientLightIntensity,0f,2f);
                                            GUILayout.Space(10);
                                            curWT.cloudsOverride.densityLayer2 = EditorGUILayout.Slider("Density", curWT.cloudsOverride.densityLayer2,0f,2f);
                                            curWT.cloudsOverride.densitySmoothnessLayer2 = EditorGUILayout.Slider("Density Smoothness", curWT.cloudsOverride.densitySmoothnessLayer2,0f,2f);
                                            curWT.cloudsOverride.baseErosionIntensityLayer2 = EditorGUILayout.Slider("Base Erosion Intensity", curWT.cloudsOverride.baseErosionIntensityLayer2,0f,1f);
                                            curWT.cloudsOverride.detailErosionIntensityLayer2 = EditorGUILayout.Slider("Detail Erosion Intensity", curWT.cloudsOverride.detailErosionIntensityLayer2,0f,1f);
                                            curWT.cloudsOverride.curlIntensityLayer2 = EditorGUILayout.Slider("Curl Intensity", curWT.cloudsOverride.curlIntensityLayer2,0f,1f);
                                        }
                                        GUILayout.EndVertical();
                                    }                    
                                }
                                GUILayout.EndVertical(); 
                                }

                                if(EnviroManager.instance == null || EnviroManager.instance.FlatClouds != null)
                                {
                                //Flat Clouds
                                GUILayout.BeginVertical ("", boxStyleModified);
                                curWT.showFlatCloudControls = GUILayout.Toggle(curWT.showFlatCloudControls, "Flat Clouds", headerFoldout);
                                
                                if(curWT.showFlatCloudControls)
                                { 
                                    GUILayout.Space(5);
                                    EditorGUILayout.LabelField("Cirrus Clouds", headerStyle);
                                    curWT.flatCloudsOverride.cirrusCloudsCoverage = EditorGUILayout.Slider("Cirrus Clouds Coverage", curWT.flatCloudsOverride.cirrusCloudsCoverage,0f,1f);
                                    curWT.flatCloudsOverride.cirrusCloudsAlpha = EditorGUILayout.Slider("Cirrus Clouds Alpha", curWT.flatCloudsOverride.cirrusCloudsAlpha,0f,1f);
                                    curWT.flatCloudsOverride.cirrusCloudsColorPower = EditorGUILayout.Slider("Cirrus Clouds Color", curWT.flatCloudsOverride.cirrusCloudsColorPower,0f,2f);
                                    GUILayout.Space(10);
                                    EditorGUILayout.LabelField("Flat Clouds", headerStyle);
                                    curWT.flatCloudsOverride.flatCloudsCoverage = EditorGUILayout.Slider("Flat Clouds Coverage", curWT.flatCloudsOverride.flatCloudsCoverage,0f,2f);
                                    curWT.flatCloudsOverride.flatCloudsLightIntensity = EditorGUILayout.Slider("Flat Clouds Light Intensity", curWT.flatCloudsOverride.flatCloudsLightIntensity,0f,2f);
                                    curWT.flatCloudsOverride.flatCloudsAmbientIntensity = EditorGUILayout.Slider("Flat Clouds Ambient Intensity", curWT.flatCloudsOverride.flatCloudsAmbientIntensity,0f,2f);
                                    curWT.flatCloudsOverride.flatCloudsAbsorbtion = EditorGUILayout.Slider("Flat Clouds Light Absorbtion", curWT.flatCloudsOverride.flatCloudsAbsorbtion,0f,2f);
                                }
                                GUILayout.EndVertical();
                                }

                                if(EnviroManager.instance == null || EnviroManager.instance.Fog != null)
                                {
                                //Fog
                                GUILayout.BeginVertical ("", boxStyleModified);
                                curWT.showFogControls = GUILayout.Toggle(curWT.showFogControls, "Fog", headerFoldout);
                                
                                if(curWT.showFogControls)
                                { 
                                    GUILayout.Space(5);
                                    EditorGUILayout.LabelField("Layer 1", headerStyle);
                                    curWT.fogOverride.fogDensity = EditorGUILayout.Slider("Fog Density 1", curWT.fogOverride.fogDensity,0f,1f);
                                    curWT.fogOverride.fogHeightFalloff = EditorGUILayout.Slider("Fog Height Falloff 1", curWT.fogOverride.fogHeightFalloff,0f,0.05f);
                                    curWT.fogOverride.fogHeight = EditorGUILayout.FloatField("Fog Height 1 ", curWT.fogOverride.fogHeight);
                                    GUILayout.Space(10);
                                    EditorGUILayout.LabelField("Layer 2", headerStyle);
                                    curWT.fogOverride.fogDensity2 = EditorGUILayout.Slider("Fog Density 2", curWT.fogOverride.fogDensity2,0f,1f);
                                    curWT.fogOverride.fogHeightFalloff2 = EditorGUILayout.Slider("Fog Height Falloff 2", curWT.fogOverride.fogHeightFalloff2,0f,0.05f);
                                    curWT.fogOverride.fogHeight2 = EditorGUILayout.FloatField("Fog Height 2", curWT.fogOverride.fogHeight2);
                                    GUILayout.Space(10);
                                    EditorGUILayout.LabelField("Color", headerStyle);
                                    curWT.fogOverride.fogColorBlend = EditorGUILayout.Slider("Fog Sky-Color Blending", curWT.fogOverride.fogColorBlend,0f,1.0f);
                                    curWT.fogOverride.fogColorMod = EditorGUILayout.ColorField("Fog Color Tint", curWT.fogOverride.fogColorMod);
                                    GUILayout.Space(10);
                           
                            #if !ENVIRO_HDRP
                                    EditorGUILayout.LabelField("Unity Fog", headerStyle);
                                    
                                    if(EnviroManager.instance != null && EnviroManager.instance.Fog.Settings.unityFogMode == FogMode.Linear)
                                    {
                                        curWT.fogOverride.unityFogStartDistance = EditorGUILayout.FloatField("Unity Fog Start Distance", curWT.fogOverride.unityFogStartDistance);
                                        curWT.fogOverride.unityFogEndDistance = EditorGUILayout.FloatField("Unity Fog End Distance", curWT.fogOverride.unityFogEndDistance);
                                    } 
                                    else
                                    {
                                        curWT.fogOverride.unityFogDensity = EditorGUILayout.FloatField("Unity Fog Density", curWT.fogOverride.unityFogDensity);
                                    }
                                   
                                    GUILayout.Space(10);
                                    EditorGUILayout.LabelField("Volumetrics", headerStyle);
                                    curWT.fogOverride.scattering = EditorGUILayout.Slider("Scattering Intensity", curWT.fogOverride.scattering,0f,2.0f);
                                    curWT.fogOverride.extinction = EditorGUILayout.Slider("Extinction Intensity", curWT.fogOverride.extinction,0f,1.0f);
                                    curWT.fogOverride.anistropy = EditorGUILayout.Slider("Anistropy", curWT.fogOverride.anistropy,0f,1.0f);
                            #else
                                    EditorGUILayout.LabelField("HDRP Fog", headerStyle);
                                    curWT.fogOverride.fogAttenuationDistance = EditorGUILayout.Slider("Attenuation Distance", curWT.fogOverride.fogAttenuationDistance,0f,400f);
                                    curWT.fogOverride.baseHeight = EditorGUILayout.FloatField("Base Height", curWT.fogOverride.baseHeight);
                                    curWT.fogOverride.maxHeight = EditorGUILayout.FloatField("Max Height", curWT.fogOverride.maxHeight);
                                    GUILayout.Space(10);
                                    EditorGUILayout.LabelField("HDRP Volumetrics", headerStyle);
                                    curWT.fogOverride.ambientDimmer = EditorGUILayout.Slider("Ambient Dimmer", curWT.fogOverride.ambientDimmer,0f,1f);
                                    curWT.fogOverride.directLightMultiplier = EditorGUILayout.Slider("Direct Light Multiplier", curWT.fogOverride.directLightMultiplier,0f,16f);
                                    curWT.fogOverride.directLightShadowdimmer = EditorGUILayout.Slider("Direct Light Shadow gimmer", curWT.fogOverride.directLightShadowdimmer,0f,1f);
                            #endif 
                                }
                                GUILayout.EndVertical();
                                }

                                if(EnviroManager.instance == null || EnviroManager.instance.Effects != null)
                                {
                                //Effects
                                GUILayout.BeginVertical ("", boxStyleModified);
                                curWT.showEffectControls = GUILayout.Toggle(curWT.showEffectControls, "Effects", headerFoldout);
                                
                                if(curWT.showEffectControls)
                                {  
                                    GUILayout.Space(10);
                                    if (GUILayout.Button ("Add")) 
                                    {
                                        curWT.effectsOverride.effectsOverride.Add (new EnviroEffectsOverrideType());
                                        EditorUtility.SetDirty(curWT);
                                    } 
                
                                    GUILayout.Space(10);
                                    
                                    for (int a = 0; a < curWT.effectsOverride.effectsOverride.Count; a++) 
                                    {      
                                        GUILayout.BeginVertical ("", boxStyleModified);
                                        EditorGUILayout.BeginHorizontal();
                                        curWT.effectsOverride.effectsOverride[a].showEditor = GUILayout.Toggle(curWT.effectsOverride.effectsOverride[a].showEditor, curWT.effectsOverride.effectsOverride[a].name, headerFoldout);
                                        GUILayout.FlexibleSpace();
                                        if(GUILayout.Button("x", EditorStyles.miniButtonRight,GUILayout.Width(18), GUILayout.Height(18)))
                                        { 
                                            curWT.effectsOverride.effectsOverride.Remove (curWT.effectsOverride.effectsOverride[a]);
                                            EditorUtility.SetDirty(curWT);
                                            return;
                                        }           
                                        EditorGUILayout.EndHorizontal();

                                        if(curWT.effectsOverride.effectsOverride[a].showEditor)
                                        {
                                            curWT.effectsOverride.effectsOverride[a].name = EditorGUILayout.TextField ("Effect Name", curWT.effectsOverride.effectsOverride[a].name);
                                            curWT.effectsOverride.effectsOverride[a].emission = EditorGUILayout.Slider ("Emission", curWT.effectsOverride.effectsOverride[a].emission,0f,1f);                                      
                                        } 
                                        GUILayout.EndVertical ();
                                    }
                                }
                                GUILayout.EndVertical();
                                }
                                if(EnviroManager.instance == null || EnviroManager.instance.Aurora != null)
                                {
                                //Aurora
                                GUILayout.BeginVertical ("", boxStyleModified);
                                curWT.showAuroraControls = GUILayout.Toggle(curWT.showAuroraControls, "Aurora", headerFoldout);
                                
                                if(curWT.showAuroraControls) 
                                { 
                                    GUILayout.Space(5);
                                    curWT.auroraOverride.auroraIntensity = EditorGUILayout.Slider("Aurora Intensity Modifier", curWT.auroraOverride.auroraIntensity,0f,1f);
                                } 
                                GUILayout.EndVertical();
                                }
                                if(EnviroManager.instance == null || EnviroManager.instance.Environment != null)
                                {
                                //Environment
                                GUILayout.BeginVertical ("", boxStyleModified);
                                curWT.showEnvironmentControls = GUILayout.Toggle(curWT.showEnvironmentControls, "Environment", headerFoldout);
                                
                                if(EnviroManager.instance == null || curWT.showEnvironmentControls) 
                                { 
                                    GUILayout.Space(5);
                                    curWT.environmentOverride.temperatureWeatherMod = EditorGUILayout.Slider("Temperature Modifier", curWT.environmentOverride.temperatureWeatherMod,-20f,20f);
                                    GUILayout.Space(5);
                                    curWT.environmentOverride.wetnessTarget = EditorGUILayout.Slider("Wetness Target", curWT.environmentOverride.wetnessTarget,0f,1f);
                                    curWT.environmentOverride.snowTarget = EditorGUILayout.Slider("Snow Target", curWT.environmentOverride.snowTarget,0f,1f);
                                    GUILayout.Space(10);
                                    curWT.environmentOverride.windDirectionX = EditorGUILayout.Slider("Wind Direction X", curWT.environmentOverride.windDirectionX,-1f,1f);
                                    curWT.environmentOverride.windDirectionY = EditorGUILayout.Slider("Wind Direction Y", curWT.environmentOverride.windDirectionY,-1f,1f);
                                    GUILayout.Space(5);
                                    curWT.environmentOverride.windSpeed = EditorGUILayout.Slider("Wind Speed", curWT.environmentOverride.windSpeed,0f,1f);
                                    curWT.environmentOverride.windTurbulence = EditorGUILayout.Slider("Wind Turbulence", curWT.environmentOverride.windTurbulence,0f,1f);                                } 
                                    GUILayout.EndVertical();
                                }


                                if(EnviroManager.instance == null || EnviroManager.instance.Lightning != null)
                                {
                                //Lightning
                                GUILayout.BeginVertical ("", boxStyleModified);
                                curWT.showLightningControls = GUILayout.Toggle(curWT.showLightningControls, "Lightning", headerFoldout);
                                
                                if(curWT.showLightningControls) 
                                {
                                    GUILayout.Space(5);
                                    curWT.lightningOverride.lightningStorm = EditorGUILayout.Toggle("Lightning Storm", curWT.lightningOverride.lightningStorm);
                                    curWT.lightningOverride.randomLightningDelay = EditorGUILayout.Slider("Lightning Delay", curWT.lightningOverride.randomLightningDelay,1f,60f);
                                }
                                GUILayout.EndVertical();
                                }


                                if(EnviroManager.instance == null || EnviroManager.instance.Audio != null)
                                {
                                //Audio
                                GUILayout.BeginVertical ("", boxStyleModified);
                                curWT.showAudioControls = GUILayout.Toggle(curWT.showAudioControls, "Audio", headerFoldout);
                                
                                if(curWT.showAudioControls)
                                {        
                                    GUILayout.Space(5);
                                    //Ambient SFX
                                    GUILayout.BeginVertical ("", boxStyleModified);
                                    curWT.showAmbientAudioControls = GUILayout.Toggle(curWT.showAmbientAudioControls, "Ambient", headerFoldout);         
                                    if(curWT.showAmbientAudioControls)
                                    {    
                                        GUILayout.Space(10);
                                        if (GUILayout.Button ("Add")) 
                                        {
                                            curWT.audioOverride.ambientOverride.Add (new EnviroAudioOverrideType());
                                             EditorUtility.SetDirty(curWT);
                                        }
                    
                                        GUILayout.Space(10);
                                        
                                        for (int a = 0; a < curWT.audioOverride.ambientOverride.Count; a++) 
                                        {      
                                            GUILayout.BeginVertical ("", boxStyleModified);
                                            EditorGUILayout.BeginHorizontal();
                                            curWT.audioOverride.ambientOverride[a].showEditor = GUILayout.Toggle(curWT.audioOverride.ambientOverride[a].showEditor, curWT.audioOverride.ambientOverride[a].name, headerFoldout);
                                            GUILayout.FlexibleSpace();
                                            if(GUILayout.Button("x", EditorStyles.miniButtonRight,GUILayout.Width(18), GUILayout.Height(18)))
                                            { 
                                                curWT.audioOverride.ambientOverride.Remove (curWT.audioOverride.ambientOverride[a]);
                                                 EditorUtility.SetDirty(curWT);
                                                return;
                                            }           
                                            EditorGUILayout.EndHorizontal();

                                            if(curWT.audioOverride.ambientOverride[a].showEditor)
                                            {
                                                curWT.audioOverride.ambientOverride[a].name = EditorGUILayout.TextField ("Audio Name", curWT.audioOverride.ambientOverride[a].name);
                                                curWT.audioOverride.ambientOverride[a].volume = EditorGUILayout.Slider ("Volume", curWT.audioOverride.ambientOverride[a].volume,0f,1f);                                      
                                            } 
                                            GUILayout.EndVertical ();
                                        }
                                    }
                                    GUILayout.EndVertical ();

                                    //Weather SFX
                                    GUILayout.BeginVertical ("", boxStyleModified);
                                    curWT.showWeatherAudioControls = GUILayout.Toggle(curWT.showWeatherAudioControls, "Weather", headerFoldout);         
                                    if(curWT.showWeatherAudioControls)
                                    {     
                                        GUILayout.Space(10);
                                        if (GUILayout.Button ("Add")) 
                                        {
                                            curWT.audioOverride.weatherOverride.Add (new EnviroAudioOverrideType());
                                            EditorUtility.SetDirty(curWT);
                                        }
                    
                                        GUILayout.Space(10);
                                        
                                        for (int a = 0; a < curWT.audioOverride.weatherOverride.Count; a++) 
                                        {      
                                            GUILayout.BeginVertical ("", boxStyleModified);
                                            EditorGUILayout.BeginHorizontal();
                                            curWT.audioOverride.weatherOverride[a].showEditor = GUILayout.Toggle(curWT.audioOverride.weatherOverride[a].showEditor, curWT.audioOverride.weatherOverride[a].name, headerFoldout);
                                            GUILayout.FlexibleSpace();
                                            if(GUILayout.Button("x", EditorStyles.miniButtonRight,GUILayout.Width(18), GUILayout.Height(18)))
                                            { 
                                                curWT.audioOverride.weatherOverride.Remove (curWT.audioOverride.weatherOverride[a]);
                                                EditorUtility.SetDirty(curWT);
                                                return;
                                            }           
                                            EditorGUILayout.EndHorizontal();

                                            if(curWT.audioOverride.weatherOverride[a].showEditor)
                                            {
                                                curWT.audioOverride.weatherOverride[a].name = EditorGUILayout.TextField ("Audio Name", curWT.audioOverride.weatherOverride[a].name);
                                                curWT.audioOverride.weatherOverride[a].volume = EditorGUILayout.Slider ("Volume", curWT.audioOverride.weatherOverride[a].volume,0f,1f);                                      
                                            } 
                                            GUILayout.EndVertical ();
                                        }
                                    }
                                    GUILayout.EndVertical ();
                                }
                                GUILayout.EndVertical();
                                }
                                //END
                            }
                            GUILayout.EndVertical ();
                            GUILayout.Space(2.5f);
                    }
                }
                GUILayout.EndVertical ();


                /// Transition Foldout
                GUI.backgroundColor = categoryModuleColor;
                GUILayout.BeginVertical("",boxStyleModified);
                GUI.backgroundColor = Color.white;
                myTarget.showTransitionControls = GUILayout.Toggle(myTarget.showTransitionControls, "Transition Controls", headerFoldout);               
                if(myTarget.showTransitionControls)
                {
                    GUILayout.Space(5);
                    EditorGUILayout.PropertyField(cloudsTransitionSpeed);
                    EditorGUILayout.PropertyField(fogTransitionSpeed);
                    EditorGUILayout.PropertyField(skyTransitionSpeed);              
                    EditorGUILayout.PropertyField(lightingTransitionSpeed);
                    EditorGUILayout.PropertyField(effectsTransitionSpeed);
                    EditorGUILayout.PropertyField(auroraTransitionSpeed); 
                    EditorGUILayout.PropertyField(environmentTransitionSpeed);           
                    EditorGUILayout.PropertyField(audioTransitionSpeed);
                
                }
                GUILayout.EndVertical ();

                ///Zone Foldout
                if(EnviroManager.instance != null)
                {
                    GUI.backgroundColor = categoryModuleColor;
                    GUILayout.BeginVertical("",boxStyleModified);
                    GUI.backgroundColor = Color.white;
                    myTarget.showZoneControls = GUILayout.Toggle(myTarget.showZoneControls, "Zone Controls", headerFoldout);               
                    
                    if(myTarget.showZoneControls)
                    {
                        GUILayout.Space(5);
                        EditorGUILayout.PropertyField(globalAutoWeatherChange);
                        GUILayout.Space(5);
                        EnviroManager.instance.defaultZone = (EnviroZone)EditorGUILayout.ObjectField ("Default Zone", EnviroManager.instance.defaultZone, typeof(EnviroZone), true);
                        EnviroManager.instance.currentZone = (EnviroZone)EditorGUILayout.ObjectField ("Current Zone", EnviroManager.instance.currentZone, typeof(EnviroZone), true);
                        GUILayout.Space(5);
                        GUILayout.Label("Zones List" , headerStyle);
                        GUILayout.Space(5);
                        for (int i = 0; i < EnviroManager.instance.zones.Count; i++) 
                        {  
                            if(EnviroManager.instance.zones[i] != null)
                            {
                                GUI.backgroundColor = EnviroManager.instance.zones[i].zoneGizmoColor;
                                GUILayout.BeginVertical(EnviroManager.instance.zones[i].gameObject.name,boxStyleModified);
                                GUI.backgroundColor = Color.white;
                                EditorGUILayout.BeginHorizontal();

                                GUILayout.FlexibleSpace();
                                if(GUILayout.Button("Show", EditorStyles.miniButtonRight,GUILayout.Width(100), GUILayout.Height(18)))
                                {
                                UnityEditor.Selection.activeObject = EnviroManager.instance.zones[i];
                                } 
                                EditorGUILayout.EndHorizontal();
                
                                EditorGUILayout.BeginHorizontal();
                                if(EnviroManager.instance.zones[i].currentWeatherType != null)
                                    GUILayout.Label("Current Weather: " + EnviroManager.instance.zones[i].currentWeatherType.name , wrapStyle);
                                else
                                    GUILayout.Label("Current Weather: Not Set" , wrapStyle);

                                if(EnviroManager.instance.zones[i].nextWeatherType != null)
                                    GUILayout.Label("Next Weather: " + EnviroManager.instance.zones[i].nextWeatherType.name, wrapStyle);
                                else
                                    GUILayout.Label("Next Weather: Not Set" , wrapStyle);        
                                EditorGUILayout.EndHorizontal();

                                GUILayout.EndVertical ();
                            }
                        }
                    }
                    GUILayout.EndVertical ();
                }
               
                /// Save Load
                GUI.backgroundColor = categoryModuleColor;
                GUILayout.BeginVertical("",boxStyleModified);
                GUI.backgroundColor = Color.white;
                myTarget.showSaveLoad = GUILayout.Toggle(myTarget.showSaveLoad, "Save/Load", headerFoldout);
                
                if(myTarget.showSaveLoad)
                {
                    EditorGUILayout.PropertyField(preset);
                    GUILayout.BeginHorizontal("",wrapStyle);

                    if(myTarget.preset != null)
                    {
                        if(GUILayout.Button("Load"))
                        {
                            myTarget.LoadModuleValues();
                        }
                        if(GUILayout.Button("Save"))
                        {
                            myTarget.SaveModuleValues(myTarget.preset);
                        }
                    }
                    if(GUILayout.Button("Save As New"))
                    {
                        myTarget.SaveModuleValues();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                /// Save Load End

                ApplyChanges ();
            }
            GUILayout.EndVertical();

            if(myTarget.showModuleInspector)
             GUILayout.Space(20);
        }
    }
}
