using UnityEngine;
using System.Collections;

[System.Serializable]
public class characterCard : Card 
{
    // Character stats
    public int ATK;
    public int DEF;
    public int SPC;
    public int MOV;
    public int maxHP;
    public string spriteName;

    public characterCard() : base()
    {
        ATK = 1;
        DEF = 1;
        SPC = 1;
        MOV = 1;
        maxHP = 1;
    }

    public characterCard(string name, string description, int attack, int defense, int special, int movement, int totalHP, string spriteString) : base (name, description)
    {
        ATK = attack;
        DEF = defense;
        SPC = special;
        MOV = movement;
        maxHP = totalHP;
        spriteName = spriteString; //Resources.Load("Textures/Heros/walk_" + spriteName + "_18", typeof(Sprite)) as Sprite;
    }
}
