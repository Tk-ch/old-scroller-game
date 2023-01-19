Shader "Unlit/Exhaust"
{

    // Just a shader that interpolates the thrust using a gradient map texture

    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _tValue ("T", float) = 0
        _NoiseTex ("Noise Texture", 2D) = "black" {}
    }  
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Blend One One
            ZWrite Off
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            fixed4 _Color;
            fixed _tValue;

            v2f vert (MeshData v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 offset = float2(0, _Time.x * 2);
                float2 scale = float2(0.4, 0.1);
                fixed2 noise1 = tex2D(_NoiseTex, (i.uv * scale + offset ) * 2);
                fixed2 noisehalf = tex2D(_NoiseTex, (i.uv * scale) / 2 + offset);
                fixed2 noisefourth = tex2D(_NoiseTex, (i.uv * scale) / 4 + offset);

                fixed2 combnoise = frac(noise1 + noisehalf + noisefourth) * 0.2 - 0.1;

                fixed tex = tex2D(_MainTex, i.uv + combnoise); 
                
                return tex > (1 - _tValue) ? clamp(tex * _Color * 2, 0, 1) : pow(tex,1.5)* _Color * 2 * _tValue;
            }
            ENDCG
        }
    }
}
