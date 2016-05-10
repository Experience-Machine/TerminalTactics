using UnityEngine;
using System.Collections;

public class CharacterCustomization : MonoBehaviour {
    GameObject cardPrefab;
    GameObject myCanvas;
	// Use this for initialization
	void Start () {
        myCanvas = GameObject.Find("Canvas");
	    cardPrefab = Instantiate(Resources.Load("Prefabs/Card")) as GameObject;
        cardPrefab.transform.SetParent(myCanvas.transform, false);
        CardUI uiComponent = cardPrefab.GetComponent<CardUI>();

        uiComponent.setName("Test name");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
