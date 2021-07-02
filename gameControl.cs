using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public class gameControl : MonoBehaviour
{
    //This isntance used to track whether or not an instance is active
    public static gameControl instance;

    //public float xScale = 1;
    //public float yScale = 1;
    //public int screenHeight;
    //public int screenWidth;
    
    //the factor of the timescale of the game
    public float timeScaleFactor = 1;
    
    
    public GameObject gameOverText;
    public bool gameOver = false;

    public int score = 0;
    public GameObject scoreTextObject;
    public Text scoreText;

    //speed the game scrolls at
    public float scrollSpeed = 2.0f;

    
    public GameObject player;
    private Character playerScript;
    
    public Text highScoreText;
    
    //script that gathers and saves the games data
    private GameData gameDataScript;
    
    //number of sides to start the game with
    public int startingSides = 3;

    //assortment of buttons and objects
    public GameObject mainMenu;
    public GameObject upgradeMenu;
    public GameObject statsMenu;
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public GameObject pauseButton;
    public GameObject unpauseButton;
    public GameObject creditsButton;
    public GameObject creditsMenu;
    public GameObject characterObject;
    public GameObject floorGenerator;
    public GameObject MainMenuUI;
    public GameObject GamePlayUI;
    public GameObject unpauseTimer;
    public GameObject darkenScreen;
    public GameObject cam;
    public GameObject pointsObject;
    public GameObject viewPathButton;
    public GameObject showMenuButton;
    public GameObject falseCode;
    public GameObject codeApproved;

    //scene names
    public string gameplayScene = "GameplayScene";
    public string menuScene = "MainMenuScene";
    public string upgradeScene = "UpgradeScene";

    public bool saved = false;

    //script and text that gets managed in this script
    public followScript cameraScript;
    public Text points;
    public Text statsText;
    public string pointsText;
    //num of revives left
    public int revives;
    
    //unlockables
    public bool[] sidesUnlocked = new bool[16];
    public int highestUnlocked = 4;

    //current scene name?
    public string sceneName = "SampleScene.unity";

    //data variables to be kept track of to save/record
    public int highScore;
    public long secondsPlayed;
    public string name = "Enter Name";
    public int numPoints;
    public float highestHeight;
    public float highestLength;
    public float totalLength;
    public float totalHeight;
    public int totalScore;
    public string versionNumber = "0.01a";
    public float lowestHeightModifierUnlocked;
    public float lowestLengthModifierUnlocked;
    public bool colorModifierUnlocked = false;
    public bool gradientUnlocked = false;
    public bool noAds = false;
    public DateTime lastPlayed = DateTime.Now;
    public DateTime currentTime = DateTime.Now;
    public float length;
    public float height;
    public long frameCount;
    public int attempts;
    public Text deathScore;
    public bool[] alreadyEncounteredGen = new bool[3];
    public int floorsSpawned = 0;
    public int stairsSpawned = 0;
    public int gapsSpawned = 0;
    public int slopesSpawned = 0;
    public int[] upgradesBought;
    public int numUpgrades;
    public int numUpgradesBought;
    public int numItemsBought;
    public int numItemsUsed; 
    public bool devMode = false;
    

    //state 0 = main menu, 1 = gameplay, 2 = pause menu, 3 = store, 4 = settings, 5 = death
    public int state = 0;
    public int previousState = 0;
   
    //code in progress below

    public const float E = 2.71828175f;
    
    public string promoCode = "";

    public string[] promoCodesArray;

    public int revivesLeft = 0;




    void Awake() {
        //initialize variables, load data, set data, and start on the main menu screen in proper positioning
        Application.targetFrameRate = 60;
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy (gameObject);
        }
        upgradesBought = new int[numUpgrades];
        Array.Clear(upgradesBought, 0, numUpgrades);
        gameDataScript = GetComponent<GameData>();
        gameDataScript.LoadGame();
        Debug.Log(upgradesBought);
        if (upgradesBought == null || upgradesBought.Length == 0) {
            upgradesBought = new int[numUpgrades];
            Array.Clear(upgradesBought, 0, numUpgrades);
            Debug.Log(upgradesBought);
            foreach( var upgrade in upgradesBought )
                {
                    Debug.Log( upgrade );
            }
        }
        cameraScript = cam.GetComponent<followScript>();
        
        //possibly legacy
        if(state == 1) {
            playerScript = player.GetComponent<Character>();
            //Debug.Log(highScore);
            highScoreText.text = "High Score: " + highScore.ToString();
        }
        
        Resolution[] resolutions = Screen.resolutions;
        //screenWidth = Screen.width;
        //screenHeight = Screen.height;

        //update sides unlocked
        for( int i = 0; i < sidesUnlocked.Length; i++) {
            if( upgradesBought[0] + 3 >= i) {
                sidesUnlocked[i] = true;
            }
            else {
                sidesUnlocked[i] = false;
            }
        }

        //yScale = 1/(screenHeight/1080);
        //xScale = 1/(screenWidth/1920);
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        //update the number of frames played, to keep track of amount of time actually spent playing the game
        frameCount++;
    }

    void Update() {
        /*
        if (gameOver && Input.GetMouseButtonDown(0)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        */
        //update the number of points each frame to have the actual number of points active on screen
        pointsText = "Points: " + numPoints.ToString();
        points.text = pointsText;     
    }

    public void scored() {
        //dont score if game overed
        if (gameOver) return;
        //otherwise score and update score and timeScale if it needs an update
        score++;
        scoreText.text = "Score: " + score.ToString();
        //timeScaleFactor *= 1.001f;
        Time.timeScale = timeScaleFactor;
    }

    public void playerDied() {
        // if there are revives, revive the player in a position behind where they last were, and let them continue playing

        if (revives >= 1) {
            height = playerScript.tf.position.y;
            length = playerScript.tf.position.x;
            //enterPlay();
            //Destroy(player);
            //player = Instantiate(characterObject, new Vector3(-20, .8f, 1), Quaternion.identity); 
            //playerScript.tf.position = new Vector3(length - 10, height + 5)
            //playerScript = player.GetComponent<Character>();
            gameOver = false;
            //score = 0;
            //scoreText.text = "Score: " + score.ToString();
            cameraScript.i = 0;
            // TODO cameraScript.reviving = true;
            cameraScript.player = player;
            floorGenerator.GetComponent<FloorGen>().character = player;
            //floorGenerator.GetComponent<FloorGen>().restartRun();
            floorGenerator.GetComponent<FloorGen>().numSpawned = 0;
            floorGenerator.GetComponent<FloorGen>().sides = 3;
        }
        //otherwise the player dies, save the data, and all the progress they were able to achieve, and set the gameover screen settings to active

        else {
            height = playerScript.tf.position.y;
            length = playerScript.tf.position.x;
            gameOverText.SetActive(true);
            gameOver = true;
            numPoints += score;
            totalScore += score;
            totalLength += length;
            totalHeight += height;
            Time.timeScale = 1;
            timeScaleFactor = 1.0f;
            string deathScoreText = "You got " + score + " points this run!";
            if(score > highScore) {
                highScoreText.text = "High Score: " + score.ToString();
                highScore = score;
                deathScoreText += "\n THATS A NEW HIGHSCORE!";
            }
            if(height > highestHeight) {
                highestHeight = height;
            }
            if(length > highestLength) {
                highestLength = length;
            }
            deathScore.text = deathScoreText;
            secondsPlayed += frameCount/60;
            gameDataScript.update();
            gameDataScript.SaveGame();
            //Debug.Log(numPoints + " GC");
            //Debug.Log(gameDataScript.numPoints + " Save data");
            pauseButton.SetActive(false);
            viewPathButton.SetActive(true);
        }
    }

    void OnApplicationPause(bool pauseStatus) {
        //if the app pauses save and reload the game (for mobile, untested) this should make it so that while the app is in background stats dont get updated
        if (pauseStatus) {
            gameDataScript.SaveGame();
        }
        else {
            gameDataScript.StartLoadGame();
        }
    }

    //Overlay settings to show preset overlays and settings to enter for certain states of the game
    public void hideGameOver() {
        gameOverText.SetActive(false);
        showMenuButton.SetActive(true);
        viewPathButton.SetActive(false);
        scoreTextObject.SetActive(false);
        pointsObject.SetActive(false);
    }

    public void showGameOver() {
        gameOverText.SetActive(true);
        showMenuButton.SetActive(false);
        viewPathButton.SetActive(true);
        scoreTextObject.SetActive(true);
        pointsObject.SetActive(true);
    }

    public void enterSettings() {
        state = 4;
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        creditsMenu.SetActive(false);
        pointsObject.SetActive(false);
    }

    public void enterCredits() {
        settingsMenu.SetActive(false);
        creditsMenu.SetActive(true);
    }

    public void exitSettings() {
        state = 0;
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
        pointsObject.SetActive(true);
    }

    public void enterPlay() {
        state = 1;
        Time.timeScale = timeScaleFactor;
        runPlay();
    }

    public void enterMenu() {
        state = 0;
        runMainMenu();
    }
    public void enterStats() {
        state = 5;
        mainMenu.SetActive(false);
        statsMenu.SetActive(true);
        pointsObject.SetActive(false);
        statsText.text = gameplayStatsString();
    }

    public void exitStats() {
        state = 0;
        statsMenu.SetActive(false);
        mainMenu.SetActive(true);
        pointsObject.SetActive(true);
    }

    public void enterUpgrades() {
        
        state = 3;
        runMainMenu();
        mainMenu.SetActive(false);
        upgradeMenu.SetActive(true);
        
    }
    
    public void exitUpgrades() {
        state = 0;
        upgradeMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void enterPause() {
        state = 2;
        runPause();
        Time.timeScale = 0.0f;
    }

    public void exitPause() {
        state = 1;
        unpause();
        
    }


    void runMainMenu() {
        //SceneManager.LoadScene(menuScene);
        GamePlayUI.SetActive(false);
        MainMenuUI.SetActive(true);
        mainMenu.SetActive(true);
        scoreTextObject.SetActive(false);
        gameOverText.SetActive(false);
        upgradeMenu.SetActive(false);
        settingsMenu.SetActive(false);
        statsMenu.SetActive(false);
        player.SetActive(false);
        darkenScreen.SetActive(false);
        pauseMenu.SetActive(false);
        pauseButton.SetActive(false);
        unpauseButton.SetActive(false);
        //floorGenerator.SetActive(false);
        
    }

    void runPause() {
        darkenScreen.SetActive(true);
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
        unpauseButton.SetActive(true);
        unpauseTimer.SetActive(false);
        stopMoving();
    }

    void unpause() {
        darkenScreen.SetActive(false);
        pauseMenu.SetActive(false);
        unpauseTimer.SetActive(true);
        pauseButton.SetActive(true);
        unpauseButton.SetActive(false);
    }

    void runPlay() {
        //SceneManager.LoadScene(gameplayScene);
        GamePlayUI.SetActive(true);
        MainMenuUI.SetActive(false);
        mainMenu.SetActive(false);
        scoreTextObject.SetActive(true);
        gameOverText.SetActive(false);
        player.SetActive(true);
        darkenScreen.SetActive(false);
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
        unpauseButton.SetActive(false);
        viewPathButton.SetActive(false);
        cameraScript.firstStart = false;
    }

    void stopMoving() {
        playerScript.paused = true;
        cameraScript.paused = true;
        cameraScript.unpaused = false;
        playerScript.unpaused = false;
    }

    public void startMoving() {
        playerScript.paused = false;
        cameraScript.paused = false;
        cameraScript.unpaused = true;
        playerScript.unpaused = true;
    }

    void runSettings() {
        
    }


    //Store page stuff below

    //purchase an upgrade in the store and save the game with it being unlocked/purchased
    public void purchaseUpgrade(int num) {
        upgradesBought[num]++;
        if (item(num)) {
            numItemsBought++;
        }
        else{
            numUpgradesBought++;
        }
        gameDataScript.update();
        gameDataScript.SaveGame();
    }

    //calculate the health stats, used for an alternative testing purpose, as health can be seen as a distance equivalent
    void calculateHealthStats() {
        int healthUpgradesBought = upgradesBought[3];
        int healingUpgradesBought = upgradesBought[2];
        int startingHealthUpgradesBought = upgradesBought[1];
        float startingHealth = (-4 / Mathf.Pow(E, startingHealthUpgradesBought/8));
        float health = (-4 / Mathf.Pow(E, healthUpgradesBought/16));
        float healingFactor = ((4 + 1) / (-(Mathf.Pow(E, healingUpgradesBought/30))) + 5);
        playerScript.recoverSpeed = healingFactor;
        playerScript.health = health;
        cameraScript.camStartMovePos = startingHealth;
        

        

    }

    //no items for now
    public bool item(int num) {
        //TODO check for items;
        return false;
    }

    //A function to write the gameplay stats when asked to display
    public string gameplayStatsString() {
        Debug.Log("stats work");
        string returnVal = "";
        returnVal += "Time played: " + secondsPlayed + " Seconds\nTotal Score: "
            + totalScore + "\nHigh Score: " + highScore
            + "\nBest Height: " + highestHeight +
            " meters\nBest Length: "+ highestLength +
            " meters\nNumber of Climbs: " + attempts
            + "\nTotal Height: " + totalHeight +
            " meters\nTotal Length: " + totalLength +  " meters";
        return returnVal;
    }

    //writes the upgrade stats when asked to display, currently not working TODO
    public string UpgradeStatsString() {
        string returnVal = "";
        return returnVal;
    }

    //When the player chooses to restart, sets the proper variables correctly and allows the game to start the restarting process
    public void restartRun() {
        
        enterPlay();
        //establish a new player as this allows a clean slate for the playerscript and playerstats
        Destroy(player);
        player = Instantiate(characterObject, new Vector3(-20, .8f, 1), Quaternion.identity); 
        playerScript = player.GetComponent<Character>();
        floorGenerator.GetComponent<FloorGen>().character = player;
        gameOver = false;
        score = 0;
        scoreText.text = "Score: " + score.ToString();
        cameraScript.i = 0;
        cameraScript.resetting = true;
        cameraScript.player = player;
        
        //restart the floorgenerators script
        //floorGenerator.GetComponent<FloorGen>().restartRun();
        floorGenerator.GetComponent<FloorGen>().numSpawned = 0;
        floorGenerator.GetComponent<FloorGen>().sides = 3;
    }

    //test a promocode vs an array of promocodes that are active, and display a match
    //TODO, write another function and array that would read out what that promo code does, and update the game data based on that.
    public void testPromoCode(string inputText) {
        falseCode.SetActive(false);
        codeApproved.SetActive(false);
        promoCode = inputText;
        int matchFound = -1;
        for (int i = 0; i < promoCodesArray.Length; i++) {
            if (promoCode == promoCodesArray[i]) {
                matchFound = i;
                break;
            }
        }
        if (matchFound == -1) {
            falseCode.SetActive(true);
        }
        else {
            codeApproved.SetActive(true);
            switch (matchFound) {
                case 0:
                    devMode = true;
                    break;
                case 1:
                    break;
            }
        }
    }

    //revive function that is in progress
    //TODO
    public void revive() {

    }
}
