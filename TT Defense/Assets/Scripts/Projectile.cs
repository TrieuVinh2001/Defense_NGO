using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private float moveSpeed;
    public float facingDir=1;
    [HideInInspector] public Vector3 dir = new Vector3(1,0,0);
    public bool isNpc;
    public bool isBounce;

    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] private float checkRange;

    private float distance=10;
    private Transform enemyTarget;

    public int indexBullet;

    [SerializeField] private int damage;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Ind  "+indexBullet);
        transform.localScale = new Vector3(facingDir, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (isBounce && enemyTarget != null)
        {
            transform.right = enemyTarget.transform.position - transform.position;
            transform.position = Vector3.MoveTowards(transform.position, enemyTarget.position, moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.position += dir * facingDir * moveSpeed * Time.deltaTime;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsOwner) return;
        if (collision.GetComponent<EnemyBase>())
        {
            switch (indexBullet)
            {
                case 1:
                    BulletBounce(collision);
                    break;
                case 2:
                    BulletPiercing(collision);
                    break;
                default:
                    Bullet(collision);
                    break;
            }
        }
    }

    private void Bullet(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<NetworkObject>().IsSpawned)
            ProjectileServerRpc(collision.gameObject);
        collision.GetComponent<EnemyBase>().TakeDamage(damage);
        DeSpawnServerRpc();
    }

    private void BulletBounce(Collider2D collision)
    {
        if (isBounce)
        {
            if (collision.gameObject.GetComponent<NetworkObject>().IsSpawned)
                ProjectileServerRpc(collision.gameObject);
            collision.GetComponent<EnemyBase>().TakeDamage(damage);
            DeSpawnServerRpc();
        }

        if (collision.CompareTag("Enemy") && !isBounce)
        {
            if (collision.gameObject.GetComponent<NetworkObject>().IsSpawned)
                ProjectileServerRpc(collision.gameObject);
            BulletBounceServerRpc(collision.gameObject);
            collision.GetComponent<EnemyBase>().TakeDamage(damage);

        }
    }

    private void BulletPiercing(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<NetworkObject>().IsSpawned)
            ProjectileServerRpc(collision.gameObject);
        collision.GetComponent<EnemyBase>().TakeDamage(damage);
    }

    [ServerRpc]
    private void BulletBounceServerRpc(NetworkObjectReference target)
    {
        if (target.TryGet(out NetworkObject targetObject))
        {
            Collider2D[] checkHit = Physics2D.OverlapCircleAll(transform.position, checkRange, whatIsEnemy);

            if (checkHit != null)
            {
                foreach (var hit in checkHit)
                {
                    if (hit.GetComponent<EnemyBase>() && hit.GetComponent<EnemyBase>() != targetObject.GetComponent<EnemyBase>())
                    {
                        float dis = Vector2.Distance(transform.position, hit.GetComponent<EnemyBase>().transform.position);
                        if (distance > dis)
                        {
                            distance = dis;
                            enemyTarget = hit.GetComponent<EnemyBase>().transform;
                        }
                    }
                }
                if(enemyTarget == null)
                {
                    DeSpawnServerRpc();
                }
                else
                {
                    IsBounceClientRpc();
                }
            }
        }
    }


    [ClientRpc]
    private void IsBounceClientRpc()
    {
        isBounce = true;
    }

    [ServerRpc]
    private void ProjectileServerRpc(NetworkObjectReference target)
    {
        if (target.TryGet(out NetworkObject targetObject))
        {
            targetObject.GetComponent<NetworkObject>().ChangeOwnership(OwnerClientId);
            if(targetObject.GetComponent<EnemyBase>())
                targetObject.GetComponent<EnemyBase>().lastHitIsNPC = isNpc;
            //Debug.Log(targetObject.GetComponent<DarkKnight>().lastHitIsNPC);
            //NetworkObject.Despawn();
            
        }
    }

    [ServerRpc]
    private void DeSpawnServerRpc()
    {
        NetworkObject.Despawn();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, checkRange);
    }
}
