using UnityEngine;
using System.Collections;


public class RewardObjectspecial : MonoBehaviour {

    //public variables are accessible from other scripts and in the editor
    public float rotateSpeed = 1;       //how fast should it spin? (set in editor)

    public float randomRange = 2.5F;        //the distance the reward can shift on the x-axis (set in the editor)
    public Vector3 lastPos = Vector3.zero;      //position of parent object
    public GameObject playerGO;     //fill this with the player root game object in the editor
    public AudioClip rewardSound;     //fill this with the audioclip to play when collected in the editor

    //private variables are not accessible from other scripts and in the editor
    private Vector3 initPos;        //saves the original position
    private Player player;      //stores the player class reference
    private bool bStartingOver = false;     //tells whether we are starting over or not
    
    // Use this for initialization
    void Start()
    {
        player = playerGO.GetComponent<Player>();      //store the player class reference
        
        initPos = transform.localPosition;      //save the original position
    }

    // Update is called once per frame
    void Update()
    {
        if (bStartingOver && !player.bStart)        //clear starting over once the player is reset
        {
            bStartingOver = false;      //clear state
            lastPos = Vector3.one;      //set to a value that will cause updates to occur
        }

        transform.Rotate(new Vector3(0F, 1F * rotateSpeed, 0F), Space.World);       //rotate the object

        Vector3 pos = transform.parent.transform.position;      //get parent object position
        if (lastPos != pos)     //has the parent moved?
        {
            if (pos == Vector3.zero)        //if the platform is at the starting point, don't randomize position
                transform.localPosition = initPos;
            else    //all other positions pick a random starting location
            {
                float r = Random.Range(-randomRange, randomRange);
                pos = transform.localPosition;      //get local coordinates position
                pos.x = r+initPos.x;                //set the x value to the starting position plus the chosen random offset value
                pos.y = initPos.y;                  //return the y to its starting value
                transform.localPosition = pos;      //set the local position
            }
        }
        lastPos = transform.parent.transform.position;      //save value of parent transform position for use later
    }

    //this function is called when the collider is entered by another object's collider and the "is trigger" box is checked on this object's collider
    void OnTriggerEnter(Collider other)
    {
        
        bStartingOver = true;       //we need to start over when moved
        Vector3 pos = transform.localPosition;          //get local position
        pos.y = -1000;                                  //move this object way down below and out of sight
        transform.localPosition = pos;                  //set local position
        player.thrust += 3;           //increase the value of the thrust "speed"
        CameraFlipper.audio.PlayOneShot(rewardSound);   //play the reward pickup sound
        player.coinTime = Time.time + 12;
        player.coins++;
    }
}
