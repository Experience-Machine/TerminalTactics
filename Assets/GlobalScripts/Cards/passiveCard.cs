using UnityEngine;
using System.Collections;

[System.Serializable]
public class passiveCard : Card 
{
    // Character stats
    public string statToChange;
    public int statValue;
    // Maybe add sprite info here?

    public passiveCard() : base()
    {
        statToChange = "health";
        statValue = 0;
    }

    public passiveCard(string name, string description, string statToChange, int statValue) : base (name, description)
    {
        this.statToChange = statToChange;
        this.statValue = statValue;
    }
}
