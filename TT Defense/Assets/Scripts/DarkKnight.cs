using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.UI;

public class DarkKnight : EnemyBase
{

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
        Collider2D[] checkHit = Physics2D.OverlapCircleAll(pointAttack.position, attackRadius, whatIsPlayer);

        if (checkHit != null)
        {
            foreach (var hit in checkHit)
            {
                if (hit.GetComponent<Player>())
                {
                    hit.GetComponent<Player>().TakeDamage(damage);
                }
                if (hit.GetComponent<Tower>())
                {
                    hit.GetComponent<Tower>().TakeDamage(damage);
                }
            }
        }
    }

    

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x - attackRange, transform.position.y));
        Gizmos.DrawWireSphere(pointAttack.position, attackRadius);

    }

    //[ServerRpc(RequireOwnership = false)]
    //private void SpawnTextServerRpc(int damage)
    //{
    //    GameObject text = Instantiate(floatingText, transform.position, Quaternion.identity);
    //    text.GetComponentInChildren<TextMeshPro>().text = "-" + damage;
    //    text.GetComponent<NetworkObject>().Spawn();
    //}

    //[ClientRpc]
    //private void SpawnTextClientRpc(int damage)
    //{
    //    SpawnTextServerRpc(damage);
    //}

}
