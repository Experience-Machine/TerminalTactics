using UnityEngine;
using System.Collections;

public class Card
{
    public string nm;   // Name
    public string desc; // Description

    public Card()
    {
        nm = "";
        desc = "";
    }
    
    public Card(string name, string description)
    {
        nm = name;
        desc= description;
    }

    public string getName()
    {
        return nm;
    }
    public string getDescription()
    {
        return desc;
    }
}
