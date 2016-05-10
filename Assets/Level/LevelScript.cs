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

    public Map map;
    private GameObject mapObject; // This is for storage

    private List<CharacterBehaviour> characters;
    private List<EnemyBehaviour> enemies;

    private int currentPlayer;
    private int currentEnemy;

    private GameObject characterObject;
    private GameObject enemyObject;
    private Tile[] tileRange;

    public GameObject charUI = null;
    public GameObject charUIInstance = null;
    public float maxHealth; // Used for Combat Menu
    public float curHealth; // 
    UIBehavior.ButtonClicked lastClicked;  

    bool setColor;

    // Use this for initialization
    void Start()
    {
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

        //Randomly generate some collideable tiles
        for (int i = 0; i < 10; i++)
        {
            float randomX = Random.Range(0f, 10f);
            float randomY = Random.Range(0f, 10f);

            Tile t = map.getTile((int)randomX, (int)randomY);

            t.setCollideable(true);
            t.defaultColor = Color.green;
            map.setTileColor(t, Color.green);
        }

        //Todo: Functionalize
        //This block adds 5 characters and 5 enemies and makes sure that they don't overlap with other units/collideable terrain
        for (int i = 0; i < 3; i++)
        {
            CharacterBehaviour cb = (Instantiate(characterObject) as GameObject).GetComponent<CharacterBehaviour>();
            bool foundTile = false;
            while (!foundTile)
            {
                float randomX = Random.Range(0f, 10f);
                float randomY = Random.Range(0f, 10f);

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
            EnemyBehaviour eb = (Instantiate(enemyObject) as GameObject).GetComponent<EnemyBehaviour>();
            bool foundTile = false;
            //Make sure that we don't spawn enemies on the same location as characters or collideable tiles
            while (!foundTile)
            {
                float randomX = Random.Range(0f, 10f);
                float randomY = Random.Range(0f, 10f);

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

        currentPlayer = 0;
        currentEnemy = 0;

        maxHealth = 100f;
        curHealth = 50f;
        
        resetCollision();
        characters[0].setState(CharacterBehaviour.CharacterState.Selected);
        state = LevelState.PlayerTurn;
        charUIInstance = Instantiate(charUI) as GameObject;
        UIBehavior script = charUIInstance.GetComponent<UIBehavior>();
        script.setContent(characters[0].GetComponent<SpriteRenderer>().sprite, characters[0].MAX_HEALTH, characters[0].currentHealth, "Sample Character " + currentPlayer);
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
                    characters[currentPlayer].setState(CharacterBehaviour.CharacterState.Selected);
                }
                else if (lastClicked == UIBehavior.ButtonClicked.Move)
                {
                    characters[currentPlayer].setState(CharacterBehaviour.CharacterState.Move);
                }
                else if (lastClicked == UIBehavior.ButtonClicked.Attack)
                {
                    characters[currentPlayer].setState(CharacterBehaviour.CharacterState.Attack);
                }
                else if(lastClicked == UIBehavior.ButtonClicked.Wait)
                {
                    characters[currentPlayer].setState(CharacterBehaviour.CharacterState.Idle);
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

            // Catch carry-over currentPlayer / currentEnemy
            if (currentPlayer >= characters.Count) currentPlayer = 0;
            if (currentEnemy >= enemies.Count) currentEnemy = 0;

            if (characters.Count == 0 || enemies.Count == 0)
            {
                 state = LevelState.Idle;
            }
            else
            {
                // Destroy Combat UI
                Destroy(charUIInstance);
                charUIInstance = null;
                
                //Set enemy turn 
                state = LevelState.EnemyTurn;
                enemies[currentEnemy].setState(EnemyBehaviour.EnemyState.Selected);
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

            // Catch carry-over currentPlayer / currentEnemy
            if (currentPlayer >= characters.Count) currentPlayer = 0;
            if (currentEnemy >= enemies.Count) currentEnemy = 0;

            if (characters.Count == 0 || enemies.Count == 0)
            {
                state = LevelState.Idle;
            }
            else
            {
                // Create Combat UI
                charUIInstance = Instantiate(charUI) as GameObject;
                UIBehavior script = charUIInstance.GetComponent<UIBehavior>();
                script.setContent(characters[currentPlayer].GetComponent<SpriteRenderer>().sprite, characters[currentPlayer].MAX_HEALTH, characters[currentPlayer].currentHealth, "Sample Character " + currentPlayer);

                // Set player turn
                state = LevelState.PlayerTurn;
                characters[currentPlayer].setState(CharacterBehaviour.CharacterState.Selected);
            }

            
        }
    }

    void serviceMoveState()
    {

    }
}
