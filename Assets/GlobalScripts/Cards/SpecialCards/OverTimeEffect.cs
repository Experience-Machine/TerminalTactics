using UnityEngine;
using System.Collections;


[System.Serializable]
public class OverTimeEffect {
    public int MAX_NUM_TURNS;
    public int numTurns;
    public int statChange;
    public string statType;
    public bool applied = false;
    public bool applyOnce;
    public bool resetAtEnd;

    public bool isAttack;
    public bool isBuff;

    public OverTimeEffect(int numTurns, int statChange, string statType, bool applyOnce, bool reset, bool isAttack)
    {
        this.numTurns = numTurns;
        this.statChange = statChange;
        this.statType = statType;
        this.applyOnce = applyOnce;
        this.resetAtEnd = reset;
        MAX_NUM_TURNS = numTurns;
        this.isAttack = isAttack;

        if (statChange >= 0)
        {
            isBuff = true;
        } else
        {
            isBuff = false;
        }
    }

    public bool equals(OverTimeEffect other)
    {
        //Check for if the other effect has the same stat type and if it's a buff/debuff
        if (statType.Equals(other.statType) && applyOnce == other.applyOnce && resetAtEnd == other.resetAtEnd &&
            isBuff == other.isBuff && isAttack == other.isAttack)
        {
            return true;
        }

        return false;
    }

    public OverTimeEffect clone()
    {
        return new OverTimeEffect(numTurns, statChange, statType, applyOnce, resetAtEnd, isAttack);
    }

    public int getEffectResult(int stat, string dmgNumberTxt, Vector3 position)
    {


        if (numTurns > 0 && !applyOnce)
        {
            stat += statChange;
            DamageNumber.createDamageNumber(statChange.ToString() + " " + dmgNumberTxt, position);
            
        }
        if (applyOnce && !applied)
        {
            stat += statChange;
            DamageNumber.createDamageNumber(statChange.ToString() + " " + dmgNumberTxt, position);
            applied = true;
        }

        

        numTurns--;
        if (resetAtEnd && numTurns == 0)
        {
            if (applyOnce) stat -= statChange;
            else stat -= MAX_NUM_TURNS * statChange;
        }

        return stat;
    }


    public int earlyReset(int stat)
    {
        if (applyOnce) stat -= statChange;
        else stat -= MAX_NUM_TURNS * statChange;
        return stat;
    }
}
