using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBehaviour : MonoBehaviour 
{

    public enum EnemyState
    {
        Moving,
        Idle, 
        Selected,
        SelectMove,
        ViewAttackRange,
        Attacking,
        Dead
    };
    protected EnemyState state;

    // Animation stuff
    public enum EnemyDirection
    {
        Up,
        Down,
        Left,
        Right
    }
    public EnemyDirection moveDirection;
    private EnemyDirection lastDirection;
    private Animator anim;

    protected characterInfo charInfo;
    protected Map map;

    // Enemy Stats stuff
    public int MAX_HEALTH = 1;
    public int currentHealth;
    public int defense;
    public int attack;
    public int currentSpecial;

    List<OverTimeEffect> currentEffects;

    // Movement Stuff
    protected Tile[] movementRange;
    protected Tile selectedTile;
    protected int MOVEMENT_RANGE = 4;

    // Attacking stuff
    protected Tile[] meleeRange;
    protected Tile selectedUnitTile;
    protected bool attacking;
    protected int ATTACK_DAMAGE = 1;
    protected int ATTACK_RANGE = 1;

    // Path finding stuff
    private List<Path> possiblePaths;
    protected Path currentPath; //Current path for move state
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float currentLerpTime = 0;

    protected float timer; // For state change waiting 

    public int posX, posY;

    GlobalGameManager manager;

    AudioSource logoffAudio;

    protected virtual void Awake()
    {
        logoffAudio = GameObject.Find("LogoffAudio").GetComponent<AudioSource>();

        manager = GameObject.Find("GlobalGameManager(Clone)").GetComponent<GlobalGameManager>();

        map = GameObject.Find("Map").GetComponent<Map>();

        movementRange = map.getMovementRangeTiles(posX, posY, MOVEMENT_RANGE);

        possiblePaths = new List<Path>();
        currentEffects = new List<OverTimeEffect>();

        state = EnemyState.Idle;
        currentPath = null;
        attacking = false;

        currentHealth = MAX_HEALTH;

        timer = 0;
        
        anim = GetComponent<Animator>();
    }

    public void setCharInfo(characterInfo cInf)
    {
        charInfo = cInf;
        MAX_HEALTH = cInf.getCharacter().maxHP;
        currentHealth = MAX_HEALTH;
        MOVEMENT_RANGE = cInf.getCharacter().MOV;
        ATTACK_DAMAGE = cInf.getAttack().getDamage();
        ATTACK_RANGE = cInf.getAttack().getRange();
        name = cInf.getCharacter().getName();
        defense = cInf.getCharacter().DEF;
        attack = cInf.getCharacter().ATK;
        currentSpecial = cInf.getCharacter().SPC;

        string walkAnimPath = "Textures/Enemies/Controllers/MainControllers/" + cInf.getCharacter().spriteName + "_walk";
        anim.runtimeAnimatorController = Resources.Load(walkAnimPath) as RuntimeAnimatorController;
    }

    public void setRandomCharInfo()
    {
        List<characterCard> enemyCharacters = manager.cardManager.getEnemyCharacterCards();
        List<attackCard> enemyAttacks = manager.cardManager.getEnemyAttackCards();
        List<specialCard> enemySpecial = manager.cardManager.getEnemySpecialCards();
        List<passiveCard> enemyPassives = manager.cardManager.getEnemyPassiveCards();

        characterInfo info = new characterInfo(enemyCharacters[Random.Range(0, enemyCharacters.Count)],
                                               enemyAttacks[Random.Range(0, enemyAttacks.Count)],
                                               enemySpecial[Random.Range(0, enemySpecial.Count)],
                                               enemyPassives[Random.Range(0, enemyPassives.Count)]);
        
        
        setCharInfo(info);
    }

    public EnemyState getState()
    {
        return state;
    }

    public void setState(EnemyState es)
    {
        state = es;
        /*
        if (state == EnemyState.Selected)
        {
            movementRange = map.getMovementRangeTiles(posX, posY, MOVEMENT_RANGE);
            map.highlightTiles(movementRange, map.movementHighlight);
        }
         */
    }

    public void giveOverTimeEffect(OverTimeEffect ote)
    {
        for (int i = 0; i < currentEffects.Count; i++)
        {
            if(ote.equals(currentEffects[i]))
            {
                if (ote.resetAtEnd) //If we were going to reset this effect when the numturns = 0, reset it now
                {
                    switch (ote.statType)
                    {
                        case "health":
                            currentHealth = ote.earlyReset(currentHealth);
                            break;
                        case "attack":
                            attack = ote.earlyReset(attack);
                            break;
                        case "defense":
                            defense = ote.earlyReset(defense);
                            break;
                        case "move":
                            MOVEMENT_RANGE = ote.earlyReset(MOVEMENT_RANGE);
                            break;
                        case "special":
                            currentSpecial = ote.earlyReset(currentSpecial);
                            break;
                    }
                }
                currentEffects[i] = ote; //Overwrite the old effect
                applyEffect(ote, transform.position);
                return;
            }
        }

        currentEffects.Add(ote);
        applyEffect(ote, transform.position);
    }

    public void applyEffect(OverTimeEffect ote, Vector3 position)
    {
        switch (ote.statType)
        {
            case "health":
                currentHealth = ote.getEffectResult(currentHealth, "HP", position);
                if (ote.numTurns == 0) currentEffects.Remove(ote);
                break;
            case "attack":
                attack = ote.getEffectResult(attack, "ATK", position);
                if (ote.numTurns == 0) currentEffects.Remove(ote);
                break;
            case "defense":
                defense = ote.getEffectResult(defense, "DEF", position);
                if (ote.numTurns == 0) currentEffects.Remove(ote);
                break;
            case "move":
                MOVEMENT_RANGE = ote.getEffectResult(MOVEMENT_RANGE, "MOV", position);
                if (ote.numTurns == 0) currentEffects.Remove(ote);
                break;
            case "special":
                currentSpecial = ote.getEffectResult(currentSpecial, "SPC", position);
                if (ote.numTurns == 0) currentEffects.Remove(ote);
                break;
        }

        if (currentHealth <= 0)
        {
            kill();
        }
    }

    public void applyEffects()
    {
        for (int i = 0; i < currentEffects.Count; i++)
        {
            Vector3 positionOffset = new Vector3(transform.position.x, transform.position.y - i, transform.position.z);
            applyEffect(currentEffects[i], positionOffset);
        }
    }

    // Update is called once per frame
    void Update () 
    {
        timer += Time.deltaTime;
        switch (state)
        {
            case EnemyState.Idle: break;
            case EnemyState.Selected: serviceSelectedState(); break;
            case EnemyState.SelectMove: serviceSelectMove(); break;
            case EnemyState.Moving: serviceMoveState(); break;
            case EnemyState.ViewAttackRange: serviceViewAttackRangeState(); break;
            case EnemyState.Attacking: serviceAttackState(); break;
        }

        if (lastDirection != moveDirection)
        {
            switch (moveDirection)
            {
                case EnemyDirection.Down:
                    anim.SetInteger("Direction", 0);
                    break;
                case EnemyDirection.Left:
                    anim.SetInteger("Direction", 1);
                    break;
                case EnemyDirection.Up:
                    anim.SetInteger("Direction", 2);
                    break;
                case EnemyDirection.Right:
                    anim.SetInteger("Direction", 3);
                    break;
            }
            lastDirection = moveDirection;
        }
	}

    protected virtual void serviceSelectedState()
    {
        if (timer > 1f)
        {
            Debug.Log("Enemy: Service Selected State");

            // Highlight movement range (for player reference, really)
            movementRange = map.getMovementRangeTiles(posX, posY, MOVEMENT_RANGE);
            map.highlightTiles(movementRange, map.movementHighlight);

            // Set next state to movement
            state = EnemyState.SelectMove;
            timer = 0;

            meleeRange = map.getMeleeRange(posX, posY, MOVEMENT_RANGE); // Get enemies within melee range
            if (Debug.isDebugBuild) // Highlight them for debug purposes
                map.highlightTiles(meleeRange, map.attackHighlight);

            if (meleeRange.Length > 0)
            {
                List<Tile> moveTiles = new List<Tile>(movementRange);
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
                // No enemies in range, pick a random square to walk to..
                selectedTile = movementRange[(int)Random.Range(0, movementRange.Length)];
            }
        }
    }

    protected virtual void serviceSelectMove()
    {
        if (timer > 1f)
        {
            Debug.Log("Enemy: Service Select Move");
            if (selectedUnitTile != null && Mathf.Abs(selectedUnitTile.x - posX) + Mathf.Abs(selectedUnitTile.y - posY) < 2)
            {
                Debug.Log("Enemy: Adjacent to target");
                state = EnemyState.ViewAttackRange;
                timer = 0;
                selectedTile = null;
                map.clearHighlights(movementRange); // Clear blue tiles
                map.clearHighlights(meleeRange);    // Clear enemies highlighted for demo purposes
                return;
            }

            // Walk to our selected tile..
            currentPath = buildPathToTile(selectedTile.x, selectedTile.y, movementRange);
            setStartAndEnd();
            state = EnemyState.Moving;
            return;
        }
    }


    //Current path should be defined if you're in move state
    private void serviceMoveState()
    {
        if (currentPath != null)
        {
            followCurrentPath();
        }
    }

    public void followCurrentPath()
    {
        currentLerpTime += Time.deltaTime;
        transform.position = Vector3.MoveTowards(startPosition, endPosition, 5.0f * currentLerpTime);
        
        if (transform.position.Equals(endPosition))
        {

            currentPath.incrementPathStep();
            if (currentPath.getPathStep() < currentPath.getNumSteps()) 
            {
                setStartAndEnd();
            } 
            else
            {
                if (attacking)
                {
                    state = EnemyState.ViewAttackRange;
                }
                else
                {
                    state = EnemyState.Idle;
                    attacking = false;
                }
                timer = 0;
                selectedTile = null;
                map.clearHighlights(movementRange); // Clear blue tiles
                map.clearHighlights(meleeRange);    // Clear enemies highlighted for demo purposes
            }
        }
    }

    private void serviceViewAttackRangeState()
    {
        if (timer > 1f)
        {
            Debug.Log("Enemy: Service Attack Range State");

            // Get melee highlights
            meleeRange = map.getRangeTiles(posX, posY, 1); // Melee range of 1
            map.highlightTiles(meleeRange, map.attackHighlight);

            // Transition to attack state
            state = EnemyState.Attacking;
            timer = 0;
        }
    }

    private void serviceAttackState()
    {
        if (timer > 1f)
        {
            Debug.Log("Enemy: Service Attack State");

            // Clear highlights
            map.clearHighlights(meleeRange);

            // Attack Unit on tile
            selectedUnitTile.attackTile(ATTACK_DAMAGE + attack);

            // We're done attacking
            selectedUnitTile = null;
            attacking = false;
            state = EnemyState.Idle;
            timer = 0;
        }
    }

    public void damage(int damageDealt)
    {
        currentHealth -= damageDealt;
        if(currentHealth <= 0)
        {
            kill();
        }
    }

    // Kill this enemy off
    public void kill()
    {
        // This will cause the turn-based system to remove
        //  the character on it's next turn. Chance for a 
        //  revive! Maybe.
        setState(EnemyState.Dead);
        gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
        logoffAudio.PlayOneShot(logoffAudio.clip, 0.75f);
    }

    #region Pathfinding
    //Set the beginning and ending points for one segment of a path
    protected void setStartAndEnd()
    {
        startPosition = transform.position;
        Vector2 currentStep = currentPath.getStep();

        if (currentStep.x == Path.VERTICAL) //Vertical step
        {

            if (currentStep.y > 0)
            {
                moveDirection = EnemyDirection.Up;
            }
            else
            {
                moveDirection = EnemyDirection.Down;
            }

            endPosition = map.getTile(posX, posY + (int)currentStep.y).transform.position;
            //Debug.Log("Subsequent: " + startPosition + " " + endPosition);
            //Debug.Log("Position: " + posY);
            //Debug.Log("CurrentStep.y: " + (int)currentStep.y);
            posY = posY + (int)currentStep.y;
        }
        else if (currentStep.x == Path.HORIZONTAL) //Horizontal step
        {

            if (currentStep.y > 0)
            {
                moveDirection = EnemyDirection.Right;
            }
            else
            {
                moveDirection = EnemyDirection.Left;
            }

            endPosition = map.getTile(posX + (int)currentStep.y, posY).transform.position;
            posX = posX + (int)currentStep.y;
        }
        currentLerpTime = 0;
    }

    public void move(int x, int y)
    {
        posX = x;
        posY = y;
        transform.position = map.getTile(x, y).transform.position;
    }

    // The following assumes no obsticles, and makes a basic path to the tile
    protected Path buildPathToTile(int tileX, int tileY)
    {
        Path path = (Path)ScriptableObject.CreateInstance(typeof(Path));
        int xDiff = tileX - posX;
        int yDiff = tileY - posY;

        path.addStep(Path.HORIZONTAL, xDiff);
        path.addStep(Path.VERTICAL, yDiff);

        return path;
    }

    // The following uses the tileList and takes into account obsticles, avoiding them
    //  as it builds a path to the tile
    protected Path buildPathToTile(int tileX, int tileY, Tile[] tileList)
    {
        Path path = (Path)ScriptableObject.CreateInstance(typeof(Path));
        Vector2 dstPos = new Vector2(tileX, tileY);
        List<Tile> openTiles = new List<Tile>(tileList);
        List<Tile> closedTiles = new List<Tile>();
        Tile throwawayTile = movementRange[0];
        path = pathFind(posX, posY, dstPos, openTiles, closedTiles, path, throwawayTile, 0, 99);

        if (path == null)
        {
            Debug.Log("Null path!");
            return buildPathToTile(tileX, tileY);
        }

        //Debug.Log("Path: " + path.ToString());

        return path;
    }

    // The following is a recursive method to help the tileList-based buildPath method
    private Path pathFind(int tileX, int tileY, Vector2 dstPos, List<Tile> openTiles, List<Tile> closedTiles, Path p, Tile t, int dist, int bestDist)
    {
        if(tileX == dstPos.x && tileY == dstPos.y)
        {
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
        int bestType = 0;
        for(int i = 0; i < bestTypes.Count; i++)
        {
            bestType = bestTypes[i];
            
            p = originalPath.clone();

            switch (bestType)
            {
                case 1:
                    t = map.getTile(tileX - 1, tileY);
                    openTiles.Remove(t);
                    closedTiles.Add(t);
                    p.addStep(Path.HORIZONTAL, -1);
                    p = pathFind(tileX - 1, tileY, dstPos, openTiles, closedTiles, p, t, dist + 1, bestDist);
                    if (p != null)
                    {
                        return p;
                    }
                    break;
                case 2:
                    t = map.getTile(tileX, tileY - 1);
                    openTiles.Remove(t);
                    closedTiles.Add(t);
                    p.addStep(Path.VERTICAL, -1);
                    p = pathFind(tileX, tileY - 1, dstPos, openTiles, closedTiles, p, t, dist + 1, bestDist);
                    if (p != null)
                    {
                        return p;
                    }
                    break;
                case 3:
                    t = map.getTile(tileX + 1, tileY);
                    openTiles.Remove(t);
                    closedTiles.Add(t);
                    p.addStep(Path.HORIZONTAL, 1);
                    p = pathFind(tileX + 1, tileY, dstPos, openTiles, closedTiles, p, t, dist + 1, bestDist);
                    if (p != null)
                    {
                        return p;
                    }
                    break;
                case 4:
                    t = map.getTile(tileX, tileY + 1);
                    openTiles.Remove(t);
                    closedTiles.Add(t);
                    p.addStep(Path.VERTICAL, 1);
                    p = pathFind(tileX, tileY + 1, dstPos, openTiles, closedTiles, p, t, dist + 1, bestDist);
                    if (p != null)
                    {
                        return p;
                    }
                    break;
            }
        }
        return null;
    }
    #endregion
}
