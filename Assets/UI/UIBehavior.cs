using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIBehavior : MonoBehaviour {

    public static bool attackButtonsGrey = false;
    public static bool moveButtonGrey = false;

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
        moveButton = GameObject.Find("CharacterUI(Clone)/Actions/Image/MoveButton").GetComponent<Button>();
        attackButton = GameObject.Find("CharacterUI(Clone)/Actions/Image/AttackButton").GetComponent<Button>();
        specialButton = GameObject.Find("CharacterUI(Clone)/Actions/Image/SpecialButton").GetComponent<Button>();
        waitButton = GameObject.Find("CharacterUI(Clone)/Actions/Image/WaitButton").GetComponent<Button>();

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
                lastClicked = ButtonClicked.Special;
                //Method call to change to special state
                

                
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

    public static void resetButtonColors()
    {
        attackButtonsGrey = false;
        moveButtonGrey = false;
    }

    public void greyOutAttackButtons()
    {
        attackButton.enabled = false;
        specialButton.enabled = false;
        attackButton.image.color = Color.gray;
        specialButton.image.color = Color.gray;
        attackButtonsGrey = true;
    }

    public void greyOutMoveButton()
    {
        moveButton.enabled = false;
        moveButton.image.color = Color.gray;
        moveButtonGrey = true;
    }

    public void setContent(Sprite s, float maxHealth, float curHealth, float maxSPC, float curSPC, string characterName)
    {
        this.maxHealth = maxHealth;
        this.curHealth = curHealth;
        this.curSPC = curSPC;
        this.maxSPC = maxSPC;

        GameObject charPortrait = GameObject.Find("CharacterUI(Clone)/Character Info/Image/Image");

        Image i = charPortrait.GetComponent<Image>();
        i.sprite = s;

        GameObject textComp = GameObject.Find("CharacterUI(Clone)/Character Info/Image/Panel (1)/CharName");
        Text charName = textComp.GetComponent<Text>();
        charName.text = characterName;

        Text curHealthText = GameObject.Find("CharacterUI(Clone)/Character Info/Image/Panel (2)/Panel/Current").GetComponent<Text>();
        curHealthText.text = curHealth.ToString();

        Text maxHealthText = GameObject.Find("CharacterUI(Clone)/Character Info/Image/Panel (2)/Panel/Max").GetComponent<Text>();
        maxHealthText.text = maxHealth.ToString();

        Text curSpecialText = GameObject.Find("CharacterUI(Clone)/Character Info/Image/Panel (3)/Panel/Current").GetComponent<Text>();
        curSpecialText.text = curSPC.ToString();

        Text maxSpecialText = GameObject.Find("CharacterUI(Clone)/Character Info/Image/Panel (3)/Panel/Max").GetComponent<Text>();
        maxSpecialText.text = maxSPC.ToString();

        //Governs Healthbar behavior
        GameObject healthBar = GameObject.Find("CharacterUI(Clone)/Character Info/Image/Panel (2)/HealthBar/Panel");
        RectTransform transformHealth = healthBar.GetComponent<RectTransform>();
        
        transformHealth.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, (curHealth / maxHealth) * 240f);
        Image imageHealth = healthBar.GetComponent<Image>();
        imageHealth.color = UnityEngine.Color.red;

        //Governs SPCbar behavior
        GameObject spcBar = GameObject.Find("CharacterUI(Clone)/Character Info/Image/Panel (3)/SpecialBar/Panel");
        RectTransform transformSPC = spcBar.GetComponent<RectTransform>();

        transformSPC.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, (curSPC / maxSPC) * 240f);
        Image imageSPC = spcBar.GetComponent<Image>();
        imageSPC.color = UnityEngine.Color.blue;
    }
}
