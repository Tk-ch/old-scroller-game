Shader "Unlit/Stars"
{
    Properties
    {
        _Percentage ("Percentage", Float) = 0
        _BrightnessRange ("Brightness Range", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"


            float _Percentage;
            float _BrightnessRange; 

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (MeshData v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; 
                return o;
            }

            float random (float2 uv)
            {
                return frac(sin(dot(uv,float2(12.9898,78.233)))*43758.5453123);
            }

            float GetStars(float2 uv) {
                return random(uv) > (1 - _Percentage);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float noise = random(i.uv);

                float stars = GetStars(i.uv) + GetStars(i.uv + 0.5) + GetStars(i.uv + 0.3) + GetStars(i.uv + 0.7);

                fixed4 col = fixed4((stars - random(i.uv) * _BrightnessRange), noise, 0, 1);
                return col;
            }
            ENDCG
        }
    }
}
