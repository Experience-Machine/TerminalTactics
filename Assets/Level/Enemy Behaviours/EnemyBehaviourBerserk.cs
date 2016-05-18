using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBehaviourBerserk : EnemyBehaviour 
{
    // Rushes a tile as defined by rushUnit method
    private bool tileReached;
    public Vector2 positionToMoveTowards;

    public EnemyBehaviourBerserk()
    {
        positionToMoveTowards = new Vector2(0, 0);
        tileReached = false;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public void rushUnit(int x, int y)
    {
        positionToMoveTowards = new Vector2(x, y);
        //Debug.Log("EnemyBerserk unit defined.");
    }

    protected override void serviceSelectedState()
    {
        if (timer > 1f)
        {
            //Debug.Log("EnemyBerserk: Service Selected State");

            // Highlight movement range (for player reference, really)
            movementRange = map.getMovementRangeTiles(posX, posY, MOVEMENT_RANGE);
            map.highlightTiles(movementRange, map.movementHighlight);

            // Set next state to movement
            state = EnemyState.SelectMove;
            timer = 0;

            meleeRange = map.getMeleeRange(posX, posY, MOVEMENT_RANGE); // Get enemies within melee range
            if (Debug.isDebugBuild) // Highlight them for debug purposes
                map.highlightTiles(meleeRange, map.attackHighlight);

            // Create a list of our tiles in movement range
            List<Tile> moveTiles = new List<Tile>(movementRange);

            if (meleeRange.Length > 0)
            {
                attacking = true;
                selectedUnitTile = meleeRange[0]; // Attack the first guy in the list..

                if (Mathf.Abs(selectedUnitTile.x - posX) + Mathf.Abs(selectedUnitTile.y - posY) < 2)
                {
                    return;
                }

                // Find which tile we can walk to around him..
                selectedTile = map.getTile(meleeRange[0].x - 1, meleeRange[0].y);
                if (moveTiles.Contains(selectedTile))
                {
                    return;
                }

                selectedTile = map.getTile(meleeRange[0].x, meleeRange[0].y - 1);
                if (moveTiles.Contains(selectedTile))
                {
                    return;
                }

                selectedTile = map.getTile(meleeRange[0].x + 1, meleeRange[0].y);
                if (moveTiles.Contains(selectedTile))
                {
                    return;
                }

                selectedTile = map.getTile(meleeRange[0].x, meleeRange[0].y + 1);
                if (moveTiles.Contains(selectedTile))
                {
                    return;
                }
            }

            // No enemies in range.
            // Move towards unit specified in selectMove
        }
    }

    protected override void serviceSelectMove()
    {
        if (timer > 1f)
        {
            //Debug.Log("EnemyBerserk: Service Select Move");
            if (selectedUnitTile != null && Mathf.Abs(selectedUnitTile.x - posX) + Mathf.Abs(selectedUnitTile.y - posY) < 2)
            {
                //Debug.Log("EnemyBerserk: Adjacent to target");
                state = EnemyState.ViewAttackRange;
                timer = 0;
                selectedTile = null;
                map.clearHighlights(movementRange); // Clear blue tiles
                map.clearHighlights(meleeRange);    // Clear enemies highlighted for demo purposes
                return;
            }

            // Walk to the proper patrol tile..
            if(!attacking)
            {
                //Debug.Log("EnemyBerserk: Moving towards patrol square: <" + positionToMoveTowards.x + ", " + positionToMoveTowards.y + ">");

                currentPath = buildPatrolPathToTile((int)positionToMoveTowards.x, (int)positionToMoveTowards.y, movementRange);
                setStartAndEnd();
                state = EnemyState.Moving;
                return;
            }

            // Walk to our selected tile..
            currentPath = buildPathToTile(selectedTile.x, selectedTile.y, movementRange);
            setStartAndEnd();
            state = EnemyState.Moving;
            //Debug.Log("EnemyBerserk: Moving towards <" + selectedTile.x + ", " + selectedTile.y + ">");
            return;
        }
    }

    protected Path buildPatrolPathToTile(int tileX, int tileY, Tile[] tileList)
    {
        //Debug.Log("buildPatrolPath..");
        Path path = (Path)ScriptableObject.CreateInstance(typeof(Path));
        Vector2 dstPos = new Vector2(tileX, tileY);
        List<Tile> openTiles = new List<Tile>(tileList);
        Tile throwawayTile = movementRange[0];
        path = patrolPathFind(posX, posY, dstPos, openTiles, path, throwawayTile, 0, 999);

        if (path == null)
        {
            //Debug.Log("Null path!");
            return buildPathToTile(posX, posY); // Aka Don't move
        }

        //Debug.Log("Path: " + path.ToString());

        return path;
    }

    protected Path patrolPathFind(int tileX, int tileY, Vector2 dstPos, List<Tile> openTiles, Path p, Tile t, int dist, int bestDist)
    {
        //Debug.Log("Dist: " + dist + ", Best: " + bestDist);
        if(tileX == dstPos.x && tileY == dstPos.y)
        {
            //Debug.Log("Tile found..");
            tileReached = !tileReached;
            return p;
        }

        if(dist == MOVEMENT_RANGE)
        {
            //Debug.Log("Mov_Range path found: <" + tileX + ", " + tileY + ">");
            return p;
        }

        int newBest = 0;
        List<int> bestTypes = new List<int>();
        Path originalPath = p.clone();

        t = map.getTile(tileX - 1, tileY);
        if (openTiles.Contains(t))
        {
            newBest = (int)(Mathf.Abs(t.x - dstPos.x) + Mathf.Abs(t.y - dstPos.y) + dist);
            if(newBest <= bestDist)
            {
                if (newBest != bestDist)
                    bestTypes.Clear();
                bestDist = newBest;
                bestTypes.Add(1);
            }
        }

        t = map.getTile(tileX, tileY - 1);
        if (openTiles.Contains(t))
        {
            newBest = (int)(Mathf.Abs(t.x - dstPos.x) + Mathf.Abs(t.y - dstPos.y) + dist);
            if (newBest <= bestDist)
            {
                if (newBest != bestDist)
                    bestTypes.Clear();
                bestDist = newBest;
                bestTypes.Add(2);
            }
        }

        t = map.getTile(tileX + 1, tileY);
        if (openTiles.Contains(t))
        {
            newBest = (int)(Mathf.Abs(t.x - dstPos.x) + Mathf.Abs(t.y - dstPos.y) + dist);
            if (newBest <= bestDist)
            {
                if (newBest != bestDist)
                    bestTypes.Clear();
                bestDist = newBest;
                bestTypes.Add(3);
            }
        }

        t = map.getTile(tileX, tileY + 1);
        if (openTiles.Contains(t))
        {
            newBest = (int)(Mathf.Abs(t.x - dstPos.x) + Mathf.Abs(t.y - dstPos.y) + dist);
            if (newBest <= bestDist)
            {
                if (newBest != bestDist)
                    bestTypes.Clear();
                bestDist = newBest;
                bestTypes.Add(4);
            }
        }

        int bestType;
        for(int i = 0; i < bestTypes.Count; i++)
        {
            bestType = bestTypes[i];
            
            p = originalPath.clone();

            switch (bestType)
            {
                case 1:
                    t = map.getTile(tileX - 1, tileY);
                    openTiles.Remove(t);
                    p.addStep(Path.HORIZONTAL, -1);
                    p = patrolPathFind(tileX - 1, tileY, dstPos, openTiles, p, t, dist + 1, bestDist);
                    if (p != null)
                    {
                        return p;
                    }
                    break;
                case 2:
                    t = map.getTile(tileX, tileY - 1);
                    openTiles.Remove(t);
                    p.addStep(Path.VERTICAL, -1);
                    p = patrolPathFind(tileX, tileY - 1, dstPos, openTiles, p, t, dist + 1, bestDist);
                    if (p != null)
                    {
                        return p;
                    }
                    break;
                case 3:
                    t = map.getTile(tileX + 1, tileY);
                    openTiles.Remove(t);
                    p.addStep(Path.HORIZONTAL, 1);
                    p = patrolPathFind(tileX + 1, tileY, dstPos, openTiles, p, t, dist + 1, bestDist);
                    if (p != null)
                    {
                        return p;
                    }
                    break;
                case 4:
                    t = map.getTile(tileX, tileY + 1);
                    openTiles.Remove(t);
                    p.addStep(Path.VERTICAL, 1);
                    p = patrolPathFind(tileX, tileY + 1, dstPos, openTiles, p, t, dist + 1, bestDist);
                    if (p != null)
                    {
                        return p;
                    }
                    break;
            }
        }
        //Debug.Log("Fall through: " + tileX + "," + tileY + ", dstPos: " + dstPos + ", Path: " + p + ", T: " + t + ", Dist: " + dist + ", BestDist: " + bestDist);
        return null;
    }
}