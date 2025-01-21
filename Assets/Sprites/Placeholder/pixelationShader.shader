Shader "Custom/Pixelation"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _PixelSize ("Pixel Size", Float) = 0.01
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _PixelSize;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Compute pixelated UV coordinates
                float2 pixelatedUV = floor(i.uv / _PixelSize) * _PixelSize;
                fixed4 col = tex2D(_MainTex, pixelatedUV) * i.color;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}