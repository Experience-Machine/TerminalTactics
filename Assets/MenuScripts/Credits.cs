using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public Button mReturnButton;
    // Use this for initialization
    void Start()
    {
        mReturnButton = GameObject.Find("CreditsButton").GetComponent<Button>();

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