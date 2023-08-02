Shader "Unlit/DoubleSidedTexture"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }
 
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="AlphaTest" }
        LOD 100
 
        Cull Off // Disable backface culling
 
        CGPROGRAM
        #pragma surface surf Lambert vertex:vert alpha:fade

        sampler2D _MainTex;
        float _Cutoff;

        struct Input
        {
            float2 uv_MainTex;
        };
 
        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.uv_MainTex = v.texcoord.xy;
        }
 
        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            clip(o.Alpha - _Cutoff);
        }
        ENDCG
    }
}
