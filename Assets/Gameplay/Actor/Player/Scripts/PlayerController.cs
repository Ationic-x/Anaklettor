using System;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : ActorController
{
	private ChangeMaterialColor _damageBlink;

	private int _level;
	private int _exp;
    private int _maxExp;

	private Vector2 _moveInput;
    
	private float _rotationMultiplier = 40f; // Rotation speed is slower than movement
	private float _immuneTime = 1.5f;

	private float _blockSpeed = 1f;
	private float _blockDelay = 0.5f; // Time required from 0 to block
	private float _blockBlendProgress = 0f;

	private float _speedDelay = 0.5f; // Time required from 0 to full speed
	private float _speedBlendProgress = 0f;

	private uint _attackID = 0; // Used to avoid multiple instances of the same attack

	// States
	private bool _isAttacking = false;
	private bool _isImmune = false;
	private bool _isDead = false;
	private bool _isBlocking = false;
	private bool _isMoving = false;

	private bool _initMove = false;
	private bool _initBlock = false;

	public bool smallDamage = false;
	public bool hugeDamage = false;

	private Coroutine _blockCoroutine = null;
	private Coroutine _speedCoroutine = null;

	private CinemachineCamera _mapCamera;

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
		MoveAnimation();

		// Modified in future
		_forward = false;
		_backWard = false;
		_left = false;
		_right = false;

		if (_moveInput.y > 0) _forward = true;
		if (_moveInput.y < 0) _backWard = true;
		if (_moveInput.x < 0) _left = true;
		if (_moveInput.x > 0) _right = true;
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
		}
	}
	#endregion


	private void Start()
	{
		_mapCamera = GameObject.Find("Map Camera").GetComponent<CinemachineCamera>();
	}


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
	private void MoveAnimation()
	{

		if (_moveInput.y == 0 && _isMoving)
		{
			if (_speedCoroutine != null)
				StopCoroutine(_speedCoroutine);

			_speedCoroutine = StartCoroutine(LerpSpeed(_speedBlendProgress, 0f));
			
			_initMove = false;
		}
		else if (_moveInput.y != 0 && !_isMoving)
		{
			if (_speedCoroutine != null)
				StopCoroutine(_speedCoroutine);

			_speedCoroutine = StartCoroutine(LerpSpeed(_speedBlendProgress, 1f));
			
			_isMoving = _initMove = true;
			animator.SetBool("IsMoving", true);
		}
	}

	private void Move()
	{
		float speedDirection = _moveInput.y < 0 ? _moveInput.y / 2 : _moveInput.y;

		speedDirection *= _blockSpeed * _speedBlendProgress; // Slow down the player when blocking

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

			float wbValue = Mathf.Lerp(0f, 0.75f, _blockBlendProgress);
			float wbbValue = Mathf.Lerp(0f, 0.75f, _blockBlendProgress);
			_blockSpeed = Mathf.Lerp(1f, 0.75f, _blockBlendProgress);

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

    IEnumerator LerpSpeed(float start, float end)
    {
        float duration = _speedDelay;
        float t = 0f;

        while (t < 1f)
        {
            _speedBlendProgress = Mathf.Lerp(start, end, t);

            float speedValue = Mathf.Lerp(1f, 0f, _speedBlendProgress);

            animator.SetFloat("BlendS", speedValue);

            t += Time.deltaTime / duration;
            yield return null;
        }

		animator.SetBool("IsMoving", _initMove);
		_isMoving = _initMove;

		_speedBlendProgress = end;

		yield return new WaitForEndOfFrame();
		if (end == 0)
			MoveAnimation();
	}

    IEnumerator AttackCooldown()
	{
		animator.SetFloat("AttackSpeed", stats.Speed);
		
		float attackDuration = animator.GetCurrentAnimatorClipInfo(1)[0].clip.length; // Attack layer 1
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
	#endregion

	#region General exposed methods
	// Skeletons be healing? That sound like a hex
	// Let the player be the only one that can heal
	public void Heal(int amount)
	{
		stats.HP += amount;
		if (stats.HP > stats.MaxHP) stats.HP = stats.MaxHP;
	}

	public void Hit(Transform sourceRef, int damage)
	{
		float playerAngle = transform.eulerAngles.y;
		float sourceAngle = sourceRef.transform.eulerAngles.y;
		float angleDif = sourceAngle > playerAngle ? sourceAngle - playerAngle : playerAngle - sourceAngle;

		if (_isBlocking && angleDif > 80)
			animator.SetTrigger("Block");
		else
		{
			TakeDamage(damage);
			rb.AddForce(sourceRef.forward * (damage + 3), ForceMode.Impulse); // currently the +3 is hardcode for testing purposes
		}

	}

	private void TakeDamage(int amount)
	{
		if (!CanTakeDamage()) return;

		stats.HP -= amount;
		animator.SetInteger("Damaged", amount);
		
		if (stats.HP <= 0)
		{
			stats.HP = 0;
			_isDead = true;
			animator.SetLayerWeight(1, 0f);
			animator.SetLayerWeight(2, 0f);
			animator.SetBool("IsDead", true);
			animator.SetTrigger("Die");
			_mapCamera.Priority = 30;

			return; // Dead people cannot be hitted
		}


		_damageBlink.TriggerBlink();
		StartCoroutine(ImmuneCooldown());
		animator.SetTrigger("Hit");
	}


	public override void Initialize(ActorData actorData, string name)
    {
        // Initialize default values
        base.Initialize(actorData, name);

        // Initialize unique player values
		_level = 1;
		_exp = 0;
		_maxExp = 100;
		_damageBlink = GetComponentInChildren<ChangeMaterialColor>();
	}
	#endregion

	#region Checks
	private bool CanMotion()
	{
		// Only skeletons can move when they are dead
		return !(_isDead);
	}

	private bool CanAttack()
	{
		// Alive people that aren't attacking or attacked can attack
		// Currently we cant attack blocking
		return !(_isAttacking || _isDead || _isBlocking);
	}

	private bool CanTakeDamage()
	{
		// Only humans can take damage, people that avoid metaphysics laws can't be here
		return !(_isDead || _isImmune);
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

    public void GainExperience(int amount) 
    {
        _exp += amount;
        
		if (_exp >= _maxExp) 
        {
			_exp = 0;
            LevelUp();
        }
    }

	#region Should be changed
	private bool _forward = false;
	private bool _backWard = false;
	private bool _left = false;
	private bool _right = false;
	private MapManager _mapManager;
	private Vector3 _enterPosition;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.TryGetComponent<EnemyController>(out EnemyController enemy))
			enemy.TakeDamage(_attackID++, 1);
		
		if (other.gameObject.TryGetComponent<WarpData>(out WarpData warp))
		{
			warp.gameObject.GetComponentInChildren<Animator>().SetTrigger("In");
			_enterPosition = transform.position;
		}
	}

	// This code have a lesser misstake, my test scenario is reversed so X and Z are positive when should be negative
	private void OnTriggerExit(Collider other)
	{
		WarpData warp = other.GetComponentInChildren<WarpData>();
		if (warp == null) return;


		float offset = 1.1f;
		other.GetComponentInChildren<Animator>()?.SetTrigger("Out");

		Vector3 position = transform.position;
		Vector2Int direction = warp.Direction;

		bool shouldWarp =
			(direction == Vector2Int.up && _enterPosition.z * offset < position.z) ||
			(direction == Vector2Int.down && _enterPosition.z * offset > position.z) ||
			(direction == Vector2Int.right && _enterPosition.x * offset < position.x) ||
			(direction == Vector2Int.left && _enterPosition.x * offset > position.x);

		if (shouldWarp)
			_mapManager.ChangeScene(warp.NewRoom, direction);
	}

	public void ChangeScene()
	{
		_backWard = false;
		_forward = false;
		_left = false;
		_right = false;
	}

	public void SetMapManager(MapManager newMapManager)
	{
		_mapManager = newMapManager;
	}
	#endregion
}
