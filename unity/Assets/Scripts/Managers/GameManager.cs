using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    
    /*  
     Balancing, all variables associated with balancing
    */
    public float cubeSegmentTransitioningTime = 5.0f; // amount that the animation speed gets mutliplied with --> the higher the value the faster the folding
    public bool allowClickWhileTransitioning; //TODO should this be in cubeSegment-class as static? but more comfortable in unity to put here for tweaking...
    public float roundDuration = 10.0f; //the amount of time in which cube must be completed
    public bool canFoldInward = true; //true as long as no cube-segment is currenty folded inward TODO: replace with states/events
    public float amountOfPointsToWinRound = 20; // the number that needs to be... reduced to 0 to win
    public float pointsForTrappedProjectile = 10;
    public int[] points = { 0, 1, 2, 3, 7 }; // amount of points that gets added on depending on how many segments are folded; 0 segements = 0 points per second, 4 segments = 7 points per second
    
    
    private static GameManager gameManagerInstance = null; //variable which keeps (only) instance of this singleton
    private SoundManager _soundManager;
    public ProjectileManager projectileManager; //needed for removing flying projectiles  TODO: merge into one?
    public GameObject[] cubeSegments; //TODO: why is this necessary? refactor?


    public int victories = 0;
    public int losses = 0;
    public int currentRound = 0; //round we are currently playing in
    private bool canControl = true; //states if player can click on segments eg. in case of game over events TODO: replace with states/events
    private bool gameJustStarted = true;  //marks if the game hast just been (re)started, needed for knowing whether to startcouroutine for spawning projectiles etc. TODO: state?
    public int segmentsFolded = 0; //number of segments that are currently folded (in)
    public float roundTimeProgress;
    public float pointsLeftToWin;
    public int pointsMultiplier = 1;
    private float restartAtTime; //time at which next time-over-restart occurs
    private float timeLeft; //time left in current round
    

    public GameObject guiClock; //object that displays passed game time in seconds
    public GameObject guiScore; //object that displays the score
    public GameObject progressBar; // the gameobjects used for the visual representation of the progress bar
    public GameObject timeBar; // the gameobjects used for the visual representation of the time bar
    private bool isGamePaused = false;
    private GUISkin guiSkin;//standard skin for gui


    private GameManager() //ctor is private in order to ensure single-instanciation
    {

    }

    public static GameManager getInstance() //method to return the singleton-instance, accessible for errrrrybody
    {
        if (gameManagerInstance == null)
        {
            gameManagerInstance = (GameManager)FindObjectOfType(typeof(GameManager)); //TODO: ?! wtf is that?
        }

        return gameManagerInstance;

    }

    private EventManager _eventManager;




    public void OnApplicationQuit()
    {
        gameManagerInstance = null;
    }




    void Start()
    {

        resetPointsLeft();



        roundTimeProgress = (restartAtTime - Time.time) / roundDuration;

        GameManager.getInstance();
        _eventManager = EventManager.getInstance();

        restartAtTime = Time.time + roundDuration; //set point in time for first time-over-restart
        _eventManager.addListener(GameManager_OnFolded, EventManager.eventName.OnFolded);
        _eventManager.addListener(GameManager_OnFolded, EventManager.eventName.OnFoldedOut);
        _eventManager.addListener(GameManager_OnFolded, EventManager.eventName.OnFoldedInward);
        _eventManager.addListener(GameManager_OnProjectileToSegmentCollision, EventManager.eventName.OnMissileToSegmentCollision);
        _eventManager.addListener(GameManager_OnProjectileTrapped, EventManager.eventName.OnProjectileTrapped);

        _eventManager.addListener(GameManager_OnRestart, EventManager.eventName.OnRestart);
        _eventManager.addListener(GameManager_OnRestartWin, EventManager.eventName.OnRestartWin);
        _eventManager.addListener(GameManager_OnRestartLoss, EventManager.eventName.OnRestartLoss);



    }

    private void GameManager_OnRestartWin(GameObject g, EventArgs e)
    {
        currentRound++;

        projectileManager.rndSpeedUpper += 0.1f;

        if (currentRound % 2 == 0)
        {
           // Debug.Log("doing stuff that's done every 2 rounds");
            projectileManager.rndSpeedLower += 0.1f;
        }

        if (currentRound % 5 == 0) {
         //   Debug.Log("doing stuff that's done every 5 rounds");
            projectileManager.spawnInterval -= 0.2f;    
        }
    }

    private void GameManager_OnRestartLoss(GameObject g, EventArgs e)
    {
       projectileManager.removeAllPowerups();
       currentRound = 0;
       projectileManager.schedulePosition = 0; // TODO: ugly hack
       
       //reset to testing default starting difficulty 
       projectileManager.rndSpeedUpper = 1f;
       projectileManager.rndSpeedLower = 0.8f;
       projectileManager.spawnInterval = 2.0f;   


    }

    private void GameManager_OnRestart(GameObject g, EventArgs e)
    {
        projectileManager.removeAllMissiles();
        //Debug.Log(timeBar.animation["anim_timeBar_turnRed"]);
        timeBar.animation["anim_timeBar_turnRed"].time = 0;
        timeBar.animation["anim_timeBar_turnRed"].speed = 0;
    }

    void GameManager_OnProjectileTrapped(GameObject g, EventArgs e)
    {
        pointsLeftToWin -= pointsForTrappedProjectile;
    }


    void GameManager_OnProjectileToSegmentCollision(GameObject g, EventArgs e) //TODO: pass state in EventArgs?
    {

        canControl = false;
        checkForRoundEnd(true);

    }

    void GameManager_OnFolded(GameObject g, EventArgs e) //TODO: pass state in EventArgs?
    {

        FoldingEventArgs state = e as FoldingEventArgs;

        // decrement number of folded segment if the currentState is no longer folded AND the previousState WAS folded (so that # of folded segment doesn't go below 0 when segment is clicked during transition)
        if ((CubeSegment.SegmentStates)state.currentState != CubeSegment.SegmentStates.Folded && (CubeSegment.SegmentStates)state.previousState == CubeSegment.SegmentStates.Folded)
        {
            segmentsFolded--;
        }
        //increment # of segements folded if the currentState is folded, and the previousState was not.
        else if ((CubeSegment.SegmentStates)state.currentState == CubeSegment.SegmentStates.Folded && (CubeSegment.SegmentStates)state.previousState != CubeSegment.SegmentStates.Folded)
        {
            segmentsFolded++;
        }

    }



    private void resetPointsLeft()
    {
        pointsLeftToWin = amountOfPointsToWinRound;
    }

    private void updateProgressBars()
    {


        roundTimeProgress = (restartAtTime - Time.time) / roundDuration;

        progressBar.renderer.material.mainTextureOffset = new Vector2((1 - pointsLeftToWin / amountOfPointsToWinRound) + 0.05f, 0); //+0.05f so that bar gets "full" at the the end (bc of gradient)


        timeBar.renderer.material.mainTextureOffset = new Vector2(roundTimeProgress, 0);

        if (roundTimeProgress < 0.7) {
            
            timeBar.animation.Blend("anim_timeBar_turnRed");
            timeBar.animation["anim_timeBar_turnRed"].time = timeBar.animation["anim_timeBar_turnRed"].length * (1 - (roundTimeProgress+0.3f));
            //Debug.Log("frame: " + timeBar.animation["anim_timeBar_turnRed"].time);
        }


        if (roundTimeProgress < 0.3)
        {
            timeBar.animation.Blend("anim_timeBar_blink");
            timeBar.animation["anim_timeBar_blink"].speed = 4 + (2 * roundTimeProgress);
            //timeBar.animation["anim_timeBar_turnRed"].time = timeBar.animation["anim_timeBar_turnRed"].length * (1 - roundTimeProgress);
        }




    }

    void Update()
    {

        if (canControl)
        {
            calculatePoints();
            timeLeft = restartAtTime - Time.time;
            updateProgressBars();
            checkForRoundEnd();
        }

        //pause game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isGamePaused = true;
            Time.timeScale = 0;

            //restart game functionality?
        }


        if (gameJustStarted) //if the game has just been (re)started, restart coroutine for spawning projectiles
        {
            projectileManager.spawnProjectiles = true;
            StartCoroutine(projectileManager.spawnProjectile());


            gameJustStarted = false;
        }

        guiClock.GetComponent<TextMesh>().text = Mathf.Round(timeLeft + 0.5f).ToString(); //only use this for debugging, string-conversion in update might be pretty fucking bad  (+0.5f so that every number is visible same time, since its only to display it doesn't matter)
        guiScore.GetComponent<TextMesh>().text = "Stage: " + currentRound.ToString(); //only use this for debugging



    }

    private void calculatePoints()
    {
        pointsLeftToWin -= points[segmentsFolded] * pointsMultiplier * Time.deltaTime;
    }

    public void increasePoints(int value) //actually this DEcreases points, but for easier understandig called 'increasePoints'
    {
        pointsLeftToWin -= value;
    }

    private void cleanUp()
    {

        _eventManager.dispatchEvent(this.gameObject, EventArgs.Empty, EventManager.eventName.OnRestart);

        if (victoryAchieved())
        {
            _eventManager.dispatchEvent(this.gameObject, EventArgs.Empty, EventManager.eventName.OnRestartWin);
        }
        else {
            _eventManager.dispatchEvent(this.gameObject, EventArgs.Empty, EventManager.eventName.OnRestartLoss);
        }

        //disable user control
        canControl = false;



        projectileManager.spawnProjectiles = false;
    }

    private void reset()
    {
        //reset all cube segments
        resetAllCubeSegments();

        //disable all coroutines (missile-spawning) so that coroutines for spawning projectiles don't overlap
        StopAllCoroutines();

        //set gameJustStarted to true and stop spawning-coroutine
        gameJustStarted = true;

        //reset number of foleded segments
        segmentsFolded = 0;

        //return control over cube-segments to the player
        canControl = true;


        restartAtTime = Time.time + roundDuration;

        canFoldInward = true;

        //amountOfPointsToWinRound = 20 + currentRound * 5; // increment difficulty, for testing-purposes only; currentRound*5 is the number of more points which have to be achieved in order to win the round; 20 = hardcoded value of points for round 1

        resetPointsLeft();


    }

    public IEnumerator restartGame(float waitForSeconds) //reset everything to the game's starting conditions
    {

        cleanUp();
        yield return new WaitForSeconds(waitForSeconds);
        reset();

    }

    void resetAllCubeSegments()
    {
        //set all cube segments to their initial state (0,0,0)-rotation using their reset()-function
        foreach (GameObject cubeSegment in cubeSegments)
        {
            cubeSegment.GetComponent<CubeSegment>().reset();
        }
    }



    private void checkForRoundEnd(bool projectileHit = false)
    {
        if (victoryAchieved() | timeRanOut() | projectileHit)
        {
            if (victoryAchieved())
            {
                victories++;

            }
            else
            {
                losses++;
            }
            StartCoroutine(restartGame(0.4f));
        }

    }

    private bool victoryAchieved()
    {
        return (pointsLeftToWin <= 0) ? true : false;

    }


    private bool timeRanOut() //checks when to force a restart based on time over
    {

        return (timeLeft <= 0) ? true : false;


    }

    public bool getControlState()
    {
        return canControl;
    }

    private void DrawPauseMenu(int windowID)
    {

        //GUI.Box(new Rect(0, 0, Screen.width, Screen.height), " ");

        //GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        //GUILayout.BeginHorizontal();

        if (GUILayout.Button("Resume"))
        {
            isGamePaused = false;
            Time.timeScale = 1.0f;
        }

        if (GUILayout.Button("Back to Main Menu"))
            Application.LoadLevel(GlobalNames.SCENE_ID_MAINMENU);
        if (GUILayout.Button("Quit"))
            Application.Quit();

        //GUILayout.EndHorizontal();
        //GUILayout.EndArea();	
    }

    void OnGUI()
    {
        GUI.skin = guiSkin;

        //draw pause menu if game is paused
        if (isGamePaused)
            GUI.Window(0, new Rect(Screen.width / 2, Screen.height / 2, 400, 400), DrawPauseMenu, "PauseMenu");
    }
}
