// Simple texture blending shader. To be used with vbuttons or UIs where a smooth transition between states is needed.
// Not affected by lighting.

Shader "textureBlender"
{
    Properties
    {
        _MainTex("Tx 01", 2D) = "black" {}
        _Texture02("Tx 02", 2D) = "black" {}
        _Texture03("Tx 03", 2D) = "black" {}
        _BlendTex01("Blend Between Tx 01 and 02", Range(0, 1)) = 0.0
        _BlendTex02("Blend Between Tx 02 and 03", Range(0, 1)) = 0.0
    }
    SubShader
    {
        //Set shader to act as a transparent material.
        Tags{ "Queue" = "Transparent" }
        LOD 300

        Pass
        {
            //None additive
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            // Start loading textures.
            SetTexture[_MainTex]
            SetTexture[_Texture02]
            {
                ConstantColor(0,0,0,[_BlendTex01])
                Combine texture Lerp(constant) previous
            }
            SetTexture[_Texture03]
            {
                ConstantColor(0,0,0,[_BlendTex02])
                Combine texture Lerp(constant) previous
            }
        }
    }
}