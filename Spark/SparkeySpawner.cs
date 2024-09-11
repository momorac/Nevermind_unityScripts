using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SparkeySpawner : MonoBehaviour
{
    public float spawnDelay_min = 5f;
    public float spawnDelay_max = 10f;
    [SerializeField]
    private Vector3[] randomSpawnLocations = new Vector3[7];
    public GameObject Spark;

    private GameManager gameManager;

    void Awake()
    {
        gameManager = gameObject.GetComponent<GameManager>();
    }

    void Start()
    {
        StartCoroutine(SpawnSpark());
    }


    private IEnumerator SpawnSpark()
    {
        while (true)
        {
            float waitTime = Random.Range(spawnDelay_min, spawnDelay_max);
            yield return new WaitForSeconds(waitTime);

            Spawn();
        }
    }
    void Spawn()
    {
        GameObject newSpark = Instantiate(Spark, randomSpawnLocations[Random.Range(0, randomSpawnLocations.Length)], Quaternion.identity);
        newSpark.SetActive(true);
        gameManager.SparkCount++;
        //Debug.Log("Spark Spawned!!");
    }

}
