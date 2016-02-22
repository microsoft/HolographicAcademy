Shader "Custom/additiveRimShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "black" {}
		_BumpMap("Bumpmap", 2D) = "bump" {}
		
		_RimColor("Rim Color", Color) = (0.62, 0.0, 1.0, 0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0
		
		_TintColor("Tint Color", Color) = (0.0, 0.0, 0.0, 0.0)
		_TintPower("Tint Power", Range(0.0, 1.0)) = 0.25
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100
		ZWrite Off
		Lighting Off
		Blend One One
		CGPROGRAM

#pragma surface surf Lambert
		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;
		};

		sampler2D _MainTex;
		sampler2D _BumpMap;
		
		float4 _RimColor;
		float _RimPower;
		
		float4 _TintColor;
		float _TintPower;

		void surf(Input IN, inout SurfaceOutput o) 
		{
			o.Albedo = lerp(tex2D(_MainTex, IN.uv_MainTex), _TintColor, _TintPower).rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			half rim = 1 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);
		}

		ENDCG
	}
	Fallback "Diffuse"
}

