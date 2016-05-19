using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMainMenu : MonoBehaviour {

    GlobalGameManager manager;

    public Button mNextLevelButton;
    public Button mCustomizeButton;
    public Button mSaveButton;
    public Button mMenuButton;

    // Use this for initialization
    void Start()
    {
        manager = GameObject.Find("GlobalGameManager(Clone)").GetComponent<GlobalGameManager>();
        mNextLevelButton = GameObject.Find("NextLevel").GetComponent<Button>();
        mCustomizeButton = GameObject.Find("Customize").GetComponent<Button>();
        mSaveButton = GameObject.Find("Save").GetComponent<Button>();
        mMenuButton = GameObject.Find("MainMenu").GetComponent<Button>();

        mNextLevelButton.onClick.AddListener(
            () =>
            {
                if (manager.level > manager.NUMBER_OF_LEVELS)
                    manager.level = 1;
    
                LoadScene("Level" + manager.level);
            });

        mCustomizeButton.onClick.AddListener(
            () =>
            {
                LoadScene("CharCustomization");
            });

        mSaveButton.onClick.AddListener(
            () =>
            {
                //Save here
                GameObject.Find("GlobalGameManager(Clone)").GetComponent<GlobalGameManager>().saveLoad.SaveAllData();
            });

        mMenuButton.onClick.AddListener(
            () =>
            {
                LoadScene("MainMenu");
            });

    }

    void LoadScene(string theLevel)
    {
        SceneManager.LoadScene(theLevel);
    }



    // Update is called once per frame
    void Update()
    {

    }
}
