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
        for (int i = 0; i < 5; i++)
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

        
        resetCollision();
        characters[0].setState(CharacterBehaviour.CharacterState.Selected);
        state = LevelState.PlayerTurn;
    }

    // Update is called once per frame
    void Update()
    {
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
            Tile tileOn = map.getTile(enemies[i].posX, enemies[i].posY);
            tileOn.setCollideable(true);
            tileOn.enemyOnTile = enemies[i];
        }
    }
    void servicePlayerTurn()
    {
        if (characters[currentPlayer].getState() == CharacterBehaviour.CharacterState.Idle)
        {
            map.clearAllHighlights();
            resetCollision();
            currentPlayer++;
            if (currentPlayer == characters.Count) currentPlayer = 0;

            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i].getState() == CharacterBehaviour.CharacterState.Dead)
                {
                    characters.Remove(characters[i]);
                }
            }
            if (characters.Count == 0)
            {
                 state = LevelState.Idle;
            }
            /*while ( characters[currentPlayer].getState() == CharacterBehaviour.CharacterState.Dead)
            {
                characters.Remove(characters[currentPlayer]);
                if (currentPlayer == characters.Count) currentPlayer = 0;
                if(characters.Count == 0)
                {
                    state = LevelState.Idle;
                }
            }*/
            if (characters.Count > 0)
            {
                state = LevelState.EnemyTurn;
                enemies[currentEnemy].setState(EnemyBehaviour.EnemyState.Selected);
            }
            
        }
    }

    void serviceEnemyTurn()
    {
        if (enemies[currentEnemy].getState() == EnemyBehaviour.EnemyState.Idle)
        {
            map.clearAllHighlights();
            resetCollision();
            currentEnemy++;
            if (currentEnemy == enemies.Count) currentEnemy = 0;
            state = LevelState.PlayerTurn;

            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i].getState() == CharacterBehaviour.CharacterState.Dead)
                {
                    characters.Remove(characters[i]);
                }
            }

            if (currentPlayer >= characters.Count) currentPlayer = 0;

            if (characters.Count == 0) state = LevelState.Idle;
            else
                characters[currentPlayer].setState(CharacterBehaviour.CharacterState.Selected);
         

        }
    }

    void serviceMoveState()
    {

    }
}
