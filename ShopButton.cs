using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


//the shop button script that updates the data of each shopbutton.
//TODO add the pointer and selector to make it controller friendly for console ports.
public class ShopButton : MonoBehaviour, IPointerExitHandler, IMoveHandler, ISelectHandler, IDeselectHandler
{

    public int level;
    public int maxLevel;
    public int[] cost;
    public string[] names;
    public float[][] requirements;
    public bool unlocked;
    public float[] currentStats;
    public gameControl gc;
    public Text itemName;
    public string[] description;
    public string missingReqs;
    public string[] reqNames;
    public Button btn;
    public int frameCounter = 0;

    //the tooltip gameobject
    public GameObject ToolTip;
    public RectTransform tf;

    //the tooltip text object
    public GameObject TT;
    public int butNum;
    public string buttonText;





    // Start is called before the first frame update
    void Awake()
    {  
        //yield WaitForFixedUpdate(1);
        tf = GetComponent<RectTransform>();
        btn = GetComponent<Button>();
        //check requirements for the purchase to see if its available, and then check which level the upgrade should be, and read the texts it should have according to the level
        checkReqs();
        level = gc.upgradesBought[butNum];
        buttonText = names[level] + "\nCost: " + cost[level];
        itemName.text = buttonText;
        if (level == maxLevel) {
            itemName.text = "Sold out";
        }
        //Debug.Log(gc.upgradesBought);
        
    }

    // Update is called once per frame
    //old stuff that doesnt work anymore since moving to a selectable interface
    void Update()
    {
        /*
        if (isHighlighted(EventSystems.BaseEventData eventData)) {
            if (frameCounter < 60) {
                frameCounter++;
                Debug.Log("ToolTip " + frameCounter);
            }
            else {
                TT = Instantiate(ToolTip, new Vector3(tf.position.x + tf.rect.width/2, tf.position.y, 0), Quaternion.identity);
            }
        }
        else frameCounter = 0;
        */
    }

    public void OnSelect (BaseEventData eventData) {
        /*
        if (frameCounter < 60) {
            frameCounter++;
            Debug.Log("ToolTip " + frameCounter);
        }
        else {
            */
            //if selected, instantiate the tooltip that it will use next to the button
            TT = Instantiate(ToolTip, new Vector3(tf.position.x + tf.rect.width/2, tf.position.y, 0), Quaternion.identity, gc.transform);
        //}

    }

    //when deselected destroy the tooltip
    public void OnDeselect(BaseEventData eventData) {
        Destroy(TT);
    }



    //or when the pointer leaves the area, delete the tooltip
    public void OnPointerExit(PointerEventData eventData) {
        //frameCounter = 0;
        Destroy(TT);
    }

    //TODO movement handling
    public void OnMove(AxisEventData eventData) {
        //moveDirection moveDir = eventData.moveDir;
    }

    //try to purchase the upgrade if its unlocked, and we have more points than is required, and the level is less than max level
    public void tryToPurchase() {
        if(unlocked && cost[level] <= gc.numPoints && level < maxLevel){
            gc.numPoints -= cost[level];
            level++;
            //check requirements after purchase to see if we can display the next item
            checkReqs();
            buttonText = names[level] + "\nCost: " + cost[level];
            itemName.text = buttonText;
            if (level == maxLevel) {
                itemName.text = "Sold out";
            }
            //send the purchase upgrade to the gameControl to update the data
            gc.purchaseUpgrade(butNum);
        }
    }

    //TODO implement this better so that it will have more realistiv functions in the game
    //as of right now there are no stats for the items to check so this will just say its allowed if its unlocked.
    void checkReqs() {
        int i = 0;
        unlocked = true;
        foreach (float stat in currentStats) {
            if (stat < requirements[i][level]) {
                unlocked = false;
            }
        }
    }
}
