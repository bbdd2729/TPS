// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TopText"
{
    Properties
    {
        [HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
        [HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
        [ASEBegin]_MainTex("_MainTex", 2D) = "white" {}
        _Text("Text", 2D) = "white" {}
        [Header(ScaleAndOffset)]_TextSize("TextSize", Range( 0.7 , 1.5)) = 0.7
        _TextSaturation("TextSaturation", Range( 0 , 1)) = 1
        _Scale_Offset("Scale_Offset", Vector) = (1,1,0,0)
        [Header(FirstLayer)]_AlphaRange("AlphaRange", Range( 0.1 , 2)) = 1
        _FirstLayerGray("FirstLayerGray", Range( 0 , 0.5)) = 0.1
        [Header(ColorTex)]_ColorHueRange1("ColorHueRange", Range( -0.1 , 0.1)) = 0.1
        [HDR]_TextColor01("TextColor01", Color) = (1,0.2948235,0,0)
        _radius01("radius01", Range( 0 , 3)) = 0.5529412
        [HDR]_TextColor02("TextColor02", Color) = (0.6886792,0,0.1026268,0)
        _radius02_Step("radius02_Step", Range( 0 , 1)) = 1
        [HDR]_TextColor03("TextColor03", Color) = (1,0,0.1481056,0)
        _radius03_Step("radius03_Step", Range( 0 , 1)) = 0.5529412
        [HDR]_TextColor04("TextColor04", Color) = (1,1,1,0)
        _radius04_step("radius04_step", Range( 0 , 0.6)) = 0
        _radius04_step02("radius04_step02", Range( 0 , 0.6)) = 0
        _radius04_step03("radius04_step03", Range( 0 , 0.6)) = 0
        [Header(BlackMode)]_Saturation("Saturation", Range( 0 , 1)) = 0
        [ASEEnd][Toggle(_BLAKWHITE_ON)] _BlakWhite("BlakWhite", Float) = 0


        //_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
        //_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
        //_TessMin( "Tess Min Distance", Float ) = 10
        //_TessMax( "Tess Max Distance", Float ) = 25
        //_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
        //_TessMaxDisp( "Tess Max Displacement", Float ) = 25

        [HideInInspector] _QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector] _QueueControl("_QueueControl", Float) = -1

        [HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }

    SubShader
    {
        LOD 0



        Tags
        {
            "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" "UniversalMaterialType"="Unlit"
        }

        Cull Back
        AlphaToMask Off



        HLSLINCLUDE
        #pragma target 3.5
        #pragma prefer_hlslcc gles
        // ensure rendering platforms toggle list is visible

        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

        #ifndef ASE_TESS_FUNCS
        #define ASE_TESS_FUNCS

        float4 FixedTess(float tessValue)
        {
            return tessValue;
        }

        float CalcDistanceTessFactor(float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos)
        {
            float3 wpos = mul(o2w, vertex).xyz;
            float dist = distance(wpos, cameraPos);
            float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
            return f;
        }

        float4 CalcTriEdgeTessFactors(float3 triVertexFactors)
        {
            float4 tess;
            tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
            tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
            tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
            tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
            return tess;
        }

        float CalcEdgeTessFactor(float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams)
        {
            float dist = distance(0.5 * (wpos0 + wpos1), cameraPos);
            float len = distance(wpos0, wpos1);
            float f = max(len * scParams.y / (edgeLen * dist), 1.0);
            return f;
        }

        float DistanceFromPlane(float3 pos, float4 plane)
        {
            float d = dot(float4(pos, 1.0f), plane);
            return d;
        }

        bool WorldViewFrustumCull(float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6])
        {
            float4 planeTest;
            planeTest.x = ((DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f) +
                ((DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f) +
                ((DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f);
            planeTest.y = ((DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f) +
                ((DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f) +
                ((DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f);
            planeTest.z = ((DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f) +
                ((DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f) +
                ((DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f);
            planeTest.w = ((DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f) +
                ((DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f) +
                ((DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f);
            return !all(planeTest);
        }

        float4 DistanceBasedTess(float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos)
        {
            float3 f;
            f.x = CalcDistanceTessFactor(v0, minDist, maxDist, tess, o2w, cameraPos);
            f.y = CalcDistanceTessFactor(v1, minDist, maxDist, tess, o2w, cameraPos);
            f.z = CalcDistanceTessFactor(v2, minDist, maxDist, tess, o2w, cameraPos);

            return CalcTriEdgeTessFactors(f);
        }

        float4 EdgeLengthBasedTess(float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams)
        {
            float3 pos0 = mul(o2w, v0).xyz;
            float3 pos1 = mul(o2w, v1).xyz;
            float3 pos2 = mul(o2w, v2).xyz;
            float4 tess;
            tess.x = CalcEdgeTessFactor(pos1, pos2, edgeLength, cameraPos, scParams);
            tess.y = CalcEdgeTessFactor(pos2, pos0, edgeLength, cameraPos, scParams);
            tess.z = CalcEdgeTessFactor(pos0, pos1, edgeLength, cameraPos, scParams);
            tess.w = (tess.x + tess.y + tess.z) / 3.0f;
            return tess;
        }

        float4 EdgeLengthBasedTessCull(float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w,
                                       float3 cameraPos, float4 scParams, float4 planes[6])
        {
            float3 pos0 = mul(o2w, v0).xyz;
            float3 pos1 = mul(o2w, v1).xyz;
            float3 pos2 = mul(o2w, v2).xyz;
            float4 tess;

            if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
            {
                tess = 0.0f;
            }
            else
            {
                tess.x = CalcEdgeTessFactor(pos1, pos2, edgeLength, cameraPos, scParams);
                tess.y = CalcEdgeTessFactor(pos2, pos0, edgeLength, cameraPos, scParams);
                tess.z = CalcEdgeTessFactor(pos0, pos1, edgeLength, cameraPos, scParams);
                tess.w = (tess.x + tess.y + tess.z) / 3.0f;
            }
            return tess;
        }
        #endif //ASE_TESS_FUNCS
        ENDHLSL


        Pass
        {

            Name "Forward"
            Tags
            {
                "LightMode"="UniversalForwardOnly"
            }

            Blend One Zero, One Zero
            ZWrite On
            ZTest LEqual
            Offset 0 , 0
            ColorMask RGBA



            HLSLPROGRAM
            #pragma multi_compile_instancing
            #define ASE_SRP_VERSION 120111


            #pragma multi_compile _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3

            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma shader_feature _ _SAMPLE_GI
            #pragma multi_compile _ DEBUG_DISPLAY

            #pragma vertex vert
            #pragma fragment frag

            #define SHADERPASS SHADERPASS_UNLIT

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging3D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceData.hlsl"

            #pragma shader_feature_local _BLAKWHITE_ON


            struct VertexInput
            {
                float4 vertex : POSITION;
                float3 ase_normal : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
            {
                float4 clipPos : SV_POSITION;
                #if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 worldPos : TEXCOORD0;
                #endif
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
                #endif
                #ifdef ASE_FOG
					float fogFactor : TEXCOORD2;
                #endif
                float4 ase_texcoord3 : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _TextColor02;
                float4 _Scale_Offset;
                float4 _TextColor04;
                float4 _TextColor01;
                float4 _TextColor03;
                float _radius04_step;
                float _radius03_Step;
                float _TextSaturation;
                float _radius02_Step;
                float _Saturation;
                float _radius01;
                float _ColorHueRange1;
                float _AlphaRange;
                float _FirstLayerGray;
                float _TextSize;
                float _radius04_step02;
                float _radius04_step03;
                #ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
                #endif
            CBUFFER_END

            sampler2D _MainTex;
            sampler2D _Text;
            sampler2D _RenderOnTopDepthTexture;


            float3 HSVToRGB(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }

            float3 RGBToHSV(float3 c)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float getColorBlack176(half2 uv, half radius)
            {
                float mask = 0;
                mask = uv.x * uv.x + uv.y * uv.y;
                mask = saturate(mask / radius);
                return mask;
            }

            int circle62(half2 uv, half radius)
            {
                int mask = 0;
                float x = uv.x * _ScreenParams.x / _ScreenParams.y;
                mask = (x * x + uv.y * uv.y < radius * radius) ? 1 : 0;
                return mask;
            }

            int circle78(half2 uv, half radius)
            {
                int mask = 0;
                float x = uv.x * _ScreenParams.x / _ScreenParams.y;
                mask = (x * x + uv.y * uv.y < radius * radius) ? 1 : 0;
                return mask;
            }

            int circle89(half2 uv, half radius)
            {
                int mask = 0;
                float x = uv.x * _ScreenParams.x / _ScreenParams.y;
                mask = (x * x + uv.y * uv.y < radius * radius) ? 1 : 0;
                return mask;
            }

            int circle106(half2 uv, half radius)
            {
                int mask = 0;
                float x = uv.x * _ScreenParams.x / _ScreenParams.y;
                mask = (x * x + uv.y * uv.y < radius * radius) ? 1 : 0;
                return mask;
            }

            int circle113(half2 uv, half radius)
            {
                int mask = 0;
                float x = uv.x * _ScreenParams.x / _ScreenParams.y;
                mask = (x * x + uv.y * uv.y < radius * radius) ? 1 : 0;
                return mask;
            }

            int circle127(half2 uv, half radius)
            {
                int mask = 0;
                float x = uv.x * _ScreenParams.x / _ScreenParams.y;
                mask = (x * x + uv.y * uv.y < radius * radius) ? 1 : 0;
                return mask;
            }

            int circle133(half2 uv, half radius)
            {
                int mask = 0;
                float x = uv.x * _ScreenParams.x / _ScreenParams.y;
                mask = (x * x + uv.y * uv.y < radius * radius) ? 1 : 0;
                return mask;
            }


            VertexOutput VertexFunction(VertexInput v)
            {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
                float4 screenPos = ComputeScreenPos(ase_clipPos);
                o.ase_texcoord3 = screenPos;


                #ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
                #else
                float3 defaultVertexValue = float3(0, 0, 0);
                #endif

                float3 vertexValue = defaultVertexValue;

                #ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
                #else
                v.vertex.xyz += vertexValue;
                #endif

                v.ase_normal = v.ase_normal;

                float3 positionWS = TransformObjectToWorld(v.vertex.xyz);
                float4 positionCS = TransformWorldToHClip(positionWS);

                #if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
                #endif

                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
                #endif

                #ifdef ASE_FOG
					o.fogFactor = ComputeFogFactor( positionCS.z );
                #endif

                o.clipPos = positionCS;

                return o;
            }

            #if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
            #if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
            #elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
            #elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
            #elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
            #endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				
            #if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
            #endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
            #else
            VertexOutput vert(VertexInput v)
            {
                return VertexFunction(v);
            }
            #endif

            half4 frag(VertexOutput IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                #if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.worldPos;
                #endif

                float4 ShadowCoords = float4(0, 0, 0, 0);

                #if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
                #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
                #endif
                #endif

                float4 screenPos = IN.ase_texcoord3;
                float4 ase_screenPosNorm = screenPos / screenPos.w;
                ase_screenPosNorm.z = (UNITY_NEAR_CLIP_VALUE >= 0) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
                float2 appendResult26 = (float2(ase_screenPosNorm.x, ase_screenPosNorm.y));
                float4 tex2DNode31 = tex2D(_MainTex, appendResult26);
                float3 hsvTorgb192 = RGBToHSV(tex2DNode31.rgb);
                float3 hsvTorgb194 = HSVToRGB(float3(hsvTorgb192.x, (hsvTorgb192.y * _Saturation), hsvTorgb192.z));
                #ifdef _BLAKWHITE_ON
				float4 staticSwitch198 = float4( hsvTorgb194 , 0.0 );
                #else
                float4 staticSwitch198 = tex2DNode31;
                #endif
                float4 ScreenColor181 = staticSwitch198;
                float2 appendResult68 = (float2(ase_screenPosNorm.x, ase_screenPosNorm.y));
                float2 temp_cast_2 = (0.5).xx;
                float2 break76 = (((((appendResult68 - temp_cast_2) * 2.0) * _TextSize) / 2.0) + 0.5);
                float2 appendResult50 = (float2(((break76.x * _Scale_Offset.x) + _Scale_Offset.z),
                                  ((break76.y * _Scale_Offset.y) + _Scale_Offset.w)));
                float4 tex2DNode29 = tex2D(_Text, appendResult50);
                float4 temp_cast_3 = ((tex2DNode29.g + _FirstLayerGray)).xxxx;
                float2 appendResult57 = (float2(ase_screenPosNorm.x, ase_screenPosNorm.y));
                float2 temp_cast_4 = (0.5).xx;
                float2 temp_output_60_0 = ((appendResult57 - temp_cast_4) * 2.0);
                float2 UV_Mid174 = temp_output_60_0;
                float2 uv176 = UV_Mid174;
                float radius176 = _AlphaRange;
                float localgetColorBlack176 = getColorBlack176(uv176, radius176);
                float4 lerpResult184 = lerp(ScreenColor181, temp_cast_3, localgetColorBlack176);
                float TextMask85 = (1.0 - tex2DNode29.g);
                float3 hsvTorgb162 = RGBToHSV(_TextColor01.rgb);
                float ScreenU146 = ase_screenPosNorm.x;
                float3 hsvTorgb163 = HSVToRGB(
                    float3((hsvTorgb162.x + ((1.0 - ScreenU146) * _ColorHueRange1)), hsvTorgb162.y, hsvTorgb162.z));
                float3 Color01169 = hsvTorgb163;
                float2 temp_cast_7 = (0.5).xx;
                float2 uv62 = temp_output_60_0;
                float radius62 = _radius01;
                int localcircle62 = circle62(uv62, radius62);
                float4 lerpResult77 = lerp(saturate(((1.0 - tex2DNode29.r) + lerpResult184)), float4((TextMask85 * Color01169), 0.0),
                    (float)localcircle62);
                float3 hsvTorgb152 = RGBToHSV(_TextColor02.rgb);
                float3 hsvTorgb153 = HSVToRGB(
                    float3((hsvTorgb152.x + ((1.0 - ScreenU146) * _ColorHueRange1)), hsvTorgb152.y, hsvTorgb152.z));
                float3 Color02159 = hsvTorgb153;
                float2 temp_cast_11 = (0.5).xx;
                float2 uv78 = temp_output_60_0;
                float temp_output_96_0 = (_radius01 - _radius02_Step);
                float radius78 = max(temp_output_96_0, 0.0);
                int localcircle78 = circle78(uv78, radius78);
                int temp_output_80_0 = (localcircle62 * localcircle78);
                float4 lerpResult82 = lerp(lerpResult77, float4((TextMask85 * Color02159), 0.0), (float)saturate(temp_output_80_0));
                float3 hsvTorgb141 = RGBToHSV(_TextColor03.rgb);
                float3 hsvTorgb142 = HSVToRGB(float3((hsvTorgb141.x + ((1.0 - ScreenU146) * _ColorHueRange1)), hsvTorgb141.y,
                                                     (hsvTorgb141.z * _TextSaturation)));
                float3 Color03150 = hsvTorgb142;
                float2 temp_cast_15 = (0.5).xx;
                float2 uv89 = temp_output_60_0;
                float temp_output_97_0 = (temp_output_96_0 - _radius03_Step);
                float radius89 = max(temp_output_97_0, 0.0);
                int localcircle89 = circle89(uv89, radius89);
                float4 lerpResult95 = lerp(lerpResult82, float4((TextMask85 * Color03150), 0.0),
                                           (float)saturate((temp_output_80_0 * localcircle89)));
                float2 temp_cast_17 = (0.5).xx;
                float2 uv106 = temp_output_60_0;
                float radius106 = max(temp_output_97_0, 0.0);
                int localcircle106 = circle106(uv106, radius106);
                float2 temp_cast_18 = (0.5).xx;
                float2 uv113 = temp_output_60_0;
                float temp_output_116_0 = max((temp_output_97_0 - _radius04_step), 0.0);
                float radius113 = temp_output_116_0;
                int localcircle113 = circle113(uv113, radius113);
                float2 temp_cast_19 = (0.5).xx;
                float2 uv127 = temp_output_60_0;
                float temp_output_128_0 = (temp_output_116_0 - _radius04_step02);
                float radius127 = max(temp_output_128_0, 0.0);
                int localcircle127 = circle127(uv127, radius127);
                float2 temp_cast_20 = (0.5).xx;
                float2 uv133 = temp_output_60_0;
                float radius133 = max((temp_output_128_0 - _radius04_step03), 0.0);
                int localcircle133 = circle133(uv133, radius133);
                float4 lerpResult118 = lerp(lerpResult95, (TextMask85 * _TextColor04),
                                            (float)((localcircle106 - localcircle113) + (localcircle127 - localcircle133)));
                float4 lerpResult32 = lerp(ScreenColor181, lerpResult118,
                                           (1.0 - saturate(
                                               (step(0.01, tex2D(_RenderOnTopDepthTexture, appendResult26).r) + tex2DNode29.g))));

                float3 BakedAlbedo = 0;
                float3 BakedEmission = 0;
                float3 Color = lerpResult32.rgb;
                float Alpha = 1;
                float AlphaClipThreshold = 0.5;
                float AlphaClipThresholdShadow = 0.5;

                #ifdef _ALPHATEST_ON
					clip( Alpha - AlphaClipThreshold );
                #endif

                #if defined(_DBUFFER)
					ApplyDecalToBaseColor(IN.clipPos, Color);
                #endif

                #if defined(_ALPHAPREMULTIPLY_ON)
				Color *= Alpha;
                #endif

                #ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
                #endif

                #ifdef ASE_FOG
					Color = MixFog( Color, IN.fogFactor );
                #endif

                return half4(Color, Alpha);
            }
            ENDHLSL
        }


        Pass
        {

            Name "ShadowCaster"
            Tags
            {
                "LightMode"="ShadowCaster"
            }

            ZWrite On
            ZTest LEqual
            AlphaToMask Off
            ColorMask 0

            HLSLPROGRAM
            #pragma multi_compile_instancing
            #define ASE_SRP_VERSION 120111


            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            #define SHADERPASS SHADERPASS_SHADOWCASTER

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"


            struct VertexInput
            {
                float4 vertex : POSITION;
                float3 ase_normal : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
            {
                float4 clipPos : SV_POSITION;
                #if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 worldPos : TEXCOORD0;
                #endif
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
                #endif

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _TextColor02;
                float4 _Scale_Offset;
                float4 _TextColor04;
                float4 _TextColor01;
                float4 _TextColor03;
                float _radius04_step;
                float _radius03_Step;
                float _TextSaturation;
                float _radius02_Step;
                float _Saturation;
                float _radius01;
                float _ColorHueRange1;
                float _AlphaRange;
                float _FirstLayerGray;
                float _TextSize;
                float _radius04_step02;
                float _radius04_step03;
                #ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
                #endif
            CBUFFER_END


            float3 _LightDirection;
            float3 _LightPosition;

            VertexOutput VertexFunction(VertexInput v)
            {
                VertexOutput o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


                #ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
                #else
                float3 defaultVertexValue = float3(0, 0, 0);
                #endif

                float3 vertexValue = defaultVertexValue;

                #ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
                #else
                v.vertex.xyz += vertexValue;
                #endif

                v.ase_normal = v.ase_normal;

                float3 positionWS = TransformObjectToWorld(v.vertex.xyz);

                #if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
                #endif

                float3 normalWS = TransformObjectToWorldDir(v.ase_normal);

                #if _CASTING_PUNCTUAL_LIGHT_SHADOW
					float3 lightDirectionWS = normalize(_LightPosition - positionWS);
                #else
                float3 lightDirectionWS = _LightDirection;
                #endif

                float4 clipPos = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

                #if UNITY_REVERSED_Z
                clipPos.z = min(clipPos.z, UNITY_NEAR_CLIP_VALUE);
                #else
					clipPos.z = max(clipPos.z, UNITY_NEAR_CLIP_VALUE);
                #endif

                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
                #endif

                o.clipPos = clipPos;

                return o;
            }

            #if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
            #if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
            #elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
            #elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
            #elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
            #endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				
            #if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
            #endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
            #else
            VertexOutput vert(VertexInput v)
            {
                return VertexFunction(v);
            }
            #endif

            half4 frag(VertexOutput IN) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                #if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.worldPos;
                #endif

                float4 ShadowCoords = float4(0, 0, 0, 0);

                #if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
                #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
                #endif
                #endif


                float Alpha = 1;
                float AlphaClipThreshold = 0.5;
                float AlphaClipThresholdShadow = 0.5;

                #ifdef _ALPHATEST_ON
                #ifdef _ALPHATEST_SHADOW_ON
						clip(Alpha - AlphaClipThresholdShadow);
                #else
						clip(Alpha - AlphaClipThreshold);
                #endif
                #endif

                #ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
                #endif
                return 0;
            }
            ENDHLSL
        }


        Pass
        {

            Name "DepthOnly"
            Tags
            {
                "LightMode"="DepthOnly"
            }

            ZWrite On
            ColorMask 0
            AlphaToMask Off

            HLSLPROGRAM
            #pragma multi_compile_instancing
            #define ASE_SRP_VERSION 120111


            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"


            struct VertexInput
            {
                float4 vertex : POSITION;
                float3 ase_normal : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
            {
                float4 clipPos : SV_POSITION;
                #if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
                #endif
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
                #endif

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _TextColor02;
                float4 _Scale_Offset;
                float4 _TextColor04;
                float4 _TextColor01;
                float4 _TextColor03;
                float _radius04_step;
                float _radius03_Step;
                float _TextSaturation;
                float _radius02_Step;
                float _Saturation;
                float _radius01;
                float _ColorHueRange1;
                float _AlphaRange;
                float _FirstLayerGray;
                float _TextSize;
                float _radius04_step02;
                float _radius04_step03;
                #ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
                #endif
            CBUFFER_END


            VertexOutput VertexFunction(VertexInput v)
            {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


                #ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
                #else
                float3 defaultVertexValue = float3(0, 0, 0);
                #endif

                float3 vertexValue = defaultVertexValue;

                #ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
                #else
                v.vertex.xyz += vertexValue;
                #endif

                v.ase_normal = v.ase_normal;

                float3 positionWS = TransformObjectToWorld(v.vertex.xyz);

                #if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
                #endif

                o.clipPos = TransformWorldToHClip(positionWS);
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
                #endif

                return o;
            }

            #if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
            #if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
            #elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
            #elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
            #elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
            #endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				
            #if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
            #endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
            #else
            VertexOutput vert(VertexInput v)
            {
                return VertexFunction(v);
            }
            #endif

            half4 frag(VertexOutput IN) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                #if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.worldPos;
                #endif

                float4 ShadowCoords = float4(0, 0, 0, 0);

                #if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
                #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
                #endif
                #endif


                float Alpha = 1;
                float AlphaClipThreshold = 0.5;

                #ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
                #endif

                #ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
                #endif
                return 0;
            }
            ENDHLSL
        }


        Pass
        {

            Name "SceneSelectionPass"
            Tags
            {
                "LightMode"="SceneSelectionPass"
            }

            Cull Off

            HLSLPROGRAM
            #pragma multi_compile_instancing
            #define ASE_SRP_VERSION 120111


            #pragma vertex vert
            #pragma fragment frag

            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define SHADERPASS SHADERPASS_DEPTHONLY

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"


            struct VertexInput
            {
                float4 vertex : POSITION;
                float3 ase_normal : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
            {
                float4 clipPos : SV_POSITION;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _TextColor02;
                float4 _Scale_Offset;
                float4 _TextColor04;
                float4 _TextColor01;
                float4 _TextColor03;
                float _radius04_step;
                float _radius03_Step;
                float _TextSaturation;
                float _radius02_Step;
                float _Saturation;
                float _radius01;
                float _ColorHueRange1;
                float _AlphaRange;
                float _FirstLayerGray;
                float _TextSize;
                float _radius04_step02;
                float _radius04_step03;
                #ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
                #endif
            CBUFFER_END


            int _ObjectId;
            int _PassValue;

            struct SurfaceDescription
            {
                float Alpha;
                float AlphaClipThreshold;
            };

            VertexOutput VertexFunction(VertexInput v)
            {
                VertexOutput o;
                    ZERO_INITIALIZE(VertexOutput, o);

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


                #ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
                #else
                float3 defaultVertexValue = float3(0, 0, 0);
                #endif

                float3 vertexValue = defaultVertexValue;

                #ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
                #else
                v.vertex.xyz += vertexValue;
                #endif

                v.ase_normal = v.ase_normal;

                float3 positionWS = TransformObjectToWorld(v.vertex.xyz);
                o.clipPos = TransformWorldToHClip(positionWS);

                return o;
            }

            #if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
            #if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
            #elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
            #elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
            #elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
            #endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				
            #if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
            #endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
            #else
            VertexOutput vert(VertexInput v)
            {
                return VertexFunction(v);
            }
            #endif

            half4 frag(VertexOutput IN) : SV_TARGET
            {
                SurfaceDescription surfaceDescription = (SurfaceDescription)0;


                surfaceDescription.Alpha = 1;
                surfaceDescription.AlphaClipThreshold = 0.5;

                #if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
                #if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
                #endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
                #endif

                half4 outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
                return outColor;
            }
            ENDHLSL
        }


        Pass
        {

            Name "ScenePickingPass"
            Tags
            {
                "LightMode"="Picking"
            }

            HLSLPROGRAM
            #pragma multi_compile_instancing
            #define ASE_SRP_VERSION 120111


            #pragma vertex vert
            #pragma fragment frag

            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define SHADERPASS SHADERPASS_DEPTHONLY

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"


            struct VertexInput
            {
                float4 vertex : POSITION;
                float3 ase_normal : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
            {
                float4 clipPos : SV_POSITION;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _TextColor02;
                float4 _Scale_Offset;
                float4 _TextColor04;
                float4 _TextColor01;
                float4 _TextColor03;
                float _radius04_step;
                float _radius03_Step;
                float _TextSaturation;
                float _radius02_Step;
                float _Saturation;
                float _radius01;
                float _ColorHueRange1;
                float _AlphaRange;
                float _FirstLayerGray;
                float _TextSize;
                float _radius04_step02;
                float _radius04_step03;
                #ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
                #endif
            CBUFFER_END


            float4 _SelectionID;


            struct SurfaceDescription
            {
                float Alpha;
                float AlphaClipThreshold;
            };

            VertexOutput VertexFunction(VertexInput v)
            {
                VertexOutput o;
                    ZERO_INITIALIZE(VertexOutput, o);

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


                #ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
                #else
                float3 defaultVertexValue = float3(0, 0, 0);
                #endif
                float3 vertexValue = defaultVertexValue;
                #ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
                #else
                v.vertex.xyz += vertexValue;
                #endif
                v.ase_normal = v.ase_normal;

                float3 positionWS = TransformObjectToWorld(v.vertex.xyz);
                o.clipPos = TransformWorldToHClip(positionWS);
                return o;
            }

            #if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
            #if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
            #elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
            #elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
            #elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
            #endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				
            #if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
            #endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
            #else
            VertexOutput vert(VertexInput v)
            {
                return VertexFunction(v);
            }
            #endif

            half4 frag(VertexOutput IN) : SV_TARGET
            {
                SurfaceDescription surfaceDescription = (SurfaceDescription)0;


                surfaceDescription.Alpha = 1;
                surfaceDescription.AlphaClipThreshold = 0.5;

                #if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
                #if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
                #endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
                #endif

                half4 outColor = 0;
                outColor = _SelectionID;

                return outColor;
            }
            ENDHLSL
        }


        Pass
        {

            Name "DepthNormals"
            Tags
            {
                "LightMode"="DepthNormalsOnly"
            }

            ZTest LEqual
            ZWrite On


            HLSLPROGRAM
            #pragma multi_compile_instancing
            #define ASE_SRP_VERSION 120111


            #pragma vertex vert
            #pragma fragment frag

            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_NORMAL_WS

            #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"


            struct VertexInput
            {
                float4 vertex : POSITION;
                float3 ase_normal : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
            {
                float4 clipPos : SV_POSITION;
                float3 normalWS : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _TextColor02;
                float4 _Scale_Offset;
                float4 _TextColor04;
                float4 _TextColor01;
                float4 _TextColor03;
                float _radius04_step;
                float _radius03_Step;
                float _TextSaturation;
                float _radius02_Step;
                float _Saturation;
                float _radius01;
                float _ColorHueRange1;
                float _AlphaRange;
                float _FirstLayerGray;
                float _TextSize;
                float _radius04_step02;
                float _radius04_step03;
                #ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
                #endif
            CBUFFER_END


            struct SurfaceDescription
            {
                float Alpha;
                float AlphaClipThreshold;
            };

            VertexOutput VertexFunction(VertexInput v)
            {
                VertexOutput o;
                    ZERO_INITIALIZE(VertexOutput, o);

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


                #ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
                #else
                float3 defaultVertexValue = float3(0, 0, 0);
                #endif

                float3 vertexValue = defaultVertexValue;

                #ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
                #else
                v.vertex.xyz += vertexValue;
                #endif

                v.ase_normal = v.ase_normal;

                float3 positionWS = TransformObjectToWorld(v.vertex.xyz);
                float3 normalWS = TransformObjectToWorldNormal(v.ase_normal);

                o.clipPos = TransformWorldToHClip(positionWS);
                o.normalWS.xyz = normalWS;

                return o;
            }

            #if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
            #if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
            #elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
            #elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
            #elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
            #endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				
            #if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
            #endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
            #else
            VertexOutput vert(VertexInput v)
            {
                return VertexFunction(v);
            }
            #endif

            half4 frag(VertexOutput IN) : SV_TARGET
            {
                SurfaceDescription surfaceDescription = (SurfaceDescription)0;


                surfaceDescription.Alpha = 1;
                surfaceDescription.AlphaClipThreshold = 0.5;

                #if _ALPHATEST_ON
					clip(surfaceDescription.Alpha - surfaceDescription.AlphaClipThreshold);
                #endif

                #ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
                #endif

                float3 normalWS = IN.normalWS;

                return half4(NormalizeNormalPerPixel(normalWS), 0.0);
            }
            ENDHLSL
        }


        Pass
        {

            Name "DepthNormalsOnly"
            Tags
            {
                "LightMode"="DepthNormalsOnly"
            }

            ZTest LEqual
            ZWrite On

            HLSLPROGRAM
            #pragma multi_compile_instancing
            #define ASE_SRP_VERSION 120111


            #pragma exclude_renderers glcore gles gles3
            #pragma vertex vert
            #pragma fragment frag

            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS

            #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"


            struct VertexInput
            {
                float4 vertex : POSITION;
                float3 ase_normal : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
            {
                float4 clipPos : SV_POSITION;
                float3 normalWS : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _TextColor02;
                float4 _Scale_Offset;
                float4 _TextColor04;
                float4 _TextColor01;
                float4 _TextColor03;
                float _radius04_step;
                float _radius03_Step;
                float _TextSaturation;
                float _radius02_Step;
                float _Saturation;
                float _radius01;
                float _ColorHueRange1;
                float _AlphaRange;
                float _FirstLayerGray;
                float _TextSize;
                float _radius04_step02;
                float _radius04_step03;
                #ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
                #endif
            CBUFFER_END


            struct SurfaceDescription
            {
                float Alpha;
                float AlphaClipThreshold;
            };

            VertexOutput VertexFunction(VertexInput v)
            {
                VertexOutput o;
                    ZERO_INITIALIZE(VertexOutput, o);

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


                #ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
                #else
                float3 defaultVertexValue = float3(0, 0, 0);
                #endif

                float3 vertexValue = defaultVertexValue;

                #ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
                #else
                v.vertex.xyz += vertexValue;
                #endif

                v.ase_normal = v.ase_normal;

                float3 positionWS = TransformObjectToWorld(v.vertex.xyz);
                float3 normalWS = TransformObjectToWorldNormal(v.ase_normal);

                o.clipPos = TransformWorldToHClip(positionWS);
                o.normalWS.xyz = normalWS;

                return o;
            }

            #if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
            #if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
            #elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
            #elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
            #elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
            #endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				
            #if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
            #endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
            #else
            VertexOutput vert(VertexInput v)
            {
                return VertexFunction(v);
            }
            #endif

            half4 frag(VertexOutput IN) : SV_TARGET
            {
                SurfaceDescription surfaceDescription = (SurfaceDescription)0;


                surfaceDescription.Alpha = 1;
                surfaceDescription.AlphaClipThreshold = 0.5;

                #if _ALPHATEST_ON
					clip(surfaceDescription.Alpha - surfaceDescription.AlphaClipThreshold);
                #endif

                #ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
                #endif

                float3 normalWS = IN.normalWS;

                return half4(NormalizeNormalPerPixel(normalWS), 0.0);
            }
            ENDHLSL
        }

    }

    CustomEditor "UnityEditor.ShaderGraphUnlitGUI"
    FallBack "Hidden/Shader Graph/FallbackError"

    Fallback Off
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.CommentaryNode;197;-551.0389,686.0592;Inherit;False;741;323.9182;Comment;4;192;195;196;194;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;191;-4271.158,1393.707;Inherit;False;2458.807;637.9594;屏幕UV处理;17;47;46;53;51;50;73;75;76;67;68;69;72;65;70;71;52;66;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;190;-2516.998,3660.402;Inherit;False;1699.318;1569.787;Comment;25;141;142;144;148;147;149;92;150;152;153;154;155;156;157;84;159;162;163;164;165;166;167;34;169;160;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;189;-1840.48,971.7004;Inherit;False;790.6664;281.9999;文字贴图，R为描边，G为文字填充轮廓;2;29;28;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;188;-1292.987,1313.515;Inherit;False;801.2453;316.3734;第一层文字透明蒙版;3;175;176;177;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;140;-1851.985,634.7629;Inherit;False;798.1713;281.9097;屏幕图片;2;31;27;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;138;-429.5792,3173.821;Inherit;False;2025.947;1107.453;白环;20;106;130;125;131;133;127;114;136;113;116;129;128;134;135;115;112;137;119;120;121;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;124;-451.8345,1754.025;Inherit;False;2647.326;1278.448;渐变;26;80;81;83;87;90;91;82;95;62;79;78;96;102;89;97;63;104;93;94;88;33;77;86;151;161;170;;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;118;2446.464,3359.736;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;622.2908,2300.495;Inherit;False;2;2;0;INT;0;False;1;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.SaturateNode;81;859.2913,2424.295;Inherit;False;1;0;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;1205.174,2188.196;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;584.8148,2712.296;Inherit;False;2;2;0;INT;0;False;1;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.SaturateNode;91;821.8155,2836.096;Inherit;False;1;0;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.LerpOp;82;1582.49,2283.728;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;95;1929.824,2666.698;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;79;-401.8346,2499.142;Inherit;False;Property;_radius02_Step;radius02_Step;13;0;Create;True;0;0;0;False;0;False;1;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;96;11.14205,2407.345;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;97;-3.857948,2743.459;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;1167.698,2599.997;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;88;-364.3719,2782.591;Inherit;False;Property;_radius03_Step;radius03_Step;15;0;Create;True;0;0;0;False;0;False;0.5529412;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;882.6788,1874.412;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;77;1169.286,1840.001;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-1340.516,3222.574;Inherit;False;Constant;_Float1;Float 1;6;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-1120.161,3145.804;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;130;1076.848,3904.674;Inherit;False;2;2;0;INT;0;False;1;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;125;232.948,3911.274;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;131;430.9482,4148.274;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;114;751.0051,3532.215;Inherit;False;2;0;INT;0;False;1;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;136;766.7127,4013.758;Inherit;False;2;0;INT;0;False;1;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;116;109.9669,3638.37;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;128;61.44823,3900.274;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;134;196.4483,4109.274;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;135;-114.5978,4163.586;Inherit;False;Property;_radius04_step03;radius04_step03;19;0;Create;True;0;0;0;False;0;False;0;0.5;0;0.6;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;115;-68.53302,3582.37;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;137;52.12768,3445.091;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;119;1361.368,3264.214;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;120;1102.022,3223.821;Inherit;False;85;TextMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;121;1087.714,3312.61;Inherit;False;Property;_TextColor04;TextColor04;16;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenPosInputsNode;54;-1943.761,3111.692;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;57;-1746.515,3141.574;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;58;-1499.161,3146.804;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-1684.16,3251.804;Inherit;False;Constant;_Float2;Float 2;6;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;146;-1596.84,2970.192;Inherit;False;ScreenU;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;94;908.3513,2559.604;Inherit;False;85;TextMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;151;919.7377,2658.975;Inherit;False;150;Color03;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;161;912.9944,2250.872;Inherit;False;159;Color02;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;32;2837.757,1851.981;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;182;2594.639,1716.686;Inherit;False;181;ScreenColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RGBToHSVNode;141;-2002.357,4857.286;Inherit;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.HSVToRGBNode;142;-1343.286,4878.202;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;144;-1583.772,5033.662;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-1803.915,5097.187;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;147;-2339.643,5090.928;Inherit;False;146;ScreenU;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;149;-2113.085,5075.909;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;92;-2310.653,4844.614;Inherit;False;Property;_TextColor03;TextColor03;14;1;[HDR];Create;True;0;0;0;False;0;False;1,0,0.1481056,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;150;-1045.679,4888.51;Inherit;False;Color03;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RGBToHSVNode;152;-2014.191,4232.646;Inherit;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.HSVToRGBNode;153;-1355.121,4253.562;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;154;-1595.606,4409.022;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;155;-1815.749,4472.547;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;156;-2351.477,4466.289;Inherit;False;146;ScreenU;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;157;-2124.919,4451.269;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;84;-2352.667,4220.958;Inherit;False;Property;_TextColor02;TextColor02;12;1;[HDR];Create;True;0;0;0;False;0;False;0.6886792,0,0.1026268,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;159;-1057.513,4263.871;Inherit;False;Color02;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RGBToHSVNode;162;-2129.712,3772.599;Inherit;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;164;-1711.127,3948.975;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;165;-1931.27,4012.5;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;166;-2466.998,4006.241;Inherit;False;146;ScreenU;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;167;-2240.44,3991.221;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;34;-2396.082,3710.402;Inherit;False;Property;_TextColor01;TextColor01;10;1;[HDR];Create;True;0;0;0;False;0;False;1,0.2948235,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;169;-1173.035,3803.823;Inherit;False;Color01;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;160;-2263.545,4643.262;Inherit;False;Property;_ColorHueRange1;ColorHueRange;9;1;[Header];Create;True;1;ColorTex;0;0;False;0;False;0.1;0;-0.1;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;163;-1470.642,3793.515;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;47;-2098.686,1632.698;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-2305.825,1603.707;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;73;-3047.223,1828.869;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;75;-2877.222,1879.869;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;76;-2725.886,1898.666;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ScreenPosInputsNode;67;-4221.158,1642.177;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;68;-4023.913,1672.059;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;69;-3776.559,1677.289;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;72;-3961.558,1782.289;Inherit;False;Constant;_Float4;Float 2;6;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-3268.463,1806.816;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-3775.914,1806.059;Inherit;False;Constant;_Float3;Float 1;6;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;-3555.557,1729.289;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;170;632.8953,1920.62;Inherit;False;169;Color01;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;86;622.3987,1804.025;Inherit;False;85;TextMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;87;945.8271,2147.802;Inherit;False;85;TextMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;112;-379.5791,3636.681;Inherit;False;Property;_radius04_step;radius04_step;17;0;Create;True;0;0;0;False;0;False;0;0.5;0;0.6;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;129;-252.598,3932.585;Inherit;False;Property;_radius04_step02;radius04_step02;18;0;Create;True;0;0;0;False;0;False;0;0.5;0;0.6;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;-391.0172,2113.602;Inherit;False;Property;_radius01;radius01;11;0;Create;True;0;0;0;False;0;False;0.5529412;0.5;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-3616.851,1910.573;Inherit;False;Property;_TextSize;TextSize;4;1;[Header];Create;True;1;ScaleAndOffset;0;0;False;0;False;0.7;0;0.7;1.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;14;-747.545,498.1817;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;16;-68,470.9999;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;17;-68,470.9999;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;18;-68,470.9999;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;19;-68,470.9999;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;20;-68,470.9999;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;SceneSelectionPass;0;6;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;21;-68,470.9999;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ScenePickingPass;0;7;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;22;-68,470.9999;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormals;0;8;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;23;-68,470.9999;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormalsOnly;0;9;DepthNormalsOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;True;9;d3d11;metal;vulkan;xboxone;xboxseries;playstation;ps4;ps5;switch;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.SimpleAddOpNode;42;-821.6405,419.9917;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;43;-564.5468,490.4278;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;10;-1595.492,195.7032;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;41;-1460.871,99.03691;Inherit;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;0;False;0;False;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;40;-1235.871,180.0368;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;31;-1374.48,686.6725;Inherit;True;Property;_TextureSample2;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;27;-1801.984,684.7629;Inherit;True;Property;_MainTex;_MainTex;1;0;Create;True;0;0;0;False;0;False;a82618d78bf154746b708516f3bd2462;a82618d78bf154746b708516f3bd2462;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.OneMinusNode;180;-318.8447,578.2826;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;171;-779.9985,992.7583;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;29;-1370.48,1023.7;Inherit;True;Property;_TextureSample1;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;28;-1790.479,1021.7;Inherit;True;Property;_Text;Text;2;0;Create;True;0;0;0;False;0;False;4d426f5b7c7fa3c47b1cf8860e798d7a;6b62dd461403ffd41a9ce75575f7ea92;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleAddOpNode;46;-2090.686,1510.698;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-2316.825,1443.707;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;50;-1973.686,1572.698;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;26;-2016.128,457.8461;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;52;-2903.823,1541.707;Inherit;False;Property;_Scale_Offset;Scale_Offset;6;0;Create;True;0;0;0;False;0;False;1,1,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CustomExpressionNode;176;-885.1079,1399.033;Inherit;False;float mask=0@$mask = uv.x*uv.x + uv.y*uv.y@$mask = saturate(mask/radius)@$$return mask@;1;Create;2;True;uv;FLOAT2;0,0;In;;Half;False;True;radius;FLOAT;1;In;;Half;False;getColorBlack;True;False;0;;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;175;-1144.737,1363.515;Inherit;False;174;UV_Mid;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;177;-1242.987,1454.105;Inherit;False;Property;_AlphaRange;AlphaRange;7;1;[Header];Create;True;1;FirstLayer;0;0;False;0;False;1;1;0.1;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;35;-934.3109,909.3402;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;192;-497.4202,736.0591;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;195;-204.0383,841.9774;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;194;-50.03831,830.9774;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;181;630.9175,960.6069;Inherit;False;ScreenColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;186;-886.3956,1179.095;Inherit;False;Property;_FirstLayerGray;FirstLayerGray;8;0;Create;True;0;0;0;False;0;False;0.1;0;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;185;-568.1341,1118.133;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;85;-771.5308,896.1011;Inherit;False;TextMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;198;360.2069,661.1034;Inherit;False;Property;_BlakWhite;BlakWhite;21;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;196;-501.0388,882.9775;Inherit;False;Property;_Saturation;Saturation;20;1;[Header];Create;True;1;BlackMode;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;62;145.9918,2071.874;Inherit;False;int mask=0@$float x = uv.x*_ScreenParams.x/_ScreenParams.y@$mask = (x*x + uv.y*uv.y < radius*radius) ? 1 : 0@$$return mask@;0;Create;2;True;uv;FLOAT2;0,0;In;;Half;False;True;radius;FLOAT;1;In;;Half;False;circle;True;False;0;;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;INT;0
Node;AmplifyShaderEditor.CustomExpressionNode;89;369.9332,2732.994;Inherit;False;int mask=0@$float x = uv.x*_ScreenParams.x/_ScreenParams.y@$mask = (x*x + uv.y*uv.y < radius*radius) ? 1 : 0@$$return mask@;0;Create;2;True;uv;FLOAT2;0,0;In;;Half;False;True;radius;FLOAT;1;In;;Half;False;circle;True;False;0;;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;INT;0
Node;AmplifyShaderEditor.CustomExpressionNode;113;274.5125,3636.058;Inherit;False;int mask=0@$float x = uv.x*_ScreenParams.x/_ScreenParams.y@$mask = (x*x + uv.y*uv.y < radius*radius) ? 1 : 0@$$return mask@;0;Create;2;True;uv;FLOAT2;0,0;In;;Half;False;True;radius;FLOAT;1;In;;Half;False;circle;True;False;0;;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;INT;0
Node;AmplifyShaderEditor.CustomExpressionNode;127;445.4938,3903.962;Inherit;False;int mask=0@$float x = uv.x*_ScreenParams.x/_ScreenParams.y@$mask = (x*x + uv.y*uv.y < radius*radius) ? 1 : 0@$$return mask@;0;Create;2;True;uv;FLOAT2;0,0;In;;Half;False;True;radius;FLOAT;1;In;;Half;False;circle;True;False;0;;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;INT;0
Node;AmplifyShaderEditor.CustomExpressionNode;133;595.4941,4145.962;Inherit;False;int mask=0@$float x = uv.x*_ScreenParams.x/_ScreenParams.y@$mask = (x*x + uv.y*uv.y < radius*radius) ? 1 : 0@$$return mask@;0;Create;2;True;uv;FLOAT2;0,0;In;;Half;False;True;radius;FLOAT;1;In;;Half;False;circle;True;False;0;;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;INT;0
Node;AmplifyShaderEditor.LerpOp;184;818.7027,1283.548;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;187;1167.724,1452.509;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;173;1010.501,1362.896;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;37;-2737.831,129.41;Inherit;True;Property;_Text1;Text;3;0;Create;True;0;0;0;False;0;False;691e2c4e79098a547b6c5fd8b94ba9d6;691e2c4e79098a547b6c5fd8b94ba9d6;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.ScreenPosInputsNode;24;-2304.317,428.057;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;11;-1943.492,115.7033;Inherit;True;Global;_RenderOnTopDepthTexture;_RenderOnTopDepthTexture;0;0;Create;True;0;0;0;False;0;False;None;;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RegisterLocalVarNode;174;-987.8334,2886.807;Inherit;False;UV_Mid;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CustomExpressionNode;106;293.3352,3432.983;Inherit;False;int mask=0@$float x = uv.x*_ScreenParams.x/_ScreenParams.y@$mask = (x*x + uv.y*uv.y < radius*radius) ? 1 : 0@$$return mask@;0;Create;2;True;uv;FLOAT2;0,0;In;;Half;False;True;radius;FLOAT;1;In;;Half;False;circle;True;False;0;;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;INT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;104;213.951,2743.975;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;102;195.2899,2367.493;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;78;388.9521,2378.945;Inherit;False;int mask=0@$float x = uv.x*_ScreenParams.x/_ScreenParams.y@$mask = (x*x + uv.y*uv.y < radius*radius) ? 1 : 0@$$return mask@;0;Create;2;True;uv;FLOAT2;0,0;In;;Half;False;True;radius;FLOAT;1;In;;Half;False;circle;True;False;0;;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;INT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;199;-1588.66,5308.365;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;200;-1897.66,5336.365;Inherit;False;Property;_TextSaturation;TextSaturation;5;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;15;3140.748,1873.092;Float;False;True;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;TopText;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;8;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForwardOnly;False;False;0;;0;0;Standard;23;Surface;0;0;  Blend;0;0;Two Sided;1;0;Forward Only;0;0;Cast Shadows;1;0;  Use Shadow Threshold;0;0;Receive Shadows;1;0;GPU Instancing;1;0;LOD CrossFade;0;0;Built-in Fog;0;0;DOTS Instancing;0;0;Meta Pass;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,;0;  Type;0;0;  Tess;16,False,;0;  Min;10,False,;0;  Max;25,False,;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Vertex Position,InvertActionOnDeselection;1;0;0;10;False;True;True;True;False;False;True;True;True;True;False;;False;0
WireConnection;118;0;95;0
WireConnection;118;1;119;0
WireConnection;118;2;130;0
WireConnection;80;0;62;0
WireConnection;80;1;78;0
WireConnection;81;0;80;0
WireConnection;83;0;87;0
WireConnection;83;1;161;0
WireConnection;90;0;80;0
WireConnection;90;1;89;0
WireConnection;91;0;90;0
WireConnection;82;0;77;0
WireConnection;82;1;83;0
WireConnection;82;2;81;0
WireConnection;95;0;82;0
WireConnection;95;1;93;0
WireConnection;95;2;91;0
WireConnection;96;0;63;0
WireConnection;96;1;79;0
WireConnection;97;0;96;0
WireConnection;97;1;88;0
WireConnection;93;0;94;0
WireConnection;93;1;151;0
WireConnection;33;0;86;0
WireConnection;33;1;170;0
WireConnection;77;0;187;0
WireConnection;77;1;33;0
WireConnection;77;2;62;0
WireConnection;60;0;58;0
WireConnection;60;1;56;0
WireConnection;130;0;114;0
WireConnection;130;1;136;0
WireConnection;125;0;128;0
WireConnection;131;0;134;0
WireConnection;114;0;106;0
WireConnection;114;1;113;0
WireConnection;136;0;127;0
WireConnection;136;1;133;0
WireConnection;116;0;115;0
WireConnection;128;0;116;0
WireConnection;128;1;129;0
WireConnection;134;0;128;0
WireConnection;134;1;135;0
WireConnection;115;0;97;0
WireConnection;115;1;112;0
WireConnection;137;0;97;0
WireConnection;119;0;120;0
WireConnection;119;1;121;0
WireConnection;57;0;54;1
WireConnection;57;1;54;2
WireConnection;58;0;57;0
WireConnection;58;1;59;0
WireConnection;146;0;54;1
WireConnection;32;0;182;0
WireConnection;32;1;118;0
WireConnection;32;2;180;0
WireConnection;141;0;92;0
WireConnection;142;0;144;0
WireConnection;142;1;141;2
WireConnection;142;2;199;0
WireConnection;144;0;141;1
WireConnection;144;1;148;0
WireConnection;148;0;149;0
WireConnection;148;1;160;0
WireConnection;149;0;147;0
WireConnection;150;0;142;0
WireConnection;152;0;84;0
WireConnection;153;0;154;0
WireConnection;153;1;152;2
WireConnection;153;2;152;3
WireConnection;154;0;152;1
WireConnection;154;1;155;0
WireConnection;155;0;157;0
WireConnection;155;1;160;0
WireConnection;157;0;156;0
WireConnection;159;0;153;0
WireConnection;162;0;34;0
WireConnection;164;0;162;1
WireConnection;164;1;165;0
WireConnection;165;0;167;0
WireConnection;165;1;160;0
WireConnection;167;0;166;0
WireConnection;169;0;163;0
WireConnection;163;0;164;0
WireConnection;163;1;162;2
WireConnection;163;2;162;3
WireConnection;47;0;53;0
WireConnection;47;1;52;4
WireConnection;53;0;76;1
WireConnection;53;1;52;2
WireConnection;73;0;65;0
WireConnection;73;1;70;0
WireConnection;75;0;73;0
WireConnection;75;1;72;0
WireConnection;76;0;75;0
WireConnection;68;0;67;1
WireConnection;68;1;67;2
WireConnection;69;0;68;0
WireConnection;69;1;72;0
WireConnection;65;0;71;0
WireConnection;65;1;66;0
WireConnection;71;0;69;0
WireConnection;71;1;70;0
WireConnection;42;0;40;0
WireConnection;42;1;29;2
WireConnection;43;0;42;0
WireConnection;10;0;11;0
WireConnection;10;1;26;0
WireConnection;40;0;41;0
WireConnection;40;1;10;1
WireConnection;31;0;27;0
WireConnection;31;1;26;0
WireConnection;180;0;43;0
WireConnection;171;0;29;1
WireConnection;29;0;28;0
WireConnection;29;1;50;0
WireConnection;46;0;51;0
WireConnection;46;1;52;3
WireConnection;51;0;76;0
WireConnection;51;1;52;1
WireConnection;50;0;46;0
WireConnection;50;1;47;0
WireConnection;26;0;24;1
WireConnection;26;1;24;2
WireConnection;176;0;175;0
WireConnection;176;1;177;0
WireConnection;35;0;29;2
WireConnection;192;0;31;0
WireConnection;195;0;192;2
WireConnection;195;1;196;0
WireConnection;194;0;192;1
WireConnection;194;1;195;0
WireConnection;194;2;192;3
WireConnection;181;0;198;0
WireConnection;185;0;29;2
WireConnection;185;1;186;0
WireConnection;85;0;35;0
WireConnection;198;1;31;0
WireConnection;198;0;194;0
WireConnection;62;0;60;0
WireConnection;62;1;63;0
WireConnection;89;0;60;0
WireConnection;89;1;104;0
WireConnection;113;0;60;0
WireConnection;113;1;116;0
WireConnection;127;0;60;0
WireConnection;127;1;125;0
WireConnection;133;0;60;0
WireConnection;133;1;131;0
WireConnection;184;0;181;0
WireConnection;184;1;185;0
WireConnection;184;2;176;0
WireConnection;187;0;173;0
WireConnection;173;0;171;0
WireConnection;173;1;184;0
WireConnection;174;0;60;0
WireConnection;106;0;60;0
WireConnection;106;1;137;0
WireConnection;104;0;97;0
WireConnection;102;0;96;0
WireConnection;78;0;60;0
WireConnection;78;1;102;0
WireConnection;199;0;141;3
WireConnection;199;1;200;0
WireConnection;15;2;32;0
ASEEND*/
//CHKSM=E134AE222EFE94978B4FDDF7E56F11532704CB7C