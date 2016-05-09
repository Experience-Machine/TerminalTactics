using UnityEngine;
// testing.....
// again......
using UnityEngine.SceneManagement;
// for SceneManager
using System.Collections;

public class GlobalGameManager : MonoBehaviour 
{

	private string mCurrentLevel = "MenuLevel";  //  

	// Use this for initialization
	void Start () 
    {
		DontDestroyOnLoad(this);
	}

	// 
	public void SetCurrentLevel(string level)
    {
		mCurrentLevel = level;
    }

    // Test comment..
	public void PrintCurrentLevel()
	{
		Debug.Log("Current Level is: " + mCurrentLevel);
	}
}
