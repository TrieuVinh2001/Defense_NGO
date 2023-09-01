using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SoldierTower : NetworkBehaviour
{
    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform pointAttack;
    private float attackTime;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform dir;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsHost || !IsOwner) return;
        attackTime -= Time.deltaTime;
        if (attackTime < 0)
        {
            attackTime = attackCooldown;
            CheckEnemy();
        }
        
    }

    private void CheckEnemy()
    {
        Collider2D[] checkHit = Physics2D.OverlapCircleAll(pointAttack.position, attackRange, whatIsEnemy);

        if (checkHit.Length>0)
        {
            Transform posEnemy = checkHit[0].transform;
            SpawnProjectileServerRpc(posEnemy.position+ new Vector3(-1,0,0));
        }
    }

    [ServerRpc]
    private void SpawnProjectileServerRpc(Vector3 enemyDir)
    {
        dir.transform.right = enemyDir - dir.position;
        GameObject bullet = Instantiate(projectilePrefab, pointAttack.position, dir.transform.rotation);
        bullet.GetComponent<NetworkObject>().Spawn();
        bullet.GetComponent<ProjectileNPC>().dir = (enemyDir - transform.position).normalized;

    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(pointAttack.position, attackRange);
    //}

}
