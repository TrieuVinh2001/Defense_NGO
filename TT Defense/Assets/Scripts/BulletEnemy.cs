using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BulletEnemy : NetworkBehaviour
{
    [SerializeField] private float moveSpeed;

    [HideInInspector] public int damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!IsHost) return;

            if (collision.GetComponent<Player>())
            {
                collision.GetComponent<Player>().TakeDamage(damage);
            }     
            if (collision.GetComponent<Tower>())
            {
                collision.GetComponent<Tower>().TakeDamage(damage);
            }

            DeSpawnServerRpc();
        }

    }

    [ServerRpc]
    private void DeSpawnServerRpc()
    {
        NetworkObject.Despawn();
    }
}
