using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterCustomization : MonoBehaviour 
{

    private const int startPositionX = -194;
    private const int startPositionY = 164;

    private int cardHeight;
    private int cardWidth;

    GameObject myCanvas;
    ArrayList charInfoCards;
    Button returnButton;

    GlobalGameManager manager;

	// Use this for initialization
	void Start () 
    {
        manager = GameObject.Find("GlobalGameManager(Clone)").GetComponent<GlobalGameManager>();

        myCanvas = GameObject.Find("Canvas");
        charInfoCards = new ArrayList();
        GameObject characterCard = Instantiate(Resources.Load("Prefabs/Card")) as GameObject;
        RectTransform transform = (RectTransform)characterCard.transform;
        cardWidth = (int)transform.rect.width;
        cardHeight = (int)transform.rect.height;

        setCards();
        returnButton = GameObject.Find("ReturnButton").GetComponent<Button>();

        returnButton.onClick.AddListener(
            () =>
            {
                
                SceneManager.LoadScene("GameMainMenu");
            });
        
    }

    private void setCards()
    {
        for (int i = 0; i < manager.characterInfos.Length; i++)
        {
            characterInfo characterInfo = manager.characterInfos[i];
            Camera camera = GetComponent<Camera>();

            GameObject characterCard = Instantiate(Resources.Load("Prefabs/CharCard")) as GameObject;
            characterCard.transform.SetParent(myCanvas.transform, false);
            CardUI uiComponent = characterCard.GetComponent<CardUI>();

            characterCard charCard = characterInfo.getCharacter();
            uiComponent.setCard(charCard);
            uiComponent.setName(charCard.getName());
            uiComponent.setDescription(charCard.getDescription());
            uiComponent.setCharacter(characterInfo);
            uiComponent.setBody(); // Uses the card's character stats
            uiComponent.setImage(charCard.spriteName); // Specifically for character cards
            Vector3 target = new Vector3(startPositionX + (i * cardWidth) + (i * 5), startPositionY, 0);
            RectTransform transform = (RectTransform)characterCard.transform;
            transform.anchoredPosition = target;
            

            GameObject attack = Instantiate(Resources.Load("Prefabs/Card")) as GameObject;
            attack.transform.SetParent(myCanvas.transform, false);
            uiComponent = attack.GetComponent<CardUI>();

            attackCard attackCard = characterInfo.getAttack();
            uiComponent.setCard(attackCard);
            uiComponent.setName(attackCard.getName());
            uiComponent.setDescription(attackCard.getDescription());
            uiComponent.setCharacter(characterInfo);
            uiComponent.setBody(); // Uses the card's damage and range
            target = new Vector3(startPositionX + (i * cardWidth) + (i * 5), startPositionY - cardHeight - 5, 0);
            transform = (RectTransform)attack.transform;
            transform.anchoredPosition = target;

            GameObject special = Instantiate(Resources.Load("Prefabs/Card")) as GameObject;
            special.transform.SetParent(myCanvas.transform, false);
            uiComponent = special.GetComponent<CardUI>();

            specialCard specialCard = characterInfo.getSpecial();
            uiComponent.setCard(specialCard);
            uiComponent.setName(specialCard.getName());
            uiComponent.setDescription(specialCard.getDescription());
            uiComponent.setCharacter(characterInfo);
            target = new Vector3(startPositionX + (i * cardWidth) + (i * 5), startPositionY - (cardHeight * 2) - 10, 0);
            transform = (RectTransform)special.transform;
            transform.anchoredPosition = target;

            GameObject passive = Instantiate(Resources.Load("Prefabs/Card")) as GameObject;
            passive.transform.SetParent(myCanvas.transform, false);
            uiComponent = passive.GetComponent<CardUI>();

            passiveCard passiveCard = characterInfo.getPassive();
            uiComponent.setCard(passiveCard);
            uiComponent.setName(passiveCard.getName());
            uiComponent.setDescription(passiveCard.getDescription());
            uiComponent.setCharacter(characterInfo);
            uiComponent.setBody(); // Uses the card's passive statToChange and statValue
            target = new Vector3(startPositionX + (i * cardWidth) + (i * 5), startPositionY - (cardHeight * 3) - 15, 0);
            transform = (RectTransform)passive.transform;
            transform.anchoredPosition = target;

        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
