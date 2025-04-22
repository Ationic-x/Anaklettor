using UnityEngine;

public class TestMovement : MonoBehaviour
{

    private bool _forward = false;
    private bool _backWard = false;
    private bool _left = false;
    private bool _right = false;
    private float _speed = 5f;
    [SerializeField] private MapManager _mapManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) { _forward = true; }
        else if (Input.GetKeyUp(KeyCode.UpArrow)) { _forward = false; }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { _left = true; }
        else if (Input.GetKeyUp(KeyCode.LeftArrow)) { _left = false; }
        if (Input.GetKeyDown(KeyCode.RightArrow)) { _right = true; }
        else if (Input.GetKeyUp(KeyCode.RightArrow)) { _right = false; }
        if (Input.GetKeyDown(KeyCode.DownArrow)) { _backWard = true; }
        else if (Input.GetKeyUp(KeyCode.DownArrow)) { _backWard = false; }
    }

    private void FixedUpdate()
    {
        if (_forward)
        {
            transform.position += transform.forward * Time.deltaTime * _speed;
        }
        if (_backWard)
        {
            transform.position -= transform.forward * Time.deltaTime * _speed;
        }
        if (_left)
        {
            transform.position -= transform.right * Time.deltaTime * _speed;
        }
        if (_right)
        {
            transform.position += transform.right * Time.deltaTime * _speed;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.name)
        {
            case "North":
                _mapManager.ChangeScene(Vector2Int.up);
                break;
            case "South":
                _mapManager.ChangeScene(Vector2Int.down);
                break;
            case "East":
                _mapManager.ChangeScene(Vector2Int.right);
                break;
            case "West":
                _mapManager.ChangeScene(Vector2Int.left);
                break;
            default:
                break;
        }
    }
}
