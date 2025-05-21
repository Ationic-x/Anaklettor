using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : ActorController
{
	private ChangeMaterialColor _damageBlink;

	protected int Level { get; set; }
    protected int Experience { get; set; }
    protected int MaxExperience { get; set; }

    private Vector2 _moveInput;
    
	private float _rotationMultiplier = 30f; // Rotation speed is slower than movement
	private float _immuneTime = 1.5f; 

	private bool _isAttacking = false;
	private bool _isImmune = false;

	public bool smallDamage = false;
	public bool hugeDamage = false;

	#region Events
	[Header("Actions")]
	[SerializeField] private InputActionReference _moveAction;
	[SerializeField] private InputActionReference _attackAction;

	private void OnEnable()
    {
		_moveAction.action.performed += OnMoveInput;
		_moveAction.action.canceled += OnMoveInput;

		_attackAction.action.performed += OnAttackInput;
	}
    
    private void OnDisable()
    {
		_moveAction.action.performed -= OnMoveInput;
		_moveAction.action.canceled -= OnMoveInput;

		_attackAction.action.performed -= OnAttackInput;
	}

    private void OnMoveInput(InputAction.CallbackContext context = default)
    {
		_moveInput = context.action.ReadValue<Vector2>();
	}

    private void OnAttackInput(InputAction.CallbackContext context = default)
    {
        Attack();
	}
	#endregion

	private void FixedUpdate()
    {
		if (smallDamage)
		{
			smallDamage = false;
			TakeDamage(1);
		}
		if (hugeDamage)
		{
			hugeDamage = false;
			TakeDamage(10);
		}

		if (!CanMotion()) return;

		Move();
		Rotate();
	}

	// Move is handle in a different way between AI(navmesh) and Player(rigidbody + rotation)
	private void Move()
	{
		float speedDirection = _moveInput.y < 0 ? _moveInput.y / 2 : _moveInput.y;
		rb.MovePosition(rb.position + (transform.forward * speedDirection * stats.MovementSpeed * Time.fixedDeltaTime));
		animator.SetFloat("Speed", _moveInput.y); // Animation works better with input value instead speed
	}

	private void Rotate()
	{
		float rotationAmount = _moveInput.x * stats.MovementSpeed * _rotationMultiplier * Time.fixedDeltaTime;
		Quaternion turnOffset = Quaternion.Euler(0, rotationAmount, 0);
		rb.MoveRotation(rb.rotation * turnOffset);
	}

	// In future this would be a actor method, but for now is splitted
	// Require a adaption between AI and Player
	private void Attack()
	{
		if (_isAttacking) return; // Prevents multiple attacks at the same time
		_isAttacking = true;
		animator.SetTrigger("Attack");
		StartCoroutine(AttackCooldown());
	}

	IEnumerator AttackCooldown()
	{
		animator.SetFloat("AttackSpeed", stats.Speed);
		
		float attackDuration = animator.GetCurrentAnimatorClipInfo(1)[0].clip.length;
		float actualDuration = attackDuration / stats.Speed;
		
		yield return new WaitForSeconds(actualDuration);
		_isAttacking = false;
	}

	IEnumerator ImmuneCooldown()
	{
		_isImmune = true;
		yield return new WaitForSeconds(_immuneTime);
		_isImmune = false;
	}

	// Enemies cannot be healed, we dont have a healer job or something
	public void Heal(int amount)
	{
		stats.HP += amount;
		if (stats.HP > stats.MaxHP) stats.HP = stats.MaxHP;
	}

	public override void TakeDamage(int amount)
	{
		if (_isImmune) return;

		_damageBlink.TriggerBlink();
		StartCoroutine(ImmuneCooldown());
		
		animator.SetInteger("Damaged", amount);
		animator.SetTrigger("IsHitted");
		stats.HP -= amount;
		if (stats.HP < 0) stats.HP = 0;
	}


	public override void Initialize(ActorData actorData)
    {
        // Initialize default values
        base.Initialize(actorData);

        // Initialize unique player values
		Level = 1;
		Experience = 0;
		MaxExperience = 100;
		_damageBlink = GetComponentInChildren<ChangeMaterialColor>();
	}

	private bool CanMotion()
	{
		string AnimationName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

		// If you're hitted or dead you cannot do anything
		return !(AnimationName.Equals("Hit_A") || AnimationName.Equals("Hit_B"));
	}

	void LevelUp() 
    {
        //Level++;
        //Stats.MaxHP += 10;
        //health = Stats.MaxHP; 
        //MaxExperience += 100;
        //attackPower += 3;
        //defense += 5;
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
