using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DestroyProjectile : NetworkBehaviour
{
    [SerializeField] private float time;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyGameObject());
    }

    private IEnumerator DestroyGameObject()
    {
        yield return new WaitForSeconds(time);
        DespawnServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnServerRpc()
    {
        NetworkObject.Despawn();
    }
}
