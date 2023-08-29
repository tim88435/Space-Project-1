Shader "Sprites/Outline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
        _Thickness("Thickness", float) = 10
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            //"RenderType" = "Transparent"
        }

        Pass
        {
            //ZTest Off
            ZWrite Off
            Cull Off
            Lighting Off
            Blend One OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
#pragma fragment frag

# include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Thickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float radius = distance(i.uv, float2(0.5, 0.5)) + 1.0 / _Thickness;
                float z = (1.0 - abs(radius - 0.5) * _Thickness + 1.0 / _Thickness);
                float alpha = smoothstep(0.5, 1.0, z);
                col.a *= alpha;
                col *= i.color * col.a;
                return col;
            }
            ENDCG
        }
    }
}
