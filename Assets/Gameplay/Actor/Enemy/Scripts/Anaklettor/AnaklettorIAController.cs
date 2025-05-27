using System.Collections;
using UnityEngine;

public class AnaklettorIAController : EnemyController
{
    private ProjectileMarker _spellSpawnPoint;

    [SerializeField] private GameObject _spellPrefab;

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


    protected override IEnumerator Attack()
    {
        _hasHit = false;
        _isAttacking = true;

        StartCoroutine(RangeAttack());

        yield return null;
    }

    public void TrhowSpell()
    {
        float randomOffsetX = Random.Range(-0.35f, 0.35f);
        float randomOffsetY = Random.Range(-0.35f, 0.35f);

        // Crear nueva posición de spawn con dispersion
        Vector3 randomOffset = new Vector3(randomOffsetX, randomOffsetY, 0);
        Vector3 spawnPosition = _spellSpawnPoint.transform.position + randomOffset;

        //Vector3 _spellAttackRotationOffset = new Vector3(0, 265, 0);

        Quaternion rotation = new Quaternion(_spellSpawnPoint.transform.rotation.x, _spellSpawnPoint.transform.rotation.y, _spellSpawnPoint.transform.rotation.z, _spellSpawnPoint.transform.rotation.w);

        GameObject spell = Instantiate(_spellPrefab, spawnPosition, rotation);
        //Debug.Break();

        Rigidbody spellRigiBody = spell.GetComponent<Rigidbody>();

        if (spellRigiBody != null)
        {
            spellRigiBody.AddForce(_spellPrefab.transform.forward * 6, ForceMode.Impulse);
        }

    }

}

