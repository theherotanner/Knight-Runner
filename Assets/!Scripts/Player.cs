using UnityEngine;
using System.Threading;


public class Player : MonoBehaviour {

    //public variables are accessible from other scripts and in the editor
    public Rigidbody rb;
    public Animator anim;
    public float thrust=0;
    public float originalTrust;
    public float jumpTime = 0;
    public float groundHeight = 0;
    public float jumpPower = 2F;
    public bool bFalling = false;
    public bool bInJump = false;
    public bool bStart = false;
    public bool bLostMomentum = false;
    public float lastX = -1F;
    public int score = 0;
    public int coins = 0;
    public int highscore = 0;
    public int scoreMultiplier = 1;
    public int LastXPosition = 0;
    public bool bGameOver = false;
    public float coinTime=0;
    public Quaternion initRotation;
    public bool paused = false;
    public bool chrouch = false;
    

    //private variables are not accessible from other scripts and in the editor
    private float bSkip = 0f;
    private float jumpEnd = 0;
    private Vector3 initPos;


    public void Chrouch()
    {
        if(chrouch == false)
        {
            anim.SetBool("chrouch", true);

        }  
    }
    //used to restore the game to it's start up state

public void Reset()
    {
        chrouch = false;
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        transform.rotation = initRotation;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        anim.SetBool("bJumping", false);
        anim.SetBool("bStarted", false);
        anim.SetBool("bDied", false);
        anim.SetBool("chrouch", false);
        
        lastX = -1F;
        bFalling = false;
        bInJump = false;
        bStart = false;
        bLostMomentum = false;
        score = 0;
        coins = 0;
        scoreMultiplier = 1;
        LastXPosition = 0;
        transform.position = initPos;
        bSkip = Time.time + 0.25f;
    }


    // Use this for initialization
    void Start ()
    {
        initPos = transform.position;
        initRotation = transform.rotation;
        originalTrust = thrust;
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        Reset();
        //anim.SetBool("bDied", false);
        //anim.SetBool("bJumping", false);
        //anim.SetBool("bStarted", false);
    }

    // Update is called once per frame
    void Update()
    {
        if(chrouch == true)
        {

            Thread.Sleep(10000);

            anim.SetBool("chrouch", false);

        }
        if (Input.GetKeyDown(KeyCode.LeftShift))     //chrouch
        {
            if(chrouch == false)
            {
                anim.SetBool("chrouch", true);
                chrouch = true;
            }
        }

        if (Time.time > coinTime)
        {
            thrust = originalTrust;
        }



        Vector3 v = rb.velocity;        //get the current velocity of the rigidbody

        if (bGameOver)      //if the game is over don't do the normal updates
        {
            if (bFalling && v.x > 0)        //when falling slow player down to a stop in the x direction and keep it from wandering on the z-axis
            {
                v.x *= 0.995F;
                v.z = 0;
                rb.velocity = v;
            }

            return;
        }

        if (Time.time < bSkip)      // this prevents accidentally starting after a reset
            return;

        if (!bStart)   //waiting to begin    
        {
            if (Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Space))     //start with a touch/click or the space bar
            {
                bStart = true;
                anim.SetBool("bStarted", true);
                PlatformPlacer.bReset = false;
                CameraFlipper.audio.Play();
            }

            return;
        }


        if (!bFalling && !bInJump)      //don't allow another jump while falling or in a jump
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))     //jump with a touch/click or the space bar
            {
                bInJump = true;
                anim.SetBool("bJumping", true);
                //Jump();
                v.y = jumpPower;
                v.z = 0;
                rb.velocity = v;
                jumpEnd = Time.time + jumpTime;
            }
            v.x = thrust;
            v.z = 0;
            rb.velocity = v;
        }
        else    //not falling and jumping
        {
            if (bFalling && v.x > 0)        //slow to stop when falling
            {
                v.x *= 0.995F;
                v.z = 0;
                rb.velocity = v;
            }
            else    //apply forward thrust and keep centered on z-axis
            {
                v.x = thrust;
                v.z = 0;
                rb.velocity = v;
            }
        }
        if (bInJump && Time.time > jumpEnd)     //end the jump after specified time
        {
            bInJump = false;
            bFalling = false;
            anim.SetBool("bJumping", false);
        }
        v = transform.position;     //find player position
        if (v.y < groundHeight)     //are we below our ground plane? If so, we are falling and should die
        {
            bFalling = true;
            rb.constraints = RigidbodyConstraints.None;
            anim.SetBool("bDied", true);
        }

    }

    void LateUpdate()
    {

    }
    

    //Fixed Update happens every time the physics is updated at a fixed time rate no matter what the framerate on screen
    void FixedUpdate()
    {


        if ((transform.position.x - lastX) < 0.01F  && lastX > 2)       //if the player is losing speeed set a boolean 
            bLostMomentum = true;
        if (transform.position.x > 0)
            lastX = transform.position.x;
        //rb.AddForce(thrust,0,0,ForceMode.VelocityChange);
    }
    // pause
    

    // This function is here in case we need to delay our jump to match an animation, but is not normally used
    //public void Jump()
    //{
    //    if (!bFalling)
    //    {
    //        Vector3 v = rb.velocity;
    //        v.y = jumpPower;
    //        v.z = 0;
    //        rb.velocity = v;
    //    }
    //}
}
