
//Simple texture blending shader. To be used with buttons or UIs where a smooth transition between states is needed.
//Not affected by lighting.

Shader "Custom/UIBlender"
{
    Properties
    {
        _MainTex("Tx 01", 2D) = "black" {}
        _BlendTex01("Blend Between Tx 01 and 02", Range(0, 1)) = 0.0
        _Texture02("Tx 02", 2D) = "black" {}
        _BlendTex02("Blend Between Tx 02 and 03", Range(0, 1)) = 0.0
        _Texture03("Tx 03", 2D) = "black" {}
        _BlendTex03("Blend Between Tx 03 and 04", Range(0, 1)) = 0.0
        _Texture04("Tx 04", 2D) = "black" {}
    }
    SubShader
    {
        //Set shader to act as a transparent material.
        Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        LOD 100
        ZWrite Off
        Lighting Off
        Blend One One
        Cull Off

        Pass
        {
            // Start loading textures.
            SetTexture[_MainTex]
            {
            }

            SetTexture[_Texture02]
            {
                ConstantColor(1,1,1,[_BlendTex01])
                Combine texture Lerp(constant) previous
            }

            SetTexture[_Texture03]
            {
                ConstantColor(1,1,1,[_BlendTex02])
                Combine texture Lerp(constant) previous
            }

            SetTexture[_Texture04]
            {
                ConstantColor(1,1,1,[_BlendTex03])
                Combine texture Lerp(constant) previous
            }
        }
    }
}