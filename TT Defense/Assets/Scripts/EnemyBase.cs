using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class EnemyBase : NetworkBehaviour
{
    [SerializeField] protected float moveSpeed;
    public int maxHealth;
    protected int currentHealth;
    [SerializeField] protected int damage;
    [SerializeField] protected GameObject floatingText;

    [SerializeField] protected LayerMask whatIsPlayer;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected Transform pointAttack;
    [SerializeField] protected float attackRadius;
    protected float attackTime;
    protected bool modeAttack;

    [SerializeField] protected Image hpImage;
    [SerializeField] protected Image hpEffectImage;
    [SerializeField] protected float hurtSpeed = 0.001f;

    [HideInInspector] public bool lastHitIsNPC;

    protected Animator anim;
    protected Rigidbody2D rb;
    protected BoxCollider2D boxC;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxC = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        attackTime -= Time.deltaTime;
        //rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
        //transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        modeAttack = Physics2D.Raycast(transform.position, Vector2.left, attackRange, whatIsPlayer);
        //modeAttack = Physics2D.BoxCast(transform.position, boxC.bounds.size, 0f, Vector2.left, attackRange, whatIsPlayer);
        if (modeAttack && attackTime <= 0)
        {

            attackTime = attackCooldown;
            anim.SetBool("Attack", true);
            rb.velocity = Vector2.zero;
        }
        else if (!modeAttack)
        {
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);

        }
        anim.SetBool("Move", rb.velocity.x != 0);



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

    public void EndAttackEventTrigger()
    {
        anim.SetBool("Attack", false);
    }

    public void TakeDamage(int damage)
    {
        if (IsHost)//Nếu là host
        {
            //SpawnTextClientRpc(damage);
            TakeDamageClientRpc(damage);//Gửi dữ liệu đến các máy khách
        }
        else
        {
            TakeDamageServerRpc(damage);//Gửi dữ liệu đến máy chủ báo rằng đã nhận dame
            //SpawnTextServerRpc(damage);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TakeDamageServerRpc(int damage)
    {
        TakeDamageClientRpc(damage);
        //SpawnTextClientRpc(damage);
    }

    [ClientRpc]
    private void TakeDamageClientRpc(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            if (IsOwner && !lastHitIsNPC)
            {
                GameManager.instance.AddScore(1);
            }

            DeathServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeathServerRpc()
    {
        NetworkObject.Despawn();
    }
}
