using UnityEngine;
using System.Collections;

public class Camerabehaviour : MonoBehaviour {

    bool hit = false;
    float mDelta = 25; // Pixels. The width border at the edge in which the movement work
    float mSpeed = 10.0f; // Speed of the movement
    private Vector3 mRightDirection = Vector3.right;

    private float cameraMaxSize = 7f;
    private float cameraMinSize = 2f;

    float xPos;
    float yPos;

    float xBoundLeft = 10.0f;
    float xBoundRight = 20.0f;

    float yBoundTop = 12.5f;
    float yBoundBottom = 6.0f;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
    
        // get the camera coordinates
        xPos = Camera.main.gameObject.transform.position.x;
        yPos = Camera.main.gameObject.transform.position.y;

        // mouse pan
        panRight();
        panLeft();
        panUp();
        panDown();

        // wasd ad arrow key support
        W();
        A();
        S();
        D();

        // zoom support
        zoom(); 
 
    }

    bool panRight() {

        if (Input.mousePosition.x >= Screen.width - mDelta) // hitting the right
        {
            if (xPos < xBoundRight)
            {
                // Move the camera
                transform.position += mRightDirection * Time.deltaTime * mSpeed;
            }
        }

        return false;
    }

    bool panLeft() {

        if (Input.mousePosition.x <= 0 + mDelta) // hitting the left
        {
            if (xPos > xBoundLeft)
            {
                transform.position -= mRightDirection * Time.deltaTime * mSpeed;
            }
        }

        return false;
    }

    bool panUp() {

        Vector3 newpos = transform.position;

        if (Input.mousePosition.y > Screen.height - mDelta) // hitting the top
        {
            if (yPos < yBoundTop)
            {
                newpos.y += Time.deltaTime * mSpeed;
                transform.position = newpos;
            }
        }

        return false;
    }

    bool panDown() {

        Vector3 newpos = transform.position;

        if (Input.mousePosition.y <= 0 + mDelta) // hitting the bottom
        {
            if (yPos > yBoundBottom)
            {
                newpos.y -= Time.deltaTime * mSpeed;
                transform.position = newpos;
            }
        }

        return false;
    }

    bool W() {
        //yPos = Camera.main.gameObject.transform.position.y;
        //Debug.Log(yPos);
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            if (yPos < yBoundTop)
            {
                transform.Translate(new Vector3(0, mSpeed * Time.deltaTime, 0));
            }
        }

        return false;
    }

    bool A()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (xPos > xBoundLeft)
            {
                transform.Translate(new Vector3(-mSpeed * Time.deltaTime, 0, 0));
            }
        }
        return false;
    }

    bool S()
    {
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if (yPos > yBoundBottom) {
                {
                    transform.Translate(new Vector3(0, -mSpeed * Time.deltaTime, 0));
                }
            }
        }

        return false;
    }

    bool D()
    {
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (xPos < xBoundRight)
            {
                transform.Translate(new Vector3(mSpeed * Time.deltaTime, 0, 0));
            }
        }

        return false;
    }

    bool zoom()
    {
        float change = Input.GetAxis("Mouse ScrollWheel");

        if (change < 0f || Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadPlus)) 
        {
            if (Camera.main.orthographicSize + 0.5f <= cameraMaxSize)
                Camera.main.orthographicSize += 0.5f;
        } else if (change > 0f || Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadMinus)) //Actually the plus key (non shift)
        {
            if (Camera.main.orthographicSize - 0.5f >= cameraMinSize) 
                Camera.main.orthographicSize -= 0.5f;
        }

        return false;
    }


}
