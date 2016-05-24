using UnityEngine;
using System.Collections;

public class Camerabehaviour : MonoBehaviour {

    bool hit = false;
    float mDelta = 25; // Pixels. The width border at the edge in which the movement work
    float mSpeed = 10.0f; // Speed of the movement
    private Vector3 mRightDirection = Vector3.right;

    private float cameraMaxSize = 7f;
    private float cameraMinSize = 4f;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
    
        panRight();
        panLeft();
        panUp();
        panDown();
        zoom();
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

    bool zoom()
    {
        float change = Input.GetAxis("Mouse ScrollWheel");

        Debug.Log(change);
        if (change > 0f || Input.GetKeyDown(KeyCode.Minus)) 
        {
            if (Camera.main.orthographicSize + 0.5f <= cameraMaxSize)
                Camera.main.orthographicSize += 0.5f;
        } else if (change < 0f || Input.GetKeyDown(KeyCode.Equals)) //Actually the plus key (non shift)
        {
            if (Camera.main.orthographicSize - 0.5f >= cameraMinSize) 
                Camera.main.orthographicSize -= 0.5f;
        }

        return false;
    }


}
