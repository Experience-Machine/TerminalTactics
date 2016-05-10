using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CardUI : MonoBehaviour {
    Text cardName;
    Text cardDescription;
    Button button;

    GameObject cardPrefab;

	// Use this for initialization
	void Awake () {

        Text[] textComponents = GetComponentsInChildren<Text>();
        cardName = textComponents[0];
        cardDescription = textComponents[1];

        button = GetComponent<Button>();

        button.onClick.AddListener(
            () =>
            {
                SceneManager.LoadScene("CardSelect");
            });
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setName(string name)
    {
        cardName.text = name;
    }

    public void setDescription(string description)
    {
        cardDescription.text = description;
    }
}
