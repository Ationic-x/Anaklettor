Shader "Custom/DungeonMinimapShader"
{
    // Unfinished
    Properties
    {
        _GridSize("Grid Size (X,Y)", Vector) = (8, 8, 0, 0)
        _Zoom("Zoom", Float) = 1
        _PlayerCell("Player Cell (X,Y)", Vector) = (0, 0, 0, 0)
        _CellDataTex("Cell Data Texture", 2D) = "white" {}

        _BGColor("Background Color", Color) = (0, 0, 0, 1)
        _CellColor("Cell Color", Color) = (0.2, 0.2, 0.2, 1)
        _BorderColor("Border Color", Color) = (1, 1, 1, 1)
        _PlayerColor("Player Marker Color", Color) = (1, 0, 0, 1)
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _CellDataTex;
            float4 _GridSize;
            float _Zoom;
            float4 _PlayerCell;

            fixed4 _BGColor;
            fixed4 _CellColor;
            fixed4 _BorderColor;
            fixed4 _PlayerColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _Zoom;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // UV to grid coordinate
                float2 gridUV = uv * _GridSize.xy;
                float2 cellCoord = floor(gridUV);
                float2 cellFrac = frac(gridUV); // local position within cell

                // Sample cell visibility (0 = hidden, 1 = visible)
                float2 texUV = cellCoord / _GridSize.xy;
                float visibility = tex2D(_CellDataTex, texUV).r;

                if (visibility < 0.1)
                    return _BGColor;

                // Border thickness
                float border = step(cellFrac.x, 0.05) + step(1.0 - cellFrac.x, 0.05) +
                               step(cellFrac.y, 0.05) + step(1.0 - cellFrac.y, 0.05);

                if (border > 0.0)
                    return _BorderColor;

                // Check if player is in this cell
                float isPlayer =
                    step(0.5, 1.0 - abs(cellCoord.x - _PlayerCell.x)) *
                    step(0.5, 1.0 - abs(cellCoord.y - _PlayerCell.y));

                return isPlayer > 0.5 ? _PlayerColor : _CellColor;
            }
            ENDCG
        }
    }
}