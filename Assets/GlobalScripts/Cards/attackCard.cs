using UnityEngine;
using System.Collections;

[System.Serializable]
public class attackCard : Card 
{
    public int dmg; // Attack Damage
    public int rng; // Attack Range

    public attackCard() : base ()
    {
        dmg = 1;
        rng = 1;
    }

    public attackCard(string name, string description, int damage, int range) : base(name, description)
    {
        dmg = damage;
        rng = range;
    }

    public int getDamage()
    {
        return dmg;
    }

    public int getRange()
    {
        return rng;
    }
}
