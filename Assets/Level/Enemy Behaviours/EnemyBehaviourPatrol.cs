using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBehaviourPatrol : EnemyBehaviour 
{
    // Default behavior of the Patrol will cause the enemy to choose
    //  a random direction to start, and they will walk in a 'box',
    //  moving Up, Down, Left, and Right at 2 squares per direction..
    //  They only choose one direction per turn.
    private enum patrol
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        DEFINED
    };

    private patrol patrolDirection;
    private int patrolRange;
    private bool tileReached;
    public Vector2 positionToMoveTowards;
    private Vector2 startingPatrolPosition;

    public EnemyBehaviourPatrol()
    {
        patrolDirection = (patrol)Random.Range(0, 3); // Start patroling a random direction
        patrolRange = 2;
        startingPatrolPosition = new Vector2(posX, posY);
        Debug.Log("New EnemyPatrol towards: " + positionToMoveTowards);

        tileReached = false;
    }

    public void setPatrol(int x, int y)
    {
        positionToMoveTowards = new Vector2(x, y);
        patrolDirection = patrol.DEFINED;
        Debug.Log("EnemyPatrol defined.");
    }

    protected override void serviceSelectedState()
    {
        if (timer > 1f)
        {
            Debug.Log("EnemyPatrol: Service Selected State");

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
            else
            {
                // No enemies in range, Move in the specified direction.
                switch (patrolDirection)
                {
                    case patrol.UP:
                        patrolDirection = patrol.RIGHT;

                        for (int i = 0; i < patrolRange; i++ )
                        {
                            selectedTile = map.getTile(posX, posY + (patrolRange-i));
                            if (moveTiles.Contains(selectedTile))
                            {
                                return;
                            }
                        }

                        goto case patrol.RIGHT;
                    case patrol.RIGHT:
                        patrolDirection = patrol.DOWN;

                        for (int i = 0; i < patrolRange; i++ )
                        {
                            selectedTile = map.getTile(posX + (patrolRange - i), posY);
                            if (moveTiles.Contains(selectedTile))
                            {
                                return;
                            }
                        }

                        goto case patrol.DOWN;
                    case patrol.DOWN:
                        patrolDirection = patrol.LEFT;
                        
                        for (int i = 0; i < patrolRange; i++ )
                        {
                            selectedTile = map.getTile(posX, posY - (patrolRange - i));
                            if (moveTiles.Contains(selectedTile))
                            {
                                return;
                            }
                        }

                        goto case patrol.DOWN;
                    case patrol.LEFT:
                        patrolDirection = patrol.UP;
                        
                        for (int i = 0; i < patrolRange; i++ )
                        {
                            selectedTile = map.getTile(posX - (patrolRange - i), posY);
                            if (moveTiles.Contains(selectedTile))
                            {
                                return;
                            }
                        }

                        break;
                    case patrol.DEFINED:
                        // This section defined in Move state
                        return;
                }
            }
        }
    }

    protected override void serviceSelectMove()
    {
        if (timer > 1f)
        {
            Debug.Log("EnemyPatrol: Service Select Move");
            if (selectedUnitTile != null && Mathf.Abs(selectedUnitTile.x - posX) + Mathf.Abs(selectedUnitTile.y - posY) < 2)
            {
                Debug.Log("EnemyPatrol: Adjacent to target");
                state = EnemyState.ViewAttackRange;
                timer = 0;
                selectedTile = null;
                map.clearHighlights(movementRange); // Clear blue tiles
                map.clearHighlights(meleeRange);    // Clear enemies highlighted for demo purposes
                return;
            }

            // Walk to the proper patrol tile..
            if(patrolDirection == patrol.DEFINED && !attacking)
            {
                Debug.Log("EnemyPatrol: Moving towards patrol square: <" + positionToMoveTowards.x + ", " + positionToMoveTowards.y + ">");
                if(tileReached)
                {
                    currentPath = buildPatrolPathToTile((int)startingPatrolPosition.x, (int)startingPatrolPosition.y, movementRange);
                }
                else
                {
                    currentPath = buildPatrolPathToTile((int)positionToMoveTowards.x, (int)positionToMoveTowards.y, movementRange);
                }
                
                setStartAndEnd();
                state = EnemyState.Moving;
                return;
            }

            // Walk to our selected tile..
            currentPath = buildPathToTile(selectedTile.x, selectedTile.y, movementRange);
            setStartAndEnd();
            state = EnemyState.Moving;
            Debug.Log("EnemyPatrol: Moving towards <" + selectedTile.x + ", " + selectedTile.y + ">");
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
        path = patrolPathFind(posX, posY, dstPos, openTiles, path, throwawayTile, 1, 999);

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
                    p = patrolPathFind(tileX - 1, tileY, dstPos, openTiles, p, t, dist + 1, bestDist + 1);
                    if (p != null)
                    {
                        return p;
                    }
                    break;
                case 2:
                    t = map.getTile(tileX, tileY - 1);
                    openTiles.Remove(t);
                    p.addStep(Path.VERTICAL, -1);
                    p = patrolPathFind(tileX, tileY - 1, dstPos, openTiles, p, t, dist + 1, bestDist + 1);
                    if (p != null)
                    {
                        return p;
                    }
                    break;
                case 3:
                    t = map.getTile(tileX + 1, tileY);
                    openTiles.Remove(t);
                    p.addStep(Path.HORIZONTAL, 1);
                    p = patrolPathFind(tileX + 1, tileY, dstPos, openTiles, p, t, dist + 1, bestDist + 1);
                    if (p != null)
                    {
                        return p;
                    }
                    break;
                case 4:
                    t = map.getTile(tileX, tileY + 1);
                    openTiles.Remove(t);
                    p.addStep(Path.VERTICAL, 1);
                    p = patrolPathFind(tileX, tileY + 1, dstPos, openTiles, p, t, dist + 1, bestDist + 1);
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