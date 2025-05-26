Shader "Custom/MinimapShader"
{
    Properties
    {
        // A room is a cell, just using the term room to be more thematic
        // The map is a grid of rooms/cell
        _MapSize("Map Size", Float) = 1
        _Zoom("Zoom", Range(0, 1)) = 1
        _PlayerRoom("Player Room (X,Y)", Vector) = (0, 0, 0, 0)
        _MainTex("_MainTex", 2D) = "white" {}

        _BGColor("Background Color", Color) = (0, 0, 0, 1)
        _RoomColor("Room Color", Color) = (0.2, 0.2, 0.2, 1)
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

            sampler2D _MainTex;
            float _MapSize;
            float _Zoom;
            float4 _PlayerRoom;

            fixed4 _BGColor;
            fixed4 _RoomColor;
            fixed4 _BorderColor;

            #define VISITED_MAX 64
            float2 _VisitedRooms[VISITED_MAX];
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

                float halfMap = (_MapSize - 1.0) * 0.5;
                float invMap = 1.0 / _MapSize; // Reduce operation cost

                // --- X Axis Zoom Center ---
                float playerToCenterX = abs(_PlayerRoom.x - halfMap);
                float minZoomX = playerToCenterX * 2.0 * invMap;
                float tX = saturate(1.0 - (_Zoom / minZoomX));
                float centerPointX = lerp(_PlayerRoom.x, halfMap, tX);

                // --- Y Axis Zoom Center ---
                float playerToCenterY = abs(_PlayerRoom.y - halfMap);
                float minZoomY = playerToCenterY * 2.0 * invMap;
                float tY = saturate(1.0 - (_Zoom / minZoomY));
                float centerPointY = lerp(_PlayerRoom.y, halfMap, tY);

                // Apply Zoom
                o.uv = v.uv * (1.0 - _Zoom);

                // Apply offset
                o.uv += float2(0.5, 0.5) * _Zoom - (float2(halfMap, halfMap) - float2(centerPointX, centerPointY)) * invMap;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // UV to map coordinate
                float2 mapUV = uv * _MapSize;
                float2 roomCoord = floor(mapUV);
                float2 roomFrac = frac(mapUV); // local position within room

                // Sample room visibility (0 = hidden, 1 = visible)
                if (any(roomCoord < 0.0) || any(roomCoord >= float2(_MapSize, _MapSize)))
                    return _BGColor;

                // Border thickness
                float border = step(roomFrac.x, 0.05) + step(1.0 - roomFrac.x, 0.05) +
                                step(roomFrac.y, 0.05) + step(1.0 - roomFrac.y, 0.05);

                if (border > 0.0)
                    return _BorderColor;

                // Check if player is in this sroom
                float isPlayer =
                    step(0.5, 1.0 - abs(roomCoord.x - _PlayerRoom.x)) *
                    step(0.5, 1.0 - abs(roomCoord.y - _PlayerRoom.y));

                if (isPlayer > 0.5)
                    return tex2D(_MainTex, roomFrac);

                float isVisited = 0.0;
                for (int i = 0; i < _VisitedCount; i++)
                {
                    float2 visited = _VisitedRooms[i];
                    isVisited += step(0.5, 1.0 - abs(roomCoord.x - visited.x)) *
                                    step(0.5, 1.0 - abs(roomCoord.y - visited.y));
                }

                return isVisited > 0.0 ? _RoomColor : _BGColor;
            }
            ENDCG
        }
    }
}