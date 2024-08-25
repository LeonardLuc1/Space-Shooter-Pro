using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour

{
    [SerializeField]
    private GameObject _enemyPrefab;
    
    [SerializeField]
    private GameObject _enemyContainer;
    
    [SerializeField]
    private GameObject[] powerUps;

    [SerializeField]
    private GameObject _powerUpsRarePrefab;
            
    private bool _stopSpawning = false;
          
    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(RarePowerUpRoutine());
    }

    IEnumerator SpawnEnemyRoutine() 
    {
        yield return new WaitForSeconds(3.0f);
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f),7, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5.0f);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f),7, 0);
            int randomPowerup = Random.Range(0, 5);
            Instantiate(powerUps[randomPowerup], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(3, 8));
        }
    }

    IEnumerator RarePowerUpRoutine()
    {
        yield return new WaitForSeconds(30.0f);
        while (_stopSpawning == false)
        {
            Vector3 randomLocation = new Vector3(Random.Range(-8f, 8f), 8f, 0);
            GameObject rareNewPowerUp = Instantiate(_powerUpsRarePrefab, randomLocation, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(20f, 30f));
        }
    }

    
    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
