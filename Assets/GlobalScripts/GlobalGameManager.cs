using UnityEngine;
using UnityEngine.SceneManagement;
// for SceneManager
using System.Collections;

public class GlobalGameManager : MonoBehaviour 
{
    public enum CardType
    {
        Character,
        Attack,
        Special,
        Passive,
        None
    }

    public SaveLoad saveLoad;

    public CardManager cardManager;
    public CardType cardTypeSelected = CardType.None;
    public characterInfo selectedCharacterInfo = null;
    public Card selectedCard = null;

	private string mCurrentLevel = "MenuLevel";
    public characterInfo[] characterInfos;

    public int NUMBER_OF_LEVELS = 5;
    public int level = 1;

    void Awake()
    {
        characterInfos = new characterInfo[3];
        cardManager = new CardManager();

        // Dummy CharInfo stuff:
        /*characterCard defaultChar = new characterCard("Char", "New char", 1, 1, 1, 3, 3, "hero");
        attackCard defaultAtk = new attackCard();
        specialCard defaultSpc = new specialCard();
        passiveCard defaultPsv = new passiveCard();
        for (int i = 0; i < characterInfos.Length; i++)
        {
            characterInfos[i] = new characterInfo(defaultChar, defaultAtk, defaultSpc, defaultPsv);
        }*/

        characterInfos = cardManager.getDefaultCharInfo();
    }
	// Use this for initialization
	void Start () 
    {
		DontDestroyOnLoad(this);
        saveLoad = new SaveLoad();
	}

	// 
	public void SetCurrentLevel(string level)
    {
		mCurrentLevel = level;
    }

    //
	public void PrintCurrentLevel()
	{
		//Debug.Log("Current Level is: " + mCurrentLevel);
	}
}
