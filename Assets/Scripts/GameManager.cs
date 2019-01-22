using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    public static readonly int nbCharacters = 3;
    public static readonly int nbGameVariables = 3;
    public static readonly int nbGameVariablesStates = 3;

    public PlayerSettings settings;

    public GameObject menu;

    public GameObject comicAnimation;
    public GameObject parallax;

    // current state of the game Variables - 0 = initial state, high number = danger (>= 2 death)
    public List<int> gameVariables = new List<int>(nbGameVariables);
    
    // Use this for initialization
    public void Awake() {
        ResetVariables();
        menu.SetActive(false);
        comicAnimation.SetActive(true);
       
        parallax.GetComponent<Parallax>().addSprite(0, 0, 0);
        parallax.GetComponent<Parallax>().addSprite(0, 0, 0);
        parallax.GetComponent<Parallax>().addSprite(0, 0, 0);

    }

    public void ResetVariables()
    {
        gameVariables.Clear();
        for (int i=0; i<nbGameVariables; i++)
        {
            gameVariables.Add(0);
        }
        // test !! Todo : remove
        gameVariables[0] = 1;
        gameVariables[1] = 1;
        gameVariables[2] = 1;
        // end test !! Todo : remove
    }

    // Update is called once per frame
    // void loop() {

    // }
}
