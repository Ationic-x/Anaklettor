using System;
using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : ActorController
{
	private ChangeMaterialColor _damageBlink;

    private readonly int _maxExp = 100;

	private Vector2 _moveInput;
    
	private float _rotationMultiplier = 40f; // Rotation speed is slower than movement
	private float _immuneTime = 1.5f;

	private float _speedDelay = 0.5f;
	private float _blockMovementSpeed = 1f;
	private float _blockBlendProgress = 0f;
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

	private Coroutine _blockCoroutine = null;
	private Coroutine _speedCoroutine = null;

	private CinemachineCamera _mapCamera;

    // Warp control
    private MapManager _mapManager;
    private Vector3 _enterPosition;

    #region Events
    // Event launched
	public event Action OnLevelUp;
	public event Action OnUpgradeSelected;
	public event Action OnHPChange;
	public event Action OnPlayerDied;

	// Eevent received
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
		if (HUDController.isBusy) return;

		_moveInput = context.action.ReadValue<Vector2>();
		MoveAnimation();
	}

    private void OnAttackInput(InputAction.CallbackContext context = default)
    {
		if (HUDController.isBusy) return;

		Attack();
	}

	private void OnBlockInput(InputAction.CallbackContext context = default)
	{
		if (HUDController.isBusy) return;

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

		speedDirection *= _blockMovementSpeed * _speedBlendProgress; // Slow down the player when blocking

		rb.MovePosition(rb.position + (transform.forward * speedDirection * stats.Speed * Time.fixedDeltaTime));
		animator.SetFloat("Speed", _moveInput.y); // Animation works better with input value instead speed
	}

	private void Rotate()
	{
		float rotationAmount = _moveInput.x * stats.Speed * _rotationMultiplier * Time.fixedDeltaTime;

		rotationAmount *= _blockMovementSpeed; // Slow down the player when blocking
		Quaternion turnOffset = Quaternion.Euler(0, rotationAmount, 0);
		rb.MoveRotation(rb.rotation * turnOffset);
	}

	// In future this would be a actor method, but for now is splitted
	// Require a adaption between AI and Player
	private void Attack()
	{
		if (!CanAttack()) return;

		_isAttacking = true;
		_attackID++;
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
		float duration = stats.Bspd;
		float t = 0f;

		while (t < 1f)
		{
			_blockBlendProgress = Mathf.Lerp(start, end, t);

			float wbValue = Mathf.Lerp(0f, 0.75f, _blockBlendProgress);
			float wbbValue = Mathf.Lerp(0f, 0.75f, _blockBlendProgress);
			_blockMovementSpeed = Mathf.Lerp(1f, 0.75f, _blockBlendProgress);

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
		animator.SetFloat("AttackSpeed", stats.Aspd);
		
		float attackDuration = animator.GetCurrentAnimatorClipInfo(1)[0].clip.length; // Attack layer 1
		float actualDuration = attackDuration / stats.Aspd;
		
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
		int resultHP = stats.HP + amount;
		if (resultHP > stats.MaxHP) amount = stats.MaxHP - resultHP;
		
		ChangeCurrentHP(amount);
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
			float radians = sourceAngle * Mathf.Deg2Rad;
			Vector3 direction = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians));
            TakeDamage(damage);
			rb.AddForce(direction * (damage + 3), ForceMode.Impulse); // currently the +3 is hardcode for testing purposes
		}

	}

	private void TakeDamage(int amount)
	{
		if (!CanTakeDamage()) return;

        int resultHP = stats.HP - amount;
        if (resultHP < 0) amount = stats.HP;
        ChangeCurrentHP(-amount);

        animator.SetInteger("Damaged", amount);
		
		if (resultHP <= 0)
		{
			OnPlayerDied.Invoke();
			gameObject.GetComponent<Rigidbody>().isKinematic = true;
            gameObject.GetComponent<BoxCollider>().enabled = false;
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


	public void Initialize(ActorData actorData, PlayerData playerData)
    {
		_immuneTime = playerData.immuneTime;
        base.Initialize(actorData, playerData.nickname);
		stats.UpdateStats(playerData.stats);


		_damageBlink = GetComponentInChildren<ChangeMaterialColor>();
	}


    public void SetMapManager(MapManager newMapManager)
    {
        _mapManager = newMapManager;
    }

	public float ImmuneTime { get { return _immuneTime; } }
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


	#region LevelUP system
	private void LevelUp() 
    {
		Stats.Level += 1;
        ChangeCurrentHP(Stats.MaxHP - Stats.HP);
        OnLevelUp.Invoke();
	}

    public void GainExperience(int amount) 
    {
        Stats.Exp += amount;
        
		if (Stats.Exp >= _maxExp) 
        {
            Stats.Exp -= _maxExp;
            LevelUp();
        }
    }

	public void IncreaseAspd()
	{
		stats.Aspd *= stats.MulAspd;
		OnUpgradeSelected.Invoke();
	}
	
	public void IncreaseBspd()
	{
		stats.Bspd *= stats.MulBspd;
		OnUpgradeSelected.Invoke();
	}

	public void IncreaseHP()
	{
		stats.MaxHP += stats.AddHP;
		ChangeCurrentHP(stats.AddHP);
		OnUpgradeSelected.Invoke();
	}

	public void IncreaseAtk()
	{
		stats.Attack += stats.AddAttack;
		OnUpgradeSelected.Invoke();
	}

	public void IncreaseSpeed()
	{
		stats.Speed *= stats.MulSpeed;
		OnUpgradeSelected.Invoke();
	}

	public void IncreaseImmunity()
	{
		_immuneTime++;
		OnUpgradeSelected.Invoke();
	}

	public void LowerAtkCD()
	{
		stats.AtkDelay *= stats.MulAtkDelay;
		OnUpgradeSelected.Invoke();
	}
	#endregion

	#region Triggers & Collisions
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.TryGetComponent<EnemyController>(out EnemyController enemy))
			enemy.TakeDamage(_attackID, 1);
		
		if (other.gameObject.TryGetComponent<WarpData>(out WarpData warp))
		{
			warp.gameObject.GetComponentInChildren<Animator>().SetTrigger("In");
			_enterPosition = transform.position;
		}
	}

	// This code have a lesser misstake, my test scenario is reversed so X and Z are positive when should be negative
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.TryGetComponent<WarpData>(out WarpData warp))
		{

			float offset = 1.1f;
			warp.GetComponentInChildren<Animator>().SetTrigger("Out");

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
	}
    #endregion

	private void ChangeCurrentHP(int amount)
	{
        Stats.HP += amount;
		OnHPChange.Invoke();
	}
}
