Shader "Hidden/EnviroCloudsRaymarchHDRP"
{
    Properties
    { 

    } 
    SubShader 
    {
        //Tags { "RenderType"="Opaque" }

       	Pass 
        {
            Cull Off ZWrite Off ZTest Always
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ ENVIRO_DEPTH_BLENDING
            #pragma multi_compile _ ENVIRO_DUAL_LAYER
            #pragma multi_compile _ ENVIRO_CLOUD_SHADOWS
            #pragma multi_compile __ ENVIROHDRP 
              
            #if defined (ENVIROHDRP)
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/AtmosphericScattering/AtmosphericScattering.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Sky/SkyUtils.hlsl"
            #include "../Includes/VolumetricCloudsInclude.cginc"
            #include "../Includes/VolumetricCloudsTexHDRPInclude.cginc"

		
			int _Frame;
            uniform float _BlueNoiseIntensity;
            uniform float4 _DepthHandleScale;

            struct appdata
            {
                uint vertexID : SV_VertexID;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.position = GetFullScreenTriangleVertexPosition(v.vertexID);
                o.uv = GetFullScreenTriangleTexCoord(v.vertexID);

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);  

               // uint2 positionSS = i.uv * _ScreenSize.xy;

                float4 cameraRay =  float4(i.uv * 2.0 - 1.0, 1.0, 1.0);
                float3 EyePosition = _CameraPosition;
                float3 ray = 0; 
 
               	//if (unity_StereoEyeIndex == 0)
	            //{
                    cameraRay = mul(_InverseProjection, cameraRay);
                    cameraRay = cameraRay / cameraRay.w;
                    ray = normalize(mul((float3x3)_InverseRotation, cameraRay.xyz));
                //}
                //else  
                // {
                //     cameraRay = mul(_InverseProjectionRight, cameraRay);
                //    cameraRay = cameraRay / cameraRay.w; 
                //    ray = normalize(mul((float3x3)_InverseRotationRight, cameraRay.xyz));
                //}
  
                float rayLength = length(ray);
                
                float sceneDepth = LOAD_TEXTURE2D_X(_DownsampledDepth, i.uv * _DepthHandleScale.zw);  
                //float sceneDepth = LOAD_TEXTURE2D_X_LOD(_CameraDepthTexture, i.uv * _ScreenSize.xy, 0);
                float raymarchEnd = GetRaymarchEndFromSceneDepth(Linear01Depth(sceneDepth, _ZBufferParams), 1000000); //* rayLenght

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

                return float4(max(bIntensity,0.0f),max(bDistance,1.0f),clamp(shadow,0.0,0.25),max(bAlpha,0.0f)); 
            }
        #else
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = v.vertex;
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;
                return col;
            }
        #endif
            ENDHLSL
        }
    }
}
