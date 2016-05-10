using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardManager{

    List<Card> allCards;
    
    List<Card> ownedCards;

    private static attackCard thread = new attackCard("Thread", "Even more dangerous than a needle", 3, 2);
    private static attackCard ctrlaltdel = new attackCard("Ctrl+Alt+Del", "The classic command always has its uses", 4, 1);

    private static characterCard whitehat = new characterCard("White hat hacker", "Support", 2, 2, 8, 3, 9);
    private static characterCard bruteForce = new characterCard("Brute force", "Attacker", 8, 2, 1, 3, 5);
    private static characterCard firewall = new characterCard("Firewall", "Tank", 2, 7, 2, 2, 12);

    private static specialCard selfModifying = new specialCard("Self-modifying", "Heal for 5 health", 3);
    private static specialCard nullPointer = new specialCard("Null pointer", "Stop an enemy from attacking for two turns", 3);
    private static specialCard blueScreen = new specialCard("Blue screen", "Everyone takes 5 damage", 8);

    public CardManager()
    {
        allCards = new List<Card>();
        ownedCards = new List<Card>();

        loadAllCards();
        loadOwnedCards();
    }

    public void loadAllCards()
    {

        allCards.Add(thread);
        allCards.Add(ctrlaltdel);
        allCards.Add(whitehat);
        allCards.Add(bruteForce);
        allCards.Add(firewall);
        allCards.Add(selfModifying);
        allCards.Add(nullPointer);
        allCards.Add(blueScreen);

    }

    //Temporarily, we have all cards
    public void loadOwnedCards()
    {
        ownedCards.Add(thread);
        ownedCards.Add(ctrlaltdel);
        ownedCards.Add(whitehat);
        ownedCards.Add(bruteForce);
        ownedCards.Add(firewall);
        ownedCards.Add(selfModifying);
        ownedCards.Add(nullPointer);
        ownedCards.Add(blueScreen);
    }

    public List<characterCard> getOwnedCharacterCards()
    {
        List<characterCard> cards = new List<characterCard>();

        for(int i = 0; i < ownedCards.Count; i++)
        {
            if (ownedCards[i] is characterCard)
                cards.Add(ownedCards[i] as characterCard);
        }

        return cards;
    }

    public List<attackCard> getOwnedAttackCards()
    {
        List<attackCard> cards = new List<attackCard>();

        for (int i = 0; i < ownedCards.Count; i++)
        {
            if (ownedCards[i] is attackCard)
                cards.Add(ownedCards[i] as attackCard);
        }

        return cards;
    }

    public List<specialCard> getOwnedSpecialCards()
    {
        List<specialCard> cards = new List<specialCard>();

        for (int i = 0; i < ownedCards.Count; i++)
        {
            if (ownedCards[i] is specialCard)
                cards.Add(ownedCards[i] as specialCard);
        }

        return cards;
    }

    public List<passiveCard> getOwnedPassiveCards()
    {
        List<passiveCard> cards = new List<passiveCard>();

        for (int i = 0; i < ownedCards.Count; i++)
        {
            if (ownedCards[i] is passiveCard)
                cards.Add(ownedCards[i] as passiveCard);
        }

        return cards;
    }

}
