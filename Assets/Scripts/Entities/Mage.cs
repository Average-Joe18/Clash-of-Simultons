using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEditor.UI;
using UnityEngine;

public class Mage : Entity
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
    public HomingExplosive projectile;

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
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, target.transform.localPosition, Time.deltaTime * movespeed);
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
                new Utilities.Altitude[] {Utilities.Altitude.Buried});


            if (target != null)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, target.transform.localPosition, Time.deltaTime * movespeed);
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
            "Mage", 
            2.0f, 
            725.0f, 
            1.0f, 
            Utilities.EntityType.Troop, 
            Utilities.Altitude.Grounded);

        firstAttackCooldown = 0.8f;
        attackCooldown = firstAttackCooldown;
        maxAttackCooldown = 1.4f;
        attackRange = 4.15f;
        visionRange = 9.0f;

        bulletLifetime = 3.0f;
        bulletSpeed = 12.5f;
        damage = 285.0f;

        state = State.Idling;
    }
    override public void Attack(Entity target)
    {
        HomingExplosive fireball = Instantiate(projectile, transform.position, transform.rotation);
        fireball.owner = gameObject.GetComponent<Entity>();
        fireball.target = target;
        fireball.maxlifetime = bulletLifetime;
        fireball.speed = bulletSpeed;
        fireball.explosionDamage = damage;
        fireball.exlosionDuration = 0.1f;
        fireball.firstTickRate = 0.0f;
        fireball.explosionTickRate = 1.0f;
        fireball.explosionSize = 0.75f;
    }

    override public void TakeDamage(float amount, GameObject attacker, Utilities.DamageType damageType)
    {
        base.TakeDamage(amount, attacker, damageType);
    }

}
