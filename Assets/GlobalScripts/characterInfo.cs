using UnityEngine;
using System.Collections;

[System.Serializable]
public class characterInfo
{
    // Our character's four cards
    public characterCard charCard;
    public attackCard atkCard;
    public specialCard spcCard;
    public passiveCard psvCard;

    public characterInfo() 
    {
        // Blank constructor
    }
    public characterInfo(characterCard c, attackCard a, specialCard s, passiveCard p)
    {
        charCard = c;
        atkCard = a;
        spcCard = s;
        psvCard = p;
    }

    // setters
    public void setCharacter(characterCard c)
    {
        charCard = c;
    }

    public void setAttack(attackCard a) 
    {
        atkCard = a;
    }

    public void setSpecial(specialCard s)
    {
        spcCard = s;
    }

    public void setPassive(passiveCard p)
    {
        psvCard = p;
    }

    // getters
    public characterCard getCharacter()
    {
        return charCard;
    }

    public attackCard getAttack() 
    {
        return atkCard;
    }

    public specialCard getSpecial()
    {
        return spcCard;
    }

    public passiveCard getPassive()
    {
        return psvCard;
    }

    public string getName()
    {
        return charCard.getName();
    }

    public int getMaxHP()
    {
        return charCard.maxHP;
    }

}
