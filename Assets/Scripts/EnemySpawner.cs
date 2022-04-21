using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] [Range(0, 3)] private float spawnTime = 2f;
    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform[] _spawnPoints;
    private static EnemySpawner es;
    private float spawnTimeCounter = 0;
    private int _enemySpawnNumber;
    private int listSize;
    private bool _canSpawn = false;
    private List<Transform> canSpawnPoints;
    public static EnemySpawner Instance => es;
    private void Awake()
    {
        if (es == null)
        {
            es = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        spawnTimeCounter -= Time.deltaTime;

        if (_canSpawn)
        {
            if (_enemySpawnNumber > 0 && spawnTimeCounter < 0)
            {
                SpawnZombies();
            }

            else if (_enemySpawnNumber <= 0)
            {
                _canSpawn = false;
            }
        }
    }

    private void SpawnZombies()
    {
        _enemySpawnNumber--;
        spawnTimeCounter = Random.Range(0.1f, spawnTime);

        int randomPos = Random.Range(0, listSize);
        int randomEnemy = Random.Range(0, _enemyPrefabs.Length);

        Instantiate(_enemyPrefabs[randomEnemy], canSpawnPoints[randomPos].position, Quaternion.identity, transform);
    }
    public void SendSpawnCommand(int enemySpawnNumber)
    {
        canSpawnPoints = new List<Transform>();

        foreach (Transform point in _spawnPoints)
        {
            if (Vector2.Distance(point.position, _player.position) < 20)
            {
                canSpawnPoints.Add(point);
            }
        }

        listSize = canSpawnPoints.Count;
        _enemySpawnNumber += enemySpawnNumber;
        _canSpawn = true;
    }
}
