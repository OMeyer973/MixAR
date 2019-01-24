using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : Singleton<GameManager> {

    public static readonly int nbCharacters = 3;
    public static readonly int nbActionsPerCharacter = 4;

    // dice roll needs to be bellow maxDiceToChangeThreat to allow a character to change a game variable with a special card (0-100)
    public static readonly float criticalSucessScore = 0.05f;
    public static readonly float sucessScore = 0.5f;
    public static readonly float failureScore = 0.95f;

    public static readonly int nbThreats = 3;
    public static readonly int nbThreatsStates = 3;

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
    
    // ActionCard[id] is the action card played by the character id
    public ActionCard[] currentActionCards = new ActionCard[nbCharacters];

    protected float[] currentCharacterDices = new float[nbCharacters];
    protected float[] currentCharacterScores = new float[nbCharacters];
    protected float currentScenarioScore = 0;

    // current state of the game Variables - 0 = initial state, high number = danger (>= 2 death)
    // threats[0] == 1 -> the variable number 0 is at the state 1
    public int[] threats;

    private enum State
    {
        Menu,
        Intro, //Animating introduction
        NumTour,
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
    
    // Use this for initialization
    public void Awake()
    {
        threats = new int[nbThreats];
        ResetVariables();
    }

    public void ResetVariables()
    {
        for (int i=0; i<nbThreats; i++)
            threats[i] = 0;
        
    }

    public void SetCardsForNextTurn(List<Card> scannedCards)
    {
        // Debug.Log("Game manager recieving scanned cards");
        foreach (Card c in scannedCards)
        {
            // Debug.Log("GameManager recieving card : " + c);

            if (c is TrapCard)
            {
                currentTrapCard = (TrapCard)c;
                // Debug.Log("GameManager recieving trap card");
            }
            else if (c is ScenarioCard)
            {
                currentScenarioCard = (ScenarioCard)c;
                // Debug.Log("GameManager recieving scenario card");
            }
            if (c is ActionCard)
            {
                ActionCard tmpCard = (ActionCard)c;
                // Debug.Log("GameManager recieving action card : " + tmpCard.CharacterId);
                currentActionCards[tmpCard.CharacterId] = tmpCard;
            }
        }
    }
    

    // play a game turn after the players have scanned the cards and CardsScanner has sent the scanned cards to the Gamemanager
    public void PlayTurn()
    {
        nextState();
        ComputeFate();
        // TODO :
        // deactivate AR camera (whole scan gameobj)
        // play begining of scenario comics animation
        // play characters actions animations
        // play end of scenario animation
        // check if game is over
        // show AR variables
    }

    private void ComputeFate()
    {
        // roll dice
        for (int i = 0; i < nbCharacters; i++)
        {
            currentCharacterDices[i] = UnityEngine.Random.Range(0f, 1f);

            Debug.Log("computing action card : " + i);
            int currCharacterAction = currentActionCards[i].GetComponent<ActionCard>().ActionId;
            // *0.01 to bring fate matrix content from 0-100 to 0-1
            float currCharacterObjective = 0.01f * (float)currentScenarioCard.GetComponent<ScenarioCard>().FateMatrix[i, currCharacterAction];

            // a character score is his dice roll centerd back on 0.5 with range [0-1] in function of his fate matrix objective.
            // ie : (score <= 0.05) == critical success, (0.05 < score <= 0.5) == sucess, (0.5 < score <= 0.95) == failure, (0.95 < score) == critical failure
            if (currentCharacterDices[i] < currCharacterObjective)
            {
                currentCharacterScores[i] = 0.5f * currentCharacterDices[i] / currCharacterObjective;
            }
            else
            {
                currentCharacterScores[i] = 1f - 0.5f * (100 - currentCharacterDices[i]) / (100 - currCharacterObjective);
            }

            UpdateThreatsWithSpecialActionCard(i);

            currentScenarioScore += currentCharacterScores[i];
            
            Debug.Log("curr action : " + currCharacterAction + "\n" +
                      "curr objective : " + currCharacterObjective + "\n" +
                      "curr dice : " + currentCharacterDices[i] + "\n" +
                      "curr score : " + currentCharacterScores[i] + "\n"
                      );
            
        }
        currentScenarioScore /= nbCharacters;

        UpdateThreatsWithScenarioCard();
        currentScenarioCard.GetComponent<ScenarioCard>().Print();
        Debug.Log("total score : " + currentScenarioScore);
    }

    // takes the ID of an action card that has been played (from the currentActionCards[]list) 
    // and updates the Threats accordingly
    // -> if the card has ActionCard.ChangeThreat at true, 
    //    and the dice roll is bellow sucessScore,
    //    it will -1 the ActionCard.GamevarChanged game variable
    private void UpdateThreatsWithSpecialActionCard(int i)
    {
        if (currentActionCards[i].ChangeThreat)
        {
            Debug.Log("character " + i + " is trying to change game var " + currentActionCards[i].ThreatChanged);
            if (currentCharacterDices[i] < sucessScore)
            {
                threats[currentActionCards[i].ThreatChanged] = Math.Max(0, threats[currentActionCards[i].ThreatChanged] - 1);
                Debug.Log("character " + i + " successfuly got game var " + currentActionCards[i].ThreatChanged + "to diminish by one");
            }
            Debug.Log("character " + i + " failed to get game var " + currentActionCards[i].ThreatChanged + "to diminish by one");
        }
        else
        {
            Debug.Log("character " + i + " will not change game var");
        }
    }

    // updates the Threats in function of the scenario card and the total score of the players
    private void UpdateThreatsWithScenarioCard()
    {
        Debug.Log("Scenario card will influence game var " + currentScenarioCard.ThreatChanged);
        if (currentScenarioScore <= criticalSucessScore)
        { // critical success : variable get -1
            threats[currentScenarioCard.ThreatChanged] = Math.Max(0, threats[currentScenarioCard.ThreatChanged] - 1);
            Debug.Log("critical Scenario success : score " + currentScenarioScore + " made game var " + currentScenarioCard.ThreatChanged + " diminishe by one");
        }
        else if (currentScenarioScore <= sucessScore)
        { // normal sucess : no change
            Debug.Log("Scenario success : score " + currentScenarioScore + " made game var " + currentScenarioCard.ThreatChanged + " not change");
        }
        else if (currentScenarioScore <= failureScore)
        { // normal failure : variable get +1
            threats[currentScenarioCard.ThreatChanged] = threats[currentScenarioCard.ThreatChanged] + 1;
            Debug.Log("Scenario failure : score " + currentScenarioScore + " made game var " + currentScenarioCard.ThreatChanged + " increase by one");
        }
        else
        { // critical failure : variable get +1
            threats[currentScenarioCard.ThreatChanged] = threats[currentScenarioCard.ThreatChanged] + 1;
            Debug.Log("critical Scenario failure : score " + currentScenarioScore + " made game var " + currentScenarioCard.ThreatChanged + " increase by one");
        }

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
            case State.NumTour:
                animationGroup.SetActive(false);
                animationGroup.transform.Find("Parallax").GetComponent<Parallax>().clear();
                textsGroup.SetActive(true);
                textToChange.GetComponent<Text>().text = "Tour "+nbTour;
                break;
            case State.Draw:
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
        foreach (int var in threats)
            if (var >= nbThreatsStates - 1)
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


