using System.Collections.Generic;
using UnityEngine;

public class EnemyPooler : MonoBehaviour
{

    public static EnemyPooler Instance;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int initialPoolSize = 20;

    private List<GameObject> enemyPool = new List<GameObject>();
    private List<GameObject> activeEnemies = new List<GameObject>();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.transform.SetParent(this.transform); // Optional: Set parent for organization
            enemy.SetActive(false);
            enemy.GetComponent<EnemyHealth>().OnEnemyDeath += HandleEnemyDeath;
            enemyPool.Add(enemy);
        }
    }


    public GameObject GetEnemy()
    {
        foreach (var enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy)
            {
                activeEnemies.Add(enemy);
                return enemy;
            }
        }
        return null; // No available enemy in the pool

        // // Optional: Expand pool
        // GameObject newEnemy = Instantiate(enemyPrefab);
        // newEnemy.SetActive(false);
        // // newEnemy.transform.SetParent(this.transform); // Optional: Set parent for organization
        // enemyPool.Add(newEnemy);
        // return newEnemy;
    }
    private void HandleEnemyDeath(GameObject enemy)
    {
        activeEnemies.Remove(enemy);

        if (activeEnemies.Count == 0)
        {
            EnemyEvents.OnAllEnemiesDefeated?.Invoke();
        }
    }

    public List<GameObject> GetActiveEnemies() => activeEnemies;

    // public List<GameObject> GetActiveEnemies()
    // {
    //     return enemyPool.FindAll(e => e.activeInHierarchy);
    // }
}


