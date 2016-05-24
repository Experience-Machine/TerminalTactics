using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public Button mStartButton;
    public Button mLoadButton;
    public Button mCreditsButton;
    public Button mQuitButton;

    // Use this for initialization
    void Start () {
        mStartButton = GameObject.Find("NewGame").GetComponent<Button>();
        mLoadButton = GameObject.Find("Load Game").GetComponent<Button>();
        mCreditsButton = GameObject.Find("Credits").GetComponent<Button>();
        mQuitButton = GameObject.Find("Quit").GetComponent<Button>();

        if (GameObject.Find("GlobalGameManager(Clone)") == null)
        {
            GameObject gameManager = Instantiate(Resources.Load("Prefabs/GlobalGameManager")) as GameObject;
            gameManager.transform.position = new Vector2(-100, 0);
        }

        mStartButton.onClick.AddListener(
            () =>
            {
                LoadScene("GameMainMenu");
            });

        mLoadButton.onClick.AddListener(
            () =>
            {
                //Load game then go to gamemainmenu
                GameObject.Find("GlobalGameManager(Clone)").GetComponent<GlobalGameManager>().saveLoad.LoadAllData();
                LoadScene("GameMainMenu");
            });

        mCreditsButton.onClick.AddListener(
            () =>
            {
                LoadScene("Credits");
            });

        mQuitButton.onClick.AddListener(
            () =>
            {
                Application.Quit();
            });

    }

    void LoadScene(string theLevel)
    {
        SceneManager.LoadScene(theLevel);
    }



    // Update is called once per frame
    void Update () {
	
	}
}
