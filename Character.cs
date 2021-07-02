using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes2D;

//the script that runs on the character to change the number of sides, acquire points, turn, etc
public class Character : MonoBehaviour
{
    //the speed at which the character will turn, when turning
    public float turningVelocity;

    //the rigidbody of the character
    private Rigidbody2D rigidbody2D;

    public bool isDead = false;

    private bool isTurning = false;

    //the characters transform for positioning
    public Transform tf;

    public int sides = 3;

    //useless rn
    private int i = 0;

    //useless rn
    public float degreeOfFreedom = 5;

    //the collider (legacy)
    private PolygonCollider2D collider;

    float height = 1;
    float width = 1;

    //sprite access (legacy)
    private SpriteRenderer sprite;

    //the object that has the gamecontroller script
    public gameControl gc;

    //Legacy
    public Sprite[] sprites;
    public Vector2[] twoSide = new Vector2[4];
    public Vector2[] threeSide = new Vector2[4];
    public Vector2[] fourSide = new Vector2[4];
    public Vector2[] fiveSide = new Vector2[4];
    public Vector2[] sixSide = new Vector2[4];
    public Vector2[] sevenSide = new Vector2[4];
    public Vector2[] eightSide = new Vector2[4];
    public Vector2[][] points;
    //\legacy

    public Vector2 lastVelocity;

    //camera and camera transform to know when to speed up until, and when dead
    public GameObject cam;
    private Transform camTrans;

    //recovery speed
    public float recoverSpeed;

    //more specific minHeights and maxHeights
    public float[] minHeights;
    public float[] maxHeights;


    //the shapescript attached to edit the shape being rendered and its 2d collisions
    public Shape shapeScript;

    //a modifier for starting sides
    public int startingSides;

    //a modifier for the list of scores required to move on to the next shape
    public int[] nextShapeScores;

    //whether the player is turning or not
    private bool coroutineRunning = false;
    private bool savedAfterDeath = false;

    //whether the game is paused or not
    public bool paused = false;
    public bool unpaused = false;

    //the trail being drawn
    public GameObject trail;
    public bool tutorialPaused = false;
    //testing purposes currently
    public float health = 0;


    // Start is called before the first frame update
    void Start()
    {
        //establish scripts and variables
        rigidbody2D = GetComponent<Rigidbody2D>();
        gc = GameObject.FindWithTag("GC").GetComponent<gameControl>();
        tf = GetComponent<Transform>();
        collider = GetComponent<PolygonCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        cam = GameObject.FindWithTag("MainCamera");
        //points = new Vector2[][] { twoSide, threeSide, fourSide, fiveSide};
        camTrans = cam.GetComponent<Transform>();
        rigidbody2D.freezeRotation = false;
        shapeScript = GetComponent<Shape>();

    }

    void FixedUpdate()
    
    {
        //if this run is continuing
        if (isDead == false) {
            
            //pause movement if paused
            if (paused) {
                rigidbody2D.velocity = new Vector2(0,0);
                rigidbody2D.constraints = RigidbodyConstraints2D.FreezePosition;
            }
            //otherwise continue movement
            else if (unpaused) {
                rigidbody2D.constraints = RigidbodyConstraints2D.None;
                //if we arent currently spinning, our velocity is just the normal velocity of the screen movement
                if (!coroutineRunning) {
                    rigidbody2D.velocity = new Vector2(gameControl.instance.scrollSpeed, lastVelocity.y);
                }
                unpaused = false;
            }
            //if our position is not behind our cameras normal position, move at normal speed.
            else if (tf.position.x > camTrans.position.x - 2f) {
                //Store last velocity
                lastVelocity = rigidbody2D.velocity;

                rigidbody2D.velocity = new Vector2(gameControl.instance.scrollSpeed, lastVelocity.y);
            }
            //increase velocity if behind but not out
            else if (tf.position.x < camTrans.position.x + health && tf.position.x > camTrans.position.x - 5f  || tf.position.x < 4) {
                lastVelocity = rigidbody2D.velocity;
                rigidbody2D.velocity = new Vector2(gameControl.instance.scrollSpeed * recoverSpeed, lastVelocity.y);
            }
        }
        //if out of bounds, die
        if (tf.position.x < camTrans.position.x - 5f && !isDead && tf.position.x > 4) {
            isDead = true;
            gc.playerDied();
            //show trail
            trail.GetComponent<TrailRenderer>().sortingOrder = 1;
        } 
    }

    void Update() {
        //if were not dead, check for inputs to turn
        if (!isDead) {
            if (Input.GetMouseButtonDown(0) && !coroutineRunning && !paused && (Input.mousePosition.x < (Screen.width*.9f) && Input.mousePosition.y < (Screen.height*.9f))) {
                //turn(i, true);
                //Debug.Log("started Running");
                StartCoroutine(turn());
                //Debug.Log("coruoutine on");
            }
            //if we are paused for the tutorial, and we get a click, turn and unpause the tutorial
            if (Input.GetMouseButtonDown(0) && tutorialPaused) {
                StartCoroutine(turn());
                tutorialPaused = false;
                Time.timeScale = gc.timeScaleFactor;
            }
        }
    }

    IEnumerator turn() { 

        //mark as running
        coroutineRunning = true;

        //unfreeze rotation for first tap
        rigidbody2D.freezeRotation = false;

        //Degrees Per side is 360/sides
        float DPS = 360 / sides;

        //where we started rotating
        float startingRotation = toDegrees(tf.rotation.eulerAngles.z);
        //float initialStartingRotation = toDegrees(transform.rotation.eulerAngles.z);

        //where we plan to end rotating
        float endingRotation = startingRotation - .6f * DPS;

        //our current rotation
        float currentRotation = 0;

        //whether we looped past 360
        bool looped = false;

        //mark looped true and add 360 if looped
        if(endingRotation < 0) {
            endingRotation += 360;
            looped = true;
        }
        
        //While we are within turning bounds keep turning
        //start turning and mark our current rotation after applying velocity and waiting a frame,
        do {
            //figure out whether or not we will be looping over the 360 degrees back to 0 degrees
            if( DPS > 360 / sides) {
                looped = false;
                endingRotation = startingRotation - .6f * ( 360 / sides);
                if(endingRotation < 0) {
                    endingRotation += 360;
                    looped = true;
                }
            } 
            //figure out our turning velocity we will need for the number of sides we are at
            rigidbody2D.angularVelocity = (-turningVelocity / (1 + (sides * .05f)));
            //if we are paused, stop rotating
            if (paused) {
                rigidbody2D.freezeRotation = true;
            }
            //when unpaused enable rotation
            else if(unpaused) {
                rigidbody2D.freezeRotation = false;
            }
            //Debug.Log("Looped " + looped);
            //wait for a fixed update
            yield return new WaitForFixedUpdate();
            //figure out what our last degree was, and set it above so it will turn on the next tick
            float lastRotation =  361f;
            //set our last rotation before changing our current rotation
            if (currentRotation != 0) lastRotation = currentRotation;
            //get our new rotation
            currentRotation = toDegrees(tf.rotation.eulerAngles.z);
            
            
            
            //Here are the big and difficult area
            //if we have looped, then show that we no longer will loop 
            if (lastRotation < currentRotation || currentRotation > startingRotation) looped = false;
            //Figure out when we are finished turning, logic checked in excel, and then translated with our variables into actual code. covers (hopefully) all edge cases
            //Debug.Log("looped: " + looped + "lastRotation: " + lastRotation + "startingRotation: " + startingRotation + " endingRotation: " + endingRotation + " currentRotation: " + toDegrees(tf.rotation.eulerAngles.z));
        } while(looped || ((currentRotation >= DPS && currentRotation > endingRotation && (currentRotation < startingRotation || startingRotation <= 1f)) || (currentRotation < endingRotation && (endingRotation < DPS || endingRotation > 359f) && currentRotation < startingRotation && startingRotation <= DPS) || currentRotation < startingRotation && endingRotation <= DPS && currentRotation > endingRotation ));
        //rigidbody2D.freezeRotation = true;

        //when we are done, mark coroutine as finished
        coroutineRunning = false;

        //Legacy

            //while(((toDegrees(tf.rotation.eulerAngles.z)) > (endingRotation) && ((toDegrees(tf.rotation.eulerAngles.z)) > DPS) && (endingRotation < DPS)  ) || (((toDegrees(tf.rotation.eulerAngles.z) ) < (endingRotation) ) && (toDegrees(tf.rotation.eulerAngles.z) ) < DPS ) && (endingRotation > DPS));
            //float rotationBack = (toDegrees(tf.rotation.eulerAngles.z)) - endingRotation;
            //tf.Rotate(0,0, rotationBack, Space.Self);

            // IF (currentRotation > endingRotation && currentRotation > DPS && endingRotation < DPS) OR 
            
            /*
            //Debug.Log("Turning Velocity = " + rigidbody2D.angularVelocity + " I = " + i + " startTurn = " + startTurn);
            if (j == 0 && startTurn) {
                i = 1; // Just started turning
            }
            else {
                //Debug.Log("Turning Velocity = " + rigidbody2D.angularVelocity + " I = " + i + " startTurn = " + startTurn);
                if(j > 0 && (rotating(j) || j < 5)) {
                    //Debug.Log("Turning Velocity = " + rigidbody2D.angularVelocity + " I = " + i + " startTurn = " + startTurn);
                    rigidbody2D.angularVelocity = (-turningVelocity);
                    i++; // Frames turning
                }
                else {
                    i = 0;
                }
            }
            */
    }

    //Legacy
    bool rotating(int l) {
        //Debug.Log("Checking rotating()");
        float DPS = 360 / sides;
        float currentDegrees = toDegrees(tf.rotation.eulerAngles.z);
        for (int k = 0; k < sides; k++) {
            //Debug.Log("DPS = " + DPS + " z = " + tf.rotation.eulerAngles.z + " currentDegrees = " + currentDegrees + " sideRange = " + ((k*DPS) + degreeOfFreedom) + " through " + (((k+1)*DPS) - degreeOfFreedom) + "I = " + l);
            if (currentDegrees > (k*DPS) + degreeOfFreedom && currentDegrees < ((k+1)*DPS) - degreeOfFreedom) {
                return true;
            }
        }
        return false;
    }

    //convert from degrees
    float fromDegrees(float x) {
        if (x < 180) {
            return x;
        }
        else {
            return x - 360;
        }

    }

    //convert to degrees
    float toDegrees(float x) {
        if (x < 0) {
            return x + 360;
        }
        else return x;
    }

    //earn points and increase sides if points are matching the amount required
    void earnPoint() {
        //set that we scored
        gc.scored();
        
        
        //int startingSides = sides;

        //if our score is higher than our next shape score boundary, then move forward
        if (gc.score >= nextShapeScores[sides-3]) {

            //if we are not capped on sides, continue
            if (!cappedSides()) {
                
                //if we have unlocked our next side, switch to it and scale our shape up
                if (gc.sidesUnlocked[sides+1]) {
                    sides++;
                    tf.localScale = Vector3.Scale(new Vector3(1.1f, 1.1f, 1f), tf.localScale);
                    switch (sides) {
                        case 4:
                            shapeScript.settings.polygonPreset = PolygonPreset.Diamond;
                            break;
                        case 5:
                            shapeScript.settings.polygonPreset = PolygonPreset.Pentagon;
                            break;
                        case 6:
                            shapeScript.settings.polygonPreset = PolygonPreset.Hexagon;
                            break;
                        case 7:
                            shapeScript.settings.polygonPreset = PolygonPreset.Heptagon;
                            break;
                        case 8:
                            shapeScript.settings.polygonPreset = PolygonPreset.Octagon;
                            break;
                        case 9:
                            shapeScript.settings.polygonPreset = PolygonPreset.Nonagon;
                            break;
                        case 10:
                            shapeScript.settings.polygonPreset = PolygonPreset.Decagon;
                            break;
                    }
                    //set the collider
                    SetPolygonCollider2D(shapeScript);

                    //set angle back to 0
                    tf.eulerAngles = new Vector3(0, 0, 0);
                }
                    
                /* LEGACY

                //check how many sides were at, and check if next is unlocked, then change our scale and shape, and add a side
                if(sides == 3) {
                    sides++;
                    tf.localScale = Vector3.Scale(new Vector3(1.1f ,1.1f ,1f ), tf.localScale);
                    shapeScript.settings.polygonPreset = PolygonPreset.Diamond;
                }
                else if (sides == 4 && gc.pentagonUnlocked == true)  {
                    tf.localScale = Vector3.Scale(new Vector3(1.1f ,1.1f ,1f ), tf.localScale);
                    sides++;
                    shapeScript.settings.polygonPreset = PolygonPreset.Pentagon;
                }
                else if (sides == 5 && gc.hexagonUnlocked == true) {
                    tf.localScale = Vector3.Scale(new Vector3(1.1f ,1.1f ,1f ), tf.localScale);
                    sides++;
                    shapeScript.settings.polygonPreset = PolygonPreset.Hexagon;
                }
                else if (sides == 6 && gc.heptagonUnlocked == true) {
                    tf.localScale = Vector3.Scale(new Vector3(1.1f ,1.1f ,1f ), tf.localScale);
                    sides++;
                    shapeScript.settings.polygonPreset = PolygonPreset.Heptagon;
                }
                else if (sides == 7 && gc.octagonUnlocked == true) {
                    tf.localScale = Vector3.Scale(new Vector3(1.1f ,1.1f ,1f ), tf.localScale);
                    sides++;
                    shapeScript.settings.polygonPreset = PolygonPreset.Octagon;
                }
                else if (sides == 8 && gc.nonagonUnlocked == true)  {
                    tf.localScale = Vector3.Scale(new Vector3(1.1f ,1.1f ,1f ), tf.localScale);
                    sides++;
                    shapeScript.settings.polygonPreset = PolygonPreset.Nonagon;
                }
                else if (sides == 9 && gc.decagonUnlocked == true)  {
                    tf.localScale = Vector3.Scale(new Vector3(1.1f ,1.1f ,1f ), tf.localScale);
                    sides++;
                    shapeScript.settings.polygonPreset = PolygonPreset.Decagon;
                }
                */

            }
        }
    }

    //set collider taken from Shapes2D
    private void SetPolygonCollider2D(Shape shape) {
        PolygonCollider2D pc2d = shape.GetComponent<PolygonCollider2D>();
        if (shape.settings.shapeType == ShapeType.Polygon) {
            if (!pc2d)
                pc2d = shape.gameObject.AddComponent<PolygonCollider2D>();
            Vector3[] points = shape.GetPolygonWorldVertices();
            Vector2[] colliderPoints = new Vector2[points.Length];
            for (int i = 0; i < points.Length; i++)
                colliderPoints[i] = shape.transform.InverseTransformPoint(points[i]);
            //Undo.RecordObject(pc2d, "Set PolygonCollider2D Points");
            pc2d.points = colliderPoints;
        }
    }

    //if we have capped sides or not
    private bool cappedSides() {
        return sides >= gc.highestUnlocked;
    }

    // earn a point on collision with trigger
    void OnTriggerEnter2D(Collider2D col) {
        col.enabled = false;
        earnPoint();
        //rigidbody2D.freezeRotation = true;
    }
}

