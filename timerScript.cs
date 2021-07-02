using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Timer script used to be an unpause timer when unpausing the game
public class timerScript : MonoBehaviour
{

    //lists frames and framerate of the game
    int frame = 0;
    int frameRate = 50;
    //the text that shows up
    public Text thisText;
    public gameControl gc;
    //the text object
    public GameObject textyboy;
    // Start is called before the first frame update
    void OnEnable()
    {
        //if the unpause script is enabled, initialize variables
        frame = 0;
        thisText.text = "3";
        thisText.fontSize = 300;
        Debug.Log("RAN ONENABLE");
        //Time.timeScale = 0.0f;
    }

    // Update is called once per frame
    public void Update() 
    {  
        //Debug.Log("entered update on timer");
        //if its been less than a second make the text smaller
        if (frame < frameRate) {
            thisText.fontSize -= 6;
        }
        //if its been a second, move to the next number and reset size
        else if (frame == frameRate) {
            thisText.fontSize = 300;
            thisText.text = "2";
        }
        else if (frame < 2*frameRate) {
            thisText.fontSize -= 6;
        }
        else if (frame == 2*frameRate) {
            thisText.fontSize = 300;
            thisText.text = "1";
        }
        else if (frame < 3*frameRate) {
            thisText.fontSize -= 6;
        }
        //once 3 seconds elapse, start moving again, unpause the game, and reset the variables in case we pause again later.
        else if (frame == 3*frameRate) {
            gc.startMoving();
            frame = 0;
            thisText.text = "3";
            thisText.fontSize = 300;
            textyboy.SetActive(false);
            Time.timeScale = 1.0f;
        }
        frame++;
        //yield return new WaitForSecondsRealtime(1);
    }
}
