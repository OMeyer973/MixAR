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

    protected float[] currentCharacterDices = new float[nbCharacters];
    protected float[] currentCharacterScores = new float[nbCharacters];
    protected float currentScenarioScore = 0;


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

    // play a game turn after the players have scanned the cards and CardsScanner has sent the scanned cards to the Gamemanager
    public void PlayTurn()
    {
        ComputeFate();
        // TODO :
        // play begining of scenario comics animation
        // play characters actions animations
        // play end of scenario animation
        // check if game is over
        // show AR variables
    }

    void ComputeFate()
    {
        // roll dice
        for (int i = 0; i < nbCharacters; i++)
        {
            int currCharacterAction = currentActionCards[i].GetComponent<ActionCard>().ActionId;
            float currCharacterObjective = currentScenarioCard.GetComponent<ScenarioCard>().FateMatrix[i, currCharacterAction];

            currentCharacterDices[i] = Random.Range(0, 100);

            // a character score is his dice roll centerd back on 0.5 with range [0-1] in function of his fate matrix objective.
            // ie : (score <= 0.05) == critical success, (0.05 < score <= 0.5) == sucess, (0.5 < score <= 0.95) == failure, (0.95 < score) == critical failure
            if (currentCharacterDices[i]<currCharacterObjective)
            {
                currentCharacterScores[i] = 0.5f * currentCharacterDices[i] / currCharacterObjective;
            }
            else
            {
                currentCharacterScores[i] = 1f - 0.5f * (100 - currentCharacterDices[i]) / (100 - currCharacterObjective);
            }

            currentScenarioScore += currentCharacterScores[i];
            /*
            Debug.Log("curr action : " + currCharacterAction + "\n" +
                      "curr objective : " + currCharacterObjective + "\n" +
                      "curr dice : " + currentCharacterDices[i] + "\n" +
                      "curr score : " + currentCharacterScores[i] + "\n"
                      );
            */
        }
        currentScenarioScore /= nbCharacters;
        /*
        currentScenarioCard.GetComponent<ScenarioCard>().Print();
        Debug.Log("player actions : " + currentActionCards[0].GetComponent<ActionCard>().ActionId + " " + 
                                        currentActionCards[1].GetComponent<ActionCard>().ActionId + " " + 
                                        currentActionCards[2].GetComponent<ActionCard>().ActionId + " " + 
                  "players dices  : " + currentCharacterDices[0] + " " + currentCharacterDices[1] + " " + currentCharacterDices[2] + "\n" +
                  "players scores : " + currentCharacterScores[0] + " " + currentCharacterScores[1] + " " + currentCharacterScores[2] + "\n" +
                  "total score : " + currentScenarioScore);
        */
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
