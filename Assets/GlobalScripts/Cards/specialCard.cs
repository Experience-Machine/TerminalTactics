﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class specialCard : Card 
{
    public int cost;

    public specialCard() : base()
    {
        this.cost = 1;
    }

    public specialCard(string name, string description, int cost) : base(name, description)
    {
        this.cost = cost;
    }
    // Implement this if you make a special attack
    public virtual void specialAttack(Map gameMap, characterInfo theCharacter, CharacterBehaviour character)
    {

    }

}
