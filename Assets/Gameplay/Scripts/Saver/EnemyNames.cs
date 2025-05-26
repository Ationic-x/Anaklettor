using UnityEngine;

public static class EnemyNames
{
	public static readonly string[] enemyNameID =
	{
		"Pablotar",
		"Franthrax",
		"Alvarrack",
		"Juankalazar",
		"Ismuelte",
		"Oscarnix",
		"Juanmortis",
		"Androskull",
		"Luestron",
		"Juantonator",
	};

	public static string GetRandomName()
	{
		return enemyNameID[Random.Range(0, enemyNameID.Length)];
	}
}
