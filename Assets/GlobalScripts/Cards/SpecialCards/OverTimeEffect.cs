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

    public OverTimeEffect(int numTurns, int statChange, string statType, bool applyOnce, bool reset)
    {
        this.numTurns = numTurns;
        this.statChange = statChange;
        this.statType = statType;
        this.applyOnce = applyOnce;
        this.resetAtEnd = reset;
        MAX_NUM_TURNS = numTurns;
    }

    public bool equals(OverTimeEffect other)
    {
        if (numTurns == other.numTurns && statChange == other.statChange &&
            statType.Equals(other.statType) && applyOnce == other.applyOnce)
        {
            return true;
        }

        return false;
    }

    public OverTimeEffect clone()
    {
        return new OverTimeEffect(numTurns, statChange, statType, applyOnce, resetAtEnd);
    }

    public int getEffectResult(int stat, string dmgNumberTxt, Vector3 position)
    {


        if (numTurns > 0 && !applyOnce)
        {
            stat += statChange;
            DamageNumber.createDamageNumber(statChange.ToString() + " " + dmgNumberTxt, position);
            
        }
        if (applyOnce && applied)
        {
            stat += statChange;
            DamageNumber.createDamageNumber(statChange.ToString() + " " + dmgNumberTxt, position);
            applied = true;
        }

        if (resetAtEnd && numTurns == 0)
        {
            if (applyOnce) stat -= statChange;
            else stat -= MAX_NUM_TURNS * statChange;
        }

        numTurns--;
        return stat;
    }
}
