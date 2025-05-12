using UnityEngine;

[System.Serializable]
public class Stats : IStats
{
    [SerializeField] protected string characterName;

    [SerializeField] protected int speed;
    [SerializeField] protected int maxHP;
    [SerializeField] protected int hp;
    [SerializeField] protected int mana;
    [SerializeField] protected int maxMana;
    [SerializeField] protected int attack;
    [SerializeField] protected int defense;

    [SerializeField] protected float movementSpeed;
    [SerializeField] protected float mulAttack;
    [SerializeField] protected float mulDefense;
    [SerializeField] protected float mulSpeed;

    #region [Integers]

    public int Speed
    {
        get => speed;
        set => speed = value;
    }

    public int MaxHP
    {
        get => maxHP;
        set => maxHP = value;
    }

    public int HP
    {
        get => hp;
        set => hp = value;
    }
    public int Mana
    {
        get => mana;
        set => mana = value;
    }

    public int MaxMana
    {
        get => maxMana;
        set => maxMana = value;
    }

    public int Attack
    {
        get => attack;
        set => attack = value;
    }

    public int Defense
    {
        get => defense;
        set => defense = value;
    }
    #endregion


    #region [Floats]
    public float MovementSpeed
    {
        get => movementSpeed;
        set => movementSpeed = value;
    }

    public float MulAttack
    {
        get => mulAttack;
        set => mulAttack = value;
    }

    public float MulDefense
    {
        get => mulDefense;
        set => mulDefense = value;
    }
    public float MulSpeed
    {
        get => mulSpeed;
        set => mulSpeed = value;
    }
    #endregion

    #region [Strings]
    public string CharacterName
    {
        get => characterName;
        set => characterName = value;
    }
    #endregion

    #region [Update]
    public void UpdateStats(IStats stats)
    {
        this.CharacterName = stats.CharacterName;
        this.MaxHP = stats.MaxHP;
        this.HP = stats.HP;
        this.Mana = stats.Mana;
        this.MaxMana = stats.MaxMana;
        this.Attack = stats.Attack;
        this.Defense = stats.Defense;

        this.MovementSpeed = stats.MovementSpeed;
        this.MulAttack = stats.MulAttack;
        this.MulDefense = stats.MulDefense;
    }
    #endregion
}