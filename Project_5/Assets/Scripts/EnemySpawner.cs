using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour
{
    public int NumberOfEnemies;

    [SerializeField]
    private GameObject _enemyPrefab;
    

    /// <summary>
    /// when server starts create and spawn enemies in random spots
    /// </summary>
    public override void OnStartServer()
    {
        for (int i = 0; i < NumberOfEnemies; i++)
        {
            var spawnPosition = new Vector3(
                Random.Range(-8.0f, 8.0f),
                1.0f,
                Random.Range(-8.0f, 8.0f));

            var spawnRotation = Quaternion.Euler(
                0.0f,
                Random.Range(0, 180),
                0.0f);

            GameObject enemy = Instantiate(_enemyPrefab, spawnPosition, spawnRotation);
            NetworkServer.Spawn(enemy);
        }
    }
}