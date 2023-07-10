Shader "Custom/BGTutorialFillShader"{
    Properties{
        _MainTex ("Texture", 2D) = "white" {}
        _ColorTopRight ("_ColorTopRight", Color) = (1, 0.5, 1, 1)
        _ColorTopLeft ("_ColorTopLeft", Color) = (0.5, 1, 0.5, 1)
        _ColorBot ("_ColorBot", Color) = (0.5, 1, 1, 1)

        _Selected ("_Selected", Integer) = -1 // from commander, clockwise
        _SelectedColor("_SelectedColor", Color) = (1, 1, 1, 1)
        _Radius("_Radius", Float) = 1

    }
    SubShader {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always Blend SrcAlpha OneMinusSrcAlpha

        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float3 world_pos : TEXCOORD0;
            };

            uniform float4 _ColorTopRight;
            uniform float4 _ColorTopLeft;
            uniform float4 _ColorBot;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.world_pos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            #define THIRD 0.333333333333333334f

            fixed4 frag (v2f i) : SV_Target {
                const float3 w_pos = normalize(i.world_pos);
                const float dp = dot(w_pos, float3(0, 1, 0));
                const float angle = acos(dp) * UNITY_INV_PI;
                if (angle <= 2 * THIRD) {
                    return i.world_pos.x > 0 ? _ColorTopRight : _ColorTopLeft;
                }
                return _ColorBot;
            }
            ENDCG
        }

        // -------------- Triangle pressed down effect --------------
        Pass {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float3 world_pos : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };


            uniform float _Selected;
            uniform float4 _SelectedColor;
            uniform float _Radius;


            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.world_pos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.uv;
                return o;
            }

            #define THIRD 0.3333333333333334f

            float3 rotateBy(float3 v, float angle) {
                float3x3 rot = float3x3(float3(cos(angle), -sin(angle), 0), float3(sin(angle), cos(angle), 0), float3(0, 0, 1));
                return normalize(mul(rot, v));
            }

            fixed4 frag (const v2f i) : SV_Target {
                if (_Selected < 0 || _Selected > 2) {
                    return 0;
                }
                const float3 w_pos = normalize(i.world_pos);

                // angles of the two borders of the triangle
                const float alpha = _Selected * UNITY_TWO_PI * THIRD;
                const float beta = (_Selected + 1) * UNITY_TWO_PI * THIRD;
                // vectors of those borders
                const float3 dir1 = normalize(rotateBy(float3(0, 1, 0), alpha));
                const float3 dir2 = normalize(rotateBy(float3(0, 1, 0), beta));
                // make sure they're valid
                if (all(dir1 == dir2)) {
                    return 0;
                }
                // measure the pixel's worldPos angle from each of the vectors
                // using dot products, and compare it to the dot of the two directions.
                const float dp1 = dot(w_pos, dir1);
                const float dp2 = dot(w_pos, dir2);
                const float dp12 = dot(dir1, dir2);

                if (dp1 < dp12 || dp2 < dp12) {
                    return 0;
                }
                float3 proj = dot(i.world_pos, dir1) / dot(dir1, dir1) * dir1;
                float3 rej = i.world_pos - proj;
                if (length(rej) < _Radius) {
                    float4 col = _SelectedColor;
                    // enable this line for fade effect
                    // col.a *= 1 - (length(rej) / _Radius);
                    return col;
                }
                proj = dot(i.world_pos, dir2) / dot(dir2, dir2) * dir2;
                rej = i.world_pos - proj;
                if (length(rej) < _Radius) {
                    float4 col = _SelectedColor;
                    // enable this line for fade effect
                    // col.a *= 1 - (length(rej) / _Radius);
                    return col;
                }
                return 0;
            }
            ENDCG
        }
    }
}