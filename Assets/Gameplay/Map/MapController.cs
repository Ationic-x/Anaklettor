using System;
using System.Collections.Generic;
using UnityEngine;

public static class MapController
{
    private struct RoomSceneData
    {
        public string sceneName;
        public float rotation;

        public RoomSceneData(string name, float rotation)
        {
            sceneName = name;
            this.rotation = rotation;
        }
    }

	// The rotation +1 means 0, is used just to allow negative and positive values
	// Cuz -0 is equal to +0, making it not work as expected
    private readonly static Dictionary<int, RoomSceneData> _sceneDictionary = new Dictionary<int, RoomSceneData>
		{
			// Start And Endpoints
			{ 1, new RoomSceneData("Start", 1f) },     // O -> North
			{ 9, new RoomSceneData("Start", 181f) },   // O -> South
			{ 17, new RoomSceneData("Start", 91f) },   // O -> East
			{ 25, new RoomSceneData("Start", 271f) },  // O -> West

			{ 3, new RoomSceneData("End", 181f) },   // South -> 0
			{ 5, new RoomSceneData("End", 91f) },   // West -> 0
			{ 7, new RoomSceneData("End", 271f) },  // East -> 0

			// Side to Side
			{ 8, new RoomSceneData("Side_to_Side", 1f) },		// North -> South
			{ 2, new RoomSceneData("Side_to_Side", 181f) },		// South -> North
			{ 22, new RoomSceneData("Side_to_Side", 91f) },		// East -> West
			{ 28, new RoomSceneData("Side_to_Side", 271f) },	// West -> East

			// Corner
			{ 16,  new RoomSceneData("Corner", 1f) },	 // North -> East
			{ 24, new RoomSceneData("Corner", -1f) },	// North -> West
			
			{ 4, new RoomSceneData("Corner", -91f) },	// East -> North
			{ 12, new RoomSceneData("Corner", 91f) },	// East -> South
			
			{ 18, new RoomSceneData("Corner", 181f) },	// South -> East
			{ 26,  new RoomSceneData("Corner", -181f) }, // South -> West

			{ 6, new RoomSceneData("Corner", 271f) },	// West -> North
			{ 14, new RoomSceneData("Corner", -271f) },	// West -> South

			// Random non-path

			// Misc
			{ 32, new RoomSceneData("Misc_1", 1f) },
			{ 33, new RoomSceneData("Misc_2", 1f) },
			{ 34, new RoomSceneData("Misc_3", 1f) },
			{ 35, new RoomSceneData("Misc_4", 1f) },
			{ 36, new RoomSceneData("Misc_5", 1f) },

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

	public static string GetSceneName(int sceneID, bool isOrigin)
	{
		if (_sceneDictionary.TryGetValue(sceneID, out RoomSceneData roomSceneData))
		{
			if (sceneID == 1 && !isOrigin)
				return "End";

			return roomSceneData.sceneName;
		}
		else
		{
			Debug.LogError($"Scene ID {sceneID} not found in dictionary.");
			return null;
		}
	}
}
