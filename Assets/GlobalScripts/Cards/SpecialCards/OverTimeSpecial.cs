using UnityEngine;
using System.Collections;

[System.Serializable]
public class OverTimeSpecial : specialCard {
    OverTimeEffect effect;
    int attackRange;

    Tile[] attackRangeTiles;

    public OverTimeSpecial(string name, string description, int cost, int attackRange, OverTimeEffect ote) : base(name, description, cost)
    {
        this.effect = ote;
        this.attackRange = attackRange;
        attackRangeTiles = new Tile[0];
    }
    // Implement this if you make a special attack
    public override void specialAttack(Map gameMap, characterInfo theCharacter, CharacterBehaviour character)
    {
        gameMap.clearHighlights(attackRangeTiles);
        attackRangeTiles = gameMap.getRangeTiles(character.posX, character.posY, attackRange);
        gameMap.highlightTiles(attackRangeTiles, gameMap.attackHighlight);

        if (gameMap.selectedTile != null)
        {
            //Cant attack another player currently
            if (gameMap.selectedTile.charOnTile != null)
            {
                gameMap.selectedTile = null;
                return;
            }

            for (int i = 0; i < attackRangeTiles.Length; i++)
            {
                // If the selected tile is in our attack range:
                if (attackRangeTiles[i] == gameMap.selectedTile)
                {
                    Tile tileToAttack = gameMap.selectedTile;
                    gameMap.clearHighlights(attackRangeTiles);
                   
                    if (tileToAttack.enemyOnTile != null)
                    {
                        tileToAttack.applyOverTimeEffectToTile(effect);
                    }

                }
            }
            character.currentSpecial -= cost;
            character.setState(CharacterBehaviour.CharacterState.Idle);
        }
        

    }
}
