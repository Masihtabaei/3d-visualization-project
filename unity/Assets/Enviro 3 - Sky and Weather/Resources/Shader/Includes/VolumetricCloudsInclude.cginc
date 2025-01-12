    	
       
            uniform Texture3D _Noise;
            SamplerState sampler_Noise;
            uniform Texture3D _DetailNoise; 
            SamplerState sampler_DetailNoise;
            uniform Texture2D _WeatherMap;
            SamplerState sampler_WeatherMap;
            uniform Texture2D _CurlNoise;
            SamplerState sampler_CurlNoise;
            uniform sampler2D _BlueNoise;
            
            float4 _BlueNoise_TexelSize;
            float3 _WorldOffset;
             
            uniform float4x4 _InverseProjection;
            uniform float4x4 _InverseRotation;
            uniform float4x4 _InverseProjectionRight;
            uniform float4x4 _InverseRotationRight;

            float4x4 _LeftWorldFromView;
		    float4x4 _RightWorldFromView;
		    float4x4 _LeftViewFromScreen;
		    float4x4 _RightViewFromScreen;
            
            uniform float4 _CloudsParameter; 
            uniform float4 _CloudsParameter2; 
            
            uniform float4 _Steps;
            uniform float4 _CloudsLighting;
            uniform float4 _CloudsLighting2;
            uniform float4 _CloudsLightingExtended; 
            uniform float4 _CloudsLightingExtended2;

            uniform float4 _CloudsMultiScattering;
            uniform float4 _CloudsMultiScattering2;

            uniform float4 _CloudsErosionIntensity; //x = Base, y = Detail
            uniform float4 _CloudsNoiseSettings; //x = Base, y = Detail
    
            uniform float4 _CloudDensityScale;
            uniform float4 _CloudsCoverageSettings; //x = _GlobalCoverage, y = Bottom Coverage Mod, z = Top coverage mod, w = Clouds Up Morph Intensity
            uniform float _GlobalCoverage;
            uniform float4 _CloudsAnimation;
            uniform float4 _CloudsWindDirection;
            
            uniform float3 _LightDir;
            uniform float _stepsInDepth;
            uniform float _LODDistance;
            uniform float3 _CameraPosition;
            uniform float4 _Resolution;
            uniform float4 _Randomness;
            uniform float _EnviroDepthTest; 
            ////              
            const float env_inf = 1e10;

            struct RaymarchParameters 
            {
                //Lighting
                float scatteringCoef;
                float hgPhase;
                float silverLiningIntensity;
                float silverLiningSpread;
                float powderTerm;
                float attenuation;
                float lightStep;
                float lightAbsorb;
                float multiScatteringA;
                float multiScatteringB;
                float multiScatteringC;

                //Height
                float4 cloudsParameter;

                //Density
                float density;
                float densitySmoothness;

                //Erosion
                float baseErosion;
                float detailErosion;

                int minSteps, maxSteps;
 
                float baseNoiseUV;
                float detailNoiseUV;

                float baseErosionIntensity;
                float detailErosionIntensity;

                float anvilBias;
            };
/*
            struct Lightning
            {
                float3 pos;
                float range;
                float intensity;
            };

            StructuredBuffer<Lightning> _Lightnings;

            float _LightningCount;

            float anisotropy(float costheta)
            {
                float g = 0.5f;
                float gsq = g*g;
                float denom = 1 + gsq - 2.0 * g * costheta;
                denom = denom * denom * denom;
                denom = sqrt(max(0, denom));
                return (1 - gsq) / denom;
            }

            float Attenuation(float distNorm)
            {
                return 1.0 / (1.0 + 25.0 * distNorm);
            }

            float Lightnings(float3 pos)
            {
                float intensity = 0.0f;
                
                for (int i = 0; i < _LightningCount; i++)
                {
                    float3 posToLight = _Lightnings[i].pos - pos;
                    //float3 posToLight = _Lightnings[i].pos - pos;
                    float distNorm = dot(posToLight, posToLight) *  _Lightnings[i].range;
                    float att = Attenuation(distNorm);

                    //#if ANISOTROPY
                    float3 cameraToPos = normalize(pos - _WorldSpaceCameraPos.xyz);
                    float costheta = dot(cameraToPos, normalize(posToLight));
                    att *= anisotropy(costheta);
                    //#endif
 
                    intensity += _Lightnings[i].intensity * att;
                }
                return intensity;
            }
*/
            void InitRaymarchParametersLayer1(inout RaymarchParameters parameters)
            {
                parameters.scatteringCoef = _CloudsLighting.x;
                parameters.hgPhase = _CloudsLighting.y;
                parameters.silverLiningIntensity = _CloudsLighting.z;
                parameters.silverLiningSpread = _CloudsLighting.w;

                parameters.multiScatteringA = _CloudsMultiScattering.x;
                parameters.multiScatteringB = _CloudsMultiScattering.y;
                parameters.multiScatteringC = _CloudsMultiScattering.z;

                parameters.powderTerm = _CloudsLightingExtended.x;
                parameters.attenuation = _CloudsLightingExtended.y;
                parameters.lightStep = _CloudsLightingExtended.z;
                parameters.lightAbsorb = _CloudsLightingExtended.w;

                parameters.cloudsParameter = _CloudsParameter;

                parameters.density = _CloudDensityScale.x;
                parameters.densitySmoothness = _CloudDensityScale.z;
                 
                parameters.baseErosion = _CloudsErosionIntensity.x;
                parameters.detailErosion = _CloudsErosionIntensity.y;

                parameters.minSteps = _Steps.x;
                parameters.maxSteps = _Steps.y;

                parameters.baseNoiseUV = _CloudsNoiseSettings.x;
                parameters.detailNoiseUV = _CloudsNoiseSettings.y;

                parameters.baseErosionIntensity = _CloudsErosionIntensity.x;
                parameters.detailErosionIntensity = _CloudsErosionIntensity.y;

                 parameters.anvilBias = _CloudsCoverageSettings.z;
            } 
 
            void InitRaymarchParametersLayer2(inout RaymarchParameters parameters)
            {
                parameters.scatteringCoef = _CloudsLighting2.x;
                parameters.hgPhase = _CloudsLighting2.y;
                parameters.silverLiningIntensity = _CloudsLighting2.z;
                parameters.silverLiningSpread = _CloudsLighting2.w;

                parameters.multiScatteringA = _CloudsMultiScattering2.x;
                parameters.multiScatteringB = _CloudsMultiScattering2.y;
                parameters.multiScatteringC = _CloudsMultiScattering2.z;

                parameters.powderTerm = _CloudsLightingExtended2.x;
                parameters.attenuation = _CloudsLightingExtended2.y;
                parameters.lightStep = _CloudsLightingExtended2.z;
                parameters.lightAbsorb = _CloudsLightingExtended2.w;

                parameters.cloudsParameter = _CloudsParameter2;

                parameters.density = _CloudDensityScale.y;
                parameters.densitySmoothness = _CloudDensityScale.w;

                parameters.baseErosion = _CloudsErosionIntensity.x;
                parameters.detailErosion = _CloudsErosionIntensity.y;

                parameters.minSteps = _Steps.z;
                parameters.maxSteps = _Steps.w;
     
                parameters.baseNoiseUV = _CloudsNoiseSettings.z;
                parameters.detailNoiseUV = _CloudsNoiseSettings.w;

                parameters.baseErosionIntensity = _CloudsErosionIntensity.z;
                parameters.detailErosionIntensity = _CloudsErosionIntensity.w;

                parameters.anvilBias = _CloudsCoverageSettings.w;
            } 


            float HenryGreenstein(float cosAngle, float g)
            {
                float g2 = g * g;
                return (1.0 / (4.0 * 3.1415926f)) * (1.0 - g2) / pow(abs(1.0 + g2 - 2.0 * g * cosAngle), 1.5);
            }

            float RemapEnviro(float org_val, float org_min, float org_max, float new_min, float new_max)
            {
                return new_min + saturate(((org_val - org_min) / (org_max - org_min))*(new_max - new_min));
            }

            float4 GetHeightGradient(float cloudType)
            {
                const float4 CloudGradient1 = float4(0.0, 0.07, 0.08, 0.15);
                const float4 CloudGradient2 = float4(0.0, 0.2, 0.42, 0.6);
                const float4 CloudGradient3 = float4(0.0, 0.08, 0.75, 0.98);
 
                float a = 1.0 - saturate(cloudType * 2.0);
                float b = 1.0 - abs(cloudType - 0.5) * 2.0;
                float c = saturate(cloudType - 0.5) * 2.0;

                return CloudGradient1 * a + CloudGradient2 * b + CloudGradient3 * c;
            }


            float GradientStep(float a, float4 gradient)
            {
                return smoothstep(gradient.x, gradient.y, a) - smoothstep(gradient.z, gradient.w, a);
            }

            float4 GetWeather(float3 pos) 
            {
                float2 uv = pos.xz * 0.0000025;
                return _WeatherMap.SampleLevel(sampler_WeatherMap,uv, 0); 
            } 

            float GetSamplingHeight(float3 pos, float3 center, float4 parameters)
            {
                return (length(pos - center) - (parameters.w + parameters.x)) * parameters.z;
            }
             
            float3 ScreenSpaceDither(float2 vScreenPos, float lum)
            {
                float d = dot(float2(131.0, 312.0), vScreenPos.xy); //+ _Time TODO
                float3 vDither = float3(d, d, d);
                vDither.rgb = frac(vDither.rgb / float3(103.0, 71.0, 97.0)) - float3(0.5, 0.5, 0.5);
                return (vDither.rgb / 15.0) * 1.0 * lum;
            }

            float GetRaymarchEndFromSceneDepth(float sceneDepth, float maxRange) 
            {
				float raymarchEnd = 0.0f;
	#if ENVIRO_DEPTH_BLENDING
				if (sceneDepth >= 0.99f) 
                {	
					raymarchEnd = maxRange;
				}
				else 
                { 
					raymarchEnd = sceneDepth * _ProjectionParams.z;	   
				}
	#else
                if(_EnviroDepthTest > 0 )
                {
                    if (sceneDepth >= 0.99f) 
                    {	
                        raymarchEnd = maxRange;
                    }
                    else 
                    { 
                        raymarchEnd = sceneDepth * _ProjectionParams.z;	   
                    }
                }
                else
                {
				raymarchEnd = maxRange;	
                }
	#endif
				return raymarchEnd;
			}


            float HeightAlter(float percent_height, float weather, float anvil) 
            {
                float cloud_anvil_amount = 0.5;
                float global_coverage = 0.5;
                // Round bottom a bit
                float ret_val = saturate(RemapEnviro(percent_height, 0.0, 0.07, 0.0, 1.0));
                // Round top a lot
                float stop_height = saturate(weather + 0.12);
                ret_val *= saturate(RemapEnviro(percent_height, stop_height *	0.2, stop_height, 1.0, 0.0));
                // Apply anvil ( cumulonimbus /" giant storm" clouds)
                ret_val = pow(ret_val, saturate(RemapEnviro(percent_height, 0.65, 0.95, 1.0, (1 - cloud_anvil_amount * global_coverage))));
                return ret_val;
            }

            float DensityAlter(float coverage, float percent_height, float anvil) 
            {
                // Have density be generally increasing over height
                float ret_val = percent_height;	
                
                // Reduce density at base
                ret_val *= saturate(RemapEnviro(percent_height, 0.0, 0.2, 0.0, 1.0));
                ret_val *= 2;	
                
                // Reduce density for the anvil (cumulonimbus clouds)
                ret_val *= lerp(ret_val, saturate(RemapEnviro(pow(percent_height, 0.5), 0.4, 0.95, 1.0, 0.2)), 1-anvil);
               
                // Reduce density at top to make better transition
                ret_val *= saturate(RemapEnviro(percent_height, 0.9, 1.0, 1.0, 0.0));	
                return ret_val;	
            }

            // Sample Cloud Density
            float CalculateCloudDensity(float3 pos, float3 PlanetCenter, RaymarchParameters parameters, float2 weather, float mip, float lod, bool details)
            {
                const float baseFreq = 1e-5;
                
                // Get Height fraction
                float height = GetSamplingHeight(pos, PlanetCenter, parameters.cloudsParameter);

                // wind settings
                float cloud_top_offset = 2000.0;
                float3 wind_direction = float3(_CloudsWindDirection.x, 0.0, _CloudsWindDirection.y);

                // skew in wind direction
                pos += height * wind_direction * cloud_top_offset;

                float mip1 = mip + lod;// + dist * _LODDistance;

                float4 coord = float4(pos * baseFreq * parameters.baseNoiseUV, mip1);

 
                // Animate Wind
                coord.xyz += float3(_CloudsWindDirection.z, 0.0f, _CloudsWindDirection.w) * 14;
                
                float4 baseNoise = 0;

                baseNoise = _Noise.SampleLevel(sampler_Noise, coord.xyz,coord.w);

                float low_freq_fBm = (baseNoise.g * 0.625) + (baseNoise.b * 0.25) + (baseNoise.a * 0.125);
                float base_cloud = RemapEnviro(baseNoise.r, -(1.0 - low_freq_fBm) * parameters.baseErosionIntensity, 1.0, 0.0, 1.0);

                float heightGradient = GradientStep(height, GetHeightGradient(saturate(weather.g * 2.0)));
 
                base_cloud *= heightGradient; 

                float cloud_coverage = saturate(1-weather.r);
                    
                float densAlter = DensityAlter(cloud_coverage,(1-height * 0.75), parameters.anvilBias);
                cloud_coverage = pow(cloud_coverage, densAlter);

               // cloud_coverage = pow(cloud_coverage, Remap(height, 0.7, 0.8, 1.0, lerp(1.0, 0.5, parameters.anvilBias)));
 
                float cloudDensity = RemapEnviro(base_cloud, cloud_coverage, 1.0, 0.0, 1.0);

                cloudDensity = max(cloudDensity * (1-cloud_coverage),0.0);

                //DETAIL
                [branch]  
                if (details)
                { 		  
                    float mip2 = mip + lod;// + dist * _LODDistance;
                    coord = float4(pos * baseFreq * parameters.detailNoiseUV, mip2);                 
  
                    //HQ Curl
                    float3 curl_noise1 = _CurlNoise.SampleLevel(sampler_CurlNoise, float2(coord.xz * 2), 0).rgb;
                    float3 curl_noise2 = _CurlNoise.SampleLevel(sampler_CurlNoise, float2(coord.xy * 2), 0).rgb;
                    coord.xy += pow(saturate(curl_noise1.rgb),0.1) * parameters.attenuation;
                    coord.xz += pow(saturate(curl_noise2.rgb),0.1) * parameters.attenuation; 
 
                    //float3 curl_noise1 = _CurlNoise.SampleLevel(sampler_CurlNoise, float2(coord.xy * 2), 0).rgb;
                    //coord.xyz += pow(saturate(curl_noise1.rgb),0.1) * parameters.attenuation;

                    coord.xyz += float3(_CloudsWindDirection.z * 14, _CloudsAnimation.z, _CloudsWindDirection.w * 14);	

                    float3 detailNoise = _DetailNoise.SampleLevel(sampler_DetailNoise, coord.xyz, coord.w).rgb;
                    float high_freq_fBm = (detailNoise.r * 0.625) + (detailNoise.g * 0.25) + (detailNoise.b * 0.125);
                    float high_freq_noise_modifier = lerp(high_freq_fBm, 1.0f - high_freq_fBm, saturate(height * 10));
                    //float high_freq_noise_modifier = 1.0f - high_freq_fBm;	 		 	
                    cloudDensity = RemapEnviro(cloudDensity, saturate(high_freq_noise_modifier * parameters.detailErosionIntensity), 1.0, 0.0, 1.0);
                } 

                return cloudDensity; 
            }

            static const float shadowSampleDistance[5] = 
            {
                0.5, 4, 6, 12.0, 48.0
            };

            static const float LightingInfluence[5] = 
            { 
                4.0f, 2.0f, 2.0f, 4.0f, 2.0f 
            };

            // Lighting Sample Function
            float GetDensityAlongRay(float3 pos, float3 PlanetCenter, RaymarchParameters parameters, float3 LightDirection, float2 weather, float lod)
            {
                float opticalDepth = 0.0;
                int mip_offset = 0.5;
               
                [unroll]
                for (int i = 0; i < 5; i++)
                {
                    float stepLength = shadowSampleDistance[i] * (512 * _CloudsLightingExtended.z);
                    float3 samplePos = pos + LightDirection * stepLength;
                    float sampleResult = CalculateCloudDensity(samplePos, PlanetCenter, parameters, weather, mip_offset, lod, true);    
                    opticalDepth += LightingInfluence[i] * sampleResult  * (stepLength / (i + 1));
                    mip_offset += 0.5;
                }
                return pow(opticalDepth * parameters.lightAbsorb,0.5);
            } 

            float PowderEffect(float cloudDensity, float cosAngle, float intensity)
            {
                float powderEffect = 1.0 - exp(-cloudDensity);
                powderEffect = saturate(powderEffect * 2.0); 
                return lerp(1.0, lerp(1.0, powderEffect * 10, smoothstep(0.5, -0.5, cosAngle)), intensity);
            }
  

            float SampleEnergy(float3 pos, float cosTheta, float3 cent, RaymarchParameters parameters, float3 LightDirection, float height, float ds_loded, float step_size, float2 weather, float lod) 
            {
                float opticsDistance = GetDensityAlongRay(pos, cent, parameters, LightDirection, weather, lod);
                float result = 0.0f;
                float powder = PowderEffect(ds_loded,cosTheta,parameters.powderTerm);

                [unroll]
                for (int octaveIndex = 0; octaveIndex < 2; octaveIndex++) 
                {	
                    //Multi scattering approximation based on Frostbite paper.
                    float transmittance = exp(-parameters.density * pow(parameters.multiScatteringB, octaveIndex) * opticsDistance);
                    float ecMult = pow(parameters.multiScatteringC, octaveIndex);
                    float phase = lerp(HenryGreenstein(cosTheta, .5 * ecMult), HenryGreenstein(cosTheta,(0.99 - parameters.silverLiningSpread) * ecMult), 0.5f); 
                    result += phase * transmittance * parameters.scatteringCoef * 25.0f * powder *  pow(parameters.multiScatteringA, octaveIndex);          
                }
 
                //float powder = 1.0 - exp(-ds_loded * (1-parameters.powderTerm));
		       // powder = max(lerp(pow(powder * 5, 1.75),  1, cosTheta * 0.5 + 0.5), 0); 

	            return result; 
            }

            float2 squareUV(float2 uv) 
            {
                float width = _Resolution.x;
                float height = _Resolution.y;
                float scale = 400;
                float x = uv.x * width;
                float y = uv.y * height;
                return float2 (x/scale, y/scale);
            }


            bool ray_trace_sphere(float3 center, float3 rd, float3 offset, float radius, out float t1, out float t2) {
                float3 p = center - offset;
                float b = dot(p, rd);
                float c = dot(p, p) - (radius * radius);
            
                float f = b * b - c;
                if (f >= 0.0) {
                    float dem = sqrt(f);
                    t1 = -b - dem;
                    t2 = -b + dem;
                    return true;
                }
                return false;
            }

            
            bool resolve_ray_start_end(float3 ws_origin, float3 ws_ray, float3 center, RaymarchParameters parameter, out float start, out float end) 
            {
                //case includes on ground, inside atm, above atm.
                float ot1, ot2, it1, it2;
                bool outIntersected = ray_trace_sphere(ws_origin, ws_ray, center, parameter.cloudsParameter.w + parameter.cloudsParameter.y, ot1, ot2);
                if (!outIntersected || ot2 < 0.0f)
                    return false;	//you see nothing.
            
                bool inIntersected = ray_trace_sphere(ws_origin, ws_ray, center, parameter.cloudsParameter.w + parameter.cloudsParameter.x, it1, it2);
                
                if (inIntersected) 
                {
                    if (it1 * it2 < 0) 
                    {
                        //we're on ground.
                        start = max(it2, 0);
                        end = ot2;
                    }
                    else 
                    {
                        start = 0.0f;
                        //we're inside atm, or above atm.
                        if (ot1 * ot2 < 0.0) 
                        {	          
                            //Inside atm.
                            if (it2 > 0.0)  
                            {
                                //Look down.
                                end = it1;
                            }
                            else  
                            {
                                //Look up.
                                end = ot2;
                            } 
      
                            start = 0.0f;
                        } 
                        else 
                        {	//Outside atm
                            if (ot1 < 0.0) 
                            {
                                return false;
                            }
                            else 
                            {
                                start = ot1;
                                end = it1;
                            }
                        }
                    }
                }     
                else 
                {
                    end = ot2;
                    start = max(ot1, 0);
                }
                return true;
            }
            
            int inside = 0;

            float2 ResolveRay(float3 pos, float3 ray, float3 center, float raymarchEnd, RaymarchParameters parameter)
            {
                float sampleStart, sampleEnd = 0;
                
                if (!resolve_ray_start_end(pos, ray,center, parameter, sampleStart, sampleEnd)) 
                {
                    return float2(0,0);
                }

                float3 sampleStartPos = pos + (ray * sampleStart);
                if (sampleEnd <= sampleStart)
                {	
                    return float2(0,0);
                }
            
                float ch = length(pos - center) -  parameter.cloudsParameter.w;
                //float height = RemapEnviro(pos.y, parameter.cloudsParameter.x, parameter.cloudsParameter.y * 0.75, 0, 1);
                //float end = lerp(_CloudsCoverageSettings.y * 0.85,_CloudsCoverageSettings.y * 1.25,height);
 
                raymarchEnd = min(raymarchEnd, _CloudsCoverageSettings.y);

                if (ch < parameter.cloudsParameter.x)
                { 
                    sampleEnd = min(raymarchEnd, sampleEnd);
                    inside = 0;
                   
                    if(ray.y < 0)
                       return float2(0,0);
                }
                else if (ch > parameter.cloudsParameter.y)
                {
                    sampleEnd = min(raymarchEnd, sampleEnd);
                    inside = 0;
                }
                else                                               
                {     
                    sampleEnd = min(raymarchEnd, sampleEnd);
                    inside = 0;  
                } 
                        
                return float2(max(0.0f,sampleStart), min(raymarchEnd, sampleEnd));
            }

            float CalculateLodMips(float distanceToCamera)
            {
                return lerp(0.0, lerp(5.0,0.0,_LODDistance), saturate((distanceToCamera - 3000.0) / (100000.0 - 3000.0)));
            }

            float3 Raymarch (float3 cameraPos, float3 ray, float2 hitDistance, float3 center, RaymarchParameters parameters, float offset, int layer)
            {                   
                float cloud_test = 0.0;
	            int zero_density_sample_count = 0;
	            float sampled_density_previous = -1.0;
      
                float alpha = 1.0; 
                float intensity = 0.0;
	            float depth = 0.0;
	            float depthWeightSum = 0.000001;
	            float trans = 1.0f;
 
                int steps = (int)lerp(parameters.minSteps, parameters.maxSteps, ray.y);
                
                //int steps = parameters.maxSteps;
                float rayStepLength = (hitDistance.y - hitDistance.x) / steps;

                float3 rayStep = ray * rayStepLength;

                float3 pos = (cameraPos + (hitDistance.x) * ray);  
                pos += (offset * rayStepLength) * ray;
                //pos += rayStep;  
                pos += rayStep;
                float3 sampleEndPos = cameraPos + ray * hitDistance.y;
                float eyeToEnd = distance(cameraPos, sampleEndPos);
                float cosTheta = dot(ray, normalize(_LightDir));
                
                [loop]
                for (int i = 0; i < steps; i++)
                {   

                //Calculate projection height
                float height = GetSamplingHeight(pos, center, parameters.cloudsParameter);

                //Get out of expensive raymarching			
                if (alpha <= 0.01 || height > 1.0 || height < 0.0 || _CloudsCoverageSettings.x <= -0.9)
                    break; 
                
                // Get Weather Data                                                                                
                float2 weather;

                if(layer == 0)
                    weather = GetWeather(pos).xy;
                else
                    weather = GetWeather(pos).zw; 

                float distanceToCamera = length(pos - cameraPos);
                float lod = CalculateLodMips(distanceToCamera);

                if (cloud_test > 0.0) 
                {  
                    float sampled_density = CalculateCloudDensity(pos, center, parameters, weather, 0, lod, true);

                    if (sampled_density == 0.0 && sampled_density_previous == 0.0)
                    { 
                        zero_density_sample_count++;
                    } 

                    if (zero_density_sample_count < 8 && sampled_density != 0.0)
                    { 
                        float extinction = pow(max(parameters.density * sampled_density,0.001),parameters.densitySmoothness); 
                        float clampedExtinction = max(extinction, 1e-7);
                         
                        float transmittance = exp(-extinction * rayStepLength);      
                        //ds += clampedExtinction * rayStepLength;               
                        float luminance = SampleEnergy(pos, cosTheta, center, parameters, _LightDir, height, sampled_density, rayStepLength, weather, lod);

                        float integScatt = (luminance - luminance * transmittance);// * powder;
                    
                        float depthWeight = trans;
                        depth += depthWeight * distanceToCamera;
                        depthWeightSum += depthWeight;
 
                        intensity += trans * integScatt;

                        trans *= transmittance;
                        alpha *= max(transmittance, 0.0);

                        if (alpha <= 0.01)
                            alpha = 0.0;
                    }
                    // if not, then set cloud_test to zero so that we go back to the cheap sample case
                    else if(zero_density_sample_count >= 8)
                    {
                        cloud_test = 0.0;
                        zero_density_sample_count = 0;
                    }

                    sampled_density_previous = sampled_density;
                    pos += rayStep;   
                }
                else 
                {   
                    // sample density the cheap way, only using the low frequency noise
                    cloud_test = CalculateCloudDensity(pos, center, parameters, weather, 0, lod, (bool)inside);
                    
                    if (cloud_test == 0.0) 
                    { 
                         pos += rayStep * 2;
                    }
                    else  //take a step back and capture area we skipped.
                    {
                        pos -= rayStep; 
                    }
                }
                
            
            }

            float distance = depth / depthWeightSum;

            if (distance <= 0.0) 
            {
                distance = length(sampleEndPos - cameraPos);
            } 

            alpha = saturate(1.0f - alpha);
            
            return float3(intensity,distance,alpha);
            
            }

            float3 CalculateWorldPosition (float2 uv, float depth)
            {
                float4x4 proj, eyeToWorld;

                if (unity_StereoEyeIndex == 0)
                {
                    proj = _LeftViewFromScreen;
                    eyeToWorld = _LeftWorldFromView;
                }
                else
                {
                    proj = _RightViewFromScreen;
                    eyeToWorld = _RightWorldFromView;
                }

                //bit of matrix math to take the screen space coord (u,v,depth) and transform to world space
                float2 uvClip = uv * 2.0 - 1.0;
                float clipDepth = depth; // Fix for OpenGl Core thanks to Lars Bertram
                clipDepth = (UNITY_NEAR_CLIP_VALUE < 0) ? clipDepth * 2 - 1 : clipDepth;
                float4 clipPos = float4(uvClip, clipDepth, 1.0);
                float4 viewPos = mul(proj, clipPos); // inverse projection by clip position
                viewPos /= viewPos.w; // perspective division
                return float3(mul(eyeToWorld, viewPos).xyz);
            }
 
            float RaymarchShadows (float3 cameraPos, float3 worldPos,float3 ray,float3 center, RaymarchParameters parameters, float offset,float depth, int layer)
            { 
                if(depth == 0.0f)
                    return 0.0;

                int steps = 16;
                float worldDotLight = saturate(dot(float3(0, 1, 0), _LightDir));
                float bottomDist = max(0, parameters.cloudsParameter.x ) / worldDotLight;
                float topDist = max(0, parameters.cloudsParameter.y ) / worldDotLight;
  
                float rayStepLength = (topDist - bottomDist) / steps;
                float3 rayStep = _LightDir * rayStepLength;

                float3 pos =  worldPos + bottomDist * _LightDir;


                float2 weather;
                float intensity = 1.0;
                if(layer == 0)
                {
                    weather = GetWeather(pos).xy;
                    intensity = 0.05;
                }
                else
                {
                    weather = GetWeather(pos).zw;
                    intensity = 0.025;
                }

                float shadowIntensity = 0.0;
 
                float _Softness = 2.0f;

                [unroll]
                for (int i = 0; i < steps; i++)
                {
                    float3 samplePos = rayStepLength * i * _LightDir + pos;
                    float sampleResult = CalculateCloudDensity(samplePos, center, parameters, weather, 0, 0, true) * intensity;
                    float result = sampleResult * (rayStepLength / (i + 1)); 
                    shadowIntensity += result;

                  //  if (shadowIntensity > 0.99) 
                  //      break;
                }
                return (shadowIntensity);
            }