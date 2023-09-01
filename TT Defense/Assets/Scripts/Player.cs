using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    [SerializeField] private int maxHealth;
    private int currentHealth;
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform pointShoot;

    private float xInput;
    private float yInput;
    private int facingDir = 1;
    private bool facingRight = true;

    private bool isAttack;

    private Rigidbody2D rb;
    private Animator anim;
    private Canvas canvas;

    [SerializeField] private Image hpImage;
    [SerializeField] private Image hpEffectImage;
    [SerializeField] private float hurtSpeed = 0.001f;

    private bool canPick = true;

    private int indexBullet;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        canvas = GetComponentInChildren<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if ((xInput != 0 || yInput != 0) && !isAttack)
        {
            Movement();
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isAttack = true;   
        }

        anim.SetBool("Move", xInput != 0 || yInput != 0);
        anim.SetBool("Attack", isAttack);

        HpImageFillServerRpc();
    }

    [ServerRpc]
    private void HpImageFillServerRpc()
    {
        HpImageFillClientRpc();
    }

    [ClientRpc]
    private void HpImageFillClientRpc()
    {
        hpImage.fillAmount = (float)currentHealth / (float)maxHealth;
        if (hpEffectImage.fillAmount > hpImage.fillAmount)
        {
            hpEffectImage.fillAmount -= hurtSpeed;
        }
        else
        {
            hpEffectImage.fillAmount = hpImage.fillAmount;
        }

    }

    public void AttackEvent()
    {
        if (!IsOwner) return;
        ShootServerRpc();
    }

    private void Shoot()
    {
        if(indexBullet == 3)
        {
            GameObject bullet1 = Instantiate(projectile, pointShoot.position + new Vector3(0, 0.25f ,0), Quaternion.identity);
            GameObject bullet2 = Instantiate(projectile, pointShoot.position - new Vector3(0, 0.25f, 0), Quaternion.identity);
            bullet1.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId, true);
            bullet1.GetComponent<Projectile>().facingDir = facingDir;
            //bullet1.GetComponent<Projectile>().indexBullet = indexBullet;

            bullet2.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId, true);
            bullet2.GetComponent<Projectile>().facingDir = facingDir;
            //bullet2.GetComponent<Projectile>().indexBullet = indexBullet;

            IndexProjectileClientRpc(bullet1);
            IndexProjectileClientRpc(bullet2);

        }
        else
        {
            GameObject bullet = Instantiate(projectile, pointShoot.position, Quaternion.identity);
            bullet.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId, true);
            bullet.GetComponent<Projectile>().facingDir = facingDir;
            //bullet.GetComponent<Projectile>().indexBullet = indexBullet;

            IndexProjectileClientRpc(bullet);

            Debug.Log(bullet.GetComponent<Projectile>().indexBullet);
        }
        EndAttackClientRpc();
    }

    [ClientRpc]
    private void IndexProjectileClientRpc(NetworkObjectReference target)
    {
        if(target.TryGet(out NetworkObject targetObject))
        {
            targetObject.GetComponent<Projectile>().indexBullet = indexBullet;
        }
    }

    [ClientRpc]
    private void EndAttackClientRpc()
    {
        isAttack = false;
    }

    [ServerRpc]
    private void ShootServerRpc()
    {
        Shoot();
    }

    private void Movement()
    {
        rb.velocity = new Vector2(xInput * moveSpeed, yInput * moveSpeed);

        if (rb.velocity.x < 0 && facingRight)
        {
            Flip();

            HealthbarLocalServerRpc();
        }
        else if (rb.velocity.x > 0 && !facingRight)
        {
            Flip();

            HealthbarLocalServerRpc();
        }
    }

    private void Flip()
    {
        //facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
        //transform.localScale = new Vector3(1 * facingDir, 1, 1);
        
        FacingDirServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void FacingDirServerRpc()
    {
        facingDir = facingDir * -1;
    }

    [ServerRpc]
    private void HealthbarLocalServerRpc()
    {
        HealthbarLocalClientRpc();
    }

    [ClientRpc]
    private void HealthbarLocalClientRpc()
    {
        if (!IsHost)
        {
            facingDir = facingDir * -1;
        }
        canvas.transform.localScale = new Vector3(-1 * facingDir, 1, 1);
    }


    public void TakeDamage(int damage)
    {
        TakeDamageClientRpc(damage);//Gửi đến tất cả máy khách khác báo mình bị nhận dame (host cũng là 1 máy khách nên cũng sẽ nhận)

        if (currentHealth <= 0)
        {
            DeathServerRpc();//Gửi đến server yêu cầu xóa
        }
    }

    [ClientRpc]
    private void TakeDamageClientRpc(int damage)
    {
        currentHealth -= damage;
    }


    [ServerRpc(RequireOwnership = false)]
    private void DeathServerRpc()
    {
        NetworkObject.Despawn();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item")&& canPick)
        {
            if (collision.GetComponent<ItemBuff>().buff == Buff.bounceBullet)
            {
                indexBullet = 1;
            }
            else if (collision.GetComponent<ItemBuff>().buff == Buff.piercingBullet)
            {
                indexBullet = 2;
            }
            else if (collision.GetComponent<ItemBuff>().buff == Buff.doubleBullet)
            {
                indexBullet = 3;
            }

            canPick = false;
            IndexBulletServerRpc(indexBullet);
            DeSpawnServerRpc(collision.gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void IndexBulletServerRpc(int index)
    {
        IndexBulletClientRpc(index);
    }

    [ClientRpc]
    private void IndexBulletClientRpc(int index)
    {
        indexBullet = index;
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeSpawnServerRpc(NetworkObjectReference target)
    {
        if (target.TryGet(out NetworkObject targetObject))
        {
            targetObject.Despawn();
        }
    }
}
