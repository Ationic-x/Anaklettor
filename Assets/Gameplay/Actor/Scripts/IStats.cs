public interface IStats
{
    public string CharacterName { get; set; }
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
    /// Multiplicative damage, how much is increased the damage by * X
    /// </summary>
    public float MulAttack { get; set; }
    /// <summary>
    /// Multiplicative defense, how much damage is reduced by * X
    /// </summary>
    public float MulDefense { get; set; }

    public void UpdateStats(IStats stats);
}