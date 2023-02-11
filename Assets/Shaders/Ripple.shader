Shader "Unlit/Ripple"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Size("Size", Range(0, .5))=.15
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                float2 waveUV : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _WaveTex;
            fixed _Size;

            v2f vert (appdata_full v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.waveUV = v.texcoord.xy;
                return o;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                fixed4 uv = tex2D(_WaveTex, IN.waveUV);
                // (0~1 --> -1~1)*_Size
                uv = (2 * uv - 1) * _Size;
                return tex2D(_MainTex, IN.uv + uv);
            }
            ENDCG
        }
    }
}
