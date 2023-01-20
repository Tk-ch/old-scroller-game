Shader "Unlit/Roadshader"
{
    // A mirrored gradient for the road

    Properties
    {
        _RoadColor ("Road Color", Color) = (0,0,0,0)
        _Width ("Road Width", float) = 0.3
        _Strength ("Light Strength", float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            
            Blend One One
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

            float4 _RoadColor;
            float _Width;
            float _Strength;

            v2f vert (MeshData v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; 
                return o;
            }



            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = fixed4(clamp(abs(i.uv.xxx - 0.5) - (1 - 0.5 - _Width), 0, 1) * (_RoadColor * _Strength), 1);
               
                return col;
            }
            ENDCG
        }
    }
}
