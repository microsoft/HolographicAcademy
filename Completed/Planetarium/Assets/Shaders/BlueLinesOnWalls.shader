Shader "Custom/BlueLinesOnWalls"
{
	Properties
	{
		_LineScale("LineScale", Float) = 0.1
		_LinesPerMeter("LinesPerMeter", Float) = 4
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			// These values map from the properties block at the beginning of the shader file.
			// They can be set at run time using renderer.material.SetFloat()
			float _LineScale;
			float _LinesPerMeter;
			
			// This is the data structure that the vertex program provides to the fragment program.
			struct VertToFrag
			{
				float4 viewPos : SV_POSITION;
				float3 normal : NORMAL;
				float4 worldPos: TEXCOORD0;
			};


			// This is the vertex program.
			VertToFrag vert (appdata_base v)
			{
				VertToFrag o;

				// Calculate where the vertex is in view space.
				o.viewPos = UnityObjectToClipPos(v.vertex);

				// Calculate the normal in WorldSpace.
				o.normal = UnityObjectToWorldNormal(v.normal);

				// Calculate where the object is in world space.
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);

				return o;
			}

			fixed4 frag (VertToFrag input) : SV_Target
			{
				// Check where this pixel is in world space.
				// wpmod is documented on the internet, it's basically a 
				// floating point mod function.
				float4 wpmodip;
				float4 wpmod = modf(input.worldPos * _LinesPerMeter, wpmodip);

				// Initialize to draw black with full alpha. This way we will occlude holograms even when
				// we are drawing black.
				fixed4 ret = float4(0,0,0,1);

				// Normals need to be renormalized in the fragment shader to overcome 
				// interpolation.
				float3 normal = normalize(input.normal);

				// If the normal isn't pointing very much up or down and the position in world space
				// is within where a line should be drawn, draw the line.
				// Since we are checking wpmod.y, we will be making horizontal blue lines.
				// If wpmod.y was replaced with wpmod.x or wpmod.z, we would be making vertical lines.
				if (abs(normal.y) < 0.2f && abs(wpmod.y) < _LineScale* _LinesPerMeter)
				{
					ret.b = 1 - (abs(wpmod.y) / (_LineScale* _LinesPerMeter));
					ret.r = 0;
					ret.g = 0;
				}

				return ret;
			}
			ENDCG
		}
	}
}