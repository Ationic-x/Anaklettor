using UnityEngine;

[System.Serializable]
public class Stats : IStats
{
	// General stats
	[SerializeField] private int _level;
	[SerializeField] private int _exp;
    [SerializeField] private int _maxHP;
    [SerializeField] private int _hp;
    [SerializeField] private int _attack;
    [SerializeField] private int _defense;
    [SerializeField] private float _aspd;
    [SerializeField] private float _bspd;
    [SerializeField] private float _speed;
    [SerializeField] private float _atkDelay;
    [SerializeField] private float _minDistanceAttack;

	// Add
	[SerializeField] private int _addExp;
	[SerializeField] private int _addAttack;
    [SerializeField] private int _addHP;

    // Multipliers
    [SerializeField] private float _mulAspd;
    [SerializeField] private float _mulBspd;
    [SerializeField] private float _mulSpeed;
    [SerializeField] private float _mulAtkDelay;

	#region Integers
	public int Level { get => _level; set => _level = value; }
	public int MaxHP { get => _maxHP; set => _maxHP = value; }
	public int HP { get => _hp; set => _hp = value; }
	public int Attack { get => _attack; set => _attack = value; }
	public int Defense { get => _defense; set => _defense = value; }
	public int Exp { get => _exp; set => _exp = value; }
	#endregion
	
    #region Floats
	public float Speed { get => _speed; set => _speed = value; }
	public float AtkDelay { get => _atkDelay; set => _atkDelay = value; }
	public float MinDistanceAttack { get => _minDistanceAttack; set => _minDistanceAttack = value; }
	#endregion

	#region Adders
	public int AddAttack { get => _addAttack; set => _addAttack = value; }
	public int AddHP { get => _addHP; set => _addHP = value; }
	public int AddExp { get => _addExp; set => _addExp = value; }
	#endregion

	#region Multipliers
	public float Aspd { get => _aspd; set => _aspd = value; }
	public float Bspd { get => _bspd; set => _bspd = value; }
	public float MulAspd { get => _mulAspd; set => _mulAspd = value; }
	public float MulBspd { get => _mulBspd; set => _mulBspd = value; }
	public float MulSpeed { get => _mulSpeed; set => _mulSpeed = value; }
	public float MulAtkDelay { get => _mulAtkDelay; set => _mulAtkDelay = value; }
	#endregion



	#region [Update]
	public void UpdateStats(IStats stats)
    {
		_level				= stats.Level;
		_exp				= stats.Exp;
		_aspd				= stats.Aspd;
		_bspd				= stats.Bspd;
		_maxHP				= stats.MaxHP;
		_hp					= stats.HP;
		_attack				= stats.Attack;
		_defense			= stats.Defense;
		_speed				= stats.Speed;
		_atkDelay			= stats.AtkDelay;
		_minDistanceAttack	= stats.MinDistanceAttack;
		_mulAspd			= stats.MulAspd;
		_addAttack			= stats.AddAttack;
		_addHP				= stats.AddHP;
		_addExp				= stats.AddExp;
		_mulSpeed			= stats.MulSpeed;
		_mulAtkDelay		= stats.MulAtkDelay;
	}
    #endregion
}