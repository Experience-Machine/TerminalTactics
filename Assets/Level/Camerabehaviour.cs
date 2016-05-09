using UnityEngine;
using System.Collections;

public class Camerabehaviour : MonoBehaviour {

    bool hit = false;
    float mDelta = 25; // Pixels. The width border at the edge in which the movement work
    float mSpeed = 10.0f; // Speed of the movement
    private Vector3 mRightDirection = Vector3.right;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
    
        panRight();
        panLeft();
        panUp();
        panDown();
    }

    bool panRight() {

        if (Input.mousePosition.x >= Screen.width - mDelta) // hitting the right
        {
            // Move the camera
            transform.position += mRightDirection * Time.deltaTime * mSpeed;
        }

        return false;
    }

    bool panLeft() {

        if (Input.mousePosition.x <= 0 + mDelta) // hitting the left
        {
            transform.position -= mRightDirection * Time.deltaTime * mSpeed;
        }

        return false;
    }

    bool panUp() {

        Vector3 newpos = transform.position;

        if (Input.mousePosition.y > Screen.height - mDelta) // hitting the top
        {
            newpos.y += Time.deltaTime * mSpeed;
            transform.position = newpos;
        }

        return false;
    }

    bool panDown() {

        Vector3 newpos = transform.position;

        if (Input.mousePosition.y <= 0 + mDelta) // hitting the bottom
        {
            newpos.y -= Time.deltaTime * mSpeed;
            transform.position = newpos;
        }

        return false;
    }


}
