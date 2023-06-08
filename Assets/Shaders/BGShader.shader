Shader "Custom/BGShader"{
    Properties{
        _MainTex ("Texture", 2D) = "white" {}
        _ColorA ("ColorA", Color) = (1, 0, 0, 1)
        _ColorB ("ColorB", Color) = (1, 0, 1, 1)
    }
    SubShader{
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

            uniform float4 _ColorA;
            uniform float4 _ColorB;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.world_pos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                const float3 w_pos = normalize(i.world_pos);
                const float angle = acos(dot(w_pos, float3(1, 0, 0))) / UNITY_PI;
                if (w_pos.y > 0) {
                    if (0.0 <= angle && angle <= 1.0/6.0 || 3.0/6.0 <= angle && angle < 5.0/6.0) {
                        return _ColorA;
                    }
                } 
                if (1.0/6.0 <= angle && angle <= 3.0/6.0 || 5.0/6.0 <= angle && angle <= 1) {
                    return _ColorB;
                }
                return _ColorA;
            }
            ENDCG
        }
    }
}