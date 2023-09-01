using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnEnemy : NetworkBehaviour
{
    [SerializeField] private GameObject darkKnight;
    [SerializeField] private GameObject wizzart;
    [SerializeField] private GameObject stoneGolem;
    [SerializeField] private Vector2 pointSpawn;
    [SerializeField] private Vector2 timeCooldown;

    private bool finish;

    // Start is called before the first frame update
    void Start()
    {
        //InvokeRepeating("SpawnEnemies", 2, timeCooldown);
        //Invoke("SpawnEnemies", 2);
        //StartCoroutine(SpawnEnemies2());
        StartCoroutine(SpawnWizzart());
        StartCoroutine(SpawnBoss());
    }

    private void SpawnEnemies()
    {
        GameObject enemy = Instantiate(darkKnight, transform.position + new Vector3(0, Random.Range(pointSpawn.x, pointSpawn.y)), Quaternion.identity);
        enemy.GetComponent<NetworkObject>().Spawn();
    }

    IEnumerator SpawnEnemies2()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(timeCooldown.x, timeCooldown.y));
            GameObject enemy = Instantiate(darkKnight, transform.position + new Vector3(0, Random.Range(pointSpawn.x, pointSpawn.y)), Quaternion.identity);
            enemy.GetComponent<NetworkObject>().Spawn();
        }
    }

    IEnumerator SpawnWizzart()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(timeCooldown.x+2, timeCooldown.y+4));
            GameObject enemy = Instantiate(wizzart, transform.position + new Vector3(0, Random.Range(pointSpawn.x, pointSpawn.y)), Quaternion.identity);
            enemy.GetComponent<NetworkObject>().Spawn();
        }
    }

    IEnumerator SpawnBoss()
    {
        yield return new WaitForSeconds(GameManager.instance.timeBoss);
        GameObject enemy = Instantiate(stoneGolem, transform.position + new Vector3(0, Random.Range(pointSpawn.x+1f, pointSpawn.y-1f)), Quaternion.identity);
        enemy.GetComponent<NetworkObject>().Spawn();
        finish = true;
        StopAllCoroutines();
    }
}
