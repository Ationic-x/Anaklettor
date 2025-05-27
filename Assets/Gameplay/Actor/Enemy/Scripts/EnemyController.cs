using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : ActorController
{

    //Objetos
    private Transform[] _eyeArray;

    private NavMeshAgent _nvAgent;

    private GameObject _playerGameObject;

    private ProjectileMarker _arrowSpawnPoint;

    [SerializeField] private GameObject _arrowGameObject;

    [SerializeField] private GameObject _soul; // Objeto de muerte del enemigo

    protected IAEventsBridge _iaEventsBridge;

    private Transform _eyeSpawnPoint;

    //Tamanyo del mapa
    private int areaSize = 18;

    //Bools
    private bool _isMoving = false;

    protected bool _isAttacking = false;

    private bool _targetFound = false;

    private bool _isSearching = false;

    private bool _isDead = false;

    protected bool _hasHit = false; // Ha golpeado al jugador? Evitamos multiple llamadas

    private uint _wasHittedID = 999; // Evita que se pueda recibir da�o varias veces del mismo ataque

    private float _runSpeed = 1f;


    protected override void Awake()
    {
        base.Awake();
        //_nvAgent = GetComponentInChildren<NavMeshAgent>();
        //GetEyes();
    }

    public override void Initialize(ActorData actorData, string name)
    {
        base.Initialize(actorData, name);

        _nvAgent = GetComponentInChildren<NavMeshAgent>();
        _nvAgent.speed = stats.Speed;
        _runSpeed = _nvAgent.speed + stats.MulSpeed;

        _eyeSpawnPoint = GetComponentInChildren<EyeMarker>().transform;

        _arrowSpawnPoint = GetComponentInChildren<ProjectileMarker>();
        _iaEventsBridge = GetComponentInChildren<IAEventsBridge>();
        SubscribeToEvents();

        if (actorData.stats.HP <= 0)
        {
            animator.SetLayerWeight(1, 1f);

            _isDead = false;
            Dead();
        }
        else
        {
            GetEyes();
            StartCoroutine(Idle(3));
        }
    }

    void Update()
    {
        if (!_isDead)
        {
            RayDetector();

            UpdateAnimationsVariables();
        }
        else
        {
            Dead();
        }
    }

    protected virtual void SubscribeToEvents()
    {
        if (_iaEventsBridge != null)
        {
            _iaEventsBridge.isShooting += SpawnArrow;
        }
    }

    protected virtual void OnDisable()
    {
        if (_iaEventsBridge != null)
        {
            _iaEventsBridge.isShooting -= SpawnArrow;
        }
    }

    protected virtual void Start()
    {
        _playerGameObject = GameObject.FindGameObjectWithTag(Tags.Player);
    }


    #region Vista

    //Se obtiene los transforms donde se lanzaran los rayos
    private void GetEyes()
    {
        _eyeArray = new Transform[_eyeSpawnPoint.childCount];

        int counter = 0;

        foreach (Transform child in _eyeSpawnPoint.transform)
        {
            _eyeArray[counter] = child.gameObject.transform;
            counter++;
        }

    }

    //Se lanzan los rayos y se comprueba si colisiona con el jugador
    private void RayDetector()
    {
        int rayDistance = 10;

        if (!_targetFound && !_isDead)
        {
            RaycastHit hit;

            if (_eyeArray != null)
            {
                foreach (Transform eye in _eyeArray)
                {
                    Vector3 origin = eye.position;

                    Vector3 direction = eye.forward;

                    //Esto permite que los rallos colisionen on todo
                    if (Physics.Raycast(origin, direction, out hit, rayDistance))
                    {

                        Debug.DrawRay(origin, direction * hit.distance, Color.red);

                        //Aqui se comprueba si el rayo colisiona con el jugador
                        if (hit.transform.gameObject.tag.Equals(Tags.Player))
                        {
                            _targetFound = true;
                        }

                    }
                    else
                    {
                        Debug.DrawRay(origin, direction * rayDistance, Color.green);
                    }
                }
            }
            else
            {
                Debug.LogError($"{transform.parent.name}: El array de ojos no est� inicializado.");
            }
        }

    }

    //Espera como maximo 2.2 segundos a realizar la animacion de idle en la que el enemigo mueve la cabeza y por lo tanto los raycast
    private IEnumerator SearchPlayer()
    {
        _isSearching = true;

        float searchDuration = 2.2f;

        float elapsed = 0f;

        while (elapsed < searchDuration)
        {
            if (_targetFound)
            {
                break;
            }

            elapsed += Time.deltaTime;

            yield return null;
        }

        Stop();

        if (_targetFound)
        {
            StartCoroutine(Chase());
        }
        else
        {
            StartCoroutine(Patrol());
        }

    }

    #endregion END Vista

    #region Movimiento

    private IEnumerator Idle(float secondsInIdle)
    {
        yield return new WaitForSeconds(secondsInIdle);

        Stop();

        if (_targetFound)
        {
            StartCoroutine(Chase());
        }
        else
        {
            StartCoroutine(Patrol());
        }
    }

    //Obtiene un punto aleatorio del escenario para la navegacion
    private Vector3 RandomPosition()
    {
        return new Vector3(Random.Range(-areaSize, areaSize), 0, Random.Range(-areaSize, areaSize)); ;
    }

    //Realiza una patrulla dirigiendose a un punto aleatorio del escenario
    //si ve al jugador en este trayecto lo perseguira
    private IEnumerator Patrol()
    {

        Move(RandomPosition());

        while (_isMoving)
        {
            yield return new WaitForFixedUpdate();

            if (_targetFound)
            {
                break;
            }
        }

        Stop();

        if (_targetFound)
        {
            StartCoroutine(Chase());
        }
        else
        {
            StartCoroutine(SearchPlayer());
        }

    }

    //Se indica a que posicion debe moverse la ia
    private void Move(Vector3 destination)
    {

        if (destination != Vector3.zero)
        {
            StartCoroutine(GoTo(destination));
        }

    }

    //Se persigue al jugador con un margen indicado al final del while
    private IEnumerator Chase()
    {
        //Como ya ha encontrado al jugador empieza a correr asi que se le asigna la velocidad de correr
        _nvAgent.speed = _runSpeed;

        Vector2 src = new Vector2(transform.position.x, transform.position.z);
        Vector2 dst = new Vector2(_playerGameObject.transform.position.x, _playerGameObject.transform.position.z);

        if (_playerGameObject)
        {

            while (Vector2.Distance(src, dst) >= stats.MinDistanceAttack)
            {
                src = new Vector2(transform.position.x, transform.position.z);
                dst = new Vector2(_playerGameObject.transform.position.x, _playerGameObject.transform.position.z);
                yield return new WaitForFixedUpdate();

                Move(_playerGameObject.transform.position);
            }

            Stop();

            StartCoroutine(Attack());

        }

    }

    //La ia se mueve a la poscion indicada
    private IEnumerator GoTo(Vector3 destination)
    {
        _isMoving = true;
        _nvAgent.SetDestination(destination);

        Vector2 src = new Vector2(transform.position.x, transform.position.z);
        Vector2 dst = new Vector2(destination.x, destination.z);

        while (Vector2.Distance(src, dst) >= stats.MinDistanceAttack)
        {
            src = new Vector2(transform.position.x, transform.position.z);
            yield return new WaitForFixedUpdate();
        }

        _isMoving = false;
    }

    private IEnumerator LookAtPlayer()
    {
        // Mientras est� atacando, rota hacia el jugador
        while (_isAttacking)
        {
            Vector3 direction = (_playerGameObject.transform.position - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime * stats.Speed
                );
            }
            yield return null;
        }
    }

    //Para el movimiento y cualquier estado de la ia
    private void Stop()
    {
        _nvAgent.SetDestination(this.transform.position);

        _isMoving = false;

        _isAttacking = false;

        _isSearching = false;

    }

    #endregion END Movimiento

    #region Ataque

    protected virtual IEnumerator Attack()
    {
        _hasHit = false;
        _isAttacking = true;

        if (job == Jobs.Archer)
        {
            StartCoroutine(RangeAttack());
        }
        else
        {
            StartCoroutine(SwordBlow());
        }

        yield return null;
    }


    //Realiza un espadazo con un cooldown determinado
    private IEnumerator SwordBlow()
    {

        animator.SetTrigger("AttackTrigger");

        StartCoroutine(LookAtPlayer());
        Vector2 src = new Vector2(transform.position.x, transform.position.z);
        Vector2 dst = new Vector2(_playerGameObject.transform.position.x, _playerGameObject.transform.position.z);

        ////Si sigue estando al alcance de golpear otra vez espera 1.5 segundos y vuelve a atacar
        while (Vector2.Distance(src, dst) <= stats.MinDistanceAttack)
        {
            animator.SetTrigger("AttackTrigger");
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length + stats.AtkDelay);

            src = new Vector2(transform.position.x, transform.position.z);
            dst = new Vector2(_playerGameObject.transform.position.x, _playerGameObject.transform.position.z);
        }

        Stop();
        StartCoroutine(Chase());

    }

    // Esto solo se lanza si es un esqueleto cuerpo a cuerpo
    // El arquero u otro instancia un proyectil con sus propios calculos
    private void OnTriggerEnter(Collider other)
    {
        if (_hasHit) return; // Evita m�ltiples llamadas

        // Deber�a estar filtrado por layer gracias al ridgidbody del enemigo, pero por si acaso
        if (other.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {
            int damage = 0;
            player.Hit(transform, damage);
        }
    }


    protected IEnumerator RangeAttack()
    {
        //Ajustar cuando realmente tiene municion y cuando no 

        StartCoroutine(LookAtPlayer());

        Vector2 src = new Vector2(transform.position.x, transform.position.z);
        Vector2 dst = new Vector2(_playerGameObject.transform.position.x, _playerGameObject.transform.position.z);

        //Si sigue estando al alcance de golpear otra vez espera 1.5 segundos y vuelve a atacar
        while (Vector2.Distance(src, dst) <= stats.MinDistanceAttack)
        {
            // Disparar
            animator.ResetTrigger("ReloadTrigger");
            animator.SetTrigger("ShootTrigger");

            //Se dispara la flecha segun la animacion controlado por un evento y el script IAEventsBridge                        
            yield return WaitForCurrentAnimation();

            // Recargar
            animator.ResetTrigger("ShootTrigger");
            animator.SetTrigger("ReloadTrigger");
            yield return WaitForCurrentAnimation();
            yield return new WaitForSeconds(stats.AtkDelay);

			src = new Vector2(transform.position.x, transform.position.z);
            dst = new Vector2(_playerGameObject.transform.position.x, _playerGameObject.transform.position.z);
        }

        Stop();
        StartCoroutine(Chase());
    }

    public void SpawnArrow()
    {
        float randomOffsetX = Random.Range(-0.35f, 0.35f);
        float randomOffsetY = Random.Range(-0.35f, 0.35f);

        // Crear nueva posici�n de spawn con dispersion
        Vector3 randomOffset = new Vector3(randomOffsetX, randomOffsetY, 0);
        Vector3 spawnPosition = _arrowSpawnPoint.transform.position + randomOffset;

        Vector3 _arrowAttackRotationOffset = new Vector3(-90, 0, 0);

        GameObject arrow = Instantiate(_arrowGameObject, spawnPosition, Quaternion.Euler(_arrowAttackRotationOffset + transform.rotation.eulerAngles));

        Rigidbody arrowRigiBody = arrow.GetComponent<Rigidbody>();

        arrowRigiBody.useGravity = true;

        if (arrowRigiBody != null)
        {
            arrowRigiBody.AddForce(-arrow.transform.up * 6, ForceMode.Impulse);
        }

    }

    //Esto hace que aparezca en los 3 puntos en el componente script este metodo para ejecutar 
    [ContextMenu("Dead")]
    public void Dead()
    {
        if (!_isDead)
        {
            _isDead = true;
            Stop();
            StopAllCoroutines();
            _nvAgent.enabled = false;
            animator.SetTrigger("DieTrigger");
            GetComponent<BoxCollider>().enabled = false;
        }
    }

    [ContextMenu("TakeDamage")]
    public void TakeDamage(uint attackID, int damage)
    {
        if (_wasHittedID == attackID) return;

        _wasHittedID = attackID;
        stats.HP -= damage;

        if (stats.HP <= 0)
        {
            DropSoul();
            Dead();
        }
        else
            StartCoroutine(IsDamaged());

    }

    private void DropSoul()
    {
        Instantiate(_soul, new Vector3(transform.position.x, 2, transform.position.z), Quaternion.identity);
    }

    private IEnumerator IsDamaged()
    {
        Stop();

        animator.SetTrigger("HitTrigger");

        //Hace que el enemigo no se mueva al recibir el da�o
        _nvAgent.isStopped = true;

        yield return WaitForCurrentAnimation();

        //Hace que el enemigo pueda seguir moviendose
        _nvAgent.isStopped = false;

        _targetFound = true;

        StartCoroutine(Chase());

    }

    #endregion END Ataque

    #region Animaciones

    private void UpdateAnimationsVariables()
    {
        if (animator)
        {
            animator.SetBool("_isMoving", _isMoving);
            animator.SetBool("_isAttacking", _isAttacking);
            animator.SetBool("_isSearching", _isSearching);
            animator.SetBool("_targetFound", _targetFound);

        }

    }

    #endregion

    #region Tools

    private IEnumerator WaitForCurrentAnimation()
    {
        // Esperar un frame para que el Animator se actualice
        yield return null;

        // Obtener duraci�n real del clip activo
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);

        if (clipInfo.Length > 0)
        {
            float duration = clipInfo[0].clip.length;
            yield return new WaitForSeconds(duration);
        }
    }

    // Encuentra un hijo dentro de una jerarqu�a especificada usando una ruta.
    //Devuelve null si no se encuentra alguno de los nodos. 
    private GameObject FindChildRecursive(Transform parentTransform, string path)
    {
        string[] segments = path.Split('/');

        Transform currentTransform = parentTransform;

        foreach (string segment in segments)
        {
            currentTransform = currentTransform.Find(segment);

            if (currentTransform == null)
            {
                return null;
            }
        }

        return currentTransform.gameObject;
    }

    #endregion END Tools
}
