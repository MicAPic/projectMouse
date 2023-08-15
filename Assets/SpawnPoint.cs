using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public List<BoxCollider> boxCollidersMap;

    public void SpawnEnemy(Enemy.EnemyController enemyForSpawn)
    {
        Instantiate(enemyForSpawn, transform.position, Quaternion.identity);
    }



}
