using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HowToPlay : MonoBehaviour {

    public Button mReturnButton;
    // Use this for initialization
    void Start()
    {
        mReturnButton = GameObject.Find("ReturnButton").GetComponent<Button>();

        mReturnButton.onClick.AddListener(
            () =>
            {
                LoadScene("MainMenu");
            });
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LoadScene(string theLevel)
    {
        SceneManager.LoadScene(theLevel);
        //        FirstGameManager.TheGameState.SetCurrentLevel(theLevel);

    }
}
