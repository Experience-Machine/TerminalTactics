using UnityEngine;
using System.Collections;

[System.Serializable]
public class specialCard : Card 
{
    public int cost;
    public string lowerDescript;

    public specialCard() : base()
    {
        this.cost = 1;
        this.lowerDescript = "";
    }

    public specialCard(string name, string description, string lowerDescript, int cost) : base(name, description)
    {
        this.cost = cost;
        this.lowerDescript = lowerDescript;
    }
    // Implement this if you make a special attack
    public virtual void specialAttack(Map gameMap, characterInfo theCharacter, CharacterBehaviour character)
    {

    }

}
