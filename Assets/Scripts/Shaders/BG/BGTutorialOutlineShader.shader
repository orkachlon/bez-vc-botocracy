Shader "Custom/BGTutorialOutlineShader" {
    Properties {
        _MainTex ("_MainTetx", 2D) = "white" {}
        _Threshold ("_Threshold", float) = 1
        _Length ("_Length", float) = -1
    }
    SubShader {
        // No culling or depth
        Cull Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR0;
            };

            struct v2f {
                float3 world_pos : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR0;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.world_pos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }
            
            uniform float _Threshold;
            uniform float _Length;

            #define THIRD 0.33333333333333333333f

            fixed4 frag (v2f i) : SV_Target {
                for (int j = 0; j < 3; j++) {
                    const float alpha = j * UNITY_TWO_PI * THIRD;
                    const float3x3 rot = float3x3(float3(cos(alpha), -sin(alpha), 0), float3(sin(alpha), cos(alpha), 0), float3(0, 0, 1));
                    const float3 dir = normalize(mul(rot, float3(0, 1, 0)));
                    const float3 proj = dot(i.world_pos, dir) / dot(dir, dir) * dir;
                    const float3 rej = i.world_pos - proj;
                    // length -1 means do not use length - always show
                    // show only the line in the direction we want
                    if (sign(i.world_pos.y) == sign(dir.y) && length(rej) < _Threshold && (_Length == -1 || length(proj) < _Length)) {
                        return i.color;
                    }
                }
                return 0;
            }
            ENDCG
        }
    }
}