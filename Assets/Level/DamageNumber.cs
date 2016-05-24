using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DamageNumber : MonoBehaviour {
    Text number;
    float delay;
    Color colorBegin = Color.white;
    Color colorEnd = new Color(1.0f, 1.0f, 1.0f, 0f);

    Vector3 startPosition;
    Vector3 endPosition;
    float currentLerpTime = 0;

    
	// Use this for initialization
	void Awake () {
        this.transform.SetParent(GameObject.Find("Canvas").transform);
        number = GetComponent<Text>();

	}

    public void setNumber(string number)
    {
        this.number.text = number;
    }

    public void setPosition(Vector3 position)
    {
        transform.position = position;
        startPosition = position;
        endPosition = new Vector3(position.x, position.y + 100, position.z);

    }
	
	// Update is called once per frame
	void Update () {

        currentLerpTime += Time.deltaTime;
        transform.position = Vector3.MoveTowards(startPosition, endPosition, 5.0f * currentLerpTime);

            delay += Time.deltaTime;
        number.color = Color.Lerp(colorBegin, colorEnd, delay);
        
        if (number.color.a <= 0)
        {
            Destroy(this.gameObject);
        }
	}

    public static void createDamageNumber(string number, Vector3 position)
    {
        GameObject damageNumber = Instantiate(Resources.Load("Prefabs/DamageText")) as GameObject;
        DamageNumber damageUi = damageNumber.GetComponent<DamageNumber>();
        damageUi.setPosition(Camera.main.WorldToScreenPoint(position));
        damageUi.setNumber(number);
    }
}
