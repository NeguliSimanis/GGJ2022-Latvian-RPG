using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{
    public static bool IsVector2Closer(Vector2Int vector2, Vector2Int vector1, Vector2Int source)
    {
        int diff1 = Mathf.Abs(vector1.x - source.x) + Mathf.Abs(vector1.y - source.y);
        int diff2 = Mathf.Abs(vector2.x - source.x) + Mathf.Abs(vector2.y - source.y);

        if (diff2 <= diff1)
        {
            //Debug.Log(source + "(source) is closer to " + vector2 + " than " + vector1);
            return true;
        }
        else
        {
            //Debug.Log(source + "(source) is closer to " + vector1 + " than " + vector2);
            return false;
        }
    }

    /// <summary>
    /// Checks if target is within damage range - range includes moving closer within the same turn not only skill range
    /// </summary>
    /// <param name="target"></param>
    /// <param name="damageSource"></param>
    /// <param name="moveSpeed"></param>
    /// <param name="damageSkill"></param>
    /// <returns></returns>
    public static bool IsWithinDamageRange(Vector2Int target, Vector2Int damageSource, int moveSpeed, Skill damageSkill)
    {
        
        int totalRange = moveSpeed + damageSkill.skillRange;

        /*      0 - source
         *      X - totalrange = 2
         *      
         *      _ _ X _ _
         *      _ X X X _
         *      X X 0 X X
         *      _ X X X _
         *      _ _ X _ _
         *
         */

        int xDiff = Mathf.Abs(target.x - damageSource.x);
        int yDiff = Mathf.Abs(target.y - damageSource.y);


        if (xDiff + yDiff <= totalRange)
        {
            Debug.Log("IsWithinDamageRange" + target + "  is SAFE FROM (not) total range (" + totalRange + ") of " + damageSource + "(xDiff + yDiff: )" + (xDiff+yDiff)
                + ". totalrange: " + totalRange);
            return true;
        }
        else
        {
            Debug.Log("IsWithinDamageRange" + target + " is SAFE FROM total range (" + totalRange + ") of " + damageSource + "(xDiff + yDiff: )" + (xDiff + yDiff)
                + ". totalrange: " + totalRange);
            return false;
        }
    }

    public static float CalculateDamage(float rawDamage, int defense, int offense)
    {
        float damageMultiplier = 1;
        
        // For each 10 points of defense above offense, incoming damage reduced by 10%
        // 100+ defense = incoming damage heals lol
        if (defense >= offense)
        {
            damageMultiplier = (1f - 0.01f*(defense - offense));
        }
        // For each 10 points of offense above defense, damage dealt increased by 10%
        else
        {
            damageMultiplier = 1 + 0.01f * (offense - defense);
        }

        float finalDamage = rawDamage * damageMultiplier;
        return finalDamage;
    }

}
