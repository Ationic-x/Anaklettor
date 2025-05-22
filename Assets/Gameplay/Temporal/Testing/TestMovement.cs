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
		WarpData warp = other.GetComponentInChildren<WarpData>();
		if (warp == null) return;

		other.gameObject.GetComponentInChildren<Animator>().SetTrigger("In");
		_enterPosition = transform.position;
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
