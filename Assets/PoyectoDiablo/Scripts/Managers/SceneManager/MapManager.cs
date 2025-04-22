using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MapManager : MonoBehaviour
{
    public TextMeshProUGUI positionText;
    public GameObject enemyPrefab;
    public Transform enemyContainer;
    private bool _isLoading = false;

    void Start()
    {
        var pos = LevelGenerator.Instance.PlayerCurrentRoom;
        positionText.text = $"Sala actual: {pos.x}, {pos.y}";

        SpawnEnemiesForRoom(pos);
    }

    private void SpawnEnemiesForRoom(Vector2Int roomPos)
    {
        var room = LevelGenerator.Instance.GetOrCreateRoomData(roomPos);

        if (!room.initialized)
        {
            int enemyCount = Random.Range(3, 6);

            for (int i = 0; i < enemyCount; i++)
            {
                var enemyData = new EnemyData();
                enemyData.position = new Vector3Int(Random.Range(-5, 6), 0, Random.Range(-5, 6));
                room.enemies.Add(enemyData);
            }

            room.initialized = true;
        }

        foreach (var enemy in room.enemies)
        {
            if (!enemy.isAlive)
                continue;

            var enemyData = Instantiate(enemyPrefab, enemyContainer);
            enemyData.transform.position = new Vector3(enemy.position.x, enemy.position.y, enemy.position.z);

            TestEnemy controller = enemyData.GetComponent<TestEnemy>();
            controller.SetData(enemy);
        }
    }

    public void ChangeScene(Vector2Int direction)
    {
        if (_isLoading) return;

        if (direction != Vector2Int.zero)
        {
            var newPos = LevelGenerator.Instance.PlayerCurrentRoom + direction;
            if (LevelGenerator.Instance.IsValid(newPos))
            {
                _isLoading = true;
                positionText.text = "Cargando...";
                StartCoroutine(LoadSceneWithDelay(newPos, 1f));
            }
        }
    }

    IEnumerator LoadSceneWithDelay(Vector2Int newPos, float delay)
    {
        yield return new WaitForSeconds(delay);

        LevelGenerator.Instance.PlayerCurrentRoom = newPos;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
