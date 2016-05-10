using UnityEngine;
using System.Collections;

// handle changing the name
public class characterCard : Card 
{
    // Character stats
    public int ATK;
    public int DEF;
    public int SPC;
    public int MOV;
    public int maxHP;
    // Maybe add sprite info here?

    public characterCard() : base()
    {
        ATK = 1;
        DEF = 1;
        SPC = 1;
        MOV = 1;
        maxHP = 1;
    }

    public characterCard(string name, string description, int attack, int defense, int special, int movement, int totalHP) : base (name, description)
    {
        ATK = attack;
        DEF = defense;
        SPC = special;
        MOV = movement;
        maxHP = totalHP;
    }
}
