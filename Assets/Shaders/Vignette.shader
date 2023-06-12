Shader "Custom/Vignette"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Radius ("_Radius", float) = 0
        _Feather ("_Feather", float) = 0
        _Tint ("_Tint", Color) = (0, 0, 0, 1)
        _Invert ("_Invert", int) = 0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            sampler2D _MainTex;
            float _Radius;
            float _Feather;
            float4 _Tint;
            bool _Invert;

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
                float2 newUV = i.uv * 2 - 1;
                float circle = length(newUV);
                float invertMask = smoothstep(_Radius, _Radius + _Feather, circle);
                float mask = 1 - invertMask;

                float3 displayColor = col.rgb * mask;
                float3 colorToUse = _Invert ? 1 - col.rgb : col.rgb;
                float3 vignetteColor = colorToUse * invertMask * _Tint;
                
                return fixed4(displayColor + vignetteColor, 1);
            }
            ENDCG
        }
    }
}