using UnityEngine;
using System.Collections;

public class MouseManager : MonoBehaviour {

	// Use this for initialization
	void Start () {

        //Debug.Log("turn is " + GameController.instance.TurnNumber);
	
	}
	
	// Update is called once per frame
	void Update () {
	    //check for mouse pressed down this frame
        if(Input.GetMouseButtonDown(0))
        {
            //left mouse button pressed
            //get a ray from our camera pointed in the direction to where the mouse cursor is located on the screen.
            Ray mouseray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(mouseray, out hitInfo))
            {
                Debug.Log("Hit Mesh:" + hitInfo.collider.gameObject.name);
            }
        }
	}
}
