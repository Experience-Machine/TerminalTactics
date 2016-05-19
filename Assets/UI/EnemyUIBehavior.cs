using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyUIBehavior : MonoBehaviour {

    private float maxHealth;
    private float curHealth;

    void Start()
    {

    }

	// Update is called once per frame
	void Update ()
    {
	
	}

    public void setContent(Sprite s, float maxHealth, float minHealth, string characterName)
    {
        this.maxHealth = maxHealth;
        this.curHealth = minHealth;

        GameObject charPortrait = GameObject.Find("EnemyUI(Clone)/Character Info/Image/Image");

        Image i = charPortrait.GetComponent<Image>();
        i.sprite = s;

        GameObject textComp = GameObject.Find("EnemyUI(Clone)/Character Info/Image/Panel (1)/CharName");
        Text charName = textComp.GetComponent<Text>();
        charName.text = characterName;

        GameObject healthBar = GameObject.Find("EnemyUI(Clone)/Character Info/Image/Panel (2)/HealthBar/Panel");
        RectTransform transform = healthBar.GetComponent<RectTransform>();
        float size = (curHealth / maxHealth) * 240f;
        
        transform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, size);
        Image image = healthBar.GetComponent<Image>();
        image.color = UnityEngine.Color.red;
    }
}
