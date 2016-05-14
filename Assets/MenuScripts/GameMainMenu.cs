using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMainMenu : MonoBehaviour {

    public Button mNextLevelButton;
    public Button mCustomizeButton;
    public Button mSaveButton;
    public Button mMenuButton;

    // Use this for initialization
    void Start()
    {
        mNextLevelButton = GameObject.Find("NextLevel").GetComponent<Button>();
        mCustomizeButton = GameObject.Find("Customize").GetComponent<Button>();
        mSaveButton = GameObject.Find("Save").GetComponent<Button>();
        mMenuButton = GameObject.Find("MainMenu").GetComponent<Button>();

        mNextLevelButton.onClick.AddListener(
            () =>
            {
                LoadScene("LevelScene");
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
                GameObject.Find("GlobalGameManager").GetComponent<GlobalGameManager>().saveLoad.SaveAllData();
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
