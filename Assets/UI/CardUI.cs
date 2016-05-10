using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CardUI : MonoBehaviour {
    Text cardName;
    Text cardDescription;
    Card card;
    characterInfo charInfo; //If we need it
    GlobalGameManager.CardType type;
    Button button;

    GameObject cardPrefab;

    // Use this for initialization
    void Awake() {
        type = GlobalGameManager.CardType.None;
        Text[] textComponents = GetComponentsInChildren<Text>();
        cardName = textComponents[1];
        cardDescription = textComponents[2];

        button = GetComponent<Button>();

        Scene scene = SceneManager.GetActiveScene();
        if (scene.name.Equals("CharCustomization")) { //If we're in the character customization scene, we want to go to the select card scene
            button.onClick.AddListener(
                () =>
                {
                    GlobalGameManager.selectedCharacterInfo = charInfo;
                    GlobalGameManager.cardTypeSelected = type;
                    SceneManager.LoadScene("CardSelect");
                });
        } else if (scene.name.Equals("CardSelect")) //If we're in the card selection scene, we want to go back to char customization
        {
            button.onClick.AddListener(
                () =>
                {
                    /*GlobalGameManager.selectedCard = card; //Let global know what the card we chose was
                    characterInfo infoToChange = GlobalGameManager.selectedCharacterInfo;
                    
                    if (card is characterCard)
                    {
                        infoToChange.charCard = card as characterCard;
                    } else if (card is attackCard)
                    {
                        infoToChange.atkCard = card as attackCard;
                    } else if (card is specialCard)
                    {
                        infoToChange.spcCard = card as specialCard;
                    } else if (card is passiveCard)
                    {
                        infoToChange.psvCard = card as passiveCard;
                    }*/
                    SceneManager.LoadScene("CharCustomization");
                });
        }


    }
	
	// Update is called once per frame
	void Update () {
	
	}
    public void setCard(Card c)
    {
        card = c;
        if (c is attackCard)
            type = GlobalGameManager.CardType.Attack;
        else if (c is characterCard)
            type = GlobalGameManager.CardType.Character;
        else if (c is passiveCard)
            type = GlobalGameManager.CardType.Passive;
        else if (c is specialCard)
            type = GlobalGameManager.CardType.Special;

    }

    public void setCharacter(characterInfo c)
    {
        charInfo = c;
    }

    public void setName(string name)
    {
        cardName.text = name;
    }

    public void setDescription(string description)
    {
        cardDescription.text = description;
    }
}
