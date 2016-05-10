using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIBehavior : MonoBehaviour {

    private float maxHealth;
    private float curHealth;

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

    public void setContent(Sprite s, float maxHealth, float minHealth, string characterName)
    {
        this.maxHealth = maxHealth;
        this.curHealth = minHealth;

        GameObject charPortrait = GameObject.Find("CharacterUI(Clone)/Character Info/Image");

        Image i = charPortrait.GetComponent<Image>();
        i.sprite = s;

        GameObject textComp = GameObject.Find("CharacterUI(Clone)/Character Info/Panel (1)/CharName");
        Text charName = textComp.GetComponent<Text>();
        charName.text = characterName;

        GameObject healthBar = GameObject.Find("CharacterUI(Clone)/Character Info/Panel (2)/HealthBar/Panel");
        RectTransform transform = healthBar.GetComponent<RectTransform>();
        float size = (curHealth / maxHealth) * 240f;
        
        transform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, size);
        Image image = healthBar.GetComponent<Image>();
        image.color = UnityEngine.Color.red;
    }
}
