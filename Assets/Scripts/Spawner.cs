using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField] GameObject enemy;
    [SerializeField] int enemiesToSpawn;
    [SerializeField] int timer;
    [SerializeField] Transform SpawnPos;

    bool isSpawning;
    bool startSpawning;
    int enemiesSpawned;

    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.updateUI();
    }
    private void Update()
    {
        if(startSpawning && !isSpawning && enemiesSpawned < enemiesToSpawn)
        {
            StartCoroutine(spawn());
        }
    }


    IEnumerator spawn()
    {
        isSpawning = true;

        Instantiate(enemy, SpawnPos.position, enemy.transform.rotation);
        enemiesSpawned++;

        yield return new WaitForSeconds(timer);
        isSpawning = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }
}
