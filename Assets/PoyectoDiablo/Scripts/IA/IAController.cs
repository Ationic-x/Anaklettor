using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class IAController : MonoBehaviour
{

    //Objetos referenciados    
    [SerializeField] private GameObject _eyeObject;

    //Objetos
    private GameObject[] _eyeArray;

    private NavMeshAgent _nvAgent;

    private GameObject _head;

    private GameObject _playerGameObject;

    //Tamanyo del mapa
    public int areaSize = 20;

    //Bools
    private bool _isMoving = false;

    private bool _isAttacking = false;

    private bool _targetFound = false;

    private bool _searching = false;

    private void Awake()
    {
        _head = GameObject.Find("head").gameObject;

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
    }


    #region Vista

    //Se obtiene los transforms donde se lanzaran los rayos
    private void GetEyes()
    {
        try
        {
            if (_eyeObject)
            {
                _eyeArray = new GameObject[_eyeObject.transform.childCount];

                int counter = 0;

                foreach (Transform child in _eyeObject.transform)
                {
                    _eyeArray[counter] = child.gameObject;
                    counter++;
                }
            }
            else
            {
                Debug.LogError($"{transform.parent.name}: El campo '_eyeObject' no está asignado en el inspector.");
            }

        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
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

    //Mueve la cabeza del modelo a la que esta asociada los raycast
    private IEnumerator SearchPlayer()
    {

        _searching = true;

        //Cantidad de rotacion de la cabeza
        float minY = -63f;
        float maxY = 63f;

        //Velociad de rotacion de la cabeza
        float speed = 2f;

        //Tiempo transcurrido de la busqueda
        float elapsedTime = 0f;

        // Duracion busqueda
        float duration = 2.5f;

        //Rotacion original de la cabeza
        Vector3 originalHeadRotation = _head.transform.localEulerAngles;

        if (_head && !_targetFound)
        {
            while (!_targetFound && _searching && elapsedTime < duration)
            {

                float angle = Mathf.Lerp(minY, maxY, (Mathf.Sin(Time.time * speed) + 1f) / 2f);

                Vector3 currentRotation = _head.transform.localEulerAngles;
                _head.transform.localEulerAngles = new Vector3(currentRotation.x, angle, currentRotation.z);

                elapsedTime += Time.deltaTime;

                if (_targetFound)
                {
                    break;
                }

                yield return null;

            }

        }

        _head.transform.localEulerAngles = originalHeadRotation;

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

        _searching = false;

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

        StartCoroutine(Chase());
    }


    #endregion END Ataque
}

