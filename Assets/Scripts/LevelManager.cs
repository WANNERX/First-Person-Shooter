using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRange = 10f;

    public int currentLevel = 1;
    public int enemiesToSpawn = 1;

    List<Actor> enemies = new List<Actor>();

    void Start()
    {
        SpawnEnemies();
        UpdateLevelUI();
    }

    void SpawnEnemies()
    {
        for(int i = 0; i < enemiesToSpawn; i++)
        {
            Vector3 position = Vector3.zero;
            bool validPosition = false;
            int attempts = 0;

            while(!validPosition && attempts < 100)
            {
                float x = Random.Range(-spawnRange, spawnRange);
                float z = Random.Range(-spawnRange, spawnRange);
                position = new Vector3(x, 0, z);

                validPosition = true;
                foreach(Actor enemy in enemies)
                {
                    if(Vector3.Distance(position, enemy.transform.position) < 2f)
                    {
                        validPosition = false;
                        break;
                    }
                }

                attempts++;
            }

            Actor newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity).GetComponent<Actor>();
            enemies.Add(newEnemy);
            newEnemy.OnDeath += EnemyDied;
        }
    }

    void EnemyDied(Actor enemy)
    {
        enemy.OnDeath -= EnemyDied;
        enemies.Remove(enemy);

        if(enemies.Count == 0)
        {
            currentLevel++;
            enemiesToSpawn += 2;
            SpawnEnemies();
            UpdateLevelUI();
        }
    }

    void UpdateLevelUI()
    {
        if(UserInterface.Singleton != null)
        { UserInterface.Singleton.UpdateLevelCounter(currentLevel); }
    }
}