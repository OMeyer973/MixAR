using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : Singleton<GameManager> {

    public static readonly int nbCharacters = 3;
    public static readonly int nbActionsPerCharacter = 4;

    public static readonly float criticalSucessScore = 0.05f;
    public static readonly float sucessScore = 0.5f;
    public static readonly float failureScore = 0.95f;

    // percentage of the objective that will be subtracted if the trap card is meant to influence a character
    public static readonly float trapCharacterHandicap = 0.2f;

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
    public GameObject threatsARGroup;
    public GameObject endScreenGroup;
    public GameObject endScreenGroup_WinnerText;
    public GameObject ARCamera;

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

    // current state of the threats - 0 = initial state, high number = danger (>= 2 death)
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
        ShowThreatsAR,
        End, //End of game
        Outro //End animation
    }
    private State _gameStatus = State.Menu;
    private uint nbTour = 1;

    #region PUBLIC_METHODS


    public void nextState()
    {
        _gameStatus++;
        manageStatusAction();
    }
    
    // Use this for initialization
    public void Awake()
    {
        threats = new int[nbThreats];
        ResetVariables();
        manageStatusAction();
    }

    public void ResetVariables()
    {
        for (int i=0; i<nbThreats; i++)
            threats[i] = 0;
        
    }

    public void backToMenu()
    {
        _gameStatus = State.Menu;
        ResetVariables();
        manageStatusAction();
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

            int currCharacterAction = currentActionCards[i].GetComponent<ActionCard>().ActionId;
            // *0.01 to bring fate matrix content from 0-100 to 0-1
            float currCharacterObjective = 0.01f * (float)currentScenarioCard.GetComponent<ScenarioCard>().FateMatrix[i, currCharacterAction];

            Debug.Log("character : " + i + " has played action " + currCharacterAction + ", his objective is " + currCharacterObjective);

            if (currentTrapCard.InfluenceCharacter && currentTrapCard.CharacterToInfluence == i)
            {
                Debug.Log("a trap has been activated ! character " + i + " has a handicap of " + trapCharacterHandicap + " on his objective");
                currCharacterObjective -= currCharacterObjective * trapCharacterHandicap;
            }

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

            Debug.Log("character : " + i + " has rolled a " + currentCharacterDices[i] + ", his score is " + currentCharacterScores);
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
    //    it will -1 the ActionCard.ThreatToChange threat
    private void UpdateThreatsWithSpecialActionCard(int i)
    {
        if (currentActionCards[i].ChangeThreat)
        {
            Debug.Log("character " + i + " action may change threat " + currentActionCards[i].ThreatToChange + ". Let's see his roll");
            if (currentCharacterDices[i] < sucessScore)
            {
                threats[currentActionCards[i].ThreatToChange] = Math.Max(0, threats[currentActionCards[i].ThreatToChange] - 1);
                Debug.Log("character " + i + " roll of " + currentCharacterDices[i] + " allowed his action to diminish threat " + currentActionCards[i].ThreatToChange + "by one");
            }
            Debug.Log("character " + i + " roll of " + currentCharacterDices[i] + " prevented his action to diminish threat " + currentActionCards[i].ThreatToChange + "by one");
        }
    }

    // updates the Threats in function of the scenario card and the total score of the players
    private void UpdateThreatsWithScenarioCard()
    {
        Debug.Log("Scenario card may change threat " + currentScenarioCard.ThreatToChange + ". Let's see the character rolls");
        if (currentScenarioScore <= criticalSucessScore)
        { // critical success : variable get -1
            threats[currentScenarioCard.ThreatToChange] = Math.Max(0, threats[currentScenarioCard.ThreatToChange] - 1);
            Debug.Log("critical Scenario success : score of " + currentScenarioScore + " made threat " + currentScenarioCard.ThreatToChange + " diminishe by one");
        }
        else if (currentScenarioScore <= sucessScore)
        { // normal sucess : no change
            Debug.Log("Scenario success : score of " + currentScenarioScore + " made threat " + currentScenarioCard.ThreatToChange + " not change");
        }
        else if (currentScenarioScore <= failureScore)
        { // normal failure : variable get +1
            threats[currentScenarioCard.ThreatToChange] = threats[currentScenarioCard.ThreatToChange] + 1;
            Debug.Log("Scenario failure : score of " + currentScenarioScore + " made threat " + currentScenarioCard.ThreatToChange + " increase by one");
        }
        else
        { // critical failure : variable get +1
            threats[currentScenarioCard.ThreatToChange] = threats[currentScenarioCard.ThreatToChange] + 1;
            Debug.Log("critical Scenario failure : score of " + currentScenarioScore + " made threat " + currentScenarioCard.ThreatToChange + " increase by one");
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
                ARCamera.SetActive(true);
                textsGroup.SetActive(false);
                scanGroup.SetActive(true);
                break;
            case State.Animation:
                ARCamera.SetActive(false);
                scanGroup.SetActive(false);
                animationGroup.SetActive(true);
                parallaxGameObject.GetComponent<Parallax>().addSprite(outroPrefab);
                break;
            case State.ShowThreatsAR:
                ARCamera.SetActive(true);
                animationGroup.SetActive(false);
                animationGroup.transform.Find("Parallax").GetComponent<Parallax>().clear();

                threatsARGroup.SetActive(true);
                break;
            case State.End:
                ARCamera.SetActive(false);
                threatsARGroup.SetActive(false);
                
                if (!isFinish())
                {
                    nbTour++;
                    _gameStatus = State.NumTour;
                    manageStatusAction();
                }
                else //If someone won
                {
                    endScreenGroup.SetActive(true);
                    if (badGuyHasWon())
                    {
                        endScreenGroup_WinnerText.GetComponent<Text>().text = "Le méchant a gagné";
                    }
                    else
                    {
                        endScreenGroup_WinnerText.GetComponent<Text>().text = "Les gentils ont gagné";
                    }
                }
                break;
            case State.Outro:
                break;
        }     
    }

    private bool isFinish()
    {
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


