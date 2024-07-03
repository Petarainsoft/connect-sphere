Shader "Custom/UnlitRotateMirrorUI"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MirrorX ("Mirror X", Float) = 0
        _MirrorY ("Mirror Y", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _MirrorX;
            float _MirrorY;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float2 uv = v.texcoord;
                uv = float2(uv.y, 1.0 - uv.x); // Rotate 90 degrees
                if (_MirrorX > 0.5) uv.x = 1.0 - uv.x; // Mirror X
                if (_MirrorY > 0.5) uv.y = 1.0 - uv.y; // Mirror Y

                // Adjust UVs based on device orientation
                #if UNITY_UV_STARTS_AT_TOP
                    uv.y = 1.0 - uv.y;
                #endif

                o.uv = TRANSFORM_TEX(uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}