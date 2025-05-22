using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] CinemachineCamera gameCamera;

    GameObject player;

    //private List<ScriptableObject> FindAll<T>(string path = "")
    //{
    //    var availableJobs = new List<ScriptableObject>();
    //    var searchFilter = $"t:{typeof(ScriptableObject).Name}";
    //    var soNames = path == "" ? AssetDatabase.FindAssets(searchFilter) : AssetDatabase.FindAssets(searchFilter, new[] { path });

    //    foreach (var soName in soNames)
    //    { 
    //        var soPath = AssetDatabase.GUIDToAssetPath(soName);
    //        var script = AssetDatabase.LoadAssetAtPath<ScriptableObject>(soPath);
    //        if (script == null)    
    //            continue;
    //        availableJobs.Add(script);
    //    }

    //    return availableJobs;
    //}

    //void Start()
    //{
    //    // Ñapa: This is a workaround to get the selected character from the PlayerSelectManager.
    //    // In a real scenario, you would use a more robust method to pass data between scenes.
    //    string selectedPlayerJob = FindAnyObjectByType<PlayerSelectManager>().selectedCharacter.ToString();

    //    InstantiatePlayer(selectedPlayerJob);
    //    InstantiateEnemies();

    //    gameCamera.Follow = player.transform;

    //    Debug.Log(selectedPlayerJob);
    //}

    //void InstantiatePlayer(string playerJob) {
    //    List<ScriptableObject> availablePlayerJobs = FindAll<ScriptableObject>("Assets/Gameplay/Actor/ScriptableObjects/Player/");

    //    GameObject playerSpawnPoint = GameObject.FindGameObjectWithTag("Respawn");
    //    ActorData actorData = availablePlayerJobs.Find(x => x.name == playerJob) as ActorData;
    //    //playerPrefab.GetComponent<PlayerController>().ActorData = actorData;

    //    player = Instantiate(playerPrefab, playerSpawnPoint.transform.position, Quaternion.identity, playerSpawnPoint.transform);
    //    player.SetActive(true);
    //}

    //void InstantiateEnemies() {
    //    List<ScriptableObject> availableEnemyJobs = FindAll<ScriptableObject>("Assets/Gameplay/Actor/ScriptableObjects/Enemy/");
    //    GameObject[] enemySpawnPoints = GameObject.FindGameObjectsWithTag("EnemySpawnPoint");

    //    foreach (GameObject spawnPoint in enemySpawnPoints) {
    //        ActorData actorData = availableEnemyJobs[Random.Range(0, availableEnemyJobs.Count)] as ActorData;
    //        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity, spawnPoint.transform);
    //        enemy.GetComponent<EnemyController>().ActorData = actorData;
    //        enemy.SetActive(true);
    //    }
    //}
}
