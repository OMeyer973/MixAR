using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : Singleton<GameManager> {

    public static readonly int nbCharacters = 3;
    public static readonly int nbActionsPerCharacter = 5;
    public static readonly int doNothingId = nbActionsPerCharacter - 1;
    public static readonly float doNothingHandicap = 0.2f;

    // tmp score used to balance dice rolls, apply item influence - range[0-1]
    public static readonly float maxCriticalSucessScore = 0.05f;
    public static readonly float maxSucessScore = 0.5f;
    public static readonly float maxFailureScore = 0.95f;
    public static readonly float neutralScore = 0.5f;


    // sucess values used to balance the game (failure + critical sucess = sucess, etc...) - range[0-1]
    public static readonly float criticalSucessValue = 0f;
    public static readonly float sucessValue = 0.25f;
    public static readonly float neutralValue = 0.5f;
    public static readonly float failureValue = 0.75f;
    public static readonly float criticalFailureValue = 1f;


    public static readonly int nbThreats = 3;
    public static readonly int nbThreatsStates = 3;

    public static readonly int nbTourMax = 5;

    public PlayerSettings settings;

    public GameObject menuGroup;
    public GameObject animationGroup;
    public GameObject parallaxGameObject;
    public GameObject scanGroup;
    public GameObject buttonNextScan;
    public GameObject textsGroup;
    public GameObject piocheBadGuyGroup;
    public GameObject piocheGentilsGroup;
    public Text textToChange;
    public GameObject endScreenGroup;
    public GameObject endScreenGroup_WinnerText;
    
    public GameObject introPrefab;
    public GameObject outroPrefab;

    // all the cards that will be used to compute the fate and play animations for this turn
    public ScenarioCard currentScenarioCard;
    public ItemCard currentItemCard;
    
    // ActionCard[id] is the action card played by the character id
    public ActionCard[] currentActionCards = new ActionCard[nbCharacters];

    // dices the player will throw. range 0-1
    protected float[] currentCharacterDices = new float[nbCharacters];
    // sucess or failure of a scenario or character action. -2 total failure, -1 failure, 1 sucess, 2 total sucess
    protected float currentScenarioSucess = 0;
    protected float[] currentCharacterSucess = new float[nbCharacters];

    // current state of the threats - 0 = initial state, high number = danger (>= 2 death)
    // threats[0] == 1 -> the variable number 0 is at the state 1
    public int[] threats;

    public bool onlyThreats = false; //Used to allow or not game cards scanning

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
        nbTour = 1;
        manageStatusAction();
    }

    public void SetCardsForNextTurn(List<Card> scannedCards)
    {
        // Debug.Log("Game manager recieving scanned cards");
        foreach (Card c in scannedCards)
        {
            // Debug.Log("GameManager recieving card : " + c);

            if (c is ItemCard)
            {
                currentItemCard = (ItemCard)c;
                // Debug.Log("GameManager recieving item card");
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
        currentScenarioCard.GetComponent<ScenarioCard>().Print();

        // handicap applied on the scenario sucess -> comes from a player doing nothing
        float totalHandicap = 0.0f;
        // roll dice
        for (int i = 0; i < nbCharacters; i++)
        {
            currentCharacterDices[i] = UnityEngine.Random.Range(0f, 1f);

            int currCharacterAction = currentActionCards[i].GetComponent<ActionCard>().ActionId;
            float currCharacterScore = 0.5f;

            // CHARACTER IS DOING NOTHING
            if (currCharacterAction == doNothingId)
            {
                Debug.Log("character " + i + " has done nothing this turn, he gives a handicap of " + doNothingHandicap + " to everybody. His dice roll wil also not be taken into account.");
                totalHandicap += doNothingHandicap;
                currCharacterScore = 0.5f;
            }
            // ELSE COMPUTING SCORE
            else
            {
                // *0.01 to bring fate matrix content from 0-100 to 0-1
                float currCharacterObjective = 0.01f * (float)currentScenarioCard.GetComponent<ScenarioCard>().FateMatrix[i, currCharacterAction];

                Debug.Log("character " + i + " has played action " + currCharacterAction + ", his objective is " + currCharacterObjective);

                // a character score is his dice roll centerd back on 0.5 with range [0-1] in function of his fate matrix objective.
                // ie : (score <= 0.05) == critical success, (0.05 < score <= 0.5) == sucess, (0.5 < score <= 0.95) == failure, (0.95 < score) == critical failure
                if (currentCharacterDices[i] < currCharacterObjective)
                {
                    currCharacterScore = 0.5f * currentCharacterDices[i] / currCharacterObjective;
                }
                else
                {
                    currCharacterScore = 1f - 0.5f * (1 - currentCharacterDices[i]) / (1 - currCharacterObjective);
                }
            }

            
                currCharacterScore = ItemCardInfluenceCharacterScore(i, currCharacterScore);


            currentCharacterSucess[i] = ComputeSucessFromScore(currCharacterScore);

            UpdateThreatsWithSpecialActionCard(i);

            currentScenarioSucess += currentCharacterSucess[i];

            Debug.Log("character " + i + " has rolled a " + currentCharacterDices[i] + " (score : " + currCharacterScore + "), his sucess is " + currentCharacterSucess[i]);
        }

        currentScenarioSucess /= nbCharacters;
        currentScenarioSucess += totalHandicap;

        UpdateThreatsWithScenarioCard();

        UpdateThreatsWithItemCard();
    }

    // check if the current item card will influence the current character score and apply this influence
    // item card has a id of character to influence, if the id is greater than the number of players, it will affect all characters
    private float ItemCardInfluenceCharacterScore(int charId, float currentScore)
    {
        if (currentItemCard.InfluenceCharacter && (currentItemCard.CharacterToInfluence == charId || currentItemCard.CharacterToInfluence >= nbCharacters))
        {
            // *0.01 because influence is between 0-100 on the json
            float itemCharacterHandicap = 0.01f * currentItemCard.InfluenceCharacterBy;
            Debug.Log("a item has been activated ! character " + charId + " has a " +
                (itemCharacterHandicap < 0 ? ("bonus of " + -itemCharacterHandicap) : ("handicap of " + itemCharacterHandicap)) +
                " on his roll");
            return Math.Min(Math.Max(currentScore + itemCharacterHandicap, 0), 1);
        }
        return currentScore;
    }

    // given a score value (0-1), return a sucess value (0-1)
    private float ComputeSucessFromScore(float score)
    {
        if (score == neutralScore) return neutralValue;
        if (score <= maxCriticalSucessScore) return criticalSucessValue;
        if (score <= maxSucessScore) return sucessValue;
        if (score <= maxFailureScore) return failureValue;
        return criticalFailureValue;
    }

    // takes the ID of an action card that has been played (from the currentActionCards[]list) 
    // and updates the Threats accordingly
    // -> if the card has ActionCard.ChangeThreat at true, 
    //    and the dice roll is bellow maxSucessScore,
    //    it will -1 the corresponding threat (ActionCard.ThreatToChange)
    private void UpdateThreatsWithSpecialActionCard(int i)
    {
        if (currentActionCards[i].ChangeThreat)
        {
            Debug.Log("character " + i + " action may change threat " + currentActionCards[i].ThreatToChange + ". Let's see his luck");
            if (currentCharacterSucess[i] < neutralValue)
            {
                threats[currentActionCards[i].ThreatToChange] = Math.Max(0, threats[currentActionCards[i].ThreatToChange] - 1);
                Debug.Log("character " + i + " sucess of " + currentCharacterSucess[i] + " allowed his action to diminish threat " + currentActionCards[i].ThreatToChange + "by one");
            }
            Debug.Log("character " + i + " failure of " + currentCharacterSucess[i] + " prevented his action to diminish threat " + currentActionCards[i].ThreatToChange + "by one");
        }
    }

    // updates the Threats in function of the scenario card and the total score of the players
    private void UpdateThreatsWithScenarioCard()
    {
        Debug.Log("Scenario card may change threat " + currentScenarioCard.ThreatToChange + ". Let's see the character rolls");
        if (currentScenarioSucess <= sucessValue)
        { // critical success : variable get -1
            threats[currentScenarioCard.ThreatToChange] = Math.Max(0, threats[currentScenarioCard.ThreatToChange] - 1);
            Debug.Log("critical Scenario success of " + currentScenarioSucess + " made threat " + currentScenarioCard.ThreatToChange + " diminishe by one");
        }
        else if (currentScenarioSucess <= neutralValue)
        { // normal sucess : no change
            Debug.Log("Scenario success of " + currentScenarioSucess + " made threat " + currentScenarioCard.ThreatToChange + " not change");
        }
        else if (currentScenarioSucess <= failureValue)
        { // normal failure : variable get +1
            threats[currentScenarioCard.ThreatToChange] += 1;
            Debug.Log("Scenario failure of " + currentScenarioSucess + " made threat " + currentScenarioCard.ThreatToChange + " increase by one");
        }
        else
        { // critical failure : variable get +1
            threats[currentScenarioCard.ThreatToChange] += 1;
            Debug.Log("critical Scenario failure of " + currentScenarioSucess + " made threat " + currentScenarioCard.ThreatToChange + " increase by one");
        }
    }

    private void UpdateThreatsWithItemCard()
    {
        if (currentItemCard.InfluenceThreat)
        {
            // bad guy Item card, will make threat increase
            if (currentItemCard.InfluenceThreatBy > 0)
            {
                // has an influence only if the characters fail their scenario
                // and if this influence doesn't end the game (hence the "< nbThreatsStates - 1")
                if (currentScenarioSucess <= failureValue && threats[currentScenarioCard.ThreatToChange] + currentItemCard.InfluenceThreatBy < nbThreatsStates - 1)
                Debug.Log("Bad guy item has been played ! Threat " + currentItemCard.ThreatToInfluence + " will increase by " + currentItemCard.InfluenceThreatBy);
                threats[currentItemCard.ThreatToInfluence] += currentItemCard.InfluenceThreatBy;
            }
        }

        // good guy Item card, will make threat decrease
        if (currentItemCard.InfluenceThreatBy < 0)
        {
            // always has an influence (only does nothing if the threat is allready at 0
            Debug.Log("Good guy item has been played ! Threat " + currentItemCard.ThreatToInfluence + " will decrease by " + currentItemCard.InfluenceThreatBy);
            threats[currentItemCard.ThreatToInfluence] = Math.Max(0, threats[currentItemCard.ThreatToInfluence] + currentItemCard.InfluenceThreatBy);
        }
    }


    // Update is called once per frame
    void manageStatusAction() {
        switch (_gameStatus)
        {
            case State.Menu:
                endScreenGroup.SetActive(false);
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
                textsGroup.SetActive(false);
                piocheBadGuyGroup.SetActive(true);
                break;
            case State.PlayersPlaying:
                piocheBadGuyGroup.SetActive(false);
                piocheGentilsGroup.SetActive(true);
                break;
            case State.Scan:
                piocheGentilsGroup.SetActive(false);
                scanGroup.SetActive(true);
                buttonNextScan.SetActive(false);
                onlyThreats = false;
                break;
            case State.Animation:
                scanGroup.SetActive(false);
                animationGroup.SetActive(true);
                parallaxGameObject.GetComponent<Parallax>().addSprite(outroPrefab);
                break;
            case State.ShowThreatsAR:
                animationGroup.SetActive(false);
                animationGroup.transform.Find("Parallax").GetComponent<Parallax>().clear();
                buttonNextScan.SetActive(true);
                onlyThreats = true;
                scanGroup.SetActive(true);
                break;
            case State.End:
                scanGroup.SetActive(false);
                
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


