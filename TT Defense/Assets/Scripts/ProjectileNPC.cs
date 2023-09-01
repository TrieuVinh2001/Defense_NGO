using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ProjectileNPC : NetworkBehaviour
{
    [SerializeField] private float moveSpeed;
    [HideInInspector] public float facingDir = 1;
    [HideInInspector] public Vector3 dir = new Vector3(1, 0, 0);
    public bool isNpc;


    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(facingDir, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += dir * facingDir * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Enemy"))
        {
            if (!IsOwner) return;
            if(collision.gameObject.GetComponent<NetworkObject>().IsSpawned)
                ProjectileServerRpc(collision.gameObject);
            collision.GetComponent<EnemyBase>().TakeDamage(2);
        }
    }

    [ServerRpc]
    private void ProjectileServerRpc(NetworkObjectReference target)
    {
        if (target.TryGet(out NetworkObject targetObject))
        {
            targetObject.GetComponent<NetworkObject>().ChangeOwnership(OwnerClientId);
            targetObject.GetComponent<EnemyBase>().lastHitIsNPC = isNpc;
            //Debug.Log(targetObject.GetComponent<DarkKnight>().lastHitIsNPC);
            NetworkObject.Despawn();
        }
    }
}
