using UnityEngine;
using System.Collections;

[System.Serializable]
public class SelfBuff : specialCard
{

    string statToChange;
    int statVal;
    public SelfBuff(string name, string description, string lowerDescript, int cost, string statToChange, int statVal) : base(name, description, lowerDescript, cost)
    {
        this.statToChange = statToChange;
        this.statVal = statVal;
    }

    public override void specialAttack(Map gameMap, characterInfo theCharacter, CharacterBehaviour character)
    {

        switch (statToChange) { 
            case "health":
                Debug.Log("Got here");
                character.heal(statVal);
                DamageNumber.createDamageNumber(statVal.ToString() + " HP", character.transform.position);
                break;
            case "special":
                character.currentSpecial += statVal;
                DamageNumber.createDamageNumber(statVal.ToString() + " SPC", character.transform.position);
                break;
        }

        character.currentSpecial -= cost;
        character.setState(CharacterBehaviour.CharacterState.AnimateWait);
    }
}
