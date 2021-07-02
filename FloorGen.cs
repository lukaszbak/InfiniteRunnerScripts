using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Gens the floor, can be placed on any object
public class FloorGen : MonoBehaviour
{
    //start height
    public float startingHeight;

    public float nextHeightMin;

    public float nextHeightMax;

    //Gameobject to spawn
    public GameObject stairFloor;

    public float nextLengthMin;

    public float nextLengthMax;
    
    //total (units of) platforms spawned
    public float numSpawned = 0;

    //starting position
    public float startingPosition;

    //current X to gen at
    public float currentX;

    //current Y to gen at
    public float currentY;

    //The players gameobject
    public GameObject character;

    //The players script, used for accessing player coords and other data
    public Character characterScript;

    //wall object for stairs
    public GameObject stairWall;

    //so we know how many points we need before scaling up our gaps
    public int[] pointsPerSide;

    //standard floor objects, and an empty object
    public GameObject floorObject;
    public GameObject floorPointObject;
    public GameObject emptyObject;

    //our game controller and its current sides
    public gameControl gc;
    public int sides = 2;
    public bool[] sidesCounted;
    //for testing purposes to see if possible
    public bool testing = true;
    public float timeIncrement;
    //public int pointsAdjusted = gc.points;
    //sign gameobject that shows an arrow
    public GameObject sign;
    //a list of all of our floors so we can delete and manage them easily
    public List<GameObject> floors = new List<GameObject>();

    public bool deleting;
    //what our current floor is and what number it is
    public GameObject currentFloor;
    public int curFloor = 0;

    //public bool tutorial = false;

    //public int[] setSpawns = new int[0];


    // Start is called before the first frame update
    void Start()
    {
        //initialize script and variables
        characterScript = character.GetComponent<Character>();
        //set X to start genning
        currentX = 5;
        currentY = 0;
        /*
        if (tutorial == true) {
            setSpawns = new int{0, 1, 2};
        }
        */
        curFloor = 0;
    }


    void FixedUpdate() {

    }
    // Update is called once per frame
    void Update()
    {
        //if were within 5 meters of our last gen
        if (characterScript.transform.position.x > currentX - 7) {
            //Debug.Log("Should Spawn");
            spawnNextItem();
            
            
        }
        /* if our state is == 0 delete floors, LEGACY
        if (gc.state == 0) {
            //Debug.Log("works");
            foreach(GameObject floor in floors) {
                Destroy(floor);
                Debug.Log("deleting floor");
            }
        }
        */
        //if we are set to delete and restart, delete all floors, clear our list, and trim it down to its minimum number. when finished clear deleting flag
        if (deleting) {
            foreach(GameObject floor in floors) {
                Destroy(floor);
                Debug.Log("deleting floor");
                
            }
            floors.Clear();
            floors.TrimExcess();
            deleting = false;
            /* LEGACY
            for ( int i = 0; i < floors.Count; i++) {
                GameObject floor = floors[i];
                if (!floor.GetComponent<Renderer>().isVisible) {
                    floors.RemoveAt(i);
                    i--;
                    Destroy(floor);
                }
            }
            /*
            foreach(GameObject floor in floors) {
                if (!floor.GetComponent<Renderer>().isVisible) {
                    Destroy(floor);
                }
            } 
            */
        }
        //if our floors are greater than 0, count each floor for tracking purposes, and its current x position to know where to spawn it at
        if(floors.Count > 0) {
            Debug.Log(floors.Count);
            if (characterScript.transform.position.x > floors[curFloor].GetComponent<FloorScript>().startingFloor.transform.position.x) {
                if (floors.Count > curFloor + 1) {
                    curFloor++;
                }
            }
        }
        
    }

    //restart our run easily in a function, by essentially running start() again
    public void restartRun() {
        deleting = true;
        //Debug.Log(character);
        //character = GameObject.FindWithTag("Player");
        //Debug.Log(character);
        Debug.Log("Getting Player");
        
        currentY = 0;
        currentX = 5;
        curFloor = 0;

    }

    //main controller for starting the next obstacle
    void spawnNextItem() {

        numSpawned++;
        //figure out the size for the obstacle
        for(int i = 0; i < characterScript.nextShapeScores.Length; i++) {
            if (numSpawned > characterScript.nextShapeScores[i] && sidesCounted[i] == false) {
                sides++;
                sidesCounted[i] = true;
                //Time.timeScale *= timeIncrement;
            }
        }

        int a;
        //pick a platform
        a = Random.Range(0,3);
        /*
        if(numSpawned < setSpawns.Length) {
            switch (numSpawned) {
                case 0:
                    spawnStair();
                    break;
                case 1:
                    spawnGap();
                    break;
                case 2:
                    spawnSlopes();
                    break;
            }
        }
        */
        //else{
        //Debug.Log(a);
        //switch case after RNG to choose the spawned platform
        switch (a) {
            
            case 0:
                spawnStair();
                gc.stairsSpawned++;
                gc.floorsSpawned++;
                break;
            case 1:
                spawnGap();
                gc.gapsSpawned++;
                gc.floorsSpawned++;
                break;
            case 2:
                spawnSlopes();
                gc.slopesSpawned++;
                gc.floorsSpawned++;
                break;
        }
        //}
    }

    //spawn a stair obstacle
    void spawnStair() {

        nextHeightMin =  (1.7f / 2);
        nextHeightMax =  (1.8f / 2);
        if (testing) {
            nextHeightMin = nextHeightMax;
        }

        //if were a pentagon, make playforms lower
        if (sides >= 4 && sides < 7) {//pointsPerSide[5] && numSpawned < pointsPerSide[6]) {
            nextHeightMin *=.9f;
            nextHeightMax *=.9f;
        }
        if (sides > 6) {
            nextHeightMin *= 1.19f;
            nextHeightMax *= 1.19f;
        }

        nextLengthMin = 10f;
        nextLengthMax = 12f;

        //Placeholder
        //if transitioning, make the length 2x as long, to allow player to adjust to new shape.

        //Generate our numbers
        float nextHeight = Random.Range(nextHeightMin, nextHeightMax);
        float nextLength = Random.Range(nextLengthMin, nextLengthMax);

        //Starting X of platform, the actual length, and where the platform ends
        float startingX = currentX;
        float actualLength = nextLength/2;
        float endingX = currentX + actualLength;
        float endPlatformX  ;
        float endPlatformY  ;
        float endPlatformLength;

        //Add our new platform to keep track
        currentX += actualLength;
        currentY += nextHeight;

        

        //midpoint of our platforms to move the position to
        float midpoint = (startingX + endingX) / 2;

        GameObject stairContainer = Instantiate(emptyObject);
        stairContainer.AddComponent<FloorScript>();
        floors.Add(stairContainer);
        //Generate a new platform
        GameObject newFloor = Instantiate(stairFloor, new Vector3(midpoint , currentY, 0), Quaternion.identity, stairContainer.transform);
        stairContainer.GetComponent<FloorScript>().startingFloor = newFloor;
        //floors.Add(newFloor);
        //Scale it to the length
        newFloor.transform.localScale = new Vector3(nextLength, 2, 1);
        //Debug.Log("currentX:" + currentX + " currentY:" + currentY + " nextLength:" + nextLength + " nextCenterX:" + currentX);
        //Debug.Log("startingX:" + startingX + " endingX:" + endingX + " nextLength*2:" + nextLength*2);
        GameObject newStairWall = Instantiate (stairWall, new Vector3(startingX, currentY - nextHeight/2, 0), Quaternion.Euler(0, 0, 90), stairContainer.transform);
        //floors.Add(newStairWall);
        newStairWall.transform.localScale = new Vector3(nextHeight*2, 2, 1);
        //var rotation = Quaternion.Euler(0, 0, 90);
        //newStairWall.transform.rotation = Quaternion.Slerp(newStairWall.transform.rotation, rotation, 1);
    }

    //spawn a gap obstacle
    void spawnGap() {

        //Debug.Log("SpawningGap");
        float startingX = currentX;
        float startingY = currentY;
        float lengthOfFirstStretch = 3f;
        float bottomPlatformLength = 10f;

        float lengthOfAngledBoard = .6f;
        float angledBoardX = currentX + lengthOfFirstStretch - (lengthOfAngledBoard / Mathf.Sqrt(2)) / 4;
        float angledBoardXLength = lengthOfAngledBoard / Mathf.Sqrt(2);
        float angledBoardY = currentY + lengthOfAngledBoard / Mathf.Sqrt(2) / 4;
        float lengthOfCatcher = 1.5f;
        
        float lengthOfGap = 1.375f + .125f * (sides - 2);
        float catcherX = startingX + lengthOfFirstStretch + lengthOfGap + lengthOfCatcher/2;
        float catcherWallX = startingX + lengthOfFirstStretch + lengthOfGap + lengthOfCatcher;
        float catcherWallHeight = 2f;
        float catcherWallY = startingY + catcherWallHeight/2;
        float bottomPlatformX = startingX + bottomPlatformLength/2;
        float bottomPlatformY = startingY - (4f + .5f * sides - 2);
        float endingY = bottomPlatformY;
        float endingX = startingX + bottomPlatformLength;

        GameObject gapContainer = Instantiate(emptyObject);
        gapContainer.AddComponent<FloorScript>();
        floors.Add(gapContainer);

        GameObject firstStretch = Instantiate(floorObject, new Vector3(startingX + lengthOfFirstStretch/2, startingY - .000001f, 0), Quaternion.identity, gapContainer.transform);
        firstStretch.transform.localScale = new Vector3(lengthOfFirstStretch*2, 2, 1);
        gapContainer.GetComponent<FloorScript>().startingFloor = firstStretch;
        //floors.Add(firstStretch);

        GameObject angledBoard = Instantiate(floorObject, new Vector3(angledBoardX, angledBoardY, 0), Quaternion.Euler(0,0,-45), gapContainer.transform);
        angledBoard.transform.localScale = new Vector3(lengthOfAngledBoard, 2, 1);
        //floors.Add(angledBoard);

        GameObject catcher = Instantiate(floorObject, new Vector3(catcherX, startingY, 0), Quaternion.identity, gapContainer.transform);
        catcher.transform.localScale = new Vector3(lengthOfCatcher*2, 2, 1);
        //floors.Add(catcher);

        GameObject catcherWall = Instantiate(floorObject, new Vector3(catcherWallX, catcherWallY, 0), Quaternion.Euler(0,0,90), gapContainer.transform);
        catcherWall.transform.localScale = new Vector3(catcherWallHeight*2, 2, 1);
        //floors.Add(catcherWall);

        GameObject bottomPlatform = Instantiate(floorPointObject, new Vector3(bottomPlatformX, bottomPlatformY, 0), Quaternion.identity, gapContainer.transform);
        bottomPlatform.transform.localScale = new Vector3(bottomPlatformLength*2, 2, 1);
        //floors.Add(bottomPlatform);
        
        GameObject gapSign = Instantiate(sign, new Vector3(angledBoardX - .4f, angledBoardY + .7f, -1300), Quaternion.Euler(0,0, -60), gapContainer.transform);
        /*
        foreach (Transform child in gapSign.GetComponent<Transform>()) {
            floors.Add(child.gameObject);
        }
        */

        currentX += bottomPlatformLength;
        currentY = bottomPlatformY;
        
        
    }

    //spawn a slope platform
    void spawnSlopes() {
        float startingX = currentX;
        float startingY = currentY;
        float firstPlatformLength = 4;
        float secondPlatformLength = 4;
        float platformAngles = 30;
        float gapSize = .5f + 0.18f * sides;
        if (sides > 4) {
            gapSize -= .065f * (sides-4); 
        }
        //Debug.Log(Mathf.Sin(platformAngles * Mathf.Deg2Rad) );
        float firstPlatformX = (Mathf.Cos(platformAngles* Mathf.Deg2Rad) * firstPlatformLength) / 2 + startingX;
        float firstPlatformY = startingY - (Mathf.Sin(platformAngles* Mathf.Deg2Rad) * firstPlatformLength) / 2;
        float secondPlatformX = (Mathf.Cos(platformAngles* Mathf.Deg2Rad) * secondPlatformLength) / 2 + startingX + gapSize + firstPlatformLength * Mathf.Cos(platformAngles* Mathf.Deg2Rad);
        float secondPlatformY = startingY - ((Mathf.Sin(platformAngles* Mathf.Deg2Rad) * secondPlatformLength) / 2);
        float pointPlatformLength = 5;
        float pointPlatformX = secondPlatformX + (Mathf.Cos(platformAngles* Mathf.Deg2Rad) * secondPlatformLength) / 2 + pointPlatformLength / 2;
        float pointPlatformY = currentY;
        float catcherLeftWallLength = 8;
        float catcherLeftWallX = firstPlatformX;
        float catcherLeftWallY = startingY - 3;
        float catcherRightWallLength = 8;
        float catcherRightWallX = secondPlatformX;
        float catcherRightWallY = catcherLeftWallY;
        float catcherBottomLength = (secondPlatformX - firstPlatformX) * 2;
        float catcherBottomX = (secondPlatformX + firstPlatformX) / 2;
        float catcherBottomY = catcherLeftWallY - catcherLeftWallLength / 4;

        GameObject slopeContainer = Instantiate(emptyObject);
        slopeContainer.AddComponent<FloorScript>();
        floors.Add(slopeContainer);

        GameObject firstPlatform = Instantiate (floorObject, new Vector3(firstPlatformX, firstPlatformY, 0), Quaternion.Euler(0, 0, -platformAngles), slopeContainer.transform);
        firstPlatform.transform.localScale = new Vector3(firstPlatformLength*2, 2, 1);
        slopeContainer.GetComponent<FloorScript>().startingFloor = firstPlatform;
        //floors.Add(firstPlatform);

        GameObject secondPlatform = Instantiate (floorObject, new Vector3(secondPlatformX, secondPlatformY, 0), Quaternion.Euler(0, 0, platformAngles), slopeContainer.transform);
        secondPlatform.transform.localScale = new Vector3(secondPlatformLength*2, 2, 1);
        //floors.Add(secondPlatform);

        GameObject pointPlatform = Instantiate (floorPointObject, new Vector3(pointPlatformX, pointPlatformY, 0), Quaternion.identity, slopeContainer.transform);
        pointPlatform.transform.localScale = new Vector3(pointPlatformLength*2, 2, 1);
        //floors.Add(pointPlatform);

        GameObject catcherLeftWall = Instantiate (floorObject, new Vector3(catcherLeftWallX, catcherLeftWallY, 0), Quaternion.Euler(0,0,90), slopeContainer.transform);
        catcherLeftWall.transform.localScale = new Vector3(catcherLeftWallLength, 2, 1);
        //floors.Add(catcherLeftWall);

        GameObject catcherRightWall = Instantiate (floorObject, new Vector3(catcherRightWallX, catcherRightWallY, 0), Quaternion.Euler(0,0,90), slopeContainer.transform);
        catcherRightWall.transform.localScale = new Vector3(catcherRightWallLength, 2, 1);
        //floors.Add(catcherRightWall);

        GameObject catcherBottom = Instantiate (floorObject, new Vector3(catcherBottomX, catcherBottomY, 0), Quaternion.identity, slopeContainer.transform);
        catcherBottom.transform.localScale = new Vector3(catcherBottomLength, 2, 1);
        //floors.Add(catcherBottom);

        GameObject slopeSign = Instantiate (sign, new Vector3(firstPlatformX + .8f, firstPlatformY + .8f, -1300), Quaternion.identity, slopeContainer.transform);
        /*
        foreach (Transform child in slopeSign.GetComponent<Transform>()) {
            floors.Add(child.gameObject);
        }
        */

        currentX = pointPlatformX + pointPlatformLength/2;
    }

}
