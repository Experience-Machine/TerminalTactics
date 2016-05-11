using UnityEngine;
using System.Collections;

// handle changing the name
public class TargetedAOEAttack : specialCard
{
    int aoeSize = 1;
    int aoeRange = 1;
    int aoeDamage = 1;

    Tile[] aoeRangeTiles;

    private Color attackHighlight = new Color(1f, 0, 0, .3f);

    public TargetedAOEAttack(string name, string description, int cost, int aoeSize, int aoeRange, int aoeDamage) : base(name, description, cost)
    {
        this.aoeSize = aoeSize;
        this.aoeRange = aoeRange;
        this.aoeDamage = aoeDamage;
        aoeRangeTiles = new Tile[0];
    }
    // Implement this if you make a special attack
    public override void specialAttack(Map gameMap, characterInfo theCharacter, CharacterBehaviour character)
    {
        gameMap.clearHighlights(aoeRangeTiles);
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
        gameMap.highlightTiles(aoeRangeTiles, attackHighlight);

        //Player selected a tile
        if (gameMap.selectedTile != null)
        {
            for (int i = 0; i < aoeRangeTiles.Length; i++)
            {
                Tile t = aoeRangeTiles[i];
                if (t.enemyOnTile != null) {
                    t.attackTile(aoeDamage);
                }
                
            }
            gameMap.clearHighlights(aoeRangeTiles);
            character.currentSpecial -= cost;
            character.setState(CharacterBehaviour.CharacterState.Idle);
        }

    }

}
