using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;



public class LevelScript : MonoBehaviour
{
    protected enum LevelState
    {
        PlayerTurn,
        EnemyTurn,
        Move, 
        Idle
    }
    LevelState state;

    // Map stuff
    public Map map;
    private GameObject mapObject; // Actual map object

    // Unit behaviours
    private List<CharacterBehaviour> characters;
    private List<EnemyBehaviour> enemies;

    // Turn counter for player and enemy
    private int currentPlayer;
    private int currentEnemy;

    // Actual GameObjects for char and enemy (temp)
    private GameObject characterObject;
    private GameObject enemyObject;
    

    // Reusable tile-range
    private Tile[] tileRange;

    // Combat menu stuff
    public GameObject charUI = null;
    public GameObject charUIInstance = null;
    public GameObject enemyUI = null;
    public GameObject enemyUIInstance = null;
    public float maxHealth; // Used for Combat Menu
    public float curHealth; // 

    UIBehavior.ButtonClicked lastClicked;

    Vector3 cameraStartPosition;
    Vector3 cameraEndPosition;
    float cameraMoveTime = 0;
    bool cameraMoving = false;

    bool setColor;
    GlobalGameManager manager;

    // Use this for initialization
    void Awake()
    {
        manager = GameObject.Find("GlobalGameManager(Clone)").GetComponent<GlobalGameManager>();
        mapObject = new GameObject("Map");
        map = mapObject.AddComponent<Map>();
        setColor = true;

        characters = new List<CharacterBehaviour>();
        enemies = new List<EnemyBehaviour>();

        if (characterObject == null)
        {
            characterObject = Resources.Load("Prefabs/Character") as GameObject;
        }

        if (enemyObject == null)
        {
            enemyObject = Resources.Load("Prefabs/Enemy") as GameObject;
        }

        if (charUI == null)
        {
            charUI = Resources.Load("Prefabs/CharacterUI") as GameObject;
        }

        if (enemyUI == null)
        {
            enemyUI = Resources.Load("Prefabs/EnemyUI") as GameObject;
        }

        loadMap();

        // Initilize enemy and player turns
        currentPlayer = 0;
        currentEnemy = 0;
        
        // Set initial enemy/player colision values on the map
        //resetCollision();

        // Set our player's first character to go first
        //characters[currentPlayer].setState(CharacterBehaviour.CharacterState.Selected);
        moveCamera(characters[currentPlayer].transform.position);

        // Set the state of the level to be the player's turn
        state = LevelState.EnemyTurn;

        // Initialize combat UI

        //charUIInstance = Instantiate(charUI) as GameObject;
        //UIBehavior script = charUIInstance.GetComponent<UIBehavior>();
        //script.setContent(characters[currentPlayer].GetComponent<SpriteRenderer>().sprite, characters[currentPlayer].MAX_HEALTH, characters[currentPlayer].currentHealth, characters[currentPlayer].MAX_SPECIAL, characters[currentPlayer].currentSpecial, characters[currentPlayer].name);
    }

    public void Start()
    {
        //charUIInstance = Instantiate(charUI) as GameObject;
        //UIBehavior script = charUIInstance.GetComponent<UIBehavior>();
        //script.setContent(characters[currentPlayer].GetComponent<SpriteRenderer>().sprite, characters[currentPlayer].MAX_HEALTH, characters[currentPlayer].currentHealth, characters[currentPlayer].MAX_SPECIAL, characters[currentPlayer].currentSpecial, characters[currentPlayer].name);

    }

    public void loadMap()
    {
        int characterNumber = 0;

        SpriteRenderer spriteRenderer = GameObject.Find("MapImage").GetComponent<SpriteRenderer>();
        Sprite sprite = spriteRenderer.sprite;
        Color[] pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                         (int)sprite.textureRect.y,
                                         (int)sprite.textureRect.width,
                                         (int)sprite.textureRect.height);


        //Debug.Log("WIDTH:" + (int)sprite.textureRect.width);
        //Debug.Log("HEIGHT:" + (int)sprite.textureRect.height);
        for (int x = 0; x < (int)sprite.textureRect.width; x++)
        {
            for (int y = 0; y < (int)sprite.textureRect.height; y++)
            {
                Color pixelColor = pixels[x + y * (int)sprite.textureRect.width];

                if (pixelColor.Equals(Color.green))
                {
                    //Debug.Log("Pixel at (" + x + ", " + y + ") is green");
                    Tile t = map.getTile(x, y);
                    t.setCollideable(true);
                    t.defaultColor = Color.green;
                    map.setTileColor(t, Color.green);
                }

                else if (pixelColor.Equals(Color.blue))
                {
                    CharacterBehaviour cb = (Instantiate(characterObject) as GameObject).GetComponent<CharacterBehaviour>();
                    if (manager.characterInfos != null) // Ensure we're running from main menu
                        cb.setCharInfo(manager.characterInfos[characterNumber]);
                    cb.move(x, y);
                    characters.Add(cb);
                    characterNumber++;
                }
                else if (pixelColor.Equals(Color.magenta)) // Horse
                {
                    EnemyBehaviourBerserk eb = (Instantiate(Resources.Load("Prefabs/Enemy 1")) as GameObject).GetComponent<EnemyBehaviourBerserk>();
                    eb.setEnemyCharInfo(1);

                    eb.rushUnit(characters[0].posX, characters[0].posY);
                    eb.move(x, y);
                    enemies.Add(eb);
                }
                else if (pixelColor.Equals(Color.red)) // Worm
                {
                    EnemyBehaviourBerserk eb = (Instantiate(Resources.Load("Prefabs/Enemy 1")) as GameObject).GetComponent<EnemyBehaviourBerserk>();
                    eb.setEnemyCharInfo(0);

                    eb.rushUnit(characters[0].posX, characters[0].posY);
                    eb.move(x, y);
                    enemies.Add(eb);
                }
                else if (pixelColor.Equals(Color.cyan)) // Bug
                {
                    EnemyBehaviourBerserk eb = (Instantiate(Resources.Load("Prefabs/Enemy 1")) as GameObject).GetComponent<EnemyBehaviourBerserk>();
                    eb.setEnemyCharInfo(2);

                    eb.rushUnit(characters[0].posX, characters[0].posY);
                    eb.move(x, y);
                    enemies.Add(eb);
                }
                else if (pixelColor.Equals(new Color(1, 1, 0))) // BloatWare
                {
                    EnemyBehaviourBerserk eb = (Instantiate(Resources.Load("Prefabs/Enemy 1")) as GameObject).GetComponent<EnemyBehaviourBerserk>();
                    eb.setEnemyCharInfo(3);

                    eb.rushUnit(characters[0].posX, characters[0].posY);
                    eb.move(x, y);
                    enemies.Add(eb);
                }
                else if (pixelColor.Equals(Color.white)) // RansomWare
                {
                    EnemyBehaviourBerserk eb = (Instantiate(Resources.Load("Prefabs/Enemy 1")) as GameObject).GetComponent<EnemyBehaviourBerserk>();
                    eb.setEnemyCharInfo(4);

                    eb.rushUnit(characters[0].posX, characters[0].posY);
                    eb.move(x, y);
                    enemies.Add(eb);
                }

                

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            GameObject exitUI = Instantiate(Resources.Load("Prefabs/ExitUI") as GameObject) as GameObject;
        }

        if (characters[currentPlayer].attackedThisTurn() && charUIInstance != null && UIBehavior.attackButtonsGrey == false)
        {
            UIBehavior script = charUIInstance.GetComponent<UIBehavior>();
            script.greyOutAttackButtons();
        }

        if (characters[currentPlayer].movementLeft == 0 && charUIInstance != null && UIBehavior.moveButtonGrey == false)
        {
            UIBehavior script = charUIInstance.GetComponent<UIBehavior>();
            script.greyOutMoveButton();
        }
        //Cheat code: Remove enemy to progress to next level 
        if (Input.GetKeyDown("space"))
            enemies.RemoveRange(0, 1);
        

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].getState() == EnemyBehaviour.EnemyState.Dead)
            {
                enemies.Remove(enemies[i]);
            }
        }

        if (enemies.Count <= 0)
        {
            manager.level++;
            SceneManager.LoadScene("VictoryScene");
        }

        if (characters.Count <= 0)
        {
            SceneManager.LoadScene("GameMainMenu");
        }

        // Check if the user has clicked a button on the Combat Menu
        if (lastClicked != UIBehavior.lastClicked)
        {
            lastClicked = UIBehavior.lastClicked;
            if(state == LevelState.PlayerTurn)
            {
                if (lastClicked == UIBehavior.ButtonClicked.None)
                {
                    map.clearAllHighlights();
                    characters[currentPlayer].setState(CharacterBehaviour.CharacterState.Selected);
                }
                else if (lastClicked == UIBehavior.ButtonClicked.Move)
                {
                    if (characters[currentPlayer].movementLeft > 0) 
                    { 
                        map.clearAllHighlights();
                        resetCollision();
                        characters[currentPlayer].setState(CharacterBehaviour.CharacterState.Move);
                    }
                }
                else if (lastClicked == UIBehavior.ButtonClicked.Attack)
                {
                    map.clearAllHighlights();
                    characters[currentPlayer].setState(CharacterBehaviour.CharacterState.Attack);

                }
                else if(lastClicked == UIBehavior.ButtonClicked.Wait)
                {
                    map.clearAllHighlights();
                    characters[currentPlayer].setState(CharacterBehaviour.CharacterState.Idle);
                } 
                else if (lastClicked == UIBehavior.ButtonClicked.Special)
                {
                    
                    map.clearAllHighlights();
                    map.selectedTile = null;
                    if (characters[currentPlayer].hasEnoughSpecial())
                        characters[currentPlayer].setState(CharacterBehaviour.CharacterState.Special);             
                }
            }
        }
        else
        {
            UIBehavior.lastClicked = UIBehavior.ButtonClicked.None;
            lastClicked = UIBehavior.ButtonClicked.None;
        }

        // Handle the Game State
        switch (state)
        {
            case LevelState.PlayerTurn: servicePlayerTurn(); break;
            case LevelState.EnemyTurn: serviceEnemyTurn(); break;
            case LevelState.Move: serviceMoveState(); break;
        }

        if (cameraMoving) 
        {
            cameraIncrementMove();
        }

        Camera.main.transform.rotation = Quaternion.identity;
    }

    void resetCollision()
    {
        //map.clearAllHighlights();

        map.selectedTile = null;

        for (int i = 0; i < map.getMapSizeX(); i++)
        {
            for(int j = 0; j < map.getMapSizeY(); j++)
            {
                Tile t = map.getTile(i, j);
                if (!(t.charOnTile == null && t.enemyOnTile == null && t.isCollideable()))
                {
                    t.setCollideable(false);
                    t.charOnTile = null;
                    t.enemyOnTile = null;
                    t.hasUnit = false;
                }
            }
        }
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].getState() != CharacterBehaviour.CharacterState.Dead)
            {
                Tile tileOn = map.getTile(characters[i].posX, characters[i].posY);
                tileOn.setCollideable(true);
                tileOn.hasUnit = true;
                tileOn.charOnTile = characters[i];
            } else
            {
                characters.Remove(characters[i]);
            }
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].getState() != EnemyBehaviour.EnemyState.Dead)
            {
                Tile tileOn = map.getTile(enemies[i].posX, enemies[i].posY);
                tileOn.setCollideable(true);
                tileOn.enemyOnTile = enemies[i];
            } else
            {
                enemies.Remove(enemies[i]);
            }
        }
       
    }
    void servicePlayerTurn()
    {


        // Wait until the selected player's turn is over
        if (characters[currentPlayer].getState() == CharacterBehaviour.CharacterState.Idle)
        {
            UIBehavior.resetButtonColors();
            // Clean the map
            map.clearAllHighlights();
            resetCollision();

            characters[currentPlayer].applyEffects();

            // Itterate who gets to go on the next player turn
            currentPlayer++;

            // Remove dead characters
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i].getState() == CharacterBehaviour.CharacterState.Dead)
                {
                    characters.Remove(characters[i]);
                }
            }

            // Remove dead enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].getState() == EnemyBehaviour.EnemyState.Dead)
                {
                    enemies.Remove(enemies[i]);
                }
            }

            if (enemies.Count <= 0)
            {
                manager.level++;
                SceneManager.LoadScene("VictoryScene");
            } 

            if (characters.Count <= 0)
            {
                SceneManager.LoadScene("GameMainMenu");
            }

            // Catch carry-over currentPlayer / currentEnemy
            if (currentPlayer >= characters.Count) currentPlayer = 0;
            if (currentEnemy >= enemies.Count) currentEnemy = 0;

            if (characters.Count == 0 || enemies.Count == 0)
            {
                 state = LevelState.Idle;
            }
            else
            {
                if (enemies[currentEnemy].getState() == EnemyBehaviour.EnemyState.Dead)
                {
                    enemies.Remove(enemies[currentEnemy]);
                    return;
                }

                //Destroy Combat UI
                Destroy(charUIInstance);
                charUIInstance = null;

                // Create Enemy UI
                enemyUIInstance = Instantiate(enemyUI) as GameObject;
                EnemyUIBehavior script = enemyUIInstance.GetComponent<EnemyUIBehavior>();
                script.setContent(enemies[currentEnemy].GetComponent<SpriteRenderer>().sprite, enemies[currentEnemy].MAX_HEALTH, enemies[currentEnemy].currentHealth, enemies[currentEnemy].name);

                //Set enemy turn 
                state = LevelState.EnemyTurn;
                enemies[currentEnemy].setState(EnemyBehaviour.EnemyState.Selected);

                if (typeof(EnemyBehaviourBerserk).IsAssignableFrom(enemies[currentEnemy].GetType()))
                {
                    EnemyBehaviourBerserk ebb = (EnemyBehaviourBerserk)enemies[currentEnemy];
                    ebb.rushUnit(characters[0].posX, characters[0].posY);
                }
                //moveCamera(enemies[currentEnemy].transform.position);
                cameraStartPosition = Camera.main.transform.position;
                cameraEndPosition = enemies[currentEnemy].transform.position;
                cameraMoving = true;
                cameraMoveTime = 0;

               
            }
        }
    }

    void serviceEnemyTurn()
    {
        // Wait until current enemy's turn is over
        if (enemies[currentEnemy].getState() == EnemyBehaviour.EnemyState.Idle)
        {
            // Clean the map
            map.clearAllHighlights();
            resetCollision();

            enemies[currentEnemy].applyEffects();

            // Itterate who gets to go on the next enemy turn
            currentEnemy++;

            // Remove dead characters
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i].getState() == CharacterBehaviour.CharacterState.Dead)
                {
                    characters.Remove(characters[i]);
                }
            }

            // Remove dead enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].getState() == EnemyBehaviour.EnemyState.Dead)
                {
                    enemies.Remove(enemies[i]);
                }
            }

            if (enemies.Count <= 0)
            {
                manager.level++;
                SceneManager.LoadScene("VictoryScene");
            }

            if (characters.Count <= 0)
            {
                SceneManager.LoadScene("GameMainMenu");
            }
            // Catch carry-over currentPlayer / currentEnemy
            if (currentPlayer >= characters.Count) currentPlayer = 0;
            if (currentEnemy >= enemies.Count) currentEnemy = 0;

            if (characters.Count == 0 || enemies.Count == 0)
            {
                state = LevelState.Idle;
            }
            else
            {
                if (characters[currentPlayer].getState() == CharacterBehaviour.CharacterState.Dead)
                {
                    characters.Remove(characters[currentPlayer]);
                    return;
                }


                // Destroy Enemy UI
                Destroy(enemyUIInstance);
                enemyUIInstance = null;

                // Create Combat UI
                charUIInstance = Instantiate(charUI) as GameObject;
                UIBehavior script = charUIInstance.GetComponent<UIBehavior>();
                script.setContent(characters[currentPlayer].charSprite, characters[currentPlayer].MAX_HEALTH, characters[currentPlayer].currentHealth, characters[currentPlayer].MAX_SPECIAL, characters[currentPlayer].currentSpecial, characters[currentPlayer].name);

                // Set player turn
                state = LevelState.PlayerTurn;
                characters[currentPlayer].setState(CharacterBehaviour.CharacterState.Selected);
                //moveCamera(characters[currentPlayer].transform.position);
                //characters[currentPlayer].movedThisTurn = false;

                cameraStartPosition = Camera.main.transform.position;
                cameraEndPosition = characters[currentPlayer].transform.position;
                cameraMoving = true;
                cameraMoveTime = 0;

            }

        }
    }

    void serviceMoveState()
    {

    }

    void moveCamera(Vector3 position)
    {
        Vector3 newPos = new Vector3(position.x, position.y, Camera.main.transform.position.z);
        Camera.main.transform.position = newPos;
    }

    void cameraIncrementMove()
    {

        if (Camera.main.transform.position.x == cameraEndPosition.x && Camera.main.transform.position.y == cameraEndPosition.y)
        {
            cameraMoving = false;
            cameraMoveTime = 0;
            return;

        }
        cameraMoveTime += Time.deltaTime;
        Vector3 position = Vector3.MoveTowards(cameraStartPosition, cameraEndPosition, 20.0f * cameraMoveTime);
        Camera.main.transform.position = new Vector3(position.x, position.y, Camera.main.transform.position.z);
        //Debug.Log("i am here");
    }
}
