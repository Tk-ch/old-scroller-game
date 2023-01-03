Shader "Unlit/BGScroll"
{

    // This shader will scroll a texture applying various effects to it. 

    Properties
    {
        _MainTex ("Stars RenderTexture", 2D) = "black" {} // texture to scroll
        _StarColors ("Star Colors Gradient", 2D) = "white" {} // possible colors to apply 
        _Y ("Y Coordinate", Float) = 0 
        _NoiseScale ("Twinkle Noise Scale", Vector) = (0.1, 0.01, 0, 0) // scale of the overlaying noise texture
        _NoiseOffset ("Twinkle Noise Offset", Float) = 0.3 // a value that is substracted from the noise to make more gaps

        _Intensity ("Intensity", Float) = 2 // resulting color is multiplied by these two before returning
        _Color ("Color", Color) = (1,1,1,1 )

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
            sampler2D _StarColors;
            float4 _MainTex_ST;
            float _Y;
            float2 _NoiseScale;
            float _NoiseOffset;
            float _Intensity;
            fixed4 _Color;

            v2f vert (MeshData v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; //TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }



            fixed4 frag (v2f i) : SV_Target
            {
                
                fixed noiseL = tex2D(_MainTex, frac(i.uv * _NoiseScale.xy + float2(0, ((_Y * 0.3) - 0.5) * _NoiseScale.y) *  2)).g; // get the noise that will determine whether the star is twinkling
                
                fixed3 starColor = tex2D(_StarColors, frac(tex2D(_MainTex, frac(i.uv * 0.5 + float2(0, _Y))).g)); // get a random color for the star on the gradient

                // three layers of star textures moving at different speeds 
                fixed tex = tex2D(_MainTex, frac(i.uv * 0.5 + float2(0, _Y))).r * clamp(1 - (noiseL.rrr - _NoiseOffset) * 4, 0, 1); 
                tex += tex2D(_MainTex, frac((i.uv * 0.5) + float2(0, _Y/4)) + float2(0.5, 0)).r / 2 * clamp(pow(noiseL, 2) - _NoiseOffset, 0, 1);
                tex += tex2D(_MainTex, frac((i.uv * 0.5) + float2(0, _Y/8))+ float2(0.5, 0)).r / 4 * clamp(pow(noiseL, 3) - _NoiseOffset, 0, 1);
                fixed4 col = fixed4(tex.rrr * lerp(1, starColor, 0.7) * _Intensity * _Color, 1);
                return col;
            }
            ENDCG
        }
    }
}
