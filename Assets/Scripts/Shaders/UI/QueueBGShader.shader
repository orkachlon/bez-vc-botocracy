// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SpriteGradient" {
Properties {
	[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
    _Color ("Left Color", Color) = (1,1,1,1)
    _Color2 ("Right Color", Color) = (1,1,1,1)
    _Scale ("Scale", Float) = 1
    
// these six unused properties are required when a shader
// is used in the UI system, or you get a warning.
// look to UI-Default.shader to see these.
_StencilComp ("Stencil Comparison", Float) = 8
_Stencil ("Stencil ID", Float) = 0
_StencilOp ("Stencil Operation", Float) = 0
_StencilWriteMask ("Stencil Write Mask", Float) = 255
_StencilReadMask ("Stencil Read Mask", Float) = 255
_ColorMask ("Color Mask", Float) = 15
// see for example
// http://answers.unity3d.com/questions/980924/ui-mask-with-shader.html

}
 
SubShader {
    Tags {"Queue"="Background"  "IgnoreProjector"="True"}
    LOD 100
 
    ZWrite On
    Blend SrcAlpha OneMinusSrcAlpha
 
    Pass {
        CGPROGRAM
        #pragma vertex vert  
        #pragma fragment frag
        #include "UnityCG.cginc"
 
        fixed4 _Color;
        fixed4 _Color2;
        fixed  _Scale;
        sampler2D _MainTex;
 
        struct v2f {
            float4 pos : SV_POSITION;
            fixed4 col : COLOR;
            float2 uv : TEXCOORD0;
        };
 
        v2f vert (appdata_full v) {
            v2f o;
            o.pos = UnityObjectToClipPos (v.vertex);
            o.col = lerp(_Color,_Color2, v.texcoord.y );
            o.uv =  v.texcoord;
            return o;
        }
       
 
        float4 frag (v2f i) : COLOR {
            fixed4 mask = tex2D(_MainTex, i.uv);
            float4 c = i.col;
            c.a *= mask.a;
            return c;
        }
            ENDCG
        }
    }
}