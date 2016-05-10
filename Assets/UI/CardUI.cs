using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CardUI : MonoBehaviour {
    Text cardName;
    Text cardDescription;
    Card card;
    GlobalGameManager.CardType type;
    Button button;

    GameObject cardPrefab;

	// Use this for initialization
	void Awake () {
        type = GlobalGameManager.CardType.None;
        Text[] textComponents = GetComponentsInChildren<Text>();
        cardName = textComponents[1];
        cardDescription = textComponents[2];

        button = GetComponent<Button>();

        button.onClick.AddListener(
            () =>
            {
                GlobalGameManager.cardTypeSelected = type;
                SceneManager.LoadScene("CardSelect");
            });
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    public void setCard(Card c)
    {
        card = c;
        if (c is attackCard)
            type = GlobalGameManager.CardType.Attack;
        else if (c is characterCard)
            type = GlobalGameManager.CardType.Character;
        else if (c is passiveCard)
            type = GlobalGameManager.CardType.Passive;
        else if (c is specialCard)
            type = GlobalGameManager.CardType.Special;

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
