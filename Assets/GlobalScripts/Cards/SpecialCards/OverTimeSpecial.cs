using UnityEngine;
using System.Collections;

[System.Serializable]
public class OverTimeSpecial : specialCard {
    OverTimeEffect effect;
    int attackRange;

    Tile[] attackRangeTiles;

    public OverTimeSpecial(string name, string description, string lowerDescript, int cost, int attackRange, OverTimeEffect ote) : base(name, description, lowerDescript, cost)
    {
        this.effect = ote;
        this.attackRange = attackRange;
        attackRangeTiles = new Tile[0];
    }
    // Implement this if you make a special attack
    public override void specialAttack(Map gameMap, characterInfo theCharacter, CharacterBehaviour character)
    {
        //gameMap.clearHighlights(attackRangeTiles);
        gameMap.clearAllHighlights();
        attackRangeTiles = gameMap.getRangeTiles(character.posX, character.posY, attackRange);
        gameMap.highlightTiles(attackRangeTiles, gameMap.attackHighlight);

        if (gameMap.selectedTile != null)
        {
            
            //Cant attack another player currently
            if (gameMap.selectedTile.charOnTile != null)
            {
                gameMap.selectedTile = null;
                gameMap.clearHighlights(attackRangeTiles);
                return;
            }
            //Don't attack empty
            if (gameMap.selectedTile.enemyOnTile == null)
            {
                gameMap.selectedTile = null;
                gameMap.clearHighlights(attackRangeTiles);
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
            gameMap.clearHighlights(attackRangeTiles);
            character.currentSpecial -= cost;
            character.setState(CharacterBehaviour.CharacterState.AnimateWait);
        }
        

    }
}
