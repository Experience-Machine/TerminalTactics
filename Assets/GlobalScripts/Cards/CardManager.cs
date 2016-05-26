using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CardManager
{

    List<Card> allCards;
    
    public List<Card> ownedCards;

    public List<Card> unownedCards;

    //Player cards
    private static attackCard thread = new attackCard("Thread", "Even more dangerous than a needle", 60, 2);
    private static attackCard ctrlaltdel = new attackCard("Ctrl+Alt+Del", "The classic command always has its uses", 90, 1);
    private static attackCard process = new attackCard("Process", "Like a thread... but bigger", 100, 2);

    // attack defense special movement hp
    private static characterCard whitehat = new characterCard("White hat", "Support", 450, 240, 400, 3, 330, "white"); 
    private static characterCard bruteForce = new characterCard("Brute force", "Attacker", 1000, 132, 90, 3, 262, "blue"); 
    private static characterCard firewall = new characterCard("Firewall", "Tank", 300, 260, 200, 3, 420, "black"); 

    private static SelfBuff selfModifying = new SelfBuff("Self-modifying", "Little to no existential risk", "Heal for 100 health", 50, "health", 100);
    //private static specialCard nullPointer = new specialCard("Null pointer", "Stop an enemy from attacking for two turns", 3);
    private static AttackAll blueScreen = new AttackAll("Blue screen", "OF DEATH", "Everyone takes 50 damage", 200, 50, true, false);

    private static TargetedAOEAttack testAOE = new TargetedAOEAttack("Logic bomb", "Death by information", "Damage in an area around the target", 90, 2, 3, 50, true);
    private static TargetedAOEAttack testAOEHeal = new TargetedAOEAttack("Refactor", "Life is messy, but code doesn't have to be", "Heal in an area around the target", 60, 2, 3, 50, false);

    private static OverTimeEffect testOTE = new OverTimeEffect(3, -35, "health", false, false, true);
    private static OverTimeEffect testOTE2 = new OverTimeEffect(3, -25, "health", false, false, true);
    private static OverTimeSpecial testOverTime = new OverTimeSpecial("Iterator", "Put those for loops away.", "Does 35 damage per turn for 3 turns to one enemy", 50, 2, testOTE2);
    private static OverTimeSpecialAOE testOverTimeAOE = new OverTimeSpecialAOE("Multiple iterator", "Everyone gets an iterator!", "Does 25 damage per turn for 3 turns to enemies in an area", 100, testOTE, 3, 3);

    private static OverTimeEffect defOTEBuff = new OverTimeEffect(3, 50, "defense", true, true, false);
    private static OverTimeSpecialAOE readOnly = new OverTimeSpecialAOE("Read only", "Press attack to enable editing", "Gives +50 defense to teammates in an area for 3 turns", 150, defOTEBuff, 2, 3);

    private static OverTimeEffect atkOTEBuff = new OverTimeEffect(3, 100, "attack", true, true, false);
    private static OverTimeSpecialAOE adminRights = new OverTimeSpecialAOE("Admin rights", "Completely safe.", "Gives +100 attack to teammates in an area for 3 turns", 200, atkOTEBuff, 2, 3);


    private static passiveCard cooling = new passiveCard("Cooling", "Good cooling improves performance. Defense +50", "defense", 50);
    private static passiveCard priority = new passiveCard("Priority", "Increased priority for faster throughput. Attack +50", "attack", 50);
    private static passiveCard overClocking = new passiveCard("Overclocking", "Crank that CPU over 9000!  Attack +100", "attack", 100);

    List<Card> enemyCards;

    // attack defense special movement hp

    // old worm and horse stats
    private static characterCard worm = new characterCard("Worm", "Average", 295, 251, 200, 4, 250, "worm"); 
    private static characterCard horse = new characterCard("Trojan Horse", "Tank", 328, 240, 284, 3, 334, "trojan"); 

    private static characterCard bug = new characterCard("Bug", "Average", 800, 100, 200, 4, 150, "bug"); // hard hitter 
    private static characterCard bloatWare = new characterCard("Bloatware", "Average", 280, 260, 200, 4, 250, "bloatWare"); // somewhat tanky
    private static characterCard ransomWare = new characterCard("Ransomware", "Tank", 2000, 260, 200, 1, 500, "ransomWare"); // boss stats
    //private static characterCard worm = new characterCard("Worm", "Average", 2000, 260, 200, 1, 500, "worm");

    private static attackCard infect = new attackCard("Infect", "...", 1, 1);
    private static attackCard backdoor = new attackCard("Backdoor", "...", 2, 2);

    private static passiveCard forprofit = new passiveCard("For-Profit", "...", "attack", 1);

    public CardManager()
    {
        allCards = new List<Card>();
        ownedCards = new List<Card>();
        enemyCards = new List<Card>();
        unownedCards = new List<Card>();

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
        //allCards.Add(nullPointer);
        allCards.Add(blueScreen);
        allCards.Add(cooling);
        allCards.Add(priority);
        allCards.Add(readOnly);
        allCards.Add(adminRights);

        allCards.Add(testAOE);
        allCards.Add(testAOEHeal);
        allCards.Add(testOverTime);
        allCards.Add(testOverTimeAOE);
        allCards.Add(process);
        allCards.Add(overClocking);

        enemyCards.Add(worm);
        enemyCards.Add(horse);
        enemyCards.Add(bug);
        enemyCards.Add(bloatWare);
        enemyCards.Add(ransomWare);
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
        //ownedCards.Add(nullPointer);
        ownedCards.Add(blueScreen);
        ownedCards.Add(cooling);
        ownedCards.Add(priority);
        ownedCards.Add(readOnly);
        ownedCards.Add(adminRights);


        
        ownedCards.Add(testAOEHeal);
        ownedCards.Add(testOverTime);
        ownedCards.Add(testOverTimeAOE);

        unownedCards.Add(testAOE);
        unownedCards.Add(process);
        unownedCards.Add(overClocking);
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

    public List<attackCard> getUnownedAttackCards()
    {
        List<attackCard> cards = new List<attackCard>();

        for (int i = 0; i < unownedCards.Count; i++)
        {
            if (unownedCards[i] is attackCard)
                cards.Add(unownedCards[i] as attackCard);
        }

        return cards;
    }

    public List<specialCard> getUnownedSpecialCards()
    {
        List<specialCard> cards = new List<specialCard>();

        for (int i = 0; i < unownedCards.Count; i++)
        {
            if (unownedCards[i] is specialCard)
                cards.Add(unownedCards[i] as specialCard);
        }

        return cards;
    }

    public List<passiveCard> getUnownedPassiveCards()
    {
        List<passiveCard> cards = new List<passiveCard>();

        for (int i = 0; i < unownedCards.Count; i++)
        {
            if (unownedCards[i] is passiveCard)
                cards.Add(unownedCards[i] as passiveCard);
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

    public characterInfo[] getDefaultCharInfo()
    {
        characterInfo[] characterInfos = new characterInfo[3];
        characterInfos[0] = new characterInfo(whitehat, thread, readOnly, cooling);
        characterInfos[1] = new characterInfo(firewall, thread, testAOEHeal, cooling);
        characterInfos[2] = new characterInfo(bruteForce, thread, testAOE, cooling);

        return characterInfos;

    }
}
