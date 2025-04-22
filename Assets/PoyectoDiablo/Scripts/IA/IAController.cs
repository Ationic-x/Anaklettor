using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IAController : MonoBehaviour
{
    private GameObject[] _eyeArray;

    private NavMeshAgent _nvAgent;

    GameObject _head;

    public float areaSize = 2.5f; // mitad del plano de 5x5

    private void Awake()
    {
        _nvAgent = GetComponent<NavMeshAgent>();

        GetEyes();

    }


    void Start()
    {
   
    }


    void Update()
    {
        RayDetector();
        SearchPlayer();
    }




    private void RandomWalk()
    {
        if (_nvAgent)
        {
            Vector3 randomDirection = new Vector3(Random.Range(-areaSize, areaSize), 0, Random.Range(-areaSize, areaSize));

            Vector3 finalPosition;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 2.5f, NavMesh.AllAreas))
            {
                finalPosition = hit.position;

                _nvAgent.SetDestination(finalPosition);

                Debug.DrawLine(transform.position, finalPosition, Color.black, 1f);
            }

        }
    }


    private void SearchPlayer()
    {
        float minY = -63f;
        float maxY = 63f;
        float speed = 1.2f;

        if (_head)
        {
            float angle = Mathf.Lerp(minY, maxY, (Mathf.Sin(Time.time * speed) + 1f) / 2f);

            Vector3 currentRotation = _head.transform.localEulerAngles;

            _head.transform.localEulerAngles = new Vector3(currentRotation.x, angle, currentRotation.z);
        }
        else
        {
            _head = GameObject.Find("head").gameObject;
        }

    }


    #region Vista

    private void GetEyes()
    {
        try
        {
            GameObject eyeParent = GameObject.Find("RayCastEyes").gameObject;

            if (eyeParent)
            {
                _eyeArray = new GameObject[eyeParent.transform.childCount];

                int counter = 0;

                foreach (Transform child in eyeParent.transform)
                {
                    _eyeArray[counter] = child.gameObject;
                    counter++;
                }

                Debug.Log("Ojos cargados: " + _eyeArray.Length);
            }

            else
            {
                Debug.LogWarning("No se encontró el GameObject 'RayCastEyes'.");
            }

        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    private void RayDetector()
    {
        RaycastHit hit;

        foreach (GameObject eye in _eyeArray)
        {
            Vector3 origin = eye.transform.position;

            Vector3 direction = eye.transform.forward;

            if (Physics.Raycast(origin, direction, out hit, 20, LayerMask.GetMask("Player")))
            {
                Debug.DrawRay(origin, direction * hit.distance, Color.red);

                Debug.Log("Did Hit: " + hit.collider.name);
            }
            else
            {
                Debug.DrawRay(origin, direction * 20, Color.green);
            }
        }
    }

    #endregion END Vista

}

