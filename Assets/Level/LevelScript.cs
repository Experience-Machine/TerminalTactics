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

        //Randomly generate some collideable tiles
        for (int i = 0; i < 25; i++)
        {
            float randomX = Random.Range(0f, 27f);
            float randomY = Random.Range(0f, 18f);

            Tile t = map.getTile((int)randomX, (int)randomY);

            t.setCollideable(true);
            t.defaultColor = Color.green;
            map.setTileColor(t, Color.green);
        }

        //Todo: Functionalize
        //This block adds 3 characters and 5 enemies and makes sure that they don't overlap with other units/collideable terrain
        for (int i = 0; i < 3; i++)
        {
            CharacterBehaviour cb = (Instantiate(characterObject) as GameObject).GetComponent<CharacterBehaviour>();
            
            if(manager.characterInfos != null) // Ensure we're running from main menu
                cb.setCharInfo(manager.characterInfos[i]);

            bool foundTile = false;
            while (!foundTile)
            {
                float randomX = Random.Range(5f, 15f);
                float randomY = Random.Range(5f, 15f);

                Tile t = map.getTile((int)randomX, (int)randomY);
                if (t.isCollideable()) continue;

                bool samePosAsChar = false;
                for (int j = 0; j < characters.Count; j++)
                {
                    CharacterBehaviour character = characters[j];
                    if (t.transform.position == character.transform.position) samePosAsChar = true;
                }

                if (samePosAsChar) continue;

                foundTile = true;
                cb.move((int)randomX, (int)randomY);
                characters.Add(cb);
            }
        }

        for (int i = 0; i < 5; i++)
        {
            EnemyBehaviourBerserk eb = (Instantiate(Resources.Load("Prefabs/Enemy 1")) as GameObject).GetComponent<EnemyBehaviourBerserk>();
            eb.setRandomCharInfo();

            eb.rushUnit(characters[0].posX, characters[0].posY);

            bool foundTile = false;
            //Make sure that we don't spawn enemies on the same location as characters or collideable tiles
            while (!foundTile)
            {
                float randomX = Random.Range(5f, 15f);
                float randomY = Random.Range(5f, 15f);

                Tile t = map.getTile((int)randomX, (int)randomY);
                if (t.isCollideable()) continue;

                bool samePosAsChar = false;
                for (int j = 0; j < characters.Count; j++)
                {
                    CharacterBehaviour character = characters[j];
                    if (t.transform.position == character.transform.position) samePosAsChar = true;
                }

                if (samePosAsChar) continue;

                bool samePosAsEnemy = false;
                for (int j = 0; j < enemies.Count; j++)
                {
                    EnemyBehaviour enemy = enemies[j];
                    if (t.transform.position == enemy.transform.position) samePosAsEnemy = true;
                }

                if (samePosAsEnemy) continue;

                foundTile = true;
                eb.move((int)randomX, (int)randomY);
                enemies.Add(eb);
            }
        }

        // Initilize enemy and player turns
        currentPlayer = 0;
        currentEnemy = 0;
        
        // Set initial enemy/player colision values on the map
        resetCollision();

        // Set our player's first character to go first
        characters[currentPlayer].setState(CharacterBehaviour.CharacterState.Selected);
        moveCamera(characters[currentPlayer].transform.position);

        // Set the state of the level to be the player's turn
        state = LevelState.PlayerTurn;

        // Initialize combat UI
        charUIInstance = Instantiate(charUI) as GameObject;
        UIBehavior script = charUIInstance.GetComponent<UIBehavior>();
        script.setContent(characters[currentPlayer].GetComponent<SpriteRenderer>().sprite, characters[currentPlayer].MAX_HEALTH, characters[currentPlayer].currentHealth, characters[currentPlayer].MAX_SPECIAL, characters[currentPlayer].currentSpecial, characters[currentPlayer].name);
    
    }

    // Update is called once per frame
    void Update()
    {
        

        // Check if the user has clicked a button on the Combat Menu
        if(lastClicked != UIBehavior.lastClicked)
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
                    map.clearAllHighlights();
                    characters[currentPlayer].setState(CharacterBehaviour.CharacterState.Move);
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
                } else if (lastClicked == UIBehavior.ButtonClicked.Special)
                {
                    map.clearAllHighlights();
                    map.selectedTile = null;
                    if (characters[currentPlayer].hasEnoughSpecial())
                        characters[currentPlayer].setState(CharacterBehaviour.CharacterState.Special);             
                }
            }
            else
            {
                UIBehavior.lastClicked = UIBehavior.ButtonClicked.None;
                lastClicked = UIBehavior.ButtonClicked.None;
            }
        }

        // Handle the Game State
        switch (state)
        {
            case LevelState.PlayerTurn: servicePlayerTurn(); break;
            case LevelState.EnemyTurn: serviceEnemyTurn(); break;
            case LevelState.Move: serviceMoveState(); break;
        }

        if (cameraMoving) {
            cameraIncrementMove();
        }

        Camera.main.transform.rotation = Quaternion.identity;
    }

    void resetCollision()
    {
        //map.clearAllHighlights();

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
            }
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].getState() != EnemyBehaviour.EnemyState.Dead)
            {
                Tile tileOn = map.getTile(enemies[i].posX, enemies[i].posY);
                tileOn.setCollideable(true);
                tileOn.enemyOnTile = enemies[i];
            }
        }

    }
    void servicePlayerTurn()
    {
        // Wait until the selected player's turn is over
        if (characters[currentPlayer].getState() == CharacterBehaviour.CharacterState.Idle)
        {
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

            if (enemies.Count <= 0 || characters.Count <= 0)
                SceneManager.LoadScene("GameMainMenu");


            // Catch carry-over currentPlayer / currentEnemy
            if (currentPlayer >= characters.Count) currentPlayer = 0;
            if (currentEnemy >= enemies.Count) currentEnemy = 0;

            if (characters.Count == 0 || enemies.Count == 0)
            {
                 state = LevelState.Idle;
            }
            else
            {
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

            if (enemies.Count <= 0 || characters.Count <= 0)
                SceneManager.LoadScene("GameMainMenu");

            // Catch carry-over currentPlayer / currentEnemy
            if (currentPlayer >= characters.Count) currentPlayer = 0;
            if (currentEnemy >= enemies.Count) currentEnemy = 0;

            if (characters.Count == 0 || enemies.Count == 0)
            {
                state = LevelState.Idle;
            }
            else
            {
                // Destroy Enemy UI
                Destroy(enemyUIInstance);
                enemyUIInstance = null;

                // Create Combat UI
                charUIInstance = Instantiate(charUI) as GameObject;
                UIBehavior script = charUIInstance.GetComponent<UIBehavior>();
                script.setContent(characters[currentPlayer].GetComponent<SpriteRenderer>().sprite, characters[currentPlayer].MAX_HEALTH, characters[currentPlayer].currentHealth, characters[currentPlayer].MAX_SPECIAL, characters[currentPlayer].currentSpecial, characters[currentPlayer].name);

                // Set player turn
                state = LevelState.PlayerTurn;
                characters[currentPlayer].setState(CharacterBehaviour.CharacterState.Selected);
                //moveCamera(characters[currentPlayer].transform.position);

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
