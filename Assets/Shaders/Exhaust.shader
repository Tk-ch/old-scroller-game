Shader "Unlit/Exhaust"
{

    // Just a shader that interpolates the thrust using a gradient map texture

    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _tValue ("T", float) = 0
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
            //float4 _MainTex_ST;
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
                fixed tex =tex2D(_MainTex, i.uv); 
                return tex > (1 - _tValue) ? clamp(tex * _Color * 2, 0, 1) : pow(tex,1.5)* _Color * 2 * _tValue;
            }
            ENDCG
        }
    }
}
