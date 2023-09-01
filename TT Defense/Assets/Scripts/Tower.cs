using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Tower : NetworkBehaviour
{
    [SerializeField] private int health;

    public void TakeDamage(int damage)
    {
        if (!IsHost) return;
        TakeDamageClientRpc(damage); 
    }

    [ServerRpc]
    private void TakeDamageServerRpc(int damage)
    {
        TakeDamageClientRpc(damage);
    }

    [ClientRpc]
    private void TakeDamageClientRpc(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            //NetworkObject.Despawn();
            DeSpawnServerRpc();
            GameManager.instance.GameLoss();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeSpawnServerRpc()
    {
        NetworkObject.Despawn();
        
    }
}
