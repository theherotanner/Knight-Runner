using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Threading;

public class CameraFlipper : MonoBehaviour {

    //public variables are accessible from other scripts and in the editor
    public GameObject playerGO;
    public Text scoreText;
    public Text distanceText;
    public Text scrText;
    public Text hsText;
    public Text coinText;
    public Text highscoreText;
    public Button gameOverButton;
    public AudioClip deathClip;
    public Button menuButton;
    public GameObject game;
    public GameObject rwing;
    public Player cpGO;
    
    
    static public AudioSource audio;        //the static keyword means their is only one variable used by all the instances of a class

    //private variables are not accessible from other scripts and in the editor
    private float lastAspectRatio = 0F;
    private float xOffset = 2F;
    private float lastX = 0F;
    private float speed = 0F;
    private float topSpeed = -1F;
    private Player player;
    private float slowdownTime;
    private float gameOverTime = 0F;
    private bool haveWrittenHighestScore = false;
    private bool didJump = false;

    public void Reset()
    {
        Restart();
    }
    public void LeaveMenu()
    {
        Thread.Sleep(100);
        audio.Play();      //play music
        player.bStart = true;
        Thread.Sleep(1000);
        player.anim.SetBool("bStarted", true);
        player.anim.SetBool("bDied", false);
        PlatformPlacer.bReset = false;
        player.paused = false;
    }
    public void Menu()
    {
        player.paused = true;
        audio.Stop();       //stop music
        
    }
    public void EndGame()
    {
        Application.Quit();
    }

    // Use this for initialization
    void Start ()
    {
        audio = GetComponent<AudioSource>();
        player = playerGO.GetComponent<Player>();
        if (File.Exists("highscore.txt"))
        {
            StreamReader r = new StreamReader("highscore.txt");
            string highscoreFile = r.ReadToEnd();
            r.Close();

            player.highscore = int.Parse(highscoreFile);
        }
        else
        {
            player.highscore = 0;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))     //pause when you hit the esc key
        {
            Menu();
            menuButton.gameObject.SetActive(false);
            player.gameObject.SetActive(false);
            game.gameObject.SetActive(true);
            rwing.gameObject.SetActive(true);
        }
        if (player.score >= player.highscore)
        {
            player.highscore = player.score;
        }
        if (!player.bGameOver)      //if the game is in progress, write the score and distance and check if we need to die
        {
            int s = (int)player.transform.position.x;
            distanceText.text = s.ToString() + " Meters";
            player.score += (s - player.LastXPosition) * player.scoreMultiplier;
            player.LastXPosition = s;
            scoreText.text = "Score: " + player.score.ToString();
            scrText.text = "Your_Score: " + player.score.ToString();
            coinText.text = player.coins.ToString() + ":Coins";
            highscoreText.text =  "High-Score:" + player.highscore.ToString();
            hsText.text = "High-Score:" + player.highscore.ToString();


            if (player.bLostMomentum && player.paused == false)       //lost momentum so time to die
            {
                player.bGameOver = true;        //set game over state
                player.thrust = player.originalTrust;
                gameOverTime = Time.time + 1;       //set a timer for when to show game over message
                player.anim.SetBool("bDied", true);     //change to death animation
                player.anim.SetBool("bJumping", false);
                audio.Stop();       //stop music
                audio.PlayOneShot(deathClip);       //play death sound
            }
        }
        else    //Game over is true
        {
            if (Time.time > gameOverTime)       //show game over message when timer runs out
                gameOverButton.gameObject.SetActive(true);
                scoreText.gameObject.SetActive(false);
                highscoreText.gameObject.SetActive(false);
                coinText.gameObject.SetActive(false);
                distanceText.gameObject.SetActive(false);
                menuButton.gameObject.SetActive(false);


            if (haveWrittenHighestScore == false)
            {
                haveWrittenHighestScore = true;
                StreamWriter w = new StreamWriter("highscore.txt");
                w.Write(player.highscore.ToString());
                w.Close();

            }

        }

        if (Camera.main.aspect != lastAspectRatio)      //check the current aspect ratio vs. the last one to see if screen size or rotation has occurred
        {
            lastAspectRatio = Camera.main.aspect;
            if (lastAspectRatio < 1)        //taller than it is wide?
                SetPortrait();
            else
                SetLandscape();
        }
        else    //no aspect change so update camera to track player
        {
            Vector3 pos = transform.position;   //get camera position

            if (!player.bFalling)       //not falling so follow exactly
            {
                pos.x = playerGO.transform.position.x + xOffset;
                transform.position = pos;
                slowdownTime = Time.time + 2F;      //reset slow down time to 2 seconds
            }
            else    //we are falling
            {
                if (!player.bGameOver)      //first time here so set game over state
                {
                    player.bGameOver = true;        //set game over state
                    player.thrust = player.originalTrust;
                    gameOverTime = Time.time + 1;       //set a timer for when to show game over message
                    player.anim.SetBool("bDied", true);     //change to death animation
                    audio.Stop();       //stop music
                    audio.PlayOneShot(deathClip);       //play death sound
                }

                if (slowdownTime > Time.time)       //Slow camera to a stop
                    pos.x += Mathf.Lerp(speed, 0F, slowdownTime-Time.time);
                transform.position = pos;
            }
        }

    }

    //set camera to landscape mode (wider than tall)
    void SetLandscape()
    {
        transform.position = new Vector3(playerGO.transform.position.x, 5F, -10F);
        transform.eulerAngles = new Vector3(10F, 0F, 0F);
        xOffset = 2F;
    }

    //set camera to portrait mode (taller than wide)
    void SetPortrait()
    {
        transform.position = new Vector3(playerGO.transform.position.x -2F, 6F, 0F);
        transform.eulerAngles = new Vector3(30F, 90F, 0F);
        xOffset = -4F;
    }
    
    //Retrun everything to start up state
    public void Restart()
    {
        PlatformPlacer.bReset = true;
        haveWrittenHighestScore = false;
        speed = 0;
        topSpeed = 0;
        lastX = 0;
        player.Reset();
        player.bGameOver = false;
        gameOverButton.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);
        highscoreText.gameObject.SetActive(true);
        coinText.gameObject.SetActive(true);
        distanceText.gameObject.SetActive(true);
        menuButton.gameObject.SetActive(true);
        menuButton.gameObject.SetActive(true);
        player.gameObject.SetActive(true);
        game.gameObject.SetActive(false);
        rwing.gameObject.SetActive(false);
        
    }

}
