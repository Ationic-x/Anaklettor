using UnityEngine;

public class PlayerController : Actor
{
    [SerializeField] string PlayerName;
    protected int Level { get; set; }
    protected int Experience { get; set; }
    protected int MaxExperience { get; set; }

    private GameObject actorModelObject;
    private Animator animator;

    void Start()
    {
        PlayerInitialize();
        actorModelObject = gameObject.transform.GetChild(0).gameObject;
        animator = actorModelObject.AddComponent<Animator>();
        animator.runtimeAnimatorController = ActorData.animatorController;
    }

    void Update()
    {
        float vAxis = Input.GetAxis("Vertical");

        base.movementSpeed = vAxis * ActorData.stats.Speed; // Modified to inverse
        base.eulerAngleVelocity = new Vector3(0, Input.GetAxis("Horizontal") * 80, 0);
        animator.SetFloat("Velocity", vAxis);

        if (Input.GetButton("Fire1"))
            animator.SetTrigger("Attack");
    }

    void FixedUpdate()
    {
        Rotate();
        Move();
    }

    public void PlayerInitialize() 
    {
        Debug.Log(ActorData.ToString());

        MaxHealth = ActorData.stats.MaxHP;
        Health = MaxHealth;
        Mana = ActorData.stats.Mana;
        MaxMana = ActorData.stats.MaxMana;
        AttackPower = ActorData.stats.Attack;
        Defense = ActorData.stats.Defense;
        Experience = 0;
        MaxExperience = 100;
        Level = 1;

        Instantiate(ActorData.model, transform.position, Quaternion.identity, transform);
    }

    void LevelUp() 
    {
        Level++;
        MaxHealth += 10;
        Health = MaxHealth; 
        MaxExperience += 100;
        AttackPower += 3;
        Defense += 5;
    }

    void GainExperience(int amount) 
    {
        Experience += amount;
        if (Experience >= MaxExperience) 
        {
            if (Experience > MaxExperience) 
            {
                Experience = Experience - MaxExperience;
            } 
            else 
            {
                Experience = 0;
            }
            LevelUp();
        }
    }
}
