using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class VictoryMenu : MonoBehaviour {

    GlobalGameManager manager;
    CardManager cardManager;

    public Button card1;
    public Button card2;
    public Button card3;

    private attackCard attackCard;
    private passiveCard passiveCard;
    private specialCard specialCard;

    List<attackCard> unownedAttacks;
    List<passiveCard> unownedPassives;
    List<specialCard> unownedSpecials;

    // Use this for initialization
    void Start()
    {
        manager = GameObject.Find("GlobalGameManager(Clone)").GetComponent<GlobalGameManager>();
        cardManager = manager.cardManager;
        unownedAttacks = cardManager.getUnownedAttackCards();
        unownedPassives = cardManager.getUnownedPassiveCards();
        unownedSpecials = cardManager.getUnownedSpecialCards();

        card1 = GameObject.Find("Canvas/Card1").GetComponent<Button>();
        card2 = GameObject.Find("Canvas/Card2").GetComponent<Button>();
        card3 = GameObject.Find("Canvas/Card3").GetComponent<Button>();

        if(unownedAttacks.Count == 0)
        {
            emptyCard(GameObject.Find("Canvas/Card1").GetComponent<CardUI>());
        }
        else
        {
            setCard(GameObject.Find("Canvas/Card1").GetComponent<CardUI>(), 0);
        }

        if(unownedPassives.Count == 0)
        {
            emptyCard(GameObject.Find("Canvas/Card2").GetComponent<CardUI>());
        }
        else
        {
            setCard(GameObject.Find("Canvas/Card2").GetComponent<CardUI>(), 1);
        }

        if(unownedSpecials.Count == 0)
        {
            emptyCard(GameObject.Find("Canvas/Card3").GetComponent<CardUI>());
        }
        else
        {
            setCard(GameObject.Find("Canvas/Card3").GetComponent<CardUI>(), 2);
        }

        card1.onClick.AddListener(
            () =>
            {
                //Unlock selected card
                cardManager.unownedCards.Remove(attackCard);
                cardManager.ownedCards.Add(attackCard);
                SceneManager.LoadScene("GameMainMenu");
            });

        card2.onClick.AddListener(
            () =>
            {
                cardManager.unownedCards.Remove(passiveCard);
                cardManager.ownedCards.Add(passiveCard);
                SceneManager.LoadScene("GameMainMenu");
            });

        card3.onClick.AddListener(
            () =>
            {
                cardManager.unownedCards.Remove(specialCard);
                cardManager.ownedCards.Add(specialCard);
                SceneManager.LoadScene("GameMainMenu");
            });
    }

    private void emptyCard(CardUI card)
    {
        card.setName("No cards of this type available!");
        card.setDescription("");
        card.setBody("");
    }

    private void setCard(CardUI card, int type)
    {
        Card cardChosen = null;
        if (type == 0)
        {
            attackCard = unownedAttacks[0];
            cardChosen = attackCard;
        }
        if (type == 1)
        {
            passiveCard = unownedPassives[0];
            cardChosen = passiveCard;
        }
        if (type == 2)
        {
            specialCard = unownedSpecials[0];
            cardChosen = specialCard;
        }

        card.setCard(cardChosen);
        card.setName(cardChosen.getName());
        card.setDescription(cardChosen.getDescription());
        card.setBody();
    }

    // Update is called once per frame
    void Update ()
    {
	    
	}
}
