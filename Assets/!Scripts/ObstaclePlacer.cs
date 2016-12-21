using UnityEngine;

public class ObstaclePlacer : MonoBehaviour {

    //public variables are accessible from other scripts and in the editor
    public float randomRange = 6F;      //the distance the obstacle can shift on the x-axis (set in the editor)
    public Vector3 lastPos = Vector3.zero;      //position of parent object
    
    //private variables are not accessible from other scripts and in the editor
    private Vector3 initPos;        //saves the original position

    // Use this for initialization
    void Start ()
    {
        initPos = transform.localPosition;      //save the original position
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 pos = transform.parent.transform.position;      //get current position of the parent
        if (lastPos != pos)     //has it moved?
        {
            if (pos == Vector3.zero)        //if the platform is at the starting point, don't randomize position
                transform.localPosition = initPos;
            else    //all other positions pick a random starting location
            {
                float r = Random.Range(-randomRange, randomRange);
                pos = transform.localPosition;      //get local coordinates position
                pos.x = r;                          //set the x value to the chosen random value
                transform.localPosition = pos;      //set the local position
                
            }
        }
        lastPos = transform.parent.transform.position;      //save value of parent transform position for use later
    }
}
