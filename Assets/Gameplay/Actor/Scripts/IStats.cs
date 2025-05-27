using UnityEngine.Rendering;

public interface IStats
{
	int MaxHP { get; set; }
	int HP { get; set; }
	int Attack { get; set; }
	int Defense { get; set; }
	float Aspd { get; set; }
	float Bspd { get; set; }
	float Speed { get; set; }
	float AtkDelay { get; set; }
	float MinDistanceAttack { get; set; }

	// Adders
	int AddAttack { get; set; }
	int AddHP { get; set; }

	// Multipliers
	float MulAspd { get; set; }
	float MulBspd { get; set; }
	float MulSpeed { get; set; }
	float MulAtkDelay { get; set; }


	public void UpdateStats(IStats stats);
}