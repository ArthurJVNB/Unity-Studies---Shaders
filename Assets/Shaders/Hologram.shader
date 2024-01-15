Shader "Unlit/Hologram"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (1,1,1,1) // white
        _Transparency ("Transparency", Range(0.0,0.5)) = 0.25
        _CutoutThresh ("Cutout Threshold", Range(0.0,1.0)) = 0.2
        _Distance ("Distance", Float) = 1
        _Amplitude ("Amplitude", Float) = 1
        _Speed ("Speed", Float) = 1
        _Amount ("Amount", Range(0.0,1.0)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        ZWrite Off // Default is On. Must be Off if drawing semitransparent effects
        Blend SrcAlpha OneMinusSrcAlpha // Blending of color. This one is for traditional transparency (check Unity docs for details - "ShaderLab: Blending")

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            // #pragma multi_compile_fog

            #include "UnityCG.cginc" // Helper functions for rendering in Unity

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f // Means: Vert (vertex) to Frag (fragment)
            {
                float2 uv : TEXCOORD0;
                // UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION; // Means: Screen Space Position
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _TintColor;
            float _Transparency;
            float _CutoutThresh;
            float _Distance;
            float _Amplitude;
            float _Speed;
            float _Amount;

            v2f vert (appdata v)
            {
                // Starts as Local Space
                v2f o;
                v.vertex.x += sin(_Time.y * _Speed + v.vertex.y * _Amplitude) * _Distance * _Amount; // _Time is provided by Unity in float4 format, and each value is a type of time.
                                                                                                     // In this case, _Time.y is the same as Time.time in C# (in seconds)
                // From now on everything bellow is Screen Space
                o.vertex = UnityObjectToClipPos(v.vertex); // Matrix multiplications to return the Screen Space: Local Space -> Model Space -> View Space -> Clip Space -> Screen Space
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); // Application of texture, tilling and offset of _MainTex
                // UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target // : Means it's binded to SV_Target, and the output will go to SV_Target, which is the render target that is the frame buffer for the screen
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) + _TintColor; // col means color
                col.a = _Transparency;
                clip(col.r - _CutoutThresh); // This discards the provided pixel data. Same as: if (col.r < _CutoutThresh) discard;
                // apply fog
                // UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
