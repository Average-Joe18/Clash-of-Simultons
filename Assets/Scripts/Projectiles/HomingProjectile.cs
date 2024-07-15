using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    public float maxlifetime;   //How long the projectile will exist until it is destroyed
    public float lifetime;      //Stores the current lifetime
    public float damage;        //Damage upon collision
    public float speed;         //speed of projectile
    public Entity owner;        //entity that shot the projectile
    public Entity target;       //entity the projectile is aimed towards
    public Utilities.DamageType damageType; //type of damage

    public void Start()
    {
        lifetime = maxlifetime;
    }
    public void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }

        transform.localPosition = Vector3.MoveTowards(transform.localPosition, target.transform.localPosition, Time.deltaTime * speed);
    }

    void OnTriggerEnter2D(Collider2D collide)
    {
        Entity hit = collide.GetComponent<Entity>();
        if (hit != null)
        {
            if (hit.GetInstanceID() == target.GetInstanceID())
            {
                target.TakeDamage(damage, owner == null ? null : owner.GetComponent<GameObject>(), damageType);
                Destroy(gameObject);
            }
        }
    }

}
