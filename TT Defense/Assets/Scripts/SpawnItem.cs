using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnItem : NetworkBehaviour
{
    [SerializeField] private GameObject[] items;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Item());
    }

    private IEnumerator Item()
    {
        yield return new WaitForSeconds(30f);
        GameObject item = Instantiate(items[Random.Range(0, items.Length)], new Vector3(4f,Random.Range(-3,1.5f),0),Quaternion.identity);
        item.GetComponent<NetworkObject>().Spawn();
    }
}
