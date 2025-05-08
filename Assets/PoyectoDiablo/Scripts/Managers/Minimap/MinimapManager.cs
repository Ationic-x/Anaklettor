using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinimapManager : MonoBehaviour
{
	public List<Vector2> visited = new List<Vector2>();
	const int MaxVisited = 64;
	Vector4[] visitedArray = new Vector4[MaxVisited];
	public Material material;
	public Vector2 playerPosition;

	void Start()
    {
		for (int i = 0; i < visited.Count && i < MaxVisited; i++)
			visitedArray[i] = new Vector4(visited[i].x, visited[i].y, 0, 0);
		
		material.SetVectorArray("_VisitedCells", visitedArray);
		material.SetInt("_VisitedCount", visited.Count);


		material.SetVector("_PlayerCell", new Vector4(playerPosition.x, playerPosition.y, 0, 0));
	}
}
