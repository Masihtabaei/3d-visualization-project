Shader "Hidden/EnviroCloudsRaymarch"
{
    Properties
    {
        //_MainTex ("Texture", any) = "white" {}
    }
    SubShader 
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass  
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ ENVIRO_DEPTH_BLENDING
            #pragma multi_compile_local _ ENVIRO_DUAL_LAYER
            #pragma multi_compile_local _ ENVIRO_CLOUD_SHADOWS
            #pragma multi_compile _ ENVIROURP
            #include "UnityCG.cginc"
            #include "../Includes/VolumetricCloudsInclude.cginc"
            #include "../Includes/VolumetricCloudsTexInclude.cginc"
 
            int _Frame;
            uniform float _BlueNoiseIntensity;
            float4 _CameraDepthTexture_TexelSize;
     
            struct v2f
            {
                float4 position : SV_POSITION;
		        float2 uv : TEXCOORD0;
                float2 uv00 : TEXCOORD1;
                float2 uv10 : TEXCOORD2;
                float2 uv01 : TEXCOORD3;
                float2 uv11 : TEXCOORD4;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            struct appdata 
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert (appdata_img v)
            {
                v2f o; 
                 
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);   
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                //o.eyeIndex = unity_StereoEyeIndex;

                #if defined(ENVIROURP)
		        o.position = float4(v.vertex.xyz,1.0);
		        #if UNITY_UV_STARTS_AT_TOP
                o.position.y *= -1;
                #endif
                #else
		        o.position = UnityObjectToClipPos(v.vertex);
                #endif   

                o.uv = v.texcoord;
                o.uv00 = v.texcoord - 0.5 * _CameraDepthTexture_TexelSize.xy;
                o.uv10 = o.uv00 + float2(_CameraDepthTexture_TexelSize.x, 0.0);
                o.uv01 = o.uv00 + float2(0.0, _CameraDepthTexture_TexelSize.y);
                o.uv11 = o.uv00 + _CameraDepthTexture_TexelSize.xy;
                return o;
            } 

             
            float4 frag (v2f i) : SV_Target
            { 
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                float4 cameraRay =  float4(i.uv * 2.0 - 1.0, 1.0, 1.0);
                float3 EyePosition = _CameraPosition;
                float3 ray = 0; 
 
               	if (unity_StereoEyeIndex == 0)
	            {
                    cameraRay = mul(_InverseProjection, cameraRay);
                    cameraRay = cameraRay / cameraRay.w;
                    ray = normalize(mul((float3x3)_InverseRotation, cameraRay.xyz));
                }
                else  
                {
                    cameraRay = mul(_InverseProjectionRight, cameraRay);
                    cameraRay = cameraRay / cameraRay.w; 
                    ray = normalize(mul((float3x3)_InverseRotationRight, cameraRay.xyz));
                }
  
                float rayLength = length(ray);
             
                /*float4 highResDepth = LinearEyeDepth(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraDepthTexture, UnityStereoTransformScreenSpaceTex(i.uv))).xxxx;

                float4 lowResDepth;
    
                lowResDepth[0] = LinearEyeDepth(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_DownsampledDepth, UnityStereoTransformScreenSpaceTex(i.uv00)));
                lowResDepth[1] = LinearEyeDepth(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_DownsampledDepth, UnityStereoTransformScreenSpaceTex(i.uv10)));
                lowResDepth[2] = LinearEyeDepth(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_DownsampledDepth, UnityStereoTransformScreenSpaceTex(i.uv01)));
                lowResDepth[3] = LinearEyeDepth(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_DownsampledDepth, UnityStereoTransformScreenSpaceTex(i.uv11)));
    
                float4 depthDiff = abs(lowResDepth - highResDepth);
    
                float accumDiff = dot(depthDiff, float4(1, 1, 1, 1));
    
               // [branch]
                //if (accumDiff < threshold) // small error, not an edge -> use bilinear filter
                //{
                //    return SampleTexture(loColor,linearSampler,input.uv);
                //}
    
                // find nearest sample
                float minDepthDiff = depthDiff[0];
                float2 nearestUv = i.uv00;
    
                if (depthDiff[1] < minDepthDiff)
                {
                    nearestUv = i.uv10;
                    minDepthDiff = depthDiff[1];
                }
    
                if (depthDiff[2] < minDepthDiff)
                {
                    nearestUv = i.uv01;
                    minDepthDiff = depthDiff[2];
                }
    
                if (depthDiff[3] < minDepthDiff)
                {
                    nearestUv = i.uv11;
                    minDepthDiff = depthDiff[3];
                }

                float sceneDepth = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraDepthTexture, UnityStereoTransformScreenSpaceTex(nearestUv));
*/
                float sceneDepth = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_DownsampledDepth, UnityStereoTransformScreenSpaceTex(i.uv));
                float3 cameraDirection = -1 * transpose(_InverseRotation)[2].xyz;
                float fwdFactor = dot(ray, cameraDirection); 

                float raymarchEnd = GetRaymarchEndFromSceneDepth(Linear01Depth(sceneDepth) / fwdFactor, 1000000); //* rayLenght
 
                float offset = tex2D(_BlueNoise, squareUV(i.uv + _Randomness.xy)).x * _BlueNoiseIntensity; 


                float3 pCent = float3(EyePosition.x, -_CloudsParameter.w, EyePosition.z);

 
                float bIntensity, bDistance, bAlpha, shadow = 0.0f;
                float3 wpos;

#if ENVIRO_DUAL_LAYER

                //Clouds Layer 1
                RaymarchParameters parametersLayer1;
                InitRaymarchParametersLayer1(parametersLayer1);
                float2 hitDistanceLayer1 = ResolveRay(EyePosition,ray,pCent, raymarchEnd, parametersLayer1);
                float3 layer1Final = Raymarch(EyePosition,ray,hitDistanceLayer1.xy,pCent,parametersLayer1,offset,0);
#if ENVIRO_CLOUD_SHADOWS
                //Clouds Shadows Layer1
                wpos = CalculateWorldPosition(i.uv,sceneDepth);
                wpos -= _WorldOffset;
              
                float shadowsLayer1 = RaymarchShadows(EyePosition,wpos,ray,pCent,parametersLayer1,offset,sceneDepth,0);
#endif 
                //Clouds Layer 2
                RaymarchParameters parametersLayer2;
                InitRaymarchParametersLayer2(parametersLayer2);
                float2 hitDistanceLayer2 = ResolveRay(EyePosition,ray,pCent,raymarchEnd, parametersLayer2);    
                float3 layer2Final = Raymarch(EyePosition,ray,hitDistanceLayer2,pCent,parametersLayer2,offset,1);
#if ENVIRO_CLOUD_SHADOWS 
                //Clouds Shadows Layer2
               
                float shadowsLayer2 = RaymarchShadows(EyePosition,wpos,ray,pCent,parametersLayer2,offset,sceneDepth,1);
#endif  
                if (EyePosition.y < _CloudsParameter2.x) 
                { 
                    bIntensity = layer2Final.x * (1-layer1Final.z) + layer1Final.x;
                    bDistance = layer2Final.y * (1-layer1Final.z) + layer1Final.y;
                }
                else
                { 
                    //if(layer2Final.b >= 1.0)
                     //  return float4(layer2Final.r,layer2Final.g,1.0,layer2Final.b); 

                    bIntensity = layer1Final.x * (1-layer2Final.z) + layer2Final.x;
                    bDistance = layer1Final.y * (1-layer2Final.z) + layer2Final.y;
                }
                bAlpha = saturate(layer1Final.b + layer2Final.b);
#if ENVIRO_CLOUD_SHADOWS
                //Combine cloud shadows.
                shadow = shadowsLayer1 + shadowsLayer2;
#endif

#else
                    RaymarchParameters parametersLayer1;
                    InitRaymarchParametersLayer1(parametersLayer1);
                    float2 hitDistanceLayer1 = ResolveRay(EyePosition,ray,pCent, raymarchEnd, parametersLayer1);
                    float3 layer1Final = Raymarch(EyePosition,ray,hitDistanceLayer1,pCent,parametersLayer1,offset,0);
#if ENVIRO_CLOUD_SHADOWS
                    //Clouds Shadows
                    wpos = CalculateWorldPosition(i.uv,sceneDepth);
                    wpos -= _WorldOffset;
                    shadow = RaymarchShadows(EyePosition,wpos,ray,pCent,parametersLayer1,offset,sceneDepth,0);
#endif
                    bIntensity = layer1Final.r;
                    bDistance = layer1Final.g;
                    bAlpha = layer1Final.b;
#endif
   
                return float4(max(bIntensity,0.0),max(bDistance,1.0f),clamp(shadow,0.0,0.25),saturate(bAlpha)); 
            }
            ENDCG
        }
    }
}