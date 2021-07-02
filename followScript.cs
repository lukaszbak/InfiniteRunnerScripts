using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//camera follor script
public class followScript : MonoBehaviour
{
    //rigidbody of camera for smooth movement
    public Rigidbody2D rb2d;

    //measurement of height and length
    public float totalHeight = 0.0f;
    public float totalLength = 0.0f;

    public Vector2 currentVelocity;
    public Transform tf;

    //distance needed to catch up to where we want the camera
    public float vertCatchUp;
    public GameObject floorGenObject;
    public FloorGen floorGenScript;
    public GameObject player;
    public Character playerScript;

    //variables to change for the cameras movement function during a restart/loss
    public float zoomSpeed = 3;
    public float zoomTime = .2f;
    //self
    public Camera cam;

    //global variable so we know what our timing is for certain movements
    public int i = 0;

    //the size we want for our camera
    public float wantedSize = 2.5f;
    public bool paused = false;
    public bool unpaused = false;

    //starting position we want to move to
    public float camStartMovePos = -4;

    //public float distance;
    //public float distanceX;
    //public float distanceY;


    //Variables for movement after loss/restart
    public float speed;
    public float speedX;
    public float speedY;

    public float acceleration;
    public float accelerationX;
    public float accelerationY;

    public float maxChangeInAcceleration;
    public float maxChangeInAccelerationX;
    public float maxChangeInAccelerationY;

    public float changeInAcceleration;
    public float changeInAccelerationX;
    public float changeInAccelerationY;

    //time in seconds we want to zoom, and the number of frames that is
    public int fullZoomTime = 3;
    public int fullZoomFrames;

    //size we want to zoom to
    public float startingSize = 3f;

    //our framerate
    public int frameRate = 60;

    //one eight of our frames for our 8 section sinusoidal wave to measure our cameras speed, distance, acceleration
    public int eighthFrames;

    //flag for resetting, startup, and first time starting
    public bool resetting = false;
    public bool startup = true;

    public bool firstStart = true;

    //the increments of our camera size, x and y
    float camSizeIncrement = 0;
    float xIncrement = 0;
    float yIncrement = 0;


    // Start is called before the first frame update
    void Start()
    {
        //initialize variables
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = new Vector2(gameControl.instance.scrollSpeed, 0);
        tf = GetComponent<Transform>();
        floorGenScript = floorGenObject.GetComponent<FloorGen>();
        playerScript = player.GetComponent<Character>();
        cam = GetComponent<Camera>();
        fullZoomFrames = fullZoomTime * frameRate;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if resetting reset to new player variable
        if(resetting) {
            playerScript = player.GetComponent<Character>();
        }
        //record last velocity and height that we traveled
        currentVelocity = rb2d.velocity;
        totalHeight = playerScript.tf.position.y; //(floorGenScript.currentY +  3 * playerScript.tf.position.y) / 4;
        //if initializing for the first time, move to the proper positon and size
        if (firstStart) {
            rb2d.velocity = new Vector2(0,0);
            if(i < 40) {
                if( i == 0 ) {
                    camSizeIncrement = (wantedSize - cam.orthographicSize) / 60;
                    xIncrement = (totalLength - tf.position.x) / 60;
                    yIncrement = (totalHeight - (tf.position.y - .5f)) / 60;
                }
                i++;
                rb2d.velocity = new Vector2(0,0);
                tf.position = new Vector3(tf.position.x + xIncrement, tf.position.y + yIncrement, tf.position.z);
                //rb2d.velocity = new Vector2((currentVelocity.x + gameControl.instance.scrollSpeed * 12 * 50) / (13 * 50), (totalHeight) - (tf.position.y) / 3000);
                cam.orthographicSize += camSizeIncrement;
                //(wantedSize * 4 + cam.orthographicSize )/5;
            }
            //when complete turn off flag and reset i
            else {
                startup = false;
                i = 0;
            }
        }
        //if were starting up after a reset, do the same thing to move us to a proper positioning
        else if (startup) {
            if(i < 60) {
                if( i == 0 ) {
                    camSizeIncrement = (wantedSize - cam.orthographicSize) / 60;
                    xIncrement = (totalLength - tf.position.x) / 60;
                    yIncrement = (totalHeight - (tf.position.y - .5f)) / 60;
                }
                i++;
                rb2d.velocity = new Vector2(0,0);
                tf.position = new Vector3(tf.position.x + xIncrement, tf.position.y + yIncrement, tf.position.z);
                //rb2d.velocity = new Vector2((currentVelocity.x + gameControl.instance.scrollSpeed * 12 * 50) / (13 * 50), (totalHeight) - (tf.position.y) / 3000);
                cam.orthographicSize += camSizeIncrement;
                //(wantedSize * 4 + cam.orthographicSize )/5;
            }
            else {
                startup = false;
                i = 0;
            }
        }
        //if paused, pause camera
        else if (paused) {
            rb2d.velocity = new Vector2(0,0);
        }
        //if unpaused, unpause and go back to original speed
        else if(unpaused) {
            
             rb2d.velocity = new Vector2(gameControl.instance.scrollSpeed, (totalHeight - (tf.position.y - .5f) ) * vertCatchUp);
            
            unpaused = false;
        }
        //if we need to reset, turn on the reset flag and move the camera accordingly to an 8 part sinusoidal wave
        else if(resetting) {
            //tf.position = new Vector3(-1.5f,0, -10);
            //cam.orthographicSize = 2.5f;
            //resetting = false;

            
            //player = GameObject.FindWithTag("Player");
            playerScript = player.GetComponent<Character>();
            if (i == 0) {
                speed = 0;
                speedX = 0;
                speedY = 0;

                acceleration = 0;
                accelerationX = 0;
                accelerationY = 0;

                maxChangeInAcceleration = 0;
                maxChangeInAccelerationX = 0;
                maxChangeInAccelerationY = 0;

                changeInAcceleration = 0 ;
                changeInAccelerationX = 0;
                changeInAccelerationY = 0;
                startingSize = cam.orthographicSize;
                floorGenScript.characterScript = player.GetComponent<Character>();
            }
            rb2d.velocity = new Vector2(0,0);
            totalLength = 0f;
            totalHeight = 1.0f;
            wantedSize = 1f;
            //fullZoomFrames = fullZoomTime * frameRate;
            if( i < fullZoomFrames) {
                eighthFrames = fullZoomFrames/8;
                maxChangeInAcceleration = (64 * (wantedSize - startingSize)) / Mathf.Pow(fullZoomFrames, 3);
                if( i < eighthFrames) {
                    changeInAcceleration += maxChangeInAcceleration/eighthFrames ;
                }
                else if (i < eighthFrames*3){
                    changeInAcceleration -= maxChangeInAcceleration/eighthFrames;
                }
                else if(i < eighthFrames*4) {
                    changeInAcceleration += maxChangeInAcceleration/eighthFrames;
                }
                else if (i < eighthFrames * 5) {
                    changeInAcceleration -= maxChangeInAcceleration/eighthFrames;
                }
                else if (i < eighthFrames * 7) {
                    changeInAcceleration += maxChangeInAcceleration/eighthFrames;
                }
                else {
                    changeInAcceleration -= maxChangeInAcceleration/eighthFrames;
                }
                
                acceleration += changeInAcceleration;
                speed += acceleration;
                if (cam.orthographicSize > 2.5f) {
                    cam.orthographicSize += speed;
                }
                //Debug.Log("accelerationx = " + accelerationX + " speedX = " + speedX + " change in accelX = " + changeInAccelerationX);
            }
            if( i < fullZoomFrames) {
                eighthFrames = fullZoomFrames/8;
                if(i == 0 ) {
                    maxChangeInAccelerationX = (64 * ((totalLength) - tf.position.x)) / Mathf.Pow(fullZoomFrames, 3);
                }
                if( i < eighthFrames) {
                    changeInAccelerationX += maxChangeInAccelerationX/eighthFrames ;
                }
                else if (i < eighthFrames*3){
                    changeInAccelerationX -= maxChangeInAccelerationX/eighthFrames;
                }
                else if(i < eighthFrames*4) {
                    changeInAccelerationX += maxChangeInAccelerationX/eighthFrames;
                }
                else if (i < eighthFrames * 5) {
                    changeInAccelerationX -= maxChangeInAccelerationX/eighthFrames;
                }
                else if (i < eighthFrames * 7) {
                    changeInAccelerationX += maxChangeInAccelerationX/eighthFrames;
                }
                else {
                    changeInAccelerationX -= maxChangeInAccelerationX/eighthFrames;
                }
                accelerationX += changeInAccelerationX;
                speedX += accelerationX;
                tf.position = new Vector3(tf.position.x + speedX, tf.position.y, tf.position.z);
                //distanceX += speedX;
            }
            if( i < fullZoomFrames) {
                eighthFrames = fullZoomFrames/8;
                if( i == 0) {
                    maxChangeInAccelerationY = (64 * ((totalHeight) - tf.position.y)) / Mathf.Pow(fullZoomFrames, 3);
                }
                if( i < eighthFrames) {
                    changeInAccelerationY += maxChangeInAccelerationY/eighthFrames ;
                }
                else if (i < eighthFrames*3){
                    changeInAccelerationY -= maxChangeInAccelerationY/eighthFrames;
                }
                else if(i < eighthFrames*4) {
                    changeInAccelerationY += maxChangeInAccelerationY/eighthFrames;
                }
                else if (i < eighthFrames * 5) {
                    changeInAccelerationY -= maxChangeInAccelerationY/eighthFrames;
                }
                else if (i < eighthFrames * 7) {
                    changeInAccelerationY += maxChangeInAccelerationY/eighthFrames;
                }
                else {
                    changeInAccelerationY -= maxChangeInAccelerationY/eighthFrames;
                }
                accelerationY += changeInAccelerationY;
                speedY += accelerationY;
                tf.position = new Vector3(tf.position.x, tf.position.y + speedY, tf.position.z);
                i++;
            }
            
            //rb2d.velocity = new Vector2(speedX,speedY)*60;
            //if were finished resetting, the reset to original status
            else if ( i == fullZoomFrames) {
                resetting = false;
                i = 0;
                startup = true;
                wantedSize = 2.5f;
                floorGenScript.restartRun();
            }
            
        //if the player is not dead, normal operation of the camera following the floor to the right
        } else if(!playerScript.isDead) {
            cam.orthographicSize = 3.0f + ((Time.timeScale - 50.0f) / 100.0f);
            if (rb2d.velocity.x >= gameControl.instance.scrollSpeed) {
                rb2d.velocity = new Vector2(gameControl.instance.scrollSpeed, (totalHeight - (tf.position.y - .5f) ) * vertCatchUp);
            } else if (playerScript.tf.position.x >= camStartMovePos) {
                rb2d.velocity = new Vector2(rb2d.velocity.x + .05f,  (totalHeight - (tf.position.y - .5f) ) * vertCatchUp);
            }
            
        //otherwise, start the resetting position because we died
        } else {
            if (i == 0) {
                startingSize = cam.orthographicSize;
                speed = 0;
                speedX = 0;
                speedY = 0;

                acceleration = 0;
                accelerationX = 0;
                accelerationY = 0;

                maxChangeInAcceleration = 0;
                maxChangeInAccelerationX = 0;
                maxChangeInAccelerationY = 0;

                changeInAcceleration = 0 ;
                changeInAccelerationX = 0;
                changeInAccelerationY = 0;
            }
            rb2d.velocity = new Vector2(0,0);
            totalLength = floorGenScript.currentX;
            totalHeight = floorGenScript.currentY;
            bool XLongest = findLongestSide();
            fullZoomFrames = fullZoomTime * frameRate;
            if (XLongest) {
                wantedSize = totalLength/16*9/2;
            }
            else {
                wantedSize = totalHeight/2;

            }
            if( i < fullZoomFrames) {
                eighthFrames = fullZoomFrames/8;
                maxChangeInAcceleration = (64 * (wantedSize - startingSize)) / Mathf.Pow(fullZoomFrames, 3);
                if( i < eighthFrames) {
                    changeInAcceleration += maxChangeInAcceleration/eighthFrames ;
                }
                else if (i < eighthFrames*3){
                    changeInAcceleration -= maxChangeInAcceleration/eighthFrames;
                }
                else if(i < eighthFrames*4) {
                    changeInAcceleration += maxChangeInAcceleration/eighthFrames;
                }
                else if (i < eighthFrames * 5) {
                    changeInAcceleration -= maxChangeInAcceleration/eighthFrames;
                }
                else if (i < eighthFrames * 7) {
                    changeInAcceleration += maxChangeInAcceleration/eighthFrames;
                }
                else {
                    changeInAcceleration -= maxChangeInAcceleration/eighthFrames;
                }
                
                acceleration += changeInAcceleration;
                speed += acceleration;
                cam.orthographicSize += speed;
                //Debug.Log("accelerationx = " + accelerationX + " speedX = " + speedX + " change in accelX = " + changeInAccelerationX);
            }
            if( i < fullZoomFrames) {
                eighthFrames = fullZoomFrames/8;
                if(i == 0 ) {
                    maxChangeInAccelerationX = (64 * ((totalLength / 2) - tf.position.x)) / Mathf.Pow(fullZoomFrames, 3);
                }
                if( i < eighthFrames) {
                    changeInAccelerationX += maxChangeInAccelerationX/eighthFrames ;
                }
                else if (i < eighthFrames*3){
                    changeInAccelerationX -= maxChangeInAccelerationX/eighthFrames;
                }
                else if(i < eighthFrames*4) {
                    changeInAccelerationX += maxChangeInAccelerationX/eighthFrames;
                }
                else if (i < eighthFrames * 5) {
                    changeInAccelerationX -= maxChangeInAccelerationX/eighthFrames;
                }
                else if (i < eighthFrames * 7) {
                    changeInAccelerationX += maxChangeInAccelerationX/eighthFrames;
                }
                else {
                    changeInAccelerationX -= maxChangeInAccelerationX/eighthFrames;
                }
                accelerationX += changeInAccelerationX;
                speedX += accelerationX;
                tf.position = new Vector3(tf.position.x + speedX, tf.position.y, tf.position.z);
                //distanceX += speedX;
            }
            if( i < fullZoomFrames) {
                eighthFrames = fullZoomFrames/8;
                if( i == 0) {
                    maxChangeInAccelerationY = (64 * ((totalHeight /2) - tf.position.y)) / Mathf.Pow(fullZoomFrames, 3);
                }
                if( i < eighthFrames) {
                    changeInAccelerationY += maxChangeInAccelerationY/eighthFrames ;
                }
                else if (i < eighthFrames*3){
                    changeInAccelerationY -= maxChangeInAccelerationY/eighthFrames;
                }
                else if(i < eighthFrames*4) {
                    changeInAccelerationY += maxChangeInAccelerationY/eighthFrames;
                }
                else if (i < eighthFrames * 5) {
                    changeInAccelerationY -= maxChangeInAccelerationY/eighthFrames;
                }
                else if (i < eighthFrames * 7) {
                    changeInAccelerationY += maxChangeInAccelerationY/eighthFrames;
                }
                else {
                    changeInAccelerationY -= maxChangeInAccelerationY/eighthFrames;
                }
                accelerationY += changeInAccelerationY;
                speedY += accelerationY;
                tf.position = new Vector3(tf.position.x, tf.position.y + speedY, tf.position.z);
                i++;
            }
            if ( i == fullZoomFrames) {
                rb2d.velocity = new Vector2(0,0);
            }
        }
    }

    bool findLongestSide() {
        return totalHeight*9 < totalLength*16;
    }


}
