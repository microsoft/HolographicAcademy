Shader "HoloLens/holoVertex"
{
    SubShader
    {
        Pass
        {
            LOD 200

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct VertexInput
            {
                float4 v : POSITION;
                float4 color: COLOR;
            };

            struct VertexOutput
            {
                float4 pos : SV_POSITION;
                float4 col : COLOR;
            };

            VertexOutput vert(VertexInput v)
            {
                VertexOutput o;
                o.pos = mul(UNITY_MATRIX_MVP, v.v);
                o.col = v.color;

                return o;
            }

            float4 frag(VertexOutput o) : COLOR
            {
                return o.col;
            }
            ENDCG
        }
    }
}