using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemyManager : MonoBehaviour
{
    public Enemy_FSM[] spawnEnemies;
    
    public void SetData()
    {
        foreach (var enemy in spawnEnemies)
        {
            enemy.SetData();
        }

        StartCoroutine(SetData(2f));
    }

    IEnumerator SetData(float activeCount)
    {
        yield return new WaitForSeconds(activeCount);
        foreach (var enemy in spawnEnemies)
        {
            enemy.enabled = true;
        }
    }
}
