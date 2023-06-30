Shader "Custom/BGTutorialFillShader"{
    Properties{
        _MainTex ("Texture", 2D) = "white" {}
        _ColorTopRight ("_ColorTopRight", Color) = (1, 0.5, 1, 1)
        _ColorTopLeft ("_ColorTopLeft", Color) = (0.5, 1, 0.5, 1)
        _ColorBot ("_ColorBot", Color) = (0.5, 1, 1, 1)
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
    }
}