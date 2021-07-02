using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

//The gamedata that the game will save

//TODO as variables are added to gameControl, add them to GameData as well.
[Serializable]
class SaveData
{
    public int highScore;
    public long secondsPlayed;
    public string name;
    public int numPoints;
    public float highestHeight;
    public float highestLength;
    public float totalLength;
    public float totalHeight;
    public int totalScore;
    public bool[] sidesUnlocked = new bool[16];
    public float lowestHeightModifierUnlocked = 1;
    public float lowestLengthModifierUnlocked = 1;
    public bool colorModifierUnlocked;
    public bool gradientUnlocked;
    public bool noAds = false;
    public DateTime lastPlayed;
    public DateTime currentTime;

    public int[] upgradesBought;
    public int numUpgrades;
    public int numUpgradesBought;
    public int numItemsBought;
    public int numItemsUsed; 

}

//a class that holds the data to write to the serializable above
public class GameData : MonoBehaviour
{
    public string sceneName = "SampleScene.unity";
    public int highScore = 0;
    public long secondsPlayed = 0;
    public string name = "Enter Name";
    public int numPoints = 0;
    public float highestHeight = 0;
    public float highestLength = 0;
    public float totalLength = 0;
    public float totalHeight = 0;
    public int totalScore = 0;
    public bool[] sidesUnlocked = new bool[16];
    public float lowestHeightModifierUnlocked = 1;
    public float lowestLengthModifierUnlocked = 1;
    public bool colorModifierUnlocked = false;
    public bool gradientUnlocked = false;
    public bool noAds = false;
    public DateTime lastPlayed = DateTime.Now;
    public DateTime currentTime = DateTime.Now;
    public gameControl gc;

    public int[] upgradesBought;
    public int numUpgrades;
    public int numUpgradesBought;
    public int numItemsBought;
    public int numItemsUsed; 
    public bool devMode;

    /*
    void awake() {

    }
    */

    //function that will save the game
    public void SaveGame()
    {
        /*
        PlayerPrefs.SetInt("SavedInteger", score);
        PlayerPrefs.SetFloat("SavedFloat", secondsPlayed);
        PlayerPrefs.SetString("SavedString", name);
        PlayerPrefs.Save();
        Debug.Log("Game data saved!");
        */

        //write all data variables
        BinaryFormatter bf = new BinaryFormatter(); 
        FileStream file = File.Create(Application.persistentDataPath 
                    + "/SaveData.dat"); 
        SaveData data = new SaveData();
        data.highScore = highScore;
        data.secondsPlayed = secondsPlayed;
        data.name = name;
        data.currentTime = DateTime.Now;
        data.lastPlayed = data.currentTime;
        data.highestLength = highestLength;
        data.highestHeight = highestHeight;
        data.totalScore = totalScore;
        data.totalHeight = totalHeight;
        data.totalLength = totalLength;
        data.numPoints = numPoints;
        data.numItemsBought = numItemsBought;
        data.numItemsUsed = numItemsUsed;
        data.numUpgradesBought = numUpgradesBought;
        data.upgradesBought = upgradesBought;

        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game data saved!");
    }



    //function that loads the save data on startup of the game, includes additional info that isnt required at the end of each round when loading data
    public void StartLoadGame()
    {
        /*
        if (PlayerPrefs.HasKey("SavedInteger"))
        {
            score = PlayerPrefs.GetInt("SavedInteger");
            secondsPlayed = PlayerPrefs.GetFloat("SavedFloat");
            name = PlayerPrefs.GetString("SavedString");
            Debug.Log("Game data loaded!");
        }
        else
            Debug.LogError("There is no save data!");
            */
        //if it exists, read the savedata, load it to this file
        if (File.Exists(Application.persistentDataPath 
                   + "/SaveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = 
                    File.Open(Application.persistentDataPath 
                    + "/SaveData.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            highScore = data.highScore;
            secondsPlayed = data.secondsPlayed;
            name = data.name;
            currentTime = DateTime.Now;
            numPoints = data.numPoints;
            highestHeight = data.highestHeight;
            highestLength = data.highestLength;
            totalLength = data.totalLength;
            totalHeight = data.totalHeight;
            totalScore = data.totalScore;
            numItemsBought = data.numItemsBought;
            numItemsUsed = data.numItemsUsed;
            numUpgradesBought = data.numUpgradesBought;
            upgradesBought = data.upgradesBought;
            if (data.lastPlayed.AddHours(1) < data.currentTime) {
                calcOffline();
            }
            updateGC();
            Debug.Log("Game data loaded!");
        }
        //otherwise write in the log that there is no data
        else
            Debug.LogError("There is no save data!");
    }

    //loads the data required to restart a game
    public void LoadGame()
    {
        /*
        if (PlayerPrefs.HasKey("SavedInteger"))
        {
            score = PlayerPrefs.GetInt("SavedInteger");
            secondsPlayed = PlayerPrefs.GetFloat("SavedFloat");
            name = PlayerPrefs.GetString("SavedString");
            Debug.Log("Game data loaded!");
        }
        else
            Debug.LogError("There is no save data!");
            */
        
        if (File.Exists(Application.persistentDataPath 
                   + "/SaveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = 
                    File.Open(Application.persistentDataPath 
                    + "/SaveData.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            highScore = data.highScore;
            secondsPlayed = data.secondsPlayed;
            name = data.name;
            currentTime = DateTime.Now;
            numPoints = data.numPoints;
            highestHeight = data.highestHeight;
            highestLength = data.highestLength;
            totalLength = data.totalLength;
            totalHeight = data.totalHeight;
            totalScore = data.totalScore;
            numItemsBought = data.numItemsBought;
            numItemsUsed = data.numItemsUsed;
            numUpgradesBought = data.numUpgradesBought;
            upgradesBought = data.upgradesBought;
            updateGC();
            
            Debug.Log("Game data loaded!");
        }
        else
            Debug.LogError("There is no save data!");
    }

    //deletes all data and restarts the game
    public void ResetData()
    {
        /*
        PlayerPrefs.DeleteAll();
        score = 0;
        secondsPlayed = 0.0f;
        name = "";
        Debug.Log("Data reset complete");
        */
        Debug.Log("Deleting");
        if (File.Exists(Application.persistentDataPath + "/SaveData.dat"))         
        {
            Debug.Log("Deleting Data");
            File.Delete(Application.persistentDataPath + "/SaveData.dat");
            SceneManager.LoadScene("MainMenuScene");
        }
    }

    //updates the GameData variables with the gameControls variables
    public void update() {
        highestHeight = gc.highestHeight;
        highestLength = gc.highestLength;
        highScore = gc.highScore;
        totalScore = gc.totalScore;
        numPoints = gc.numPoints;
        totalHeight = gc.totalHeight;
        totalLength = gc.totalLength;
        numUpgrades = gc.numUpgrades;
        numUpgradesBought = gc.numUpgradesBought;
        upgradesBought = gc.upgradesBought;
        numItemsBought = gc.numItemsBought;
        numItemsUsed = gc.numItemsUsed;

    }

    //updates the gameControl variables with the GameData variables
    public void updateGC() {
        gc.highestHeight = highestHeight;
        gc.highestLength = highestLength;
        gc.highScore = highScore;
        gc.totalScore = totalScore;
        gc.numPoints = numPoints;
        gc.totalHeight = totalHeight;
        gc.totalLength = totalLength;
        gc.numUpgradesBought = numUpgradesBought;
        gc.upgradesBought = upgradesBought;
        gc.numItemsBought = numItemsBought;
        gc.numItemsUsed = numItemsUsed;

    }

    //used to calculate offline games, TODO
    void calcOffline() {

    }

 }