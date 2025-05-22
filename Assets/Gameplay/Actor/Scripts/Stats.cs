using UnityEngine;

[System.Serializable]
public class Stats : IStats
{
    [SerializeField] private int _speed;
    [SerializeField] private int _maxHP;
    [SerializeField] private int _hp;
    [SerializeField] private int _mana;
    [SerializeField] private int _maxMana;
    [SerializeField] private int _attack;
    [SerializeField] private int _defense;
                     
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _mulAttack;
    [SerializeField] private float _mulDefense;
    [SerializeField] private float _mulHP;

	#region [Integers]

	public int Speed
    {
        get => _speed;
        set => _speed = value;
    }

    public int MaxHP
    {
        get => _maxHP;
        set => _maxHP = value;
    }

    public int HP
    {
        get => _hp;
        set => _hp = value;
    }
    public int Mana
    {
        get => _mana;
        set => _mana = value;
    }

    public int MaxMana
    {
        get => _maxMana;
        set => _maxMana = value;
    }

    public int Attack
    {
        get => _attack;
        set => _attack = value;
    }

    public int Defense
    {
        get => _defense;
        set => _defense = value;
    }
    #endregion

    #region [Floats]
    public float MovementSpeed
    {
        get => _movementSpeed;
        set => _movementSpeed = value;
    }

    public float MulAttack
    {
        get => _mulAttack;
        set => _mulAttack = value;
    }

    public float MulDefense
    {
        get => _mulDefense;
        set => _mulDefense = value;
    }
	public float MulHP 
    { 
        get => _mulHP; 
        set => _mulHP = value; 
    }
	#endregion

	#region [Update]
	public void UpdateStats(IStats stats)
    {
        this.MaxHP = stats.MaxHP;
        this.HP = stats.HP;
        this.Mana = stats.Mana;
        this.MaxMana = stats.MaxMana;
        this.Attack = stats.Attack;
        this.Defense = stats.Defense;

        this.MovementSpeed = stats.MovementSpeed;
        this.MulAttack = stats.MulAttack;
        this.MulDefense = stats.MulDefense;
        this.MulHP = stats.MulHP;
    }
    #endregion
}