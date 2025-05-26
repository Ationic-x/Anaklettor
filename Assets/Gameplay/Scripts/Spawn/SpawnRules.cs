using System.Collections.Generic;
using UnityEngine;


// Called Rol to avoid conflicts, should be unified with Job Enum as a public Enum
// Shouldnt be used like a interface

[CreateAssetMenu(fileName = "NewSpawnRules", menuName = "Spawn/SpawnRules")]
public class SpawnRules : ScriptableObject
{
	[Header("Amount of enemies")]
	public int minEnemies = 1;
	public int maxEnemies = 5;

	[Header("Min distance")]
	public float minSpacingBetweenEnemies = 1f;
	[Range(0, 1)] public float spawnAreaRatio = 1f;

	[Header("Roles")]
	public List<Jobs> jobs;

	public bool randomizeJobs = true; // true = random proportional, false = iterate
}
