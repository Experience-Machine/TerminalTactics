using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour
{

    public bool collideable;
    private static Map map;
    public SpriteRenderer tileRenderer;

    // hasUnit is currently intended to show if a tile holds *specifically* a player 
    //  character unit. (This was early implem.)
    public bool hasUnit;
    public CharacterBehaviour charOnTile = null;
    public EnemyBehaviour enemyOnTile = null;

    public int x, y;

    // The present color of the tile. This can be temporarily change
    //  by mouseover, but the tile returns to 'curent color' after
    //  mouse-out.
    public Color currentColor;
    public Color defaultColor;
    private static Color highlightColor = new Color(.3f,.3f,.3f,.15f);

    //private static GameObject tileSelector;
    //private GameObject selectRef;
    // Use this for initialization
    void Start () 
    {
        if(map == null)
        {
            map = GameObject.Find("Map").GetComponent<Map>();
        }

        //collideable = false;
        if (tileRenderer == null)
        {
            tileRenderer = gameObject.GetComponent<SpriteRenderer>();
        }

        //defaultColor = Color.grey;
        //currentColor = defaultColor;
        //tileRenderer.color = currentColor;

    }

    void Awake()
    {
        if (map == null)
        {
            map = GameObject.Find("Map").GetComponent<Map>();
        }
        if (tileRenderer == null)
        {
            tileRenderer = gameObject.GetComponent<SpriteRenderer>();
        }
        defaultColor = Color.grey;
        currentColor = defaultColor;
        tileRenderer.color = currentColor;
        collideable = false;
        hasUnit = false;
    }

    // Update is called once per frame
    void Update () 
    {
        
    }

    void OnMouseEnter()
    {
        //renderer.enabled = true;
        if(tileRenderer != null)
            tileRenderer.color = currentColor + highlightColor;

        //Debug.Log(selectRef.transform.position.ToString() + " collideable: " + collideable);

    }
    void OnMouseExit()
    {
        //renderer.enabled = true;
        tileRenderer.color = currentColor;

        //Destroy(selectRef);
        //selectRef = null;
        
    }

    void OnMouseDown()
    {
        /*
        selectRef = Instantiate(tileSelector) as GameObject;
        selectRef.transform.position = transform.position; 
        SpriteRenderer sr = selectRef.GetComponent<SpriteRenderer>();
        sr.enabled = true;
        */
        //Debug.Log("Tile " + transform.position.ToString() + " clicked");
        map.lastTileClicked = this;
    }

    public void attackTile(int damageDealt)
    {
        AudioSource attackAudio = GameObject.Find("AttackAudio").GetComponent<AudioSource>();

        GameObject damageNumber = Instantiate(Resources.Load("Prefabs/DamageText")) as GameObject;
        DamageNumber damageUi = damageNumber.GetComponent<DamageNumber>();
        
        int actualDamage = -1;
        if (charOnTile != null)
        {
            hasUnit = false;

            actualDamage = damageDealt / charOnTile.GetComponent<CharacterBehaviour>().defense;
            if (actualDamage == 0) actualDamage = 1; //Don't want 0 damage
            charOnTile.damage(actualDamage);

            attackAudio.PlayOneShot(attackAudio.clip, 0.75f);
           
            damageUi.setPosition(Camera.main.WorldToScreenPoint(transform.position));

            if (charOnTile.getState() == CharacterBehaviour.CharacterState.Dead)
            {
                charOnTile = null;
            }
        }
        else if(enemyOnTile != null)
        {
            actualDamage = damageDealt / enemyOnTile.defense;
            if (actualDamage == 0) actualDamage = 1; //Don't want 0 damage
            enemyOnTile.damage(actualDamage);
            attackAudio.PlayOneShot(attackAudio.clip, 0.75f);

            //Debug.Log("Damage: " + damageDealt + " " + enemyOnTile.defense);
            //Debug.Log("Actual: " + actualDamage);

            damageUi.setPosition(Camera.main.WorldToScreenPoint(transform.position));
        }

        damageUi.setNumber("-" + actualDamage.ToString());


    }

    public void healTile(int healingDealt)
    {
        GameObject damageNumber = Instantiate(Resources.Load("Prefabs/DamageText")) as GameObject;
        DamageNumber damageUi = damageNumber.GetComponent<DamageNumber>();


        if (charOnTile != null)
        {
            hasUnit = false;

            charOnTile.heal(healingDealt);
            damageUi.setPosition(Camera.main.WorldToScreenPoint(transform.position));
            damageUi.setNumber("+" + healingDealt.ToString());

        }

        //No current support for enemy healing
    }

    public void applyOverTimeEffectToTile(OverTimeEffect ote)
    {
        if (charOnTile != null)
        {
            hasUnit = false;
            //Clone the effect so we can avoid impacting the one in the special card
            charOnTile.giveOverTimeEffect(ote.clone());
        }
        else if (enemyOnTile != null)
        {
            enemyOnTile.giveOverTimeEffect(ote.clone());
        }
    }

    // This method can be used one of two ways:
    //  1) There is a unit present on this tile. In this case, the tile will 
    //      call the kill enemy on the unit present on the tile.
    //  2) There is something special about this tile that allows it to be
    //       destroyable. In this case, it will call the killThisTile method on
    //       itself.
    public void killTile()
    {
        if(charOnTile != null)
        {
            hasUnit = false;
            charOnTile.kill();
            charOnTile = null;
        }
        else
        {
            killThisTile();
        }
    }

    void killThisTile()
    {
        // Temporary kill this tile info
        hasUnit = false;
        defaultColor = Color.black;
        currentColor = Color.black;
        tileRenderer.color = Color.black;
    }

    //Getters/Setters
    public bool isCollideable() { return collideable; }
    public void setCollideable(bool collide) { collideable = collide; }
}