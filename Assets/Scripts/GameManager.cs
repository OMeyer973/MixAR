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
    public GameObject scanner;

    public GameObject introPrefab;

    private enum State
    {
        WaitingForStart,
        Intro, //Animating introduction
        Draw, //Ask player to draw cards
        BadGuyPlaying, //Ask badGuy to play
        PlayersPlaying, //Timer + Scan button
        Scan, //Scan cards
        Animation, //Calcul of destiny and Animation 
        End //End of game
    }
    private State _gameStatus = State.WaitingForStart;

    #region PUBLIC_METHODS


    public void nextState()
    {
        _gameStatus++;
        manageStatusAction();
    }
    // current state of the game Variables - 0 = initial state, high number = danger (>= 2 death)
    public List<int> gameVariables = new List<int>(nbGameVariables);
    
    // Use this for initialization
    public void begin() {
        ResetVariables();
        menu.SetActive(false);
        comicAnimation.SetActive(true);
        nextState();
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
    void manageStatusAction() {
        switch (_gameStatus)
        {
            case State.Intro:
                parallax.GetComponent<Parallax>().addSprite(introPrefab);
                break;
            case State.Draw:
                nextState();
                break;
            case State.BadGuyPlaying:
                nextState();
                break;
            case State.PlayersPlaying:
                nextState();
                break;
            case State.Scan:
                comicAnimation.SetActive(false);
                scanner.SetActive(true);
                break;
            case State.Animation:
                break;
        }
        
    }

    #endregion
}
