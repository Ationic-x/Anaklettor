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

			// ID 1 is reserved for the end point

			// Side to Side
			{ 8, new RoomSceneData("Side_to_Side", 1f) },		// North -> South
			{ 2, new RoomSceneData("Side_to_Side", 181f) },		// South -> North
			{ 22, new RoomSceneData("Side_to_Side", 91f) },		// East -> West
			{ 28, new RoomSceneData("Side_to_Side", 271f) },	// West -> East

			// Corner
			{ 16,  new RoomSceneData("Corner", 1f) },	 // North -> East
			{ 4, new RoomSceneData("Corner", -1f) },	// North -> West
			
			{ 12, new RoomSceneData("Corner", 91f) },	// East -> South
			{ 6, new RoomSceneData("Corner", -91f) },	// East -> North
			
			{ 18,  new RoomSceneData("Corner", -181f) }, // South -> West
			{ 26, new RoomSceneData("Corner", 181f) },	// South -> East

			{ 24, new RoomSceneData("Corner", 271f) },	// West -> North
			{ 14, new RoomSceneData("Corner", -271f) },	// West -> South

			// Random non-path

			// Misc1
			{ 32, new RoomSceneData("Misc_1", 1f) },
			{ 33, new RoomSceneData("Misc_1", -1f) },
			{ 34, new RoomSceneData("Misc_1", 91f) },
			{ 35, new RoomSceneData("Misc_1", -91f) },
			{ 36, new RoomSceneData("Misc_1", 181f) },
			{ 37, new RoomSceneData("Misc_1", -181f) },
			{ 38, new RoomSceneData("Misc_1", 271f) },
			{ 39, new RoomSceneData("Misc_1", -271f) },
			
			// Misc2
			{ 40, new RoomSceneData("Misc_2", 1f) },
			{ 41, new RoomSceneData("Misc_2", -1f) },
			{ 42, new RoomSceneData("Misc_2", 91f) },
			{ 43, new RoomSceneData("Misc_2", -91f) },
			{ 44, new RoomSceneData("Misc_2", 181f) },
			{ 45, new RoomSceneData("Misc_2", -181f) },
			{ 46, new RoomSceneData("Misc_2", 271f) },
			{ 47, new RoomSceneData("Misc_2", -271f) },
			
			// Misc3
			{ 48, new RoomSceneData("Misc_3", 1f) },
			{ 49, new RoomSceneData("Misc_3", -1f) },
			{ 50, new RoomSceneData("Misc_3", 91f) },
			{ 51, new RoomSceneData("Misc_3", -91f) },
			{ 52, new RoomSceneData("Misc_3", 181f) },
			{ 53, new RoomSceneData("Misc_3", -181f) },
			{ 54, new RoomSceneData("Misc_3", 271f) },
			{ 55, new RoomSceneData("Misc_3", -271f) },

			// Misc4
			{ 56, new RoomSceneData("Misc_4", 1f) },
			{ 57, new RoomSceneData("Misc_4", -1f) },
			{ 58, new RoomSceneData("Misc_4", 91f) },
			{ 59, new RoomSceneData("Misc_4", -91f) },
			{ 60, new RoomSceneData("Misc_4", 181f) },
			{ 61, new RoomSceneData("Misc_4", -181f) },
			{ 62, new RoomSceneData("Misc_4", 271f) },
			{ 63, new RoomSceneData("Misc_4", -271f) },

			// Misc5
			{ 64, new RoomSceneData("Misc_5", 1f) },
			{ 65, new RoomSceneData("Misc_5", -1f) },
			{ 66, new RoomSceneData("Misc_5", 91f) },
			{ 67, new RoomSceneData("Misc_5", -91f) },
			{ 68, new RoomSceneData("Misc_5", 181f) },
			{ 69, new RoomSceneData("Misc_5", -181f) },
			{ 70, new RoomSceneData("Misc_5", 271f) },
			{ 71, new RoomSceneData("Misc_5", -271f) },

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

	public static string GetSceneName(int sceneID)
	{
		if (_sceneDictionary.TryGetValue(sceneID, out RoomSceneData roomSceneData))
		{
			return roomSceneData.sceneName;
		}
		else
		{
			Debug.LogError($"Scene ID {sceneID} not found in dictionary.");
			return null;
		}
	}
}
