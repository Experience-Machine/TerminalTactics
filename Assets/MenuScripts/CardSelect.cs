using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardSelect : MonoBehaviour {

    private const int startPositionX = -194;
    private const int startPositionY = 164;

    private int cardHeight;
    private int cardWidth;

    GameObject myCanvas;

    GlobalGameManager manager;
   
    // Use this for initialization
    void Start()
    {
        manager = GameObject.Find("GlobalGameManager").GetComponent<GlobalGameManager>() ;
        myCanvas = GameObject.Find("Canvas");

        GameObject characterCard = Instantiate(Resources.Load("Prefabs/Card")) as GameObject;
        RectTransform transform = (RectTransform)characterCard.transform;
        cardWidth = (int)transform.rect.width;
        cardHeight = (int)transform.rect.height;

        setCards();

    }

    private void setCards()
    {
        
        if (manager.cardTypeSelected == GlobalGameManager.CardType.Character)
        {
            List<characterCard> characterCards = manager.cardManager.getOwnedCharacterCards();
            int cardOffset = ((characterCards.Count * (cardWidth + 5) / 2)) - 75;
            for (int i = 0; i < characterCards.Count; i++)
            {
                GameObject characterCard = Instantiate(Resources.Load("Prefabs/Card")) as GameObject;
                characterCard.transform.SetParent(myCanvas.transform, false);
                CardUI uiComponent = characterCard.GetComponent<CardUI>();
                uiComponent.setCard(characterCards[i]);
                uiComponent.setName(characterCards[i].getName());
                uiComponent.setDescription(characterCards[i].getDescription());
                Vector3 target = new Vector3((i * cardWidth) + (i * 5) - cardOffset, 0, 0);
                RectTransform transform = (RectTransform)characterCard.transform;
                transform.anchoredPosition = target;
            }
        } else if (manager.cardTypeSelected == GlobalGameManager.CardType.Attack)
        {
            List<attackCard> attackCards = manager.cardManager.getOwnedAttackCards();
            //int cardOffset = ((attackCards.Count - 1 * cardWidth) + (attackCards.Count - 1 * 5) / 2);
            int cardOffset = ((attackCards.Count * (cardWidth + 5) / 2)) - 75;
            for (int i = 0; i < attackCards.Count; i++)
            {
                GameObject attackCard = Instantiate(Resources.Load("Prefabs/Card")) as GameObject;
                attackCard.transform.SetParent(myCanvas.transform, false);
                CardUI uiComponent = attackCard.GetComponent<CardUI>();
                uiComponent.setCard(attackCards[i]);
                uiComponent.setName(attackCards[i].getName());
                uiComponent.setDescription(attackCards[i].getDescription());
                Vector3 target = new Vector3((i * cardWidth) + (i * 5) - cardOffset, 0, 0);
                RectTransform transform = (RectTransform)attackCard.transform;
                transform.anchoredPosition = target;
            }
        } else if (manager.cardTypeSelected == GlobalGameManager.CardType.Special)
        {
            List<specialCard> specialCards = manager.cardManager.getOwnedSpecialCards();
            int cardOffset = ((specialCards.Count * (cardWidth+5) / 2)) - 75;
            for (int i = 0; i < specialCards.Count; i++)
            {
                GameObject specialCard = Instantiate(Resources.Load("Prefabs/Card")) as GameObject;
                specialCard.transform.SetParent(myCanvas.transform, false);
                CardUI uiComponent = specialCard.GetComponent<CardUI>();
                uiComponent.setCard(specialCards[i]);
                uiComponent.setName(specialCards[i].getName());
                uiComponent.setDescription(specialCards[i].getDescription());
                Vector3 target = new Vector3((i * cardWidth) + (i * 5) - cardOffset, 0, 0);
                RectTransform transform = (RectTransform)specialCard.transform;
                transform.anchoredPosition = target;
            }
        } else if (manager.cardTypeSelected == GlobalGameManager.CardType.Passive)
        {
            List<passiveCard> passiveCards = manager.cardManager.getOwnedPassiveCards();
            int cardOffset = ((passiveCards.Count * (cardWidth + 5) / 2)) - 75;
            for (int i = 0; i < passiveCards.Count; i++)
            {
                GameObject passiveCard = Instantiate(Resources.Load("Prefabs/Card")) as GameObject;
                passiveCard.transform.SetParent(myCanvas.transform, false);
                CardUI uiComponent = passiveCard.GetComponent<CardUI>();
                uiComponent.setCard(passiveCards[i]);
                uiComponent.setName(passiveCards[i].getName());
                uiComponent.setDescription(passiveCards[i].getDescription());
                Vector3 target = new Vector3((i * cardWidth) + (i * 5) + cardOffset, 0, 0);
                RectTransform transform = (RectTransform)passiveCard.transform;
                transform.anchoredPosition = target;
            }
        }


    }

    // Update is called once per frame
    void Update () {
	
	}
}
