using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

public class IAController : MonoBehaviour
{

    //Objetos
    private GameObject[] _eyeArray;

    private NavMeshAgent _nvAgent;

    private GameObject _playerGameObject;

    private Animator _animator;

    //Tamanyo del mapa
    private int areaSize = 20;

    //Bools
    private bool _isMoving = false;

    private bool _isAttacking = false;

    private bool _targetFound = false;

    private bool _isSearching = false;

    private bool _isInCooldown = false;

    private void Awake()
    {
        _animator = GetComponentInParent<Animator>();

        _nvAgent = GetComponent<NavMeshAgent>();

        GetEyes();
    }

    void Start()
    {
        StartCoroutine(Patrol());
    }

    void Update()
    {
        RayDetector();

        UpdateAnimationsVariables();
    }

    #region Vista

    //Se obtiene los transforms donde se lanzaran los rayos
    private void GetEyes()
    {

        GameObject eyeObject = FindChildRecursive(transform, "root/hips/spine/chest/head/RayCastEyes");

        if (eyeObject)
        {
            _eyeArray = new GameObject[eyeObject.transform.childCount];

            int counter = 0;

            foreach (Transform child in eyeObject.transform)
            {
                _eyeArray[counter] = child.gameObject;
                counter++;
            }
        }
        else
        {
            Debug.LogError($"{transform.parent.name}: El campo 'eyeObject' no está asignado en el inspector.");
        }

    }

    //Se lanzan los rayos y se comprueba si colisiona con el jugador
    private void RayDetector()
    {

        if (!_targetFound)
        {
            RaycastHit hit;

            if (_eyeArray != null)
            {
                foreach (GameObject eye in _eyeArray)
                {
                    Vector3 origin = eye.transform.position;

                    Vector3 direction = eye.transform.forward;

                    //Esto permite que los rallos colisionen on todo
                    if (Physics.Raycast(origin, direction, out hit, 20))
                    {

                        Debug.DrawRay(origin, direction * hit.distance, Color.red);

                        //Aqui se comprueba si el rayo colisiona con el jugador
                        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                        {
                            _playerGameObject = hit.transform.GameObject();

                            _targetFound = true;
                        }

                    }
                    else
                    {
                        Debug.DrawRay(origin, direction * 20, Color.green);
                    }
                }
            }
            else
            {
                Debug.LogError($"{transform.parent.name}: El array de ojos no está inicializado.");
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
        if (_playerGameObject)
        {

            while (Vector3.Distance(transform.position, _playerGameObject.transform.position) >= 2f)
            {
                yield return new WaitForFixedUpdate();

                Move(_playerGameObject.transform.position);
            }

            Stop();

            StartCoroutine(SwordBlow());

        }

    }

    //La ia se mueve a la poscion indicada
    private IEnumerator GoTo(Vector3 destination)
    {
        _isMoving = true;

        _nvAgent.SetDestination(destination);

        while (Vector3.Distance(transform.position, destination) >= 1.5f)
        {
            yield return new WaitForFixedUpdate();
        }

        _isMoving = false;
    }

    //Para el movimiento y cualquier estado de la ia
    private void Stop()
    {
        _nvAgent.SetDestination(this.transform.position);

        _isMoving = false;

        _isAttacking = false;

        _isSearching = false;

        _isInCooldown = false;

    }

    #endregion END Movimiento

    #region Ataque

    private void Attack()
    {
        StartCoroutine(SwordBlow());
    }

    //Realiza un espadazo con un cooldown determinado
    private IEnumerator SwordBlow()
    {
        _isAttacking = true;

        //yield return new WaitForEndOfFrame();

        //float duration = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        //float cooldown = 2f;

        //float elapsedTime = 0f;

        //while(duration + cooldown > elapsedTime)
        //{
        //    elapsedTime += Time.deltaTime;
        //}

        float elapsedTime = 0f;

        float coolDownTime = 1f;

        while (Vector3.Distance(transform.position, _playerGameObject.transform.position) <= 2f)
        {

            elapsedTime += Time.deltaTime;

            if (elapsedTime >= coolDownTime)
            {

                Debug.Log("Ataca");

                break;
            }

            yield return null;
        }


        Stop();

        //yield return new WaitForEndOfFrame();

        StartCoroutine(Chase());
    }
   


    #endregion END Ataque

    #region Animaciones

    private void UpdateAnimationsVariables()
    {
        if (_animator)
        {
            _animator.SetBool("_isMoving", _isMoving);
            _animator.SetBool("_isAttacking", _isAttacking);
            _animator.SetBool("_isSearching", _isSearching);

            _animator.SetBool("_isInCooldown", _isInCooldown);
        }

    }

    #endregion

    #region Tools

    // Encuentra un hijo dentro de una jerarquía especificada usando una ruta.
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

