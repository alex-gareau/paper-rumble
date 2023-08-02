using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private List<Enemy> enemyPrefab = new List<Enemy>();

    [SerializeField]
    private List<EnemySpawnPosition> spawnPosition = new List<EnemySpawnPosition>();

    public int waveEnemies = 10;

    private int remainingEnemies = 0;


    public float enemySpawnCounter = 1f;
    private float currentEnemyCounter;


    

    // Start is called before the first frame update
    void Start()
    {
        remainingEnemies = waveEnemies;
        currentEnemyCounter = enemySpawnCounter;

    }

    // Update is called once per frame
    void Update()
    {
        if(remainingEnemies > 0)
        {
            currentEnemyCounter -= Time.deltaTime;
            if (currentEnemyCounter < 0)
            {
                SpawnEnemy();

            }
        }
    }

    private void SpawnEnemy()
    {
        Enemy newEnemy = Instantiate(enemyPrefab[0], spawnPosition[0].transform.position, spawnPosition[0].transform.rotation);

        newEnemy.SetupEnemy(3, spawnPosition[0].EntrancePosition, 2f);

        remainingEnemies--;

        currentEnemyCounter = enemySpawnCounter;

    }
}
