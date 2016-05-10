using UnityEngine;
using System.Collections;

// handle changing the name
public class specialCard : Card 
{
    int cost;

    public specialCard() : base()
    {
        this.cost = 1;
    }

    public specialCard(string name, string description, int cost) : base()
    {
        this.cost = cost;
    }
    // Implement this if you make a special attack
    public virtual void specialAttack()
    {

    }

}
