// Fast Vertex->Fragment shader which only renders occluded pixels.
// This shader can apply a color to the texture during rendering, but it will use the alpha channel set by the texture.
// This shader does not respond to lighting or shadows, so use with simple objects that do not need these effects.
Shader "Custom/OcclusionGrid" 
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1) // Color to apply when rendering
		_MainTex("Base (RGB)", 2D) = "white" {} // Texture to apply when rendering
	}

	SubShader
	{
		Tags{"Queue" = "Transparent" "RenderType" = "Transparent"}
		ZWrite Off // Turn ZWrite off to ignore the depth buffer, use ZTest to determine if the pixel should be rendered.
		ZTest Greater // 'Greater' will only render pixels that are occluded.
		Blend SrcAlpha OneMinusSrcAlpha // Alpha blending.
		LOD 80 // Level of detail.

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex; // Main texture that was set in the Unity inspector.
			float4 _Color; // Additional color that was set in the Unity inspector.
			float4 _MainTex_ST; // Used by TRANSFORM_TEX to hold texture scale and offset.

			struct v2f
			{
				float4 pos : SV_POSITION; // Screen position of the vertex.
				float2 uv : TEXCOORD0; // Texture coordinates that map to the current vertex.
			};

			// Finds position and texture coordinates for each vertex.
			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex); // Converts from model to screen space.
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex); // Finds the texture coordinates which map to this vertex.
				return o;
			}

			// Sets pixel color for rendering.
			half4 frag(v2f i) : COLOR
			{
				// Get the pixel's color from the texture.
				half4 col = tex2D(_MainTex, i.uv);
				// Apply additional RGB color to the texture, but keep the alpha channel the same for transparency. 
				col.rgb *= _Color.rgb;
				return col;
			}

			ENDCG
		}
	}
}