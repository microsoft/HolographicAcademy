//Very fast shader that uses the Unity lighting model
//Compiles down to only performing the operations you're actually using
//Does not currently support stereo instancing
//Creates a blue rim effect when the hologram is occluded.

Shader "Custom/OccusionRim" 
{
	Properties
	{
		[Header(Occlusion Rim)]
		[Toggle] _UseOcclusionRim("Enabled?", Float) = 1
		_RimColor("Rim Color", Color) = (1, 0, 0, 1)
		_RimPower("Rim Power", Float) = 3.0
		[Space(20)]

		[Header(Main Color)]
		[Toggle] _UseColor("Enabled?", Float) = 0
		_Color("Main Color", Color) = (1,1,1,1)
		[Space(20)]

		[Header(Base(RGB))]
		[Toggle] _UseMainTex("Enabled?", Float) = 1
		_MainTex("Base (RGB)", 2D) = "white" {}
		[Space(20)]

		//uses UV scale, etc from main texture
		[Header(Normalmap)]
		[Toggle] _UseBumpMap("Enabled?", Float) = 0
			[NoScaleOffset] _BumpMap("Normalmap", 2D) = "bump" {}
		[Space(20)]

		//uses UV scale, etc from main texture	
		[Header(Emission(RGB))]
		[Toggle] _UseEmissionTex("Enabled?", Float) = 0
			[NoScaleOffset] _EmissionTex("Emission (RGB)", 2D) = "white" {}
		[Space(20)]

		[Header(Blend State)]
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("SrcBlend", Float) = 1 //"One"
			[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("DestBlend", Float) = 0 //"Zero"
			[Space(20)]

		[Header(Other)]
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull", Float) = 2 //"Back"
			[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4 //"LessEqual"
		}

			SubShader
		{
			Pass
			{
				Name "OcclusionRim"
				Tags
				{
					"RenderType" = "transparent"
					"Queue" = "Transparent"
				}
				Blend SrcAlpha OneMinusSrcAlpha
				ZTest Greater // There is something in front of this object.
				ZWrite Off
				Cull Back
				LOD 200

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _RimColor;
				float _RimPower;

				struct v2f
				{
					float4 viewPos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float3 normal : TEXCOORD1;
					float3 viewDir : TEXCOORD2;
				};

				v2f vert(appdata_tan v)
				{
					v2f o;
					o.viewPos = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.normal = UnityObjectToWorldNormal(v.normal);
					o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
					return o;
				}

				half4 frag(v2f i) : COLOR
				{
					half Rim = 1 - saturate(dot(normalize(i.viewDir), i.normal));
					half4 RimOut = _RimColor * pow(Rim, _RimPower);
					return RimOut;
				}
				ENDCG
			}

			Tags{ "RenderType" = "Opaque" }
			Blend[_SrcBlend][_DstBlend]
			ZTest[_ZTest]
			Cull[_Cull]
			LOD 300

			CGPROGRAM
			#pragma shader_feature _USECOLOR_ON
			#pragma shader_feature _USEMAINTEX_ON
			#pragma shader_feature _USEBUMPMAP_ON
			#pragma shader_feature _USEEMISSIONTEX_ON

			#pragma surface surf Lambert noforwardadd

			//we only target the hololens (and the unity editor) so take advantage of shader model 5
			#pragma target 5.0
			#pragma only_renderers d3d11

#if _USEMAINTEX_ON
			sampler2D _MainTex;
#endif

#if _USECOLOR_ON		
		fixed4 _Color;
#endif

#if _USEBUMPMAP_ON
		sampler2D _BumpMap;
#endif

#if _USEEMISSIONTEX_ON
		sampler2D _EmissionTex;
#endif

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c;

#if _USEMAINTEX_ON
			c = tex2D(_MainTex, IN.uv_MainTex);
#else
			c = 1;
#endif

#if _USECOLOR_ON
			c *= _Color;
#endif

			o.Albedo = c.rgb;
			o.Alpha = c.a;

#if _USEBUMPMAP_ON
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
#endif

#if _USEEMISSIONTEX_ON
			o.Emission = tex2D(_EmissionTex, IN.uv_MainTex);
#endif
		}
		ENDCG
		}

			FallBack "Legacy Shaders/Diffuse"
	}