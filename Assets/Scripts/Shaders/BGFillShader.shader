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

            fixed4 frag (v2f i) : SV_Target {
                if (_Selected < 0 || _Selected > 5) {
                    return 0;
                }
                const float3 w_pos = normalize(i.world_pos);
                const float dp = dot(w_pos, float3(1, 0, 0));
                const float angle = acos(dp) * UNITY_INV_PI;
                
                
                fixed4 col = _SelectedColor;
                col.a  *= (1 / length(i.world_pos)) * _Radius;
                if (dp <= -1 || 1 - SIXTH <= angle) {
                    return _Selected == 1 ? col : 0;
                }
                if (dp >= 1 || angle <= SIXTH) {
                    return _Selected == 4 ? col : 0;
                }
                if (SIXTH <= angle && angle <= 0.5f) {
                    if (i.world_pos.y > 0) {
                        return _Selected == 5 ? col : 0;
                    }
                    return _Selected == 3 ? col : 0;
                }
                if (0.5f <= angle && angle <= 1 - SIXTH) {
                    if (i.world_pos.y > 0) {
                        return _Selected == 0 ? col : 0;
                    }
                    return _Selected == 2 ? col : 0;
                }
                return 0;
            }
            ENDCG
        }
    }
}