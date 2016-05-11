using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIBehavior : MonoBehaviour {

    private float maxHealth;
    private float curHealth;

    private float maxSPC;
    private float curSPC;

    public Button moveButton;
    public Button attackButton;
    public Button specialButton;
    public Button waitButton;

    public enum ButtonClicked
    {
        None,
        Move,
        Attack,
        Special,
        Wait
    }
    public static ButtonClicked lastClicked;

    void Start ()
    {
        moveButton = GameObject.Find("CharacterUI(Clone)/Actions/MoveButton").GetComponent<Button>();
        attackButton = GameObject.Find("CharacterUI(Clone)/Actions/AttackButton").GetComponent<Button>();
        specialButton = GameObject.Find("CharacterUI(Clone)/Actions/SpecialButton").GetComponent<Button>();
        waitButton = GameObject.Find("CharacterUI(Clone)/Actions/WaitButton").GetComponent<Button>();

        lastClicked = ButtonClicked.None;

        moveButton.onClick.AddListener(
            () =>
            {
                Debug.Log("Moving now");
                //Method call to change to move state
                lastClicked = ButtonClicked.Move;
            });

        attackButton.onClick.AddListener(
            () =>
            {
                Debug.Log("Attacking now");
                //Method call to change to attack state
                lastClicked = ButtonClicked.Attack;
            });

        specialButton.onClick.AddListener(
            () =>
            {
                Debug.Log("Specialing Now");
                //Method call to change to special state
                lastClicked = ButtonClicked.Special;
            });

        waitButton.onClick.AddListener(
            () =>
            {
                Debug.Log("Waiting Now");
                //Method call to change to wait state
                lastClicked = ButtonClicked.Wait;
            });
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void setContent(Sprite s, float maxHealth, float curHealth, float maxSPC, float curSPC, string characterName)
    {
        this.maxHealth = maxHealth;
        this.curHealth = curHealth;
        this.curSPC = curSPC;
        this.maxSPC = maxSPC;

        GameObject charPortrait = GameObject.Find("CharacterUI(Clone)/Character Info/Image");

        Image i = charPortrait.GetComponent<Image>();
        i.sprite = s;

        GameObject textComp = GameObject.Find("CharacterUI(Clone)/Character Info/Panel (1)/CharName");
        Text charName = textComp.GetComponent<Text>();
        charName.text = characterName;


        //Governs Healthbar behavior
        GameObject healthBar = GameObject.Find("CharacterUI(Clone)/Character Info/Panel (2)/HealthBar/Panel");
        RectTransform transformHealth = healthBar.GetComponent<RectTransform>();
        
        transformHealth.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, (curHealth / maxHealth) * 240f);
        Image imageHealth = healthBar.GetComponent<Image>();
        imageHealth.color = UnityEngine.Color.red;

        //Governs SPCbar behavior
        GameObject spcBar = GameObject.Find("CharacterUI(Clone)/Character Info/Panel (3)/SpecialBar/Panel");
        RectTransform transformSPC = spcBar.GetComponent<RectTransform>();

        transformSPC.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, (curSPC / maxSPC) * 240f);
        Image imageSPC = spcBar.GetComponent<Image>();
        imageSPC.color = UnityEngine.Color.blue;
    }
}
