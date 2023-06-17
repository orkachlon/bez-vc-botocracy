Shader "Custom/BGFillShader"{
    Properties{
        _MainTex ("Texture", 2D) = "white" {}
        _ColorBotRight ("_ColorBotRight", Color) = (1, 1, 0.5, 1)
        _ColorTopRight ("_ColorTopRight", Color) = (1, 0.5, 1, 1)
        _ColorTopLeft ("_ColorTopLeft", Color) = (0.5, 1, 0.5, 1)
        _ColorBotLeft ("_ColorBotLeft", Color) = (0.5, 0.5, 1, 1)
        _ColorRight ("_ColorRight", Color) = (0.5, 1, 1, 1)
        _ColorLeft ("_ColorLeft", Color) = (1, 0.5, 0.5, 1)
    }
    SubShader {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            fixed4 frag (v2f i) : SV_Target {
                const float3 w_pos = normalize(i.world_pos);
                const float angle = acos(dot(w_pos, float3(1, 0, 0))) / UNITY_PI;
                if (0.0 <= angle && angle < 1.0/6.0) {
                    return _ColorRight;
                }
                if (3.0/6.0 <= angle && angle < 5.0/6.0) {
                    if (w_pos.y > 0) {
                        return _ColorTopLeft;
                    }
                    return _ColorBotLeft;
                }
                if (1.0/6.0 <= angle && angle <= 3.0/6.0) {
                    if (w_pos.y > 0) {
                        return _ColorTopRight;
                    }
                    return _ColorBotRight;
                }
                if (5.0/6.0 <= angle && angle <= 1) {
                    return _ColorLeft;
                }
                return 0;
            }
            ENDCG
        }
    }
}