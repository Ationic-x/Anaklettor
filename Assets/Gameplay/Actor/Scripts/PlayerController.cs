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
    
	private float _rotationMultiplier = 40f; // Rotation speed is slower than movement
	private float _immuneTime = 1.5f;
	private float _blockSpeed = 1f;
	private float _blockDelay = 0.5f;
	private float _blockBlendProgress = 0f;

	private bool _isAttacking = false;
	private bool _isImmune = false;
	private bool _isDead = false;
	private bool _isRecovering = false;
	private bool _isBlocking = false;
	private bool _initBlock = false;

	public bool smallDamage = false;
	public bool hugeDamage = false;

	private Coroutine _blockCoroutine = null;

	#region Events
	[Header("Actions")]
	[SerializeField] private InputActionReference _moveAction;
	[SerializeField] private InputActionReference _attackAction;
	[SerializeField] private InputActionReference _blockAction;

	private void OnEnable()
    {
		_moveAction.action.performed += OnMoveInput;
		_moveAction.action.canceled += OnMoveInput;

		_blockAction.action.performed += OnBlockInput;
		_blockAction.action.canceled += OnBlockInput;

		_attackAction.action.performed += OnAttackInput;
	}
    
    private void OnDisable()
    {
		_moveAction.action.performed -= OnMoveInput;
		_moveAction.action.canceled -= OnMoveInput;

		_blockAction.action.performed -= OnBlockInput;
		_blockAction.action.canceled -= OnBlockInput;

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

	private void OnBlockInput(InputAction.CallbackContext context = default)
	{
		switch (context.phase)
		{
			case InputActionPhase.Performed:
				Block();
				break;
			case InputActionPhase.Canceled:
				UnBlock();
				break;
			default:
				break;
		}
	}
	#endregion

	private void FixedUpdate()
    {
		// Tests
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
		// End of tests

		if (!CanMotion()) return;

		Move();
		Rotate();
	}

	#region Input Actions
	private void Move()
	{
		float speedDirection = _moveInput.y < 0 ? _moveInput.y / 2 : _moveInput.y;

		speedDirection *= _blockSpeed; // Slow down the player when blocking

		rb.MovePosition(rb.position + (transform.forward * speedDirection * stats.MovementSpeed * Time.fixedDeltaTime));
		animator.SetFloat("Speed", _moveInput.y); // Animation works better with input value instead speed
	}

	private void Rotate()
	{
		float rotationAmount = _moveInput.x * stats.MovementSpeed * _rotationMultiplier * Time.fixedDeltaTime;

		rotationAmount *= _blockSpeed; // Slow down the player when blocking
		Quaternion turnOffset = Quaternion.Euler(0, rotationAmount, 0);
		rb.MoveRotation(rb.rotation * turnOffset);
	}

	// In future this would be a actor method, but for now is splitted
	// Require a adaption between AI and Player
	private void Attack()
	{
		if (!CanAttack()) return;

		_isAttacking = true;
		animator.SetTrigger("Attack");
		StartCoroutine(AttackCooldown());
	}

	void Block()
	{
		if (!CanBlock()) return;

		_initBlock = true;

		if (_blockCoroutine != null)
			StopCoroutine(_blockCoroutine);

		_blockCoroutine = StartCoroutine(LerpBlock(_blockBlendProgress, 1f));
	}

	void UnBlock()
	{
		_isBlocking = _initBlock = false;
		animator.SetBool("IsBlocking", false);

		if (_blockCoroutine != null)
			StopCoroutine(_blockCoroutine);

		_blockCoroutine = StartCoroutine(LerpBlock(_blockBlendProgress, 0f));
	}
	#endregion

	#region Cooldowns, Delays and Waits
	IEnumerator LerpBlock(float start, float end)
	{
		float duration = _blockDelay;
		float t = 0f;

		while (t < 1f)
		{
			_blockBlendProgress = Mathf.Lerp(start, end, t);

			float wbValue = Mathf.Lerp(0, 0.75f, _blockBlendProgress);
			float wbbValue = Mathf.Lerp(0, 0.75f, _blockBlendProgress);
			_blockSpeed = Mathf.Lerp(1f, 0.5f, _blockBlendProgress);

			animator.SetFloat("BlendWB", wbValue);
			animator.SetFloat("BlendWBB", wbbValue);
			animator.SetFloat("BlendIB", _blockBlendProgress);

			t += Time.deltaTime / duration;
			yield return null;
		}

		animator.SetBool("IsBlocking", _initBlock);
		_isBlocking = _initBlock;
		_blockBlendProgress = end;
	}

	IEnumerator AttackCooldown()
	{
		animator.SetFloat("AttackSpeed", stats.Speed);
		
		float attackDuration = animator.GetCurrentAnimatorClipInfo(1)[0].clip.length; // Attack layer 1
		float actualDuration = attackDuration / stats.Speed;
		
		yield return new WaitForSeconds(actualDuration);
		_isAttacking = false;
	}

	IEnumerator RecoveringCooldown()
	{
		_isRecovering = true;
		animator.SetBool("IsRecovering", true);
		float recoveringDuration = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
		yield return new WaitForSeconds(recoveringDuration);
		_isRecovering = false;
		animator.SetBool("IsRecovering", false);
	}

	IEnumerator ImmuneCooldown()
	{
		_isImmune = true;
		yield return new WaitForSeconds(_immuneTime);
		_isImmune = false;
	}
	#endregion

	// Skeletons be healing? That sound like a hex
	// Let the player be the only one that can heal
	public void Heal(int amount)
	{
		stats.HP += amount;
		if (stats.HP > stats.MaxHP) stats.HP = stats.MaxHP;
	}

	public override void TakeDamage(int amount)
	{
		if (!CanTakeDamage()) return;

		stats.HP -= amount;
		animator.SetInteger("Damaged", amount);
		
		if (stats.HP <= 0)
		{
			stats.HP = 0;
			_isDead = true;
			animator.SetBool("IsDead", true);
			animator.SetTrigger("Die");

			return; // Dead people cannot be hitted
		}


		_damageBlink.TriggerBlink();
		StartCoroutine(RecoveringCooldown());
		StartCoroutine(ImmuneCooldown());
		animator.SetTrigger("IsHitted");
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

	#region Checks
	private bool CanMotion()
	{
		// Only skeletons can move when they are dead
		return !(_isRecovering || _isDead);
	}

	private bool CanAttack()
	{
		// Alive people that aren't attacking or attacked can attack
		// Currently we cant attack blocking
		return !(_isAttacking || _isDead || _isRecovering || _isBlocking);
	}

	private bool CanTakeDamage()
	{
		// Only humans can take damage, people that avoid metaphysics laws can't be here
		return !(_isDead || _isImmune || _isBlocking);
	}

	private bool CanBlock()
	{
		// Only humans can block, people that avoid metaphysics laws can't be here
		return !(_isDead || _isAttacking || _isBlocking);
	}
	#endregion

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
