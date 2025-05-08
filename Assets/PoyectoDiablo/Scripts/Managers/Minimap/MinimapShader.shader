Shader "Custom/MinimapShader"
{
    Properties
    {
        _GridSize("Grid Size", Integer) = 1
        _Zoom("Zoom", Range(0, 1)) = 1
        _PlayerCell("Player Cell (X,Y)", Vector) = (0, 0, 0, 0)
        _PlayerTex("Player Texture", 2D) = "white" {}

        _BGColor("Background Color", Color) = (0, 0, 0, 1)
        _CellColor("Cell Color", Color) = (0.2, 0.2, 0.2, 1)
        _BorderColor("Border Color", Color) = (1, 1, 1, 1)
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

            sampler2D _PlayerTex;
            int _GridSize;
            float _Zoom;
            float4 _PlayerCell;

            fixed4 _BGColor;
            fixed4 _CellColor;
            fixed4 _BorderColor;

            #define VISITED_MAX 64
            float2 _VisitedCells[VISITED_MAX];
            int _VisitedCount;

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

                float halfMap = (_GridSize - 1.0) * 0.5;
                float invGrid = 1.0 / _GridSize; // Reduce operation cost

                // --- X Axis Zoom Center ---
                float playerToCenterX = abs(_PlayerCell.x - halfMap);
                float minZoomX = playerToCenterX * 2.0 * invGrid;
                float tX = saturate(1.0 - (_Zoom / minZoomX));
                float centerPointX = lerp(_PlayerCell.x, halfMap, tX);

                // --- Y Axis Zoom Center ---
                float playerToCenterY = abs(_PlayerCell.y - halfMap);
                float minZoomY = playerToCenterY * 2.0 * invGrid;
                float tY = saturate(1.0 - (_Zoom / minZoomY));
                float centerPointY = lerp(_PlayerCell.y, halfMap, tY);

                // Apply Zoom
                o.uv = v.uv * (1.0 - _Zoom);

                // Apply offset
                o.uv += float2(0.5, 0.5) * _Zoom - (float2(halfMap, halfMap) - float2(centerPointX, centerPointY)) * invGrid;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // UV to grid coordinate
                float2 gridUV = uv * _GridSize;
                float2 cellCoord = floor(gridUV);
                float2 cellFrac = frac(gridUV); // local position within cell

                // Sample cell visibility (0 = hidden, 1 = visible)
                if (any(cellCoord < 0.0) || any(cellCoord >= float2(_GridSize, _GridSize)))
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

                if (isPlayer > 0.5)
                    return tex2D(_PlayerTex, cellFrac);

                float isVisited = 0.0;
                for (int i = 0; i < _VisitedCount; i++)
                {
                    float2 visited = _VisitedCells[i];
                    isVisited += step(0.5, 1.0 - abs(cellCoord.x - visited.x)) *
                                    step(0.5, 1.0 - abs(cellCoord.y - visited.y));
                }

                return isVisited > 0.0 ? _CellColor : _BGColor;
            }
            ENDCG
        }
    }
}