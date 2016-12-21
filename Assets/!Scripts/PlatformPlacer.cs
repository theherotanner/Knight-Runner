using UnityEngine;
using System.Collections;

public class PlatformPlacer : MonoBehaviour {

    //public variables are accessible from other scripts and in the editor
    public float platformSpacing = 14F;     //How many units between platforms (Set in editor)
    public float numPlatforms = 4F;     //How many total platforms (Set in editor)

    static public bool bReset = false;      // one global variable to tell all platforms to return to original positions

    //private variables are not accessible from other scripts and in the editor
    private Vector3 initPos;        //saves the original position

    // Use this for initialization
    void Start ()
    {
        initPos = transform.position;        //save the original position
    }
	
	// Update is called once per frame
	void Update ()
    {
        float diff = Camera.main.transform.position.x - transform.position.x;       //calculate how far the camera is past the platform position
        if (diff > platformSpacing)     //If it is more than the platform spacing we can move the platform
        {
            Vector3 pos = transform.position;       //get our current position
            pos.x += platformSpacing * numPlatforms;        //move the platform by the spacing times the number of platforms total
            transform.position = pos;       //set new position
        }	
        if (bReset)     //if it is time to reset, move back to starting position
            transform.position = initPos;
	}
}
