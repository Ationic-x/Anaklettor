using UnityEngine.Rendering;

public interface IStats
{
    /// <summary>
    /// Maximum ammount of life
    /// </summary>
    public int MaxHP { get; set; }
    /// <summary>
    /// Current ammount of life
    /// </summary>
    public int HP { get; set; }
    /// <summary>
    /// Curent amount of energy to do special actions
    /// </summary>
    public int Mana { get; set; }
    /// <summary>
    /// Maximum amount of energy to do special actions
    /// </summary>
    public int MaxMana { get; set; }
    /// <summary>
    /// Attack, how much is increased the damage by + X
    /// </summary>
    public int Attack { get; set; }
    /// <summary>
    /// Defense, how much damage is reduced by - X
    /// </summary>
    public int Defense { get; set; }
    /// <summary>
    /// How fast can moves the character
    /// </summary>
    public float MovementSpeed { get; set; }
	/// <summary>
	/// Multiplicative damage, how much damage increase per level
	/// </summary>
	public float MulAttack { get; set; }
    /// <summary>
    /// Multiplicative defense, how much defense increase per level
    /// </summary>
    public float MulDefense { get; set; }
    /// <summary>
    /// Multiplicative speed, how much speed is multiplication by base speed when running
    /// </summary>
    public float MulSpeed { get; set; }
    /// <summary>
    /// Minimun distance to attack the player (For IA)
    /// </summary>
    public float MinDistanceAttack { get; set; }
    /// Multiplicative hp, how much hp increased per level
    /// </summary>
    public float MulHP { get; set; }


    public void UpdateStats(IStats stats);
}