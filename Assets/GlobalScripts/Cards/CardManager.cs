using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CardManager
{

    List<Card> allCards;
    
    List<Card> ownedCards;

    //Player cards
    private static attackCard thread = new attackCard("Thread", "Even more dangerous than a needle", 3, 2);
    private static attackCard ctrlaltdel = new attackCard("Ctrl+Alt+Del", "The classic command always has its uses", 4, 1);

    private static characterCard whitehat = new characterCard("White hat", "Support", 2, 2, 8, 3, 9, "white");
    private static characterCard bruteForce = new characterCard("Brute force", "Attacker", 8, 2, 1, 3, 5, "blue");
    private static characterCard firewall = new characterCard("Firewall", "Tank", 2, 7, 2, 2, 12, "hero");

    private static SelfBuff selfModifying = new SelfBuff("Self-modifying", "Heal for 5 health", 3, "health", 5);
    private static specialCard nullPointer = new specialCard("Null pointer", "Stop an enemy from attacking for two turns", 3);
    private static AttackAll blueScreen = new AttackAll("Blue screen", "Everyone takes 5 damage", 8, 5, true, false);

    private static TargetedAOEAttack testAOE = new TargetedAOEAttack("Test aoe", "Damage in an area around the target", 1, 2, 3, 5, true);
    private static TargetedAOEAttack testAOEHeal = new TargetedAOEAttack("Test aoe heal", "Heal in an area around the target", 1, 2, 3, 2, false);

    private static OverTimeEffect testOTE = new OverTimeEffect(3, -1, "health", false, false, true);
    private static OverTimeSpecial testOverTime = new OverTimeSpecial("Test overtime attack", "...", 1, 2, testOTE);
    private static OverTimeSpecialAOE testOverTimeAOE = new OverTimeSpecialAOE("Test overtime aoe attack", "...", 1, testOTE, 3, 3);


    private static passiveCard cooling = new passiveCard("Cooling", "Good cooling improves performance", "defense", 1);
    private static passiveCard priority = new passiveCard("Priority", "Increased priority for faster throughput", "movement", 1);

    List<Card> enemyCards;

    private static characterCard worm = new characterCard("Worm", "Average", 2, 1, 4, 4, 3, "enemy");
    private static characterCard horse = new characterCard("Trojan Horse", "Tank", 2, 4, 3, 3, 6, "enemy");

    private static attackCard infect = new attackCard("Infect", "...", 1, 1);
    private static attackCard backdoor = new attackCard("Backdoor", "...", 2, 2);

    private static passiveCard forprofit = new passiveCard("For-Profit", "...", "attack", 1);

    public CardManager()
    {
        allCards = new List<Card>();
        ownedCards = new List<Card>();
        enemyCards = new List<Card>();

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
        allCards.Add(cooling);
        allCards.Add(priority);

        allCards.Add(testAOE);
        allCards.Add(testAOEHeal);
        allCards.Add(testOverTime);
        allCards.Add(testOverTimeAOE);

        enemyCards.Add(worm);
        enemyCards.Add(horse);
        enemyCards.Add(infect);
        enemyCards.Add(backdoor);
        enemyCards.Add(forprofit);
        enemyCards.Add(blueScreen);

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
        ownedCards.Add(cooling);
        ownedCards.Add(priority);

        ownedCards.Add(testAOE);
        ownedCards.Add(testAOEHeal);
        ownedCards.Add(testOverTime);
        ownedCards.Add(testOverTimeAOE);
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

    public List<characterCard> getEnemyCharacterCards()
    {
        List<characterCard> cards = new List<characterCard>();

        for (int i = 0; i < enemyCards.Count; i++)
        {
            if (enemyCards[i] is characterCard)
                cards.Add(enemyCards[i] as characterCard);
        }

        return cards;
    }

    public List<attackCard> getEnemyAttackCards()
    {
        List<attackCard> cards = new List<attackCard>();

        for (int i = 0; i < enemyCards.Count; i++)
        {
            if (enemyCards[i] is attackCard)
                cards.Add(enemyCards[i] as attackCard);
        }

        return cards;
    }

    public List<specialCard> getEnemySpecialCards()
    {
        List<specialCard> cards = new List<specialCard>();

        for (int i = 0; i < enemyCards.Count; i++)
        {
            if (enemyCards[i] is specialCard)
                cards.Add(enemyCards[i] as specialCard);
        }

        return cards;
    }

    public List<passiveCard> getEnemyPassiveCards()
    {
        List<passiveCard> cards = new List<passiveCard>();

        for (int i = 0; i < enemyCards.Count; i++)
        {
            if (enemyCards[i] is passiveCard)
                cards.Add(enemyCards[i] as passiveCard);
        }

        return cards;
    }
}
