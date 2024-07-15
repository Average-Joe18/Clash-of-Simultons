using System.Buffers;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEditor.UI;
using UnityEngine;

public class Necromancer : Entity
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
    public Entity summonable;

    State state;

    //attacking related variables
    public float firstAttackCooldown;
    public float maxAttackCooldown;
    public float attackCooldown;
    public float attackRange;

    public float summonCooldown;
    public float maxSummonCooldown;

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
        summonCooldown -= Time.deltaTime;

        if (summonCooldown <= 0)
        {
            SummonFour(0.75f);
            summonCooldown = maxSummonCooldown;
        }

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
            "Necromancer", 
            2.0f, 
            835.0f, 
            1.0f, 
            Utilities.EntityType.Troop, 
            Utilities.Altitude.Grounded);

        firstAttackCooldown = 1.0f;
        attackCooldown = firstAttackCooldown;
        maxAttackCooldown = 1.1f;
        attackRange = 4.15f;
        visionRange = 9.0f;

        bulletLifetime = 3.0f;
        bulletSpeed = 15.0f;
        damage = 135.0f;

        summonCooldown = 3.0f;
        maxSummonCooldown = 7.0f;

        state = State.Idling;
    }
    override public void Attack(Entity target)
    {
        HomingExplosive magic = Instantiate(projectile, transform.position, transform.rotation);
        magic.owner = gameObject.GetComponent<Entity>();
        magic.target = target;
        magic.maxlifetime = bulletLifetime;
        magic.speed = bulletSpeed;
        magic.explosionDamage = damage;
        magic.exlosionDuration = 0.1f;
        magic.firstTickRate = 0.0f;
        magic.explosionTickRate = 1.0f;
        magic.explosionSize = 0.55f;
    }

    override public void TakeDamage(float amount, GameObject attacker, Utilities.DamageType damageType)
    {
        base.TakeDamage(amount, attacker, damageType);
    }

    /*  Summons 4 summonable Entity around the necromancer. 
    */
    public void SummonFour(float dist)
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                Entity summon = Instantiate(summonable, new Vector2(transform.position.x - dist + 2*i*dist, transform.position.y - dist + 2*j*dist), transform.rotation);
                summon.team = team;
            }
        }
    }

}
