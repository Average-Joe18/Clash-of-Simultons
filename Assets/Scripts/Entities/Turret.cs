using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEditor.UI;
using UnityEngine;

public class Turret : Entity
{
    private enum State
    {
        Attacking,
        Tracking,
        Idling
    }
    public float damage;
    public float bulletSpeed;
    public float bulletLifetime;
    public Entity target;
    public HomingProjectile projectile;

    State state;

    //attacking related variables
    public float firstAttackCooldown;
    public float maxAttackCooldown;
    public float attackCooldown;
    public float attackRange;

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
            if (Vector2.Distance(transform.position, target.transform.position) - target.transform.localScale.x/2 > attackRange)
            {
                target = null;
            }
            else
            {
                if (attackCooldown <= 0)
                {
                    Attack(target);
                    attackCooldown = maxAttackCooldown;
                }
                attackCooldown -= Time.deltaTime;
            }
        }
        else
        {
            attackCooldown = firstAttackCooldown;
            target = Utilities.FindTargetSingle(
                transform.position, 
                visionRange, 
                sortFunc, 
                new int[] {GetInstanceID()}, 
                null,
                new Utilities.Team[] {team},
                new Utilities.Altitude[] {Utilities.Altitude.Buried, Utilities.Altitude.Flying});

        }
    }
    private float sortFunc(Entity entity)
    {
        return Vector2.Distance(transform.position, entity.transform.position);
    } 


    public void InitializeStats()
    {
        size = 0.5f;
        target = null;
        base.InitializeStats(
            "Turret", 
            1000.0f, 
            850.0f, 
            0.0f, 
            Utilities.EntityType.Building, 
            Utilities.Altitude.Grounded);


        firstAttackCooldown = 1.2f;
        attackCooldown = firstAttackCooldown;
        maxAttackCooldown = 0.9f;
        attackRange = 4.2f;
        visionRange = 9.0f;

        bulletLifetime = 20.0f;
        bulletSpeed = 17.5f;
        damage = 200.0f;

        state = State.Idling;
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
