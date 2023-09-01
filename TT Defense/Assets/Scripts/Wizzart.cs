using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Wizzart : EnemyBase
{
    [SerializeField] private GameObject bulletPrefab;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public void AttackEventTrigger()
    {
        if (!IsHost) return;
        GameObject bullet = Instantiate(bulletPrefab, pointAttack.position, Quaternion.identity);
        bullet.GetComponent<NetworkObject>().Spawn();
        bullet.GetComponent<BulletEnemy>().damage = damage;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x - attackRange, transform.position.y));
        Gizmos.DrawWireSphere(pointAttack.position, attackRadius);
    }

}
