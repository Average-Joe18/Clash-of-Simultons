using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

abstract public class Entity : MonoBehaviour
{
    public String ID;
    public float maxhealth;
    public float currenthealth;
    public float movespeed;
    public float visionRange;
    public float size;
    public float mass;
    public Boolean targetable;
    public Utilities.Team team;
    public Utilities.EntityType entityType;
    public Utilities.Altitude altitudeType;
    public Rigidbody2D rbody;
    public CircleCollider2D col;
    abstract public void Attack(Entity target);

    //sprite animation for damage
    private float redDuration;
    SpriteRenderer render;
    Color color;

    virtual public void Start()
    {
        render = GetComponent<SpriteRenderer>();
        redDuration = 0.0f;
        color = render.color;
    }
    virtual public void Update()
    {
        if (redDuration > 0)
        {
            redDuration -= Time.deltaTime;
            render.color = Color.red;
        }
        else
        {
            render.color = color;
        }
    }
    virtual public void TakeDamage(float amount, GameObject attacker, Utilities.DamageType damageType)
    {
        redDuration = 0.15f;

        currenthealth -= amount;
        
        if (currenthealth <= 0)
        {
            onKilled(attacker);
        }
    }

    /*  Initializes some stats that all entities will have.

        params:
        id - a string storing the name of the entity
        mass - a float storing the mass of the troop for physics calcs
        maxheal - a float storing the max health of the entity
        movespeed - a float storing the movespeed of the entity
        type - a EntityType enum storing whether the entity is a troop or building etc.
        alt - a Altitude enum storing whether the troop is flying or grounded etc.
    */
    public void InitializeStats(
        string id, 
        float mass,
        float maxheal,
        float movpeed,
        Utilities.EntityType type,
        Utilities.Altitude alt
        )
    {
        //team = Utilities.Team.FFA;

        ID = id;
        col.radius = 0.5f;
        rbody.mass = mass;
        maxhealth = maxheal;
        currenthealth = maxhealth;
        movespeed = movpeed;

        entityType = type;
        altitudeType = alt;

        targetable = true;
    }
    public void onKilled(GameObject killer)
    {
            Terminate();
    }
    public void Terminate()
    {
        Destroy(gameObject);
    }
}
  