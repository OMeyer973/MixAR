using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    public static readonly int nbCharacters = 3;
    public static readonly int nbActionsPerCharacter = 4;
    public static readonly int nbGameVariables = 3;
    public static readonly int nbGameVariablesStates = 3;

    public PlayerSettings settings;

    public GameObject menu;
    public GameObject comicAnimation;
    public GameObject parallax;
    public GameObject scanner;

    public GameObject introPrefab;

    // all the cards that will be used to compute the fate and play animations for this turn
    public ScenarioCard currentScenarioCard;
    public TrapCard currentTrapCard;
    // ActionCard[id] is the card played by the character id
    public ActionCard[] currentActionCards = new ActionCard[nbCharacters];


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
    // gameVariables[0] == 1 -> the variable number 0 is at the state 1
    public int[] gameVariables = new int[nbGameVariables];

    // Use this for initialization
    public void Awake()
    {
        ResetVariables();
    }

    public void begin() {
        menu.SetActive(false);
        comicAnimation.SetActive(true);
        nextState();
    }

    public void ResetVariables()
    {
        for (int i=0; i<nbGameVariables; i++)
        {
            gameVariables[i] = 0;
        }
        // test !! Todo : remove
        gameVariables[0] = 1;
        gameVariables[1] = 2;
        gameVariables[2] = 0;
        // end test !! Todo : remove
    }

    public void SetCardsForNextTurn(List<Card> scannedCards)
    {
        foreach (Card c in scannedCards)
        {
            if (c is TrapCard)                
                currentTrapCard = (TrapCard)c;
            else if (c is ScenarioCard)
                currentScenarioCard = (ScenarioCard)c;
            if (c is ActionCard)
            {
                ActionCard tmpCard = (ActionCard)c;
                currentActionCards[tmpCard.CharacterId] = tmpCard;
            }
        }
    }

    void ComputeFate()
    {
        // roll dice

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
