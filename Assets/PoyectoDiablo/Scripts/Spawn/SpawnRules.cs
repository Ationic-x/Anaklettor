using System.Collections.Generic;
using UnityEngine;


public enum Rol
{
	Swordman,
	Archer,
	Mage,
}

[CreateAssetMenu(fileName = "NewSpawnRules", menuName = "Spawn/SpawnRules")]
public class SpawnRules : ScriptableObject
{
	[Header("Amount of enemies")]
	public int minEnemies = 1;
	public int maxEnemies = 5;

	[Header("Min distance")]
	public float minSpacingBetweenEnemies = 1f;
	public float minDistanceFromEdge = 1f;

	[Header("Roles")]
	public List<Rol> roles;

	public bool randomizeRoles = true; // true = random proportional, false = iterate
}
