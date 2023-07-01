Shader "Custom/BGFillShader"{
    Properties{
        _MainTex ("Texture", 2D) = "white" {}
        _ColorBotRight ("_ColorBotRight", Color) = (1, 1, 0.5, 1)
        _ColorTopRight ("_ColorTopRight", Color) = (1, 0.5, 1, 1)
        _ColorTopLeft ("_ColorTopLeft", Color) = (0.5, 1, 0.5, 1)
        _ColorBotLeft ("_ColorBotLeft", Color) = (0.5, 0.5, 1, 1)
        _ColorRight ("_ColorRight", Color) = (0.5, 1, 1, 1)
        _ColorLeft ("_ColorLeft", Color) = (1, 0.5, 0.5, 1)

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

            uniform float4 _ColorBotRight;
            uniform float4 _ColorTopRight;
            uniform float4 _ColorTopLeft;
            uniform float4 _ColorBotLeft;
            uniform float4 _ColorRight;
            uniform float4 _ColorLeft;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.world_pos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            #define SIXTH 0.1666666666666667f

            fixed4 frag (v2f i) : SV_Target {
                const float3 w_pos = normalize(i.world_pos);
                const float dp = dot(w_pos, float3(1, 0, 0));
                const float angle = acos(dp) * UNITY_INV_PI;
                if (dp <= -1) {
                    return _ColorLeft;
                }
                if (dp >= 1) {
                    return _ColorRight;
                }
                if (angle <= SIXTH) {
                    return _ColorRight;
                }
                if (SIXTH <= angle && angle <= 0.5f) {
                    if (i.world_pos.y > 0) {
                        return _ColorTopRight;
                    }
                    return _ColorBotRight;
                }
                if (0.5f <= angle && angle <= 1 - SIXTH) {
                    if (i.world_pos.y > 0) {
                        return _ColorTopLeft;
                    }
                    return _ColorBotLeft;
                }
                if (1 - SIXTH <= angle) {
                    return _ColorLeft;
                }
                return 0;
            }
            ENDCG
        }

        // Pass {

        //     CGPROGRAM
        //     #pragma vertex vert
        //     #pragma fragment frag
            
        //     #include "UnityCG.cginc"

        //     struct appdata {
        //         float4 vertex : POSITION;
        //         float2 uv : TEXCOORD0;
        //     };

        //     struct v2f {
        //         float4 vertex : SV_POSITION;
        //         float3 world_pos : TEXCOORD0;
        //         float2 uv : TEXCOORD1;
        //     };


        //     uniform float _Selected;
        //     uniform float4 _SelectedColor;
        //     uniform float _Radius;


        //     v2f vert (appdata v) {
        //         v2f o;
        //         o.vertex = UnityObjectToClipPos(v.vertex);
        //         o.world_pos = mul(unity_ObjectToWorld, v.vertex);
        //         o.uv = v.uv;
        //         return o;
        //     }

        //     #define SIXTH 0.1666666666666667f

        //     fixed4 frag (v2f i) : SV_Target {
        //         if (_Selected < 0 || _Selected > 5) {
        //             return 0;
        //         }
        //         const float3 w_pos = normalize(i.world_pos);
        //         const float dp = dot(w_pos, float3(1, 0, 0));
        //         const float angle = acos(dp) * UNITY_INV_PI;
                
                
        //         fixed4 col = _SelectedColor;
        //         col.a  *= (1 / length(i.world_pos)) * _Radius;
        //         if (dp <= -1 || 1 - SIXTH <= angle) {
        //             return _Selected == 1 ? col : 0;
        //         }
        //         if (dp >= 1 || angle <= SIXTH) {
        //             return _Selected == 4 ? col : 0;
        //         }
        //         if (SIXTH <= angle && angle <= 0.5f) {
        //             if (i.world_pos.y > 0) {
        //                 return _Selected == 5 ? col : 0;
        //             }
        //             return _Selected == 3 ? col : 0;
        //         }
        //         if (0.5f <= angle && angle <= 1 - SIXTH) {
        //             if (i.world_pos.y > 0) {
        //                 return _Selected == 0 ? col : 0;
        //             }
        //             return _Selected == 2 ? col : 0;
        //         }
        //         return 0;
        //     }
        //     ENDCG
        // }

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

            #define SIXTH 0.1666666666666667f
            #define THIRD 0.33333333333333333333333333333f

            float3 rotateBy(float3 v, float angle) {
                float3x3 rot = float3x3(float3(cos(angle), -sin(angle), 0), float3(sin(angle), cos(angle), 0), float3(0, 0, 1));
                return normalize(mul(rot, v));
            }

            fixed4 frag (const v2f i) : SV_Target {
                if (_Selected < 0 || _Selected > 5) {
                    return 0;
                }
                const float3 w_pos = normalize(i.world_pos);

                // angles of the two borders of the triangle
                const float alpha = _Selected * UNITY_TWO_PI * SIXTH;
                const float beta = (_Selected + 1) * UNITY_TWO_PI * SIXTH;
                // vectors of those borders
                const float3 dir1 = normalize(rotateBy(float3(0, 1, 0), alpha));
                const float3 dir2 = normalize(rotateBy(float3(0, 1, 0), beta));
                // make sure they're valid
                if (all(dir1 == dir2)) {
                    return 0;
                }
                // measure the pixel's worldPos angle from each of the vectors
                const float angle_from_dir1 = acos(dot(w_pos, dir1)) * UNITY_INV_PI;
                const float angle_from_dir2 = acos(dot(w_pos, dir2)) * UNITY_INV_PI;
                // if (angle_from_dir2 < 0 || angle_from_dir1 < 0) {
                //     return float4(1, 0, 0, 1);
                // }
                if (angle_from_dir1 + angle_from_dir2 >= THIRD) {
                    return 0;
                }
                float3 proj = dot(i.world_pos, dir1) / dot(dir1, dir1) * dir1;
                float3 rej = i.world_pos - proj;
                if (length(rej) < _Radius) {
                    float4 col = _SelectedColor;
                    // enable this line for fade effect
                    // col.a *= (_Radius - length(rej)) / _Radius;
                    return col;
                }
                proj = dot(i.world_pos, dir2) / dot(dir2, dir2) * dir2;
                rej = i.world_pos - proj;
                if (length(rej) < _Radius) {
                    float4 col = _SelectedColor;
                    // enable this line for fade effect
                    // col.a *= (_Radius - length(rej)) / _Radius;
                    return col;
                }
                return 0;
            }
            ENDCG
        }
    }
}