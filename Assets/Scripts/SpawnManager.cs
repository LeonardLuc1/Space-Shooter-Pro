﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
        
{
    [SerializeField]
    private GameObject _enemyprefab;
    [SerializeField]
    private GameObject _EnemyContainer;
        
    private bool _stopSpawning = false;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    // Update is called once per frame
    void Update()
    {       
        
    }

    IEnumerator SpawnRoutine() 
    {
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f),7, 0);
            GameObject newEnemy = Instantiate(_enemyprefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _EnemyContainer.transform;            
            yield return new WaitForSeconds(5.0f);
        }     
 
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

}
