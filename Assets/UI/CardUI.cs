using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CardUI : MonoBehaviour 
{
    Text cardName;
    Text cardDescription;
    Text cardBody;
    Image charImage;
    Card card;
    characterInfo charInfo; //If we need it
    GlobalGameManager.CardType type;
    Button button;

    GameObject cardPrefab;
    GlobalGameManager manager;

    // Use this for initialization
    void Awake() 
    {

        manager = GameObject.Find("GlobalGameManager(Clone)").GetComponent<GlobalGameManager>();
        type = GlobalGameManager.CardType.None;
        Text[] textComponents = GetComponentsInChildren<Text>();
        cardName = textComponents[1];
        cardDescription = textComponents[2];
        cardBody = textComponents[0];

        button = GetComponent<Button>();

        Scene scene = SceneManager.GetActiveScene();
        if (scene.name.Equals("CharCustomization")) //If we're in the character customization scene, we want to go to the select card scene
        {
            button.onClick.AddListener(
                () =>
                {
                    manager.selectedCharacterInfo = charInfo;
                    manager.cardTypeSelected = type;
                    SceneManager.LoadScene("CardSelect");
                });
        } 
        else if (scene.name.Equals("CardSelect")) //If we're in the card selection scene, we want to go back to char customization
        {
            button.onClick.AddListener(
                () =>
                {
                    manager.selectedCard = card; //Let global know what the card we chose was
                    characterInfo infoToChange = manager.selectedCharacterInfo;
                    
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
                    }
                    SceneManager.LoadScene("CharCustomization");
                });
        }


    }
	
	// Update is called once per frame
	void Update () 
    {
	
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
    
    // Set body with empty paramaters sets the body to
    //  the card's character stats
    public void setBody()
    {
        if (card is characterCard)
        {
            characterCard cCard = (characterCard)card;
            cardBody.text = "HP: " + cCard.maxHP
             + "   ATK: " + cCard.ATK
             + " DEF: " + cCard.DEF
             + " SPC: " + cCard.SPC
             + " MOV: " + cCard.MOV;
        }
        else if(card is attackCard)
        {
            attackCard aCard = (attackCard)card;
            cardBody.text = "Deals " + aCard.getDamage() + " damage at a range of " + aCard.getRange();
        }
        else if(card is specialCard)
        {

        }
        else if(card is passiveCard)
        {
            passiveCard pCard = (passiveCard)card;
            cardBody.text = "Increases " + pCard.statToChange + " by " + pCard.statValue;
        }
    }

    // Set the body to the specified text
    public void setBody(string bodyText)
    {
        cardBody.text = bodyText;
    }
    public void setImage(string spriteName)
    {
        if (card is characterCard) // Only do this if we have a characterCard, to avoid null pointer
        {
            Transform imageChild = transform.FindChild("Image");
            if (imageChild)
            {
                charImage = imageChild.GetComponent<Image>();
                if(spriteName == "hero")
                {
                    charImage.sprite = Resources.Load("Textures/" + spriteName, typeof(Sprite)) as Sprite;
                    Debug.Log("Loaded " + "Textures/" + spriteName + " into: " + charImage);
                    return;
                }

                Sprite[] sprites = Resources.LoadAll<Sprite>("Textures/Heros/walk_" + spriteName);

                charImage.sprite = sprites[18]; // <--------------- This is assuming we'll find it at 18. THIS MIGHT NOT BE RIGHT.
                Debug.Log("Loaded " + "Textures/Heros/walk_" + spriteName + "_18" + " into: " + charImage);
                return;
            }
            
        }
    }
}
