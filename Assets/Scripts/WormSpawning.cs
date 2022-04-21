using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormSpawning : MonoBehaviour
{
    [SerializeField] private Transform wormSpawnPoint;
    [SerializeField] private int numOfSpawningWorms;
    [SerializeField] private GameObject wormPrefab;
    private void SpawnWorms()
    {
        Transform enemyParent = GameObject.FindGameObjectWithTag("Enemies").transform;

        for (int i = 0; i < numOfSpawningWorms; i++)
        {
            Vector2 pos = wormSpawnPoint.position;
            Vector2 randomPos = pos + Random.insideUnitCircle;

            Instantiate(wormPrefab, randomPos, Quaternion.identity, enemyParent);
        }
    }
}
