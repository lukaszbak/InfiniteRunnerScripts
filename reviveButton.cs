using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//the reviveButton script to update the revive buttons features
public class reviveButton : MonoBehaviour
{

    public int numRevives = 0;
    public gameControl gc;
    public Text buttonText;
    // Start is called before the first frame update
    void Start()
    {
        numRevives = gc.revivesLeft;
        buttonText.text = "Revives (" + numRevives + ")";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
