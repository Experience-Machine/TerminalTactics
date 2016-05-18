using UnityEngine;
using System.Collections;

public class AttackAll : specialCard {
    int damage;
    bool hurt;
    bool heal;

	public AttackAll (string name, string description, int cost, int damage, bool hurt, bool heal) : base(name, description, cost)
    {
        this.damage = damage;
        this.hurt = hurt;
        this.heal = heal;
    }

    public override void specialAttack(Map gameMap, characterInfo theCharacter, CharacterBehaviour character)
    {
        for (int i = 0; i < gameMap.getMapSizeX(); i++)
        {
            for (int j = 0; j < gameMap.getMapSizeY(); j++)
            {
                Tile t = gameMap.getTile(i, j);
                if (t.enemyOnTile != null)
                {
                    if (hurt)
                    {
                        t.attackTile(damage);
                    }
                }

                if (t.charOnTile != null && heal)
                {
                    t.healTile(damage);
                } //Attack the character too if we're not healing them
                else if (t.charOnTile != null && !heal)
                {
                    t.attackTile(damage);
                }
            }
        }
        character.currentSpecial -= cost;
        character.setState(CharacterBehaviour.CharacterState.Idle);
    }

}
