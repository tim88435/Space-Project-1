Shader "Sprites/Circle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
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
                float radius = distance(i.uv, float2(0.5, 0.5));
                float alpha = smoothstep(0.5, 0.5 - radius * 0.003, radius);
                col.a *= alpha;
                col *= i.color * col.a * i.color.a;
                return col;
            }
            ENDCG
        }
    }
}
