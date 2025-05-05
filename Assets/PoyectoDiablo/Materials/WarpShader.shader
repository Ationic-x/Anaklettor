Shader "Custom/TriangleTileShader"
{
    Properties
    {
        _TriangleColor("Triangle Color", Color) = (1, 0, 0, 1)
        _BackgroundColor("Background Color", Color) = (0, 0, 0, 0)
        _TilingAndOffset("Tiling XY | Offset ZW", Vector) = (1, 1, 0, 0)
        _ScrollSpeed("Scroll Speed XY", Vector) = (0, 0, 0, 0)
        _Height("Height", Range(0, 1)) = 0.5
        _Width("Width", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        ZTest LEqual
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed4 _TriangleColor;
            fixed4 _BackgroundColor;
            float4 _TilingAndOffset; // x,y = tiling; z,w = offset
            float4 _ScrollSpeed;     // x = speedX, y = speedY
            float _Height;
            float _Width;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 tiling = _TilingAndOffset.xy;
                float2 offset = _TilingAndOffset.zw;
                float2 scroll = _ScrollSpeed.xy;

                // Scroll and tile UVs
                float2 uv = frac(i.uv * tiling + offset + _Time.y * scroll);

                // Define triangle shape in UV space (bottom-centered)
                float edge1 = uv.x;
                float edge2 = uv.y;
                float edge3 = 0.5 - abs(uv.x / _Width - 0.5) - uv.y / (_Height*2);

                return (edge1 >= 0 && edge2 >= 0 && edge3 >= 0) 
                    ? _TriangleColor 
                    : _BackgroundColor;
            }
            ENDCG
        }
    }
}