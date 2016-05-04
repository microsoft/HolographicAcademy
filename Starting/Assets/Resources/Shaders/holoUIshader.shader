// A shader for simple additive UIs can turn backface culling off or on. Does not accept any lighting or normals.
Shader "HoloLens/holoUIshader"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "black" {}
        _Color("Color", Color) = (1.0,1.0,1.0,1.0)
        _Tint("Color Tint", Color) = (1.0,1.0,1.0,1.0)
        _Brightness("Brightness", Float) = 1.0
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "False" "RenderType" = "Transparent"}
        LOD 100
        ZWrite Off
        Lighting Off
        Blend One One
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                half2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4  _Tint;
            float1  _Brightness;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord) + _Color;

                return fixed4(col.r*_Brightness, col.g*_Brightness, col.b*_Brightness, col.a*_Brightness)*_Tint;
            }
            ENDCG
        }
    }
}