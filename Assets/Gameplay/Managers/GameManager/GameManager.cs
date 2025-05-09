using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    List<ScriptableObject> availablePlayerJobs = new List<ScriptableObject>();
    List<ScriptableObject> availableEnemyJobs = new List<ScriptableObject>();
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject enemyPrefab;
    string playerJob;

    private List<ScriptableObject> FindAll<T>(string path = "")
    {
        var availableJobs = new List<ScriptableObject>();
        var searchFilter = $"t:{typeof(ScriptableObject).Name}";
        var soNames = path == "" ? AssetDatabase.FindAssets(searchFilter) : AssetDatabase.FindAssets(searchFilter, new[] { path });

        foreach (var soName in soNames)
        { 
            var soPath = AssetDatabase.GUIDToAssetPath(soName);
            var script = AssetDatabase.LoadAssetAtPath<ScriptableObject>(soPath);
            if (script == null)    
                continue;
            availableJobs.Add(script);
        }

        return availableJobs;
    }

    void Start()
    {
        // Ñapa: This is a workaround to get the selected character from the PlayerSelectManager.
        // In a real scenario, you would use a more robust method to pass data between scenes.
        playerJob = FindAnyObjectByType<PlayerSelectManager>().SelectedCharacter;
        InstantiatePlayer();
        InstantiateEnemies();
    }

    void InstantiatePlayer() {
        GameObject playerSpawnPoint = GameObject.FindGameObjectWithTag("Respawn");
        availablePlayerJobs = FindAll<ScriptableObject>("Assets/Gameplay/Actor/ScriptableObjects/Player/");
        ActorDataSO actorData = availablePlayerJobs.Find(x => x.name == playerJob) as ActorDataSO;
        playerPrefab.GetComponent<PlayerController>().ActorData = actorData;
        GameObject player = Instantiate(playerPrefab, playerSpawnPoint.transform.position, Quaternion.identity, playerSpawnPoint.transform);
        player.SetActive(true);
    }

    void InstantiateEnemies() {
        availableEnemyJobs = FindAll<ScriptableObject>("Assets/Gameplay/Actor/ScriptableObjects/Enemy/");
        GameObject[] enemySpawnPoints = GameObject.FindGameObjectsWithTag("EnemySpawnPoint");
        foreach (GameObject spawnPoint in enemySpawnPoints) {
            ActorDataSO actorData = availableEnemyJobs[Random.Range(0, availableEnemyJobs.Count)] as ActorDataSO;
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity, spawnPoint.transform);
            enemy.GetComponent<EnemyController>().ActorData = actorData;
            enemy.SetActive(true);
        }
    }
}
