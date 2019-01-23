using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : Singleton<GameManager> {

    public static readonly int nbCharacters = 3;
    public static readonly int nbActionsPerCharacter = 4;
    public static readonly int nbGameVariables = 3;
    public static readonly int nbGameVariablesStates = 3;
    public static readonly int nbTourMax = 5;

    public PlayerSettings settings;

    public GameObject menuGroup;
    public GameObject animationGroup;
    public GameObject parallaxGameObject;
    public GameObject scanGroup;
    public GameObject textsGroup;
    public Text textToChange;

    public GameObject introPrefab;
    public GameObject outroPrefab;
    // all the cards that will be used to compute the fate and play animations for this turn
    public ScenarioCard currentScenarioCard;
    public TrapCard currentTrapCard;
    // ActionCard[id] is the card played by the character id
    public ActionCard[] currentActionCards = new ActionCard[nbCharacters];


    private enum State
    {
        Menu,
        Intro, //Animating introduction
        Draw, //Ask player to draw cards
        BadGuyPlaying, //Ask badGuy to play
        PlayersPlaying, //Timer + Scan button
        Scan, //Scan cards
        Animation, //Calcul of destiny and Animation 
        End, //End of game
        Outro //End animation
    }
    private State _gameStatus = State.Menu;
    private uint nbTour = 1;

    #region PUBLIC_METHODS


    public void nextState()
    {
        _gameStatus++;
        Debug.Log("TESLTJ : "+_gameStatus);
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

    public void ResetVariables()
    {
        for (int i=0; i<nbGameVariables; i++)
            gameVariables[i] = 0;
        
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
            case State.Menu:
                menuGroup.SetActive(true);
                break;
            case State.Intro:
                menuGroup.SetActive(false);
                animationGroup.SetActive(true);
                parallaxGameObject.GetComponent<Parallax>().addSprite(introPrefab);
                break;
            case State.Draw:
                animationGroup.SetActive(false);
                animationGroup.transform.Find("Parallax").GetComponent<Parallax>().clear();
                textsGroup.SetActive(true);
                textToChange.GetComponent<Text>().text = "Piochez svp (Faudra faire un vrai texte tout beau tout joli, bsx)";
                break;
            case State.BadGuyPlaying:
                textToChange.GetComponent<Text>().text = "Au tour du bad guy";
                break;
            case State.PlayersPlaying:
                textToChange.GetComponent<Text>().text = "Au tour des gentils :)";
                break;
            case State.Scan:
                textsGroup.SetActive(false);
                scanGroup.SetActive(true);
                break;
            case State.Animation:
                scanGroup.SetActive(false);
                animationGroup.SetActive(true);
                parallaxGameObject.GetComponent<Parallax>().addSprite(outroPrefab);
                break;
            case State.End:
                animationGroup.SetActive(false);
                animationGroup.transform.Find("Parallax").GetComponent<Parallax>().clear();
                textsGroup.SetActive(true);

                if (!isFinish())
                {
                    textToChange.GetComponent<Text>().text = "C la fin du tour";
                    nbTour++;
                    _gameStatus = State.Draw;
                    manageStatusAction();
                }
                else //If someone won
                {
                    textToChange.GetComponent<Text>().text = "C la fin du jeu";
                }
                break;
            case State.Outro:
                break;
        }     
    }

    private bool isFinish()
    {
        Debug.Log("BAD GUY" + badGuyHasWon());
        Debug.Log("Good Guys As Won" + goodGuysHasWon());
        return badGuyHasWon() || goodGuysHasWon();
    }

    private bool badGuyHasWon()
    {
        foreach (int var in gameVariables)
            if (var >= nbGameVariablesStates - 1)
                return true;    
        return false;
    }

    private bool goodGuysHasWon()
    {
        if (nbTour >= nbTourMax)
            return true;
        return false;
    }
    #endregion
}


