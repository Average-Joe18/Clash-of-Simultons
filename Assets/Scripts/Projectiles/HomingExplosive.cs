using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingExplosive : HomingProjectile
{
    /*  HomingExplosive inheirits from HomingProjectile. Upon impact it creates a DamageZone to deal AOE damage.

        To edit the impact damage, look at the members of HomingProjectile.
    */

    public float explosionSize;        //radius of the explosion
    public float explosionDamage;      //damage per tick of the explosion
    public float exlosionDuration;     //duration that the explosion remains
    public float explosionTickRate;    //rate of ticks of explosion
    public float firstTickRate;        //time before the initial damage
    public DamageZone explosion;       //stores the explosion prefab

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    void OnTriggerEnter2D(Collider2D collide)
    {
        Entity hit = collide.GetComponent<Entity>();
        if (hit != null)
        {
            if (hit.GetInstanceID() == target.GetInstanceID())
            {
                target.TakeDamage(damage, owner == null ? null : owner.GetComponent<GameObject>(), Utilities.DamageType.Ranged);

                DamageZone exp = Instantiate(explosion, target.transform.position, transform.rotation);
                exp.owner = owner;
                exp.damage = explosionDamage;
                exp.zoneSize = explosionSize;
                exp.maxlifetime = exlosionDuration;
                exp.maxTickRate = explosionTickRate;
                exp.tickRate = firstTickRate;
                exp.damageType = Utilities.DamageType.Ranged;
                Destroy(gameObject);
            }
        }
        
    }
}
