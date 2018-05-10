using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour
{
    public int NumberOfEnemies;

    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    private Transform[] _spawnPoints;

    /// <summary>
    /// when server starts create and spawn enemies in random spots
    /// </summary>
    public override void OnStartServer()
    {
        for (int i = 0; i < NumberOfEnemies; i++)
        {
            int randomIndex = Random.Range(0, _spawnPoints.Length);
            GameObject enemy = Instantiate(_enemyPrefab, _spawnPoints[randomIndex].position, _spawnPoints[randomIndex].rotation);
            NetworkServer.Spawn(enemy);
        }
    }
}