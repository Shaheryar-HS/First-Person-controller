using System.Collections;
using UnityEngine;

public class EnemyWaveManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    public int enemiesPerWave = 3;
    public float spawnDelay = 1f;

    private int currentWave = 1;

    void OnEnable()
    {
        EnemyEvents.OnAllEnemiesDefeated += HandleWaveComplete;
    }

    void OnDisable()
    {
        EnemyEvents.OnAllEnemiesDefeated -= HandleWaveComplete;
    }

    void Start()
    {
        StartCoroutine(SpawnWave());
    }

    void HandleWaveComplete()
    {
        currentWave++;
        enemiesPerWave += 2;
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        yield return new WaitForSeconds(2f); // Delay before wave

        for (int i = 0; i < enemiesPerWave; i++)
        {
            GameObject enemy = EnemyPooler.Instance.GetEnemy();

            if (enemy != null)
            {
                Vector3 spawnPos = new Vector3(Random.Range(-50, 45), 1.974f, Random.Range(-51, 40));
                enemy.transform.position = spawnPos;
                enemy.SetActive(true);
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
