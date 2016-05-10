﻿using UnityEngine;
// testing.....
// again......
using UnityEngine.SceneManagement;
// for SceneManager
using System.Collections;

public class GlobalGameManager : MonoBehaviour 
{

	private string mCurrentLevel = "MenuLevel";
    private characterInfo[] characterInfos;

	// Use this for initialization
	void Start () 
    {
		DontDestroyOnLoad(this);
        characterInfos = new characterInfo[3];

        // Dummy CharInfo stuff:
        characterCard defaultChar = new characterCard("Char", "New char", 1, 1, 1, 3, 3);
        attackCard defaultAtk = new attackCard();
        specialCard defaultSpc = new specialCard();
        passiveCard defaultPsv = new passiveCard();
        for(int i = 0; i < characterInfos.Length; i++)
        {
            characterInfos[i] = new characterInfo(defaultChar, defaultAtk, defaultSpc, defaultPsv);
        }
	}

	// 
	public void SetCurrentLevel(string level)
    {
		mCurrentLevel = level;
    }

    //
	public void PrintCurrentLevel()
	{
		Debug.Log("Current Level is: " + mCurrentLevel);
	}
}
