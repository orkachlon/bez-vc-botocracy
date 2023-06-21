﻿Shader "Custom/Neurons/NeuronConnectionShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _StencilRef ("StencilRef", Range(0, 255)) = 0
    }
    SubShader {
        Tags { "RenderType"="Opaque"}
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite off
        
        Stencil {
            Ref [_StencilRef]
            Comp NotEqual
        }  

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata  {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                return col;
            }
            ENDCG
        }
    }
}