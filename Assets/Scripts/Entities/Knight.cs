using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEditor.UI;
using UnityEngine;

public class Knight : Entity
{   
    public float damage;
    public float bulletSpeed;
    public float bulletLifetime;
    public Entity target;
    public HomingProjectile projectile;

    //attacking related variables
    public float firstAttackCooldown;
    public float maxAttackCooldown;
    public float attackCooldown;
    public float attackRange;

    //animation
    public Animator anim;

    override public void Start()
    {
        base.Start();
        
        rbody = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
        InitializeStats();
    }

    override public void Update()
    {
        base.Update();

        if (target != null)
        {   
            //turns towards the target
            transform.right = target.transform.position - transform.position;

            if (Vector2.Distance(transform.position, target.transform.position) - target.transform.localScale.x/2 > attackRange)
            {
                anim.SetInteger("State", 1);
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, target.transform.localPosition, Time.deltaTime * movespeed);
                target = null;
            }
            else
            {
                anim.SetInteger("State", 2);
                if (attackCooldown <= 0)
                {
                    Attack(target);
                    attackCooldown = maxAttackCooldown;
                    anim.SetInteger("State", 0);
                }
                attackCooldown -= Time.deltaTime;
            }
        }
        else
        {
            anim.SetInteger("State", 0);

            attackCooldown = firstAttackCooldown;
            
            target = Utilities.FindTargetSingle(
                transform.position, 
                visionRange, 
                sortFunc, 
                new int[] {GetInstanceID()}, 
                null,
                new Utilities.Team[] {team},
                new Utilities.Altitude[] {Utilities.Altitude.Buried, Utilities.Altitude.Flying});


            if (target != null)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, target.transform.localPosition, Time.deltaTime * movespeed);
                anim.SetInteger("State", 1);
            }
        }
    }

    //This function is used to give a "score" to another entity. It is used by findTargetSingle to decide targets.
    private float sortFunc(Entity entity)
    {
        return Vector2.Distance(transform.position, entity.transform.position);
    } 

    public void InitializeStats()
    {
        size = 0.5f;
        target = null;
        base.InitializeStats(
            "Knight", 
            2.0f, 
            1750.0f, 
            1.0f, 
            Utilities.EntityType.Troop, 
            Utilities.Altitude.Grounded);

        firstAttackCooldown = 0.65f;
        attackCooldown = firstAttackCooldown;
        maxAttackCooldown = 1.1f;
        attackRange = 0.5f;
        visionRange = 9.0f;

        bulletLifetime = 1.0f;
        bulletSpeed = 35.0f;
        damage = 200.0f;
    }
    override public void Attack(Entity target)
    {
        HomingProjectile bullet = Instantiate(projectile, transform.position, transform.rotation);
        bullet.owner = gameObject.GetComponent<Entity>();
        bullet.target = target;
        bullet.maxlifetime = bulletLifetime;
        bullet.speed = bulletSpeed;
        bullet.damage = damage;
    }

    override public void TakeDamage(float amount, GameObject attacker, Utilities.DamageType damageType)
    {
        base.TakeDamage(amount, attacker, damageType);
    }

}
