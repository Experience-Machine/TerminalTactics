﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CharacterBehaviour : MonoBehaviour 
{
    public enum CharacterState
    {
        Move,
        Moving,
        Attack,
        Attacking,
        Idle, 
        Selected,
        Special,
        AnimateWait,
        Dead
    };
    private CharacterState state;

    // Animation stuff
    public enum CharacterDirection
    {
        Up = 2,
        Down = 0, 
        Left = 1,
        Right = 3,
        None
    }
    public CharacterDirection moveDirection;
    private CharacterDirection lastDirection;
    private Animator anim;
    public static float yOffset = 0.3f; // Flat value to offset in Y direction by  
    private RuntimeAnimatorController moveControl;
    private RuntimeAnimatorController attackControl;
    private bool animAttackState = false;
    public Sprite charSprite;

    private Map map; // Used for interfacing with tiles
    private characterInfo charInfo;

    // Character Stats stuff
    public int MAX_HEALTH = 3;
    public int MAX_SPECIAL;
    public int currentHealth;
    public int currentSpecial;
    public int defense = 1;
    public int attack = 1;

    private List<OverTimeEffect> currentEffects;

    // Movement stuff
    private Tile[] movementRange;
    Color movementHighlight = new Color(0, 0, 1f, .3f);
    public int MOVEMENT_RANGE = 3;
    public int movementLeft;
    public int oldPosX, oldPosY;
    
    // Pathing stuff
    private List<Path> possiblePaths;
    private Path currentPath; //Current path for move state
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float currentLerpTime = 0;

    private float characterFlashLerp = 0;
    private Color baseColor;

    // Current tile position
    public int posX, posY;

    // Attack stuff
    private Tile[] attackRange;
    private Tile tileToAttack;
    Color attackHighlight = new Color(1f, 0, 0, .3f);
    private int ATTACK_RANGE = 1;
    public int ATTACK_DAMAGE = 1; // attack of the character
    public int ATTACK_DAMAGE_CARD = 1; // attack of the card
    private bool hasAttacked; // I made this private, aren't you proud?

    AudioSource specialAudio;
    AudioSource logoffAudio;

    void Awake()
    {
        specialAudio = GameObject.Find("SpecialAttackAudio").GetComponent<AudioSource>();
        logoffAudio = GameObject.Find("LogoffAudio").GetComponent<AudioSource>();

        map = GameObject.Find("Map").GetComponent<Map>();
        posX = 3;
        posY = 3;

        currentHealth = MAX_HEALTH;

        movementRange = map.getMovementRangeTiles(posX, posY, MOVEMENT_RANGE);
        attackRange = new Tile[0];
        //movementRange = map.getMovementRangeTiles(posX, posY, MOVEMENT_RANGE);
        //map.highlightTiles(movementRange, movementHighlight);

        possiblePaths = new List<Path>();
        currentEffects = new List<OverTimeEffect>();

        setState(CharacterState.Idle);
        currentPath = null;

        anim = GetComponent<Animator>();

        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        baseColor = renderer.color;

        Vector2 newPos = transform.position;
        newPos.y += yOffset;
        transform.position = newPos;

        hasAttacked = false;
    }

    void Start()
    {
        addPassives();
    }
	
    public void setCharInfo(characterInfo cInf)
    {
        charInfo = cInf;
        MAX_HEALTH = cInf.getCharacter().maxHP;
        currentHealth = MAX_HEALTH;
        MOVEMENT_RANGE = cInf.getCharacter().MOV;
        ATTACK_DAMAGE = cInf.getCharacter().ATK; // add character attack
        ATTACK_DAMAGE_CARD = cInf.getAttack().getDamage();
        ATTACK_RANGE = cInf.getAttack().getRange();
        name = cInf.getCharacter().getName();
        defense = cInf.getCharacter().DEF;
        MAX_SPECIAL = cInf.getCharacter().SPC;
        currentSpecial = MAX_SPECIAL;

        Sprite[] sprites = Resources.LoadAll<Sprite>("Textures/Heros/walk_" + cInf.getCharacter().spriteName);
        charSprite = sprites[18];

        string animPath = "Textures/Heros/Animation/" + cInf.getCharacter().spriteName;
        moveControl = Resources.Load(animPath + "_walk") as RuntimeAnimatorController;
        attackControl = Resources.Load(animPath + "_spellcast") as RuntimeAnimatorController;
        anim.runtimeAnimatorController = moveControl;
        
    }

    private void addPassives()
    {
        passiveCard passive = charInfo.psvCard;
        if (passive.statToChange.Equals("health"))
        {
            MAX_HEALTH += passive.statValue;
            currentHealth = MAX_HEALTH;
        }
        else if (passive.statToChange.Equals("attack"))
            ATTACK_DAMAGE += passive.statValue;
        else if (passive.statToChange.Equals("defense"))
            defense += passive.statValue;
        else if (passive.statToChange.Equals("special"))
            currentSpecial += passive.statValue;
        else if (passive.statToChange.Equals("movement"))
            MOVEMENT_RANGE += passive.statValue;
        else
            //Debug.Log("Invalid passive card type");
            ;
        
    }

    public void giveOverTimeEffect(OverTimeEffect ote)
    {
        for (int i = 0; i < currentEffects.Count; i++)
        {
            if (ote.equals(currentEffects[i]))
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
                GameObject buffObj = Object.Instantiate(Resources.Load("Prefabs/DefenseBuff")) as GameObject;
                buffObj.transform.position = transform.position;

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

    public bool hasEnoughSpecial()
    {
        if (currentSpecial >= charInfo.spcCard.cost) return true;
        return false;
    }

    public CharacterState getState()
    {
        return state;
    }

    public void setState(CharacterState cs)
    {
        // Clear highlights
        if (state == CharacterState.Move && movementRange.Length > 0)
        {
            map.clearHighlights(movementRange);
        }
        else if (state == CharacterState.Attack && attackRange.Length > 0)
        {
            map.clearHighlights(attackRange);
        }

        //Get rid of special ui
        if (state == CharacterState.Special)
        {
            GameObject.Destroy(GameObject.Find("SpecialAttackUI(Clone)"));
        }


        if (movementLeft == 0 && cs == CharacterState.Move) return;
        else if (hasAttacked && cs == CharacterState.Attack) return;
        else if (hasAttacked && cs == CharacterState.Special) return;

        state = cs;

        if (state == CharacterState.Move)
        {
            movementRange = map.getMovementRangeTiles(posX, posY, movementLeft);
            map.highlightTiles(movementRange, movementHighlight);
        }
        else if (state == CharacterState.Attack)
        {
            attackRange = map.getRangeTiles(posX, posY, ATTACK_RANGE); // Updated to not attack around walls
            map.highlightTiles(attackRange, attackHighlight);
        } 
        else if (state == CharacterState.Special)
        {
            GameObject specialUi = Instantiate(Resources.Load("Prefabs/SpecialAttackUI")) as GameObject;
            Text[] textComponents = specialUi.GetComponentsInChildren<Text>();
            Text uiText = textComponents[0];
            Text specialDescription = textComponents[1];
            uiText.text = charInfo.getSpecial().nm;
            specialDescription.text = charInfo.getSpecial().lowerDescript + " Cost: " + charInfo.getSpecial().cost;
        }
        else if(state == CharacterState.Idle)
        {
            movementLeft = MOVEMENT_RANGE;
            hasAttacked = false;
            SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
            renderer.color = new Color(1f, 1f, 1f, 1f);
        }

    }

    public bool attackedThisTurn()
    {
        return hasAttacked;
    }

    // Update is called once per frame
    void Update () 
    {
        if (currentSpecial > MAX_SPECIAL)
        {
            currentSpecial = MAX_SPECIAL;
        }

        if (currentHealth > MAX_HEALTH)
        {
            currentHealth = MAX_HEALTH;
        }

        if (MOVEMENT_RANGE > 4)
        {
            MOVEMENT_RANGE = 4;
        }

        switch (state)
        {
            case CharacterState.Idle: break;
            case CharacterState.Dead: serviceDeadState(); break;
            case CharacterState.Selected: break;
            case CharacterState.Attack: serviceAttackState(); break;
            case CharacterState.Attacking: serviceAttackingState();  break;
            case CharacterState.Move: serviceMoveState(); break;
            case CharacterState.Special: serviceSpecialState(); break;
            case CharacterState.Moving: serviceMovingState(); break;
        }

        //Give the currently selected character a flashing effect
        if (state != CharacterState.Idle)
        {
            characterFlashLerp += Time.deltaTime;
            SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
            float colorValue = Mathf.PingPong (characterFlashLerp, 0.75f);
            renderer.color = new Color(colorValue, colorValue, renderer.color.b, renderer.color.a);
        }


        if (lastDirection != moveDirection)
        {
            switch (moveDirection)
            {
                case CharacterDirection.Down:
                    anim.SetInteger("Direction", 0);
                    break;
                case CharacterDirection.Left:
                    anim.SetInteger("Direction", 1);
                    break;
                case CharacterDirection.Up:
                    anim.SetInteger("Direction", 2);
                    break;
                case CharacterDirection.Right:
                    anim.SetInteger("Direction", 3);
                    break;
            }
            lastDirection = moveDirection;
        }

        if(animAttackState && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.IsInTransition(0))
        {
            setState(CharacterState.Selected);
            hasAttacked = true;
            anim.runtimeAnimatorController = moveControl;
            anim.SetInteger("Direction", (int)moveDirection);
            animAttackState = false;
        }

        if(hasAttacked && movementLeft == 0)
        {
            setState(CharacterState.Idle);
        }
	}

    private void serviceMoveState()
    {
        if (map.selectedTile != null)
        {
            for (int i = 0; i < movementRange.Length; i++)
            {
                if (movementRange[i] == map.selectedTile)
                {
                    //Tile tileOn = map.getTile(posX, posY);
                    //tileOn.setCollideable(false);

                    map.clearHighlights(movementRange);
                    //move(map.selectedTile.x, map.selectedTile.y);
                    //movementRange = map.getMovementRangeTiles(posX, posY, MOVEMENT_RANGE);
                    //map.highlightTiles(movementRange, movementHighlight);
                    oldPosX = posX;
                    oldPosY = posY;

                    currentPath = buildPathToTile(map.selectedTile.x, map.selectedTile.y, movementRange); 
                    setStartAndEnd();

                    setState(CharacterState.Moving);
                }
            }
        }
    }

    //Current path should be defined if you're in moving state
    private void serviceMovingState()
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
                //Debug.Log("Current path step: " + currentPath.getStep().ToString());
                //Debug.Log("Subsequent: " + startPosition + " " + endPosition);
            } 
            else
            {                
                //Debug.Log(posX + " " + posY);
                setState(CharacterState.Selected);
                //movedThisTurn = true;
                movementLeft -= Mathf.Abs((oldPosX - map.selectedTile.x));
                movementLeft -= Mathf.Abs((oldPosY - map.selectedTile.y));
                map.selectedTile = null;
                map.clearHighlights(movementRange);
                //movementRange = map.getMovementRangeTiles(posX, posY, MOVEMENT_RANGE);
                //map.highlightTiles(movementRange, movementHighlight);
            }
        }
    }

    private void serviceAttackState()
    {
        if (map.selectedTile != null)
        {
            //Make sure we can't attack another player
            if (map.selectedTile.charOnTile != null)
            {
                map.selectedTile = null;
                return;
            }

            //Don't attack empty tiles
            if (map.selectedTile.enemyOnTile == null)
            {
                map.selectedTile = null;
                return;
            }

            for (int i = 0; i < attackRange.Length; i++)
            {
                // If the selected tile is in our attack range:
                if (attackRange[i] == map.selectedTile)
                {
                    // This does not prevent us from attacking an empty tile
                    tileToAttack = map.selectedTile;
                    map.clearHighlights(attackRange);
                    setState(CharacterState.Attacking);

                    anim.runtimeAnimatorController = attackControl;
                    animAttackState = true;

                    float angle = Mathf.Atan2(posY - map.selectedTile.y, posX - map.selectedTile.x) * (180 / Mathf.PI); // In degrees

                    // Find direction based on attack tile position
                    if(angle >= -45 && angle < 45)
                    {
                        // Direction left
                        moveDirection = CharacterDirection.Left;
                    }
                    else if(angle >= 45 && angle < 135)
                    {
                        // Direction down
                        moveDirection = CharacterDirection.Down;
                    }
                    else if(angle >= 135 || angle < -135)
                    {
                        // Direction right
                        moveDirection = CharacterDirection.Right;
                    }
                    else if (angle < -45 && angle >= -135)
                    {
                        // Direction up
                        moveDirection = CharacterDirection.Up;
                    }

                    lastDirection = CharacterDirection.None;
                }
            }
            
        }
    }

    private void serviceAttackingState()
    {
        // Do any attack animations here
        map.clearHighlights(attackRange);
        tileToAttack.setCard(ATTACK_DAMAGE_CARD);
        tileToAttack.attackTile(ATTACK_DAMAGE);
        tileToAttack = null;
        setState(CharacterState.AnimateWait); // Idle after animation is over..
        //hasAttacked = true;
    }

    public void damage(int damageDealt)
    {
        currentHealth -= damageDealt;
        if(currentHealth <= 0)
        {
            kill();
        }
    }

    public void heal(int healingDealt)
    {
        currentHealth += healingDealt;
        if (currentHealth >= MAX_HEALTH)
        {
            currentHealth = MAX_HEALTH;
        }
    }


    // Kill this character off
    public void kill()
    {
        // This will cause the turn-based system to remove
        //  the character on it's next turn. Chance for a 
        //  revive! Maybe.
        setState(CharacterState.Dead);
        gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
        logoffAudio.PlayOneShot(logoffAudio.clip, 0.75f);
    }

    public void serviceSpecialState()
    {
        charInfo.spcCard.specialAttack(map, charInfo, this);
        if (state == CharacterState.AnimateWait) //Special attack is over
        {
            anim.runtimeAnimatorController = attackControl;
            animAttackState = true;
            specialAudio.PlayOneShot(specialAudio.clip, 0.75f);
        }
        
    }

    float lerpTime = 0;

    public void serviceDeadState()
    {
        lerpTime += Time.deltaTime;
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        float alphaValue = Mathf.Lerp(1f, 0f, lerpTime);
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alphaValue);


        if (alphaValue <= 0f)
        {
            Tile currentTile = map.getTile(posX, posY);
            currentTile.charOnTile = null;
            currentTile.collideable = false;
            Destroy(this);
        }
    }

    #region Pathfinding and movement
    //Set the beginning and ending points for one segment of a path
    private void setStartAndEnd()
    {
        startPosition = transform.position;
        Vector2 currentStep = currentPath.getStep();

        if (currentStep.x == Path.VERTICAL) //Vertical step
        {
            if(currentStep.y > 0)
            {
                moveDirection = CharacterDirection.Up;
            }
            else
            {
                moveDirection = CharacterDirection.Down;
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
                moveDirection = CharacterDirection.Right;
            }
            else
            {
                moveDirection = CharacterDirection.Left;
            }
            endPosition = map.getTile(posX + (int)currentStep.y, posY).transform.position;
            posX = posX + (int)currentStep.y;
        }
        endPosition.y += yOffset;
        currentLerpTime = 0;
    }

    public void move(int x, int y)
    {
        posX = x;
        posY = y;
        Vector2 newPos = map.getTile(x, y).transform.position;
        newPos.y += yOffset;
        transform.position = newPos;
    }

    private Path buildPathToTile(int tileX, int tileY)
    {
        Path path = (Path)ScriptableObject.CreateInstance(typeof(Path));
        int xDiff = tileX - posX;
        int yDiff = tileY - posY;

        path.addStep(Path.HORIZONTAL, xDiff);
        path.addStep(Path.VERTICAL, yDiff);

        return path;
    }

    private Path buildPathToTile(int tileX, int tileY, Tile[] tileList)
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
                    p = pathFind(tileX - 1, tileY, dstPos, openTiles, closedTiles, p, t, dist++, bestDist);
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
                    p = pathFind(tileX, tileY - 1, dstPos, openTiles, closedTiles, p, t, dist++, bestDist);
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
                    p = pathFind(tileX + 1, tileY, dstPos, openTiles, closedTiles, p, t, dist++, bestDist);
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
                    p = pathFind(tileX, tileY + 1, dstPos, openTiles, closedTiles, p, t, dist++, bestDist);
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
