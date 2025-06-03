using System;
using System.Collections;
using UnityEngine;

public class AnaklettorIAController : EnemyController
{
    private ProjectileMarker _spellSpawnPoint;

    [SerializeField] private GameObject _spellPrefab;

    public Action OnDead;

	public override void Initialize(ActorData actorData, string name)
	{
		base.Initialize(actorData, name);
        targetFound = true;
	}

	protected override void SubscribeToEvents()
    {
        if (_iaEventsBridge != null)
        {
            _iaEventsBridge.spellThrow += TrhowSpell;
        }
    }

    protected override void OnDisable()
    {
        if (_iaEventsBridge != null)
        {
            _iaEventsBridge.spellThrow -= TrhowSpell;
        }
    }

    protected override void Start()
    {
        base.Start();
        _spellSpawnPoint = GetComponentInChildren<ProjectileMarker>();
    }

    protected override void Dead()
	{
        if (isDead) return;

		base.Dead();
		OnDead.Invoke();
	}


	protected override IEnumerator Attack()
    {
        _hasHit = false;
        _isAttacking = true;

        StartCoroutine(RangeAttack());

        yield return null;
    }

    public void TrhowSpell()
    {
        float randomOffsetX = UnityEngine.Random.Range(-0.35f, 0.35f);
        float randomOffsetY = UnityEngine.Random.Range(-0.35f, 0.35f);

        // Crear nueva posición de spawn con dispersion
        Vector3 randomOffset = new Vector3(randomOffsetX, randomOffsetY, 0);
        Vector3 spawnPosition = _spellSpawnPoint.transform.position + randomOffset;

        GameObject spell = Instantiate(_spellPrefab, spawnPosition, _spellSpawnPoint.transform.rotation);        

        Rigidbody spellRigiBody = spell.GetComponent<Rigidbody>();

        if (spellRigiBody != null)
        {
            spellRigiBody.AddForce(spell.transform.forward * 6, ForceMode.Impulse);
        }

    }

}

