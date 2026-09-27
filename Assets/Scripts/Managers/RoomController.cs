using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoomController : MonoBehaviour
{
    public struct SpawnData
    {
        public GameObject prefab;
        public Vector3 spawnPoint;
    }
    [System.Serializable]
    public struct EnemyData
    {
        public GameObject enemy;
        public GameObject prefab;
    }
    public List<EnemyData> enemies = new List<EnemyData>();
    private List<SpawnData> enemySaver= new List<SpawnData>();

    void OnEnable()
    {
        SaveEnemies();
    }
    void Start()
    {
        SaveEnemies();
    }

    private void SaveEnemies()
    {
        foreach (var enem in enemies)
        {
            SpawnData enemy = new SpawnData();
            enemy.prefab = enem.prefab;
            enemy.spawnPoint = enem.enemy.transform.position;
            enemySaver.Add(enemy);
        }
    }

    private void DestroyEnemies()
    {
        foreach (var enem in enemies)
        {
            Destroy(enem.enemy);
        }
    }

    public void RestartLevel()
    {
        DestroyEnemies();
        foreach (var enemy in enemySaver)
        {
            EnemyData enemyData = new EnemyData();
            enemyData.enemy = Instantiate(enemy.prefab, enemy.spawnPoint, Quaternion.identity);
            enemies.Add(enemyData);
        }
    }
}
