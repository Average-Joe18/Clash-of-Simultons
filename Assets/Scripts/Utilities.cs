using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utilities
{
    public enum Team
    {
        FFA,
        A,
        B,
        C,
        D
    }
    public enum DamageType
    {
        True,
        Melee,
        Ranged
    }
    public enum EntityType
    {
        All,
        Troop,
        Building
    }
    //This enum stores whether a troop is flying, grounded or underground
    public enum Altitude
    {
        All,
        Buried,
        Grounded,
        Flying
    }

    /*  This function using OverlapCircle to find all entities in a circular radius. It then iterates through all entities and returns
        the one that has the lowest "score" when run through sortFunc.

        params: 
        center - A Vector2 storing the center of the search area
        radius - A float which stores the radius of the search area
        sortfunc - a function that must take in an Entity and return a float. findTargetSingle will return the entity that returns the lowest value.
        IDFilter - stores an array of int. These ints should be obtained using GetInstanceID(). These IDs will be excluded from the search.
        typeFilter - stores an array of EntityType. Entities of these types will be excluded from the search.
        teamFilter - stores an array of Team. Entities of the specified teams will be excluded from the search.
        altitudeFilter - stores an array of Altitude. Entities of the specified altitudes will be excluded from the search.

        return:
        Entity that was found, or null if no entities were found.
    */
    public static Entity FindTargetSingle(
        Vector2 center, 
        float radius, 
        Func<Entity, float> sortFunc, 
        int[] IDFilter = null,
        EntityType[] typeFilter = null, 
        Team[] teamFilter = null, 
        Altitude[] altitudeFilter = null)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);
        Entity candidate = null;
        float maxscore = int.MaxValue;
        foreach (Collider2D hit in hits)
        {
            Entity entity = hit.GetComponent<Entity>();
            if (entity != null)
            {
                if ((entity.team == Utilities.Team.FFA || !teamFilter.Contains(entity.team))
                    && !(IDFilter?.Contains(entity.GetInstanceID()) ?? false)
                    && !(typeFilter?.Contains(entity.entityType) ?? false)
                    && !(altitudeFilter?.Contains(entity.altitudeType) ?? false)
                    && entity.targetable)
                {

                    float score = sortFunc(entity);
                    if (score < maxscore)
                    {
                        candidate = entity;
                        maxscore = score;
                    }
                }
            }
        }
        if (candidate != null)
        {
            return candidate;
        }
        return null;
    }
}
