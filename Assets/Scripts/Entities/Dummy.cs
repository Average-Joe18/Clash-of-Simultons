using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : Entity
{
    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();
        rbody = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();

        InitializeStats();
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
    }
    public void InitializeStats()
    {
        size = 0.5f;
        base.InitializeStats(
            "Dummy", 
            1.0f, 
            10000.0f, 
            0.0f, 
            Utilities.EntityType.All, 
            Utilities.Altitude.All);

        visionRange = 0.0f;
    }
    override public void Attack(Entity target)
    {
        return;
    }
    override public void TakeDamage(float amount, GameObject attacker, Utilities.DamageType damageType)
    {
        base.TakeDamage(amount, attacker, damageType);
        string damageName = "";
        switch (damageType)
        {
            case Utilities.DamageType.True:
                damageName = "True";
                break;
            case Utilities.DamageType.Melee:
                damageName = "Melee";
                break;
            case Utilities.DamageType.Ranged:
                damageName = "Ranged";
                break;
        }
        Debug.Log($"I've taken {amount} {damageName} damage from {attacker.GetInstanceID()}!");
    }
}
