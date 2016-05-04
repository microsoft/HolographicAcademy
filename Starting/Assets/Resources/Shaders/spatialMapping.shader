//
// Copyright (C) Microsoft. All rights reserved.
//

/* 151030 - Currently only used for look dev purposes */

Shader "Surface Reconstruction/Spatial Mapping"
{
    Properties
    {
        _BaseColor("Base color", Color) = (0.0, 0.0, 0.0, 1.0)
        _WireColor("Wire color", Color) = (1.0, 1.0, 1.0, 1.0)
        _WireThickness("Wire thickness", Range(0, 800)) = 100
        _PointColor("Point color", Color) = (0.0, 0.0, 0.0, 1.0)
        _Sprite("Sprite", 2D) = "white" {}
        _PointSize("Point size", Range(0, 800)) = 100
    }
    SubShader
    {
        //Lighting Off
        Tags { "RenderType" = "Opaque" }

        //FILL PASS
        //Dynamic fill color, UV Texture, Wireframe color,
        //and wireframe thickness
        Pass
        {
            Name "Fill"
            Offset 50, 100
            //Tags { "LightMode" = "ForwardAdd" }

            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _BaseColor;
            float4 _WireColor;
            float _WireThickness;
            sampler2D _Sprite;

            struct v2g
            {
                float4 viewPos : SV_POSITION;
            };

            v2g vert(appdata_base v)
            {
                v2g o;
                o.viewPos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }

            // inverseW is to counter-act the effect of perspective-correct interpolation so that the lines look the same thickness
            // regardless of their depth in the scene.
            struct g2f
            {
                float4 viewPos : SV_POSITION;
                float inverseW : TEXCOORD0;
                float3 dist : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            // GEOMETERY SHADER -----------------------------------------------------------------

            [maxvertexcount(3)]
            void geom(triangle v2g i[3], inout TriangleStream<g2f> triStream)
            {
                // Calculate the vectors that define the triangle from the input points.
                float2 point0 = i[0].viewPos.xy / i[0].viewPos.w;
                float2 point1 = i[1].viewPos.xy / i[1].viewPos.w;
                float2 point2 = i[2].viewPos.xy / i[2].viewPos.w;

                // Calculate the area of the triangle.
                float2 vector0 = point2 - point1;
                float2 vector1 = point2 - point0;
                float2 vector2 = point1 - point0;
                float area = abs(vector1.x * vector2.y - vector1.y * vector2.x);

                float wireScale = 800 - _WireThickness;

                // Output each original vertex with its distance to the opposing line defined
                // by the other two vertices.

                g2f o;

                o.viewPos = i[0].viewPos;
                o.inverseW = 1.0 / o.viewPos.w;
                o.uv = float2(0,0);
                o.dist = float3(area / length(vector0), 0, 0) * o.viewPos.w * wireScale;
                triStream.Append(o);

                o.viewPos = i[1].viewPos;
                o.inverseW = 1.0 / o.viewPos.w;
                o.uv = float2(1,0);
                o.dist = float3(0, area / length(vector1), 0) * o.viewPos.w * wireScale;
                triStream.Append(o);

                o.viewPos = i[2].viewPos;
                o.inverseW = 1.0 / o.viewPos.w;
                o.uv = float2(0,1);
                o.dist = float3(0, 0, area / length(vector2)) * o.viewPos.w * wireScale;
                triStream.Append(o);
            }

            //FRAGMENT SHADER -----------------------------------------------------------------

            float4 frag(g2f i) : COLOR
            {
                // Calculate  minimum distance to one of the triangle lines, making sure to correct
                // for perspective-correct interpolation.
                float dist = min(i.dist[0], min(i.dist[1], i.dist[2])) * i.inverseW;

                // Make the intensity of the line very bright along the triangle edges but fall-off very
                // quickly.
                float I = exp2(-2 * dist * dist);

                // Fade out the alpha but not the color so we don't get any weird halo effects from
                // a fade to a different color.
                float4 color = I * _WireColor + (1 - I) * _BaseColor;
                color.a = I;

                return tex2D(_Sprite, i.uv) * color; // <-- Uses selected texture or color
                //return color;
            }
            ENDCG
        }

        /*
        //POINT PASS
        //Generate and center geo at each vertex
        //Geo should be capable of global resize but maintain
        //size over distance

        Pass
        {
            Name "Points"
            Offset 50, 100

            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #include "UnityCG.cginc"

            // VARS  -----------------------------------------------------------------
            float4 _PointColor;
            float _PointSize;

            // DATA STRUCTURES  ------------------------------------------------------
            struct v2g
            {
                //float4 viewPos : SV_POSITION;
                half4 Pos        : SV_POSITION;
                half2 UVStart    : TEXCOORD0;
                half2 UVLen      : TEXCOORD1;
                half2 TileLen    : TEXCOORD2;
                half4 Face		 : TEXCOORD3;
            };

            // inverseW is to counteract the effect of perspective-correct interpolation so that the lines look the same thickness
            // regardless of their depth in the scene.
            struct g2f
            {
                //float4 viewPos : SV_POSITION;
                //float inverseW : TEXCOORD0;
                // float3 dist : TEXCOORD1;
                half4 Pos   : SV_POSITION;
                half4 UV    : TEXCOORD0;
                half2 UVLen : TEXCOORD2;
            };

            // VERTEX SHADER -----------------------------------------------------------------

            //v2g vert(appdata_base v)
            //{
            //    v2g o;
            //    o.viewPos = mul(UNITY_MATRIX_MVP, v.vertex);
            //    return o;
            //}

            g2f vert(v2g v)
            {
                //v2g o;
                //o.viewPos = v.Pos;
                g2f o = (g2f)0;

                o.Pos  = v.Pos;
                //o.viewPos = v.Pos;
                //o.UV = half4(v.UVStart.xy, v.UVLen.xy);
                //o.Face = half4(v.TileLen.xy, v.SizeDir.xy);

                return o;
            }

            // GEOMETERY SHADER -----------------------------------------------------------------
            // Create a square at every vertex

            [maxvertexcount(4)]

            void geom(point v2g p[1], inout TriangleStream<g2f> triStream)
            {
                half4 pos = p[0].Pos;
                float halfSize = .01;
                half3 right = half3(1,0,0);
                half3 up = half3(0,1,0);

                // define the corner positions square
                half4 v[4];

                v[0] = half4(pos - (halfSize * right) - (halfSize * up), 1.0f); // 1 left bottom
                v[1] = half4(pos - (halfSize * right) + (halfSize * up), 1.0f); // 2 left top
                v[2] = half4(pos + (halfSize * right) - (halfSize * up), 1.0f); // 3 right bottom
                v[3] = half4(pos + (halfSize * right) + (halfSize * up), 1.0f); // 4 right top

                // create the vertices, passing in everything fragment shader needs to calculate fragment UV pos
                g2f o = (g2f)0;
                o.Pos = mul(UNITY_MATRIX_MVP, v[0]); // left bottom
                //o.UV  = p[0].UV;
                //o.UVLen = 0.5;
                triStream.Append(o);

                o.Pos = mul(UNITY_MATRIX_MVP, v[1]); // left top
                //o.UV  = p[0].UV;
                //o.UVLen = 0.5;
                triStream.Append(o);

                o.Pos = mul(UNITY_MATRIX_MVP, v[2]); // right bottom
                //o.UV  = p[0].UV;
                //o.UVLen = 0.5;
                triStream.Append(o);

                o.Pos = mul(UNITY_MATRIX_MVP, v[3]); // right top
                // o.UV  = p[0].UV;
                //o.UVLen = 0.5;
                triStream.Append(o);
            }

            //FRAGMENT SHADER -----------------------------------------------------------------

            float4 frag(g2f i) : COLOR
            {
                // Calculate  minimum distance to one of the triangle lines, making sure to correct
                // for perspective-correct interpolation.
                //float dist = min(i.dist[0], min(i.dist[1], i.dist[2])) * i.inverseW;

                // Make the intensity of the line very bright along the triangle edges but fall-off very
                // quickly.
                //float I = exp2(-2 * dist * dist);

                // Fade out the alpha but not the color so we don't get any weird halo effects from
                // a fade to a different color.

                //float4 color = I * _PointColor;
                float4 color = _PointColor;
                //color.a = I;

                return color;
            }
            ENDCG
        }

        */

        //SQ Points HACK

        //Fast Hack - For Look Dev
        //Only resizes triangle to simulate result for look dev

        Pass
        {
            Name "Points"
            Offset 50, 100

            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _PointColor;
            float _PointSize;

            // Based on approach described in "Shader-Based Wireframe Drawing", http://cgg-journal.com/2008-2/06/index.html

            struct v2g
            {
                float4 viewPos : SV_POSITION;
            };

            v2g vert(appdata_base v)
            {
                v2g o;
                o.viewPos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }

            // inverseW is to counter-act the effect of perspective-correct interpolation so that the lines look the same thickness
            // regardless of their depth in the scene.
            struct g2f
            {
                float4 viewPos : SV_POSITION;
                float inverseW : TEXCOORD0;
                float3 dist : TEXCOORD1;
            };

            [maxvertexcount(3)]

            //Fast hack
            void geom(triangle v2g i[3], inout TriangleStream<g2f> triStream)
            {
                float2 point0 = i[0].viewPos.xy / i[0].viewPos.w;
                float2 point1 = i[1].viewPos.xy / i[1].viewPos.w;
                float2 point2 = i[2].viewPos.xy / i[2].viewPos.w;

                // Calculate the area of the triangle.
                float2 vector0 = point2 - point1;
                float2 vector1 = point2 - point0;
                float2 vector2 = point1 - point0;
                float area = abs(vector1.x * vector2.y - vector1.y * vector2.x);

                float pointScale = 800 - _PointSize;

                // Output each original vertex with its distance to the opposing line defined
                // by the other two vertices.

                g2f o;

                //o.viewPos = i[0].viewPos+(i[0].viewPos);
                o.viewPos = i[0].viewPos;
                o.inverseW = 1.0 / o.viewPos.w;
                o.dist = float3(area / length(vector0), 0, 0) * o.viewPos.w * pointScale;
                triStream.Append(o);

                //o.viewPos = i[1].viewPos;
                o.viewPos = i[1].viewPos / 8 + (i[0].viewPos / 1);
                o.inverseW = 1.0 / o.viewPos.w;
                o.dist = float3(0, area / length(vector1), 0) * o.viewPos.w * pointScale;
                triStream.Append(o);

                //o.viewPos = i[2].viewPos;
                //o.viewPos = i[2].viewPos+(i[1].viewPos)+(i[0].viewPos);
                o.viewPos = i[2].viewPos / 8 + (i[0].viewPos);
                o.inverseW = 1.0 / o.viewPos.w;
                o.dist = float3(0, 0, area / length(vector2)) * o.viewPos.w * pointScale;
                triStream.Append(o);
            }

            float4 frag(g2f i) : COLOR
            {
                // Calculate  minimum distance to one of the triangle lines, making sure to correct
                // for perspective-correct interpolation.
                //float dist = min(i.dist[0], min(i.dist[1], i.dist[2])) * i.inverseW;
                //float dist = 110;

                // Make the intensity of the line very bright along the triangle edges but fall-off very
                // quickly.
                // float I = exp2(-2 * dist * dist);

                // Fade out the alpha but not the color so we don't get any weird halo effects from
                // a fade to a different color.
                //float4 color = I * _PointColor;
                float4 color = _PointColor;
                //color.a = I;

                return color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}