using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    //Unlike projectiles the owner must set the tickRate AND maxTickRate!
    public float zoneSize;      //Radius of the zone
    public float maxlifetime;   //How long the zone will exist until it is destroyed
    public float lifetime;      //Stores the current lifetime
    public float damage;        //Damage per tick collision
    public float maxTickRate;   //Rate of ticks
    public float tickRate;      //Stores current tick progress
    public Entity owner;        //entity that created the zone
    public Utilities.DamageType damageType;     //type of damage

    void Start()
    {
        lifetime = maxlifetime;
    }
    void Update()
    {
        tickRate -= Time.deltaTime;
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
        if (tickRate <= 0)
        {
            DamageNearby();
            tickRate = maxTickRate;
        }
    }

    public void DamageNearby()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, zoneSize);

        foreach (Collider2D hit in hits)
        {
            Entity entity = hit.GetComponent<Entity>();

            if (entity != null)
            {
                if ((entity.team == Utilities.Team.FFA || entity.team != owner.team) && entity.targetable == true && entity.GetInstanceID() != owner.GetInstanceID())
                {
                    entity.TakeDamage(damage, owner == null ? null : owner.GetComponent<GameObject>(), damageType);
                }
            }
        }
    }

}
