using UnityEngine;
using System.Collections;

[System.Serializable]
public class TargetedAOEAttack : specialCard
{
    int aoeSize = 1;
    int aoeRange = 1;
    int aoeDamage = 1;

    Tile[] aoeRangeTiles;

    bool hurtOrHeal;

    public TargetedAOEAttack(string name, string description, string lowerDescript, int cost, int aoeSize, int aoeRange, int aoeDamage, bool hurtOrHeal) : base(name, description, lowerDescript, cost)
    {
        this.aoeSize = aoeSize;
        this.aoeRange = aoeRange;
        this.aoeDamage = aoeDamage;
        aoeRangeTiles = new Tile[0];
        this.hurtOrHeal = hurtOrHeal;
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
            //Debug.Log("Distance: " + distance);

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
            bool noTilesAttacked = true;
            for (int i = 0; i < aoeRangeTiles.Length; i++)
            {
                Tile t = aoeRangeTiles[i];
                if (t.enemyOnTile != null) 
                {

                    if (hurtOrHeal == true) 
                    {
                        t.setCard(character.ATTACK_DAMAGE_CARD);
                        t.attackTile(character.ATTACK_DAMAGE);
                        noTilesAttacked = false;
                    } 
                }

                if (t.charOnTile != null && hurtOrHeal == false)
                {
                    t.healTile(aoeDamage);
                    noTilesAttacked = false;
                }
                
            }

            if (noTilesAttacked)
            {
                gameMap.selectedTile = null;
                return;
            }

            if (hurtOrHeal) // Hurt
            {
                GameObject buffObj = Object.Instantiate(Resources.Load("Prefabs/AoE Attack")) as GameObject;
                Vector2 spawnPosition = gameMap.selectedTile.transform.position;
                spawnPosition.y -= 1.8f;
                buffObj.transform.position = spawnPosition;
            }

            gameMap.clearHighlights(aoeRangeTiles);
            character.currentSpecial -= cost;
            character.setState(CharacterBehaviour.CharacterState.AnimateWait);
        }

    }

}
