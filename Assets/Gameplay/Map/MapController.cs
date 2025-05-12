using System;
using System.Collections.Generic;
using UnityEngine;

public static class MapController
{
    private struct RoomSceneData
    {
        public int sceneId;
        public float rotation;

        public RoomSceneData(int id, float rotation)
        {
            sceneId = id;
            this.rotation = rotation;
        }
    }

	// The rotation +1 means 0, is used just to allow negative and positive values
	// Cuz -0 is equal to +0, making it not work as expected
    private readonly static Dictionary<int, RoomSceneData> _sceneDictionary = new Dictionary<int, RoomSceneData>
		{
			// Start And Endpoints
			{ 1, new RoomSceneData(0, 1f) },     // O -> North
			{ 9, new RoomSceneData(0, 181f) },   // O -> South
			{ 17, new RoomSceneData(0, 91f) },   // O -> East
			{ 25, new RoomSceneData(0, 271f) },  // O -> West

			// ID 1 is reserved for the end point

			// Path
			{ 2, new RoomSceneData(2, 1f) },     // North -> South
			{ 4, new RoomSceneData(3, 1f) },     // North -> East
			{ 6, new RoomSceneData(3, -1f) },    // North -> West

			{ 8, new RoomSceneData(2, 181f) },   // South -> North
			{ 12, new RoomSceneData(3, 181f) },  // South -> East
			{ 14, new RoomSceneData(3, -181f) }, // South -> West

			{ 16, new RoomSceneData(2, -91f) },  // East -> North
			{ 18, new RoomSceneData(3, 91f) },   // East -> South
			{ 22, new RoomSceneData(3, 91f) },   // East -> West

			{ 24, new RoomSceneData(2, -271f) }, // West -> North
			{ 26, new RoomSceneData(3, 271f) },  // West -> South
			{ 28, new RoomSceneData(3, 271f) },  // West -> East

			// Random non-path
			{ 32, new RoomSceneData(4, 1f) }
		};

	public static void RotateMap(int sceneID, GameObject floor, GameObject map)
    {

		float rotation = _sceneDictionary[sceneID].rotation;
		Vector3 mapScale = map.transform.localScale;
		Vector3 floorScale = floor.transform.localScale;


        map.transform.Rotate(new Vector3(0, Math.Abs(rotation) - 1, 0));

		if (rotation < 0)
		{
            floor.transform.localScale = new Vector3(-floorScale.x, floorScale.y, floorScale.z);
            map.transform.localScale = new Vector3(-mapScale.x, mapScale.y, mapScale.z);
		}
    }
}
