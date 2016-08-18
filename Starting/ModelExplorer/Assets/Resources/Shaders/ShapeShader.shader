Shader "HoloLens/HoloShape"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "black" {}
        [HDR]_outlineColor("Outline Color", Color) = (1,1,1,1)
        [HDR]_fillColor("Fill Color", Color) = (1,1,1,1)
        [HDR]_fillShadingColor("Shading Color", Color) = (1,1,1,1)
        _Dist("Shift", float) = 0.0000
    }
    SubShader
    {
        /// first pass
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha // Alpha blending
        Lighting Off
        ZWrite On
        ZTest Less
        Cull Front

        CGPROGRAM
        #pragma surface surf StandardSpecular fullforwardshadows addshadow alphatest:_Cutoff vertex:vert
        //#pragma target 3.0
        #include "UnityCG.cginc"

        float4 _fillColor;
        float _Dist;
        float4 _fillShadingColor;

        struct Input
        {
            float2 uv_MainTex;
        };

        void vert(inout appdata_full v)
        {
            v.vertex.xyz += float3(v.normal.xyz)*_Dist*0.0005*-1;
        }

        void surf(Input i, inout SurfaceOutputStandardSpecular o)
        {
            o.Emission = _fillColor.rgb;
            o.Albedo = _fillShadingColor.rgb;
            o.Alpha = _fillColor.a;
        }
        ENDCG

        Tags { "Queue" = "Transparent" "IgnoreProjector" = "False" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha // Alpha blending
        Lighting Off
        ZWrite On
        Cull Front

        CGPROGRAM
        #pragma surface surf StandardSpecular fullforwardshadows addshadow alphatest:_Cutoff
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        float4 _outlineColor;
        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input i, inout SurfaceOutputStandardSpecular o)
        {
            fixed4 tex = tex2D(_MainTex, i.uv_MainTex);
            o.Emission = _outlineColor.rgb;
            o.Alpha = _outlineColor.a;
        }
        ENDCG
    }
}