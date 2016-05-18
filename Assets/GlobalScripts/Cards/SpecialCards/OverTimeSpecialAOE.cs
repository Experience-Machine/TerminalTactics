using UnityEngine;
using System.Collections;

[System.Serializable]
public class OverTimeSpecialAOE : specialCard
{
    OverTimeEffect effect;
 
    int aoeSize = 1;
    int aoeRange = 1;

    Tile[] aoeRangeTiles;

    public OverTimeSpecialAOE(string name, string description, int cost, OverTimeEffect ote, int aoeSize, int aoeRange) : base(name, description, cost)
    {
        this.aoeSize = aoeSize;
        this.aoeRange = aoeRange;
        this.effect = ote;
        aoeRangeTiles = new Tile[0];
    }
    // Implement this if you make a special attack
    public override void specialAttack(Map gameMap, characterInfo theCharacter, CharacterBehaviour character)
    {
        
        gameMap.clearAllHighlights();
        
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        //Debug.Log("Mouse position: " + mousePosition);

        Tile[] characterAttackRange = gameMap.getRangeTiles(character.posX, character.posY, this.aoeRange);
        //Debug.Log("Attack range size: " + characterAttackRange.Length);

        Tile closestMouseTile = characterAttackRange[0]; //Default value
        float minDistance = 10000; //Need some large value
        for (int i = 0; i < characterAttackRange.Length; i++)
        {
            Tile tile = characterAttackRange[i];
            float distance = Mathf.Abs(Vector3.Distance(tile.transform.position, mousePosition));
            Debug.Log("Distance: " + distance);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestMouseTile = tile;
            }
        }

        aoeRangeTiles = gameMap.getRangeTiles(closestMouseTile.x, closestMouseTile.y, aoeSize);
        gameMap.highlightTiles(aoeRangeTiles, gameMap.attackHighlight);

        //Player selected a tile
        if (gameMap.selectedTile != null)
        {
            for (int i = 0; i < aoeRangeTiles.Length; i++)
            {
                Tile t = aoeRangeTiles[i];
                if (t.enemyOnTile != null && effect.isAttack)
                {
                    t.applyOverTimeEffectToTile(effect);
                }

                if (t.charOnTile != null && !effect.isAttack)
                {
                    t.applyOverTimeEffectToTile(effect);
                }

            }
            gameMap.clearHighlights(aoeRangeTiles);
            character.currentSpecial -= cost;
            character.setState(CharacterBehaviour.CharacterState.Idle);
        }

    }
}
