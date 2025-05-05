using UnityEngine;

public class TestMovement : MonoBehaviour
{

    private bool _forward = false;
    private bool _backWard = false;
    private bool _left = false;
    private bool _right = false;
    private float _speed = 5f;
    private MapManager _mapManager;
    private Vector3 _enterPosition;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
		_forward = false;
		_backWard = false;
        _left = false;
		_right = false;
		if (Input.GetKey(KeyCode.UpArrow)) { _forward = true; }
        if (Input.GetKey(KeyCode.LeftArrow)) { _left = true; }
        if (Input.GetKey(KeyCode.RightArrow)) { _right = true; }
        if (Input.GetKey(KeyCode.DownArrow)) { _backWard = true; }
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
        if (other.CompareTag("Warp"))
        {
			other.gameObject.GetComponentInChildren<Animator>().SetTrigger("In");
			other.gameObject.SetActive(true);
			_enterPosition = transform.position;
		}
	}

	// This code have a lesser misstake, my test scenario is reversed so X and Z are positive when should be negative
	private void OnTriggerExit(Collider other)
	{
        if (other.CompareTag("Warp"))
        {
			float offset = 0.2f; // small offset to avoid unexpected behaviours 
			other.gameObject.GetComponentInChildren<Animator>().SetTrigger("Out");
			switch (other.gameObject.name)
			{
				case "North":
					if (_enterPosition.z - offset > transform.position.z)
						_mapManager.ChangeScene(Vector2Int.up);
					break;
				case "South":
					if (_enterPosition.z + offset < transform.position.z)
						_mapManager.ChangeScene(Vector2Int.down);
					break;
				case "East":
					if (_enterPosition.x - offset > transform.position.x)
						_mapManager.ChangeScene(Vector2Int.right);
					break;
				case "West":
					if (_enterPosition.x + offset < transform.position.x)
						_mapManager.ChangeScene(Vector2Int.left);
					break;
				default:
					break;
			}
		}
	}

    // Method to reduce possible bugs when change between scenes
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
}
