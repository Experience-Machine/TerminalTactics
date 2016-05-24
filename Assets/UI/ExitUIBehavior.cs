using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExitUIBehavior : MonoBehaviour {

    public Button yesButton;
    public Button noButton;
    // Use this for initialization
    void Start () {
        yesButton = GameObject.Find("ExitUI(Clone)/Actions/Image/YesButton").GetComponent<Button>();
        noButton = GameObject.Find("ExitUI(Clone)/Actions/Image/NoButton").GetComponent<Button>();

        yesButton.onClick.AddListener(
            () =>
            {
                SceneManager.LoadScene("GameMainMenu");
            });
        noButton.onClick.AddListener(
            () =>
            {
                Destroy(this.gameObject);
            });

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
