using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : Singleton<GameManager> {

    #region PUBLIC_MEMBERS

    public static readonly int nbCharacters = 3;
    public static readonly int nbActionsPerCharacter = 5;
    public static readonly int doNothingId = nbActionsPerCharacter - 1;
    public static readonly float doNothingHandicap = 0.2f;

    // score used to get sucess value in function of dice rolls and apply item influence - range[0-1]
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

    public static readonly int nbItems = 8;

    public static readonly int nbTurnsMax = 5;

    public PlayerSettings settings;

    public GameObject menuGroup;
    public GameObject scanGroup;
    public GameObject textsGroup;
    public GameObject badGuyTurnGroup;
    public GameObject goodGuyTurnGroup;
    public GameObject endScreenGroup;
    public GameObject endScreenGroup_WinnerText;
    public GameObject animationGroup;

    public bool scanOnlyThreats = false; //Used to allow or not game cards scanning

    // current state of the threats - 0 = initial state, high number = danger (>= 2 death)
    // threats[0] == 1 -> the variable number 0 is at the state 1
    public int[] threats;

    #endregion // PUBLIC_MEMBERS

    #region PRIVATE_MEMBERS

    //canvas to activate or de activate in the scan screen
    private GameObject ARThreatsCanvas;
    private GameObject cardsScanCanvas;

    // text to change in the textScreen
    private Text bigTextToChange;
    private Text smallTextToChange;


    // all the cards that will be used to compute the fate and play animations for this turn
    private ScenarioCard currentScenarioCard;
    private ItemCard currentItemCard;
    
    // ActionCard[id] is the action card played by the character id
    private ActionCard[] currentActionCards = new ActionCard[nbCharacters];

    // dices the player will throw. range 0-1
    protected float[] currentCharacterDices = new float[nbCharacters];
    // sucess or failure of a scenario or character action. 1 total failure, 0.75 failure, 0.25 sucess, 0 total sucess
    protected float currentScenarioSucess = 0;
    protected float[] currentCharacterSucess = new float[nbCharacters];


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
    private uint nbTurn = 1;

    #endregion // PRIVATE_MEMBERS

    #region PUBLIC_METHODS

    public void nextState()
    {
        _gameStatus++;
        ManageStatusAction();
    }
    
    // Use this for initialization
    public void Awake()
    {
        ARThreatsCanvas = scanGroup.transform.Find("ARThreatsCanvas").gameObject;
        cardsScanCanvas = scanGroup.transform.Find("cardsScanCanvas").gameObject;

        bigTextToChange = textsGroup.transform.Find("TextCanvas/bigTextPanel/bigText").GetComponent<Text>();
        smallTextToChange = textsGroup.transform.Find("TextCanvas/smallTextPanel/smallText").GetComponent<Text>();

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        threats = new int[nbThreats];
        // just getting an instance of Texts to call init and initiate the static lists of texts. we will not need it later
        // there is probably a cleaner way to do this but meh
        Texts texts = Texts.Instance;
        texts.Init();
        ResetVariables();
        ManageStatusAction();
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
        nbTurn = 1;
        ManageStatusAction();
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


    #endregion //PUBLIC_METHODS
    
    #region PRIVATE_METHODS

    private void ComputeFate()
    {
        currentScenarioCard.GetComponent<ScenarioCard>().Print();

        // handicap applied on the scenario sucess -> comes from a player doing nothing
        float totalHandicap = 0.0f;
        
        for (int i = 0; i < nbCharacters; i++)
        {
            // roll dice
            currentCharacterDices[i] = UnityEngine.Random.Range(0f, 1f);

            int currCharacterAction = currentActionCards[i].GetComponent<ActionCard>().ActionId;
            float currCharacterScore;

            // CHARACTER IS DOING NOTHING
            if (currCharacterAction == doNothingId)
            {
                currentActionCards[i].GetComponent<ActionCard>()._message = (Texts.Characters[i] + " " + Texts.Actions[i, doNothingId] +
                " Il gêne ses coéquipiers et leur donne à tous un handicap de " + (100f * doNothingHandicap) + "% sur leur action !");
                totalHandicap += doNothingHandicap;
                currCharacterScore = 0.5f;
            }
            // ELSE COMPUTING SCORE
            else
            {
                // *0.01 to bring fate matrix content from 0-100 to 0-1
                float currCharacterObjective = 0.01f * (float)currentScenarioCard.GetComponent<ScenarioCard>().FateMatrix[i, currCharacterAction];

                // Debug.Log("character " + i + " has played action " + currCharacterAction + ", his objective is " + currCharacterObjective);
                currentActionCards[i].GetComponent<ActionCard>()._message = (Texts.Characters[i] + " " + Texts.Actions[i, currCharacterAction]);
                
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

            // Debug.Log("character " + i + " has rolled a " + currentCharacterDices[i] + " (score : " + currCharacterScore + "), his sucess is " + currentCharacterSucess[i]);
            currentActionCards[i].GetComponent<ActionCard>()._message += (
                (currentCharacterSucess[i] <= sucessValue ? Texts.CharactersSucess[i] : Texts.CharactersFailure[i]) +
                (currentCharacterSucess[i] <= criticalSucessValue ? " " + Texts.CriticalSucess : "") +
                (currentCharacterSucess[i] >= criticalFailureValue ? " " + Texts.CriticalFailure : "") +
                (currentCharacterSucess[i] != neutralValue ? " (" + (int)(100f-100f*currCharacterScore) + "% de réussite du scénario)" : "")
            );
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
            currentItemCard.GetComponent<ItemCard>()._message = (Texts.Items[currentItemCard.ItemId] + " " + Texts.Characters[charId] + " a un " +
                (itemCharacterHandicap < 0 ? "bonus de " : "handicap de ") +
                (int)(100f * -itemCharacterHandicap) + "% sur son action !");
            /*Debug.Log("a item has been activated ! character " + charId + " has a " +
                (itemCharacterHandicap < 0 ? ("bonus of " + -itemCharacterHandicap) : ("handicap of " + itemCharacterHandicap)) +
                " on his roll");*/
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
            // Debug.Log("character " + i + " action may change threat " + currentActionCards[i].ThreatToChange + ". Let's see his luck");
            if (currentCharacterSucess[i] < neutralValue)
            {
                threats[currentActionCards[i].ThreatToChange] = Math.Max(0, threats[currentActionCards[i].ThreatToChange] - 1);
                //Debug.Log("character " + i + " sucess of " + currentCharacterSucess[i] + " allowed his action to diminish threat " + currentActionCards[i].ThreatToChange + "by one");
                currentActionCards[i].GetComponent<ActionCard>()._message += "\n" + ("Grace à l'action de " + Texts.Characters[i] + ", " + Texts.ThreatMinus[currentActionCards[i].ThreatToChange]);
            }
            //Debug.Log("character " + i + " failure of " + currentCharacterSucess[i] + " prevented his action to diminish threat " + currentActionCards[i].ThreatToChange + "by one");
            currentActionCards[i].GetComponent<ActionCard>()._message += "\n" + ("L'action de " + Texts.Characters[i] + " n'a pas été très utile, " + Texts.ThreatStay);
        }
    }

    // updates the Threats in function of the scenario card and the total score of the players
    private void UpdateThreatsWithScenarioCard()
    {
        //Debug.Log("Scenario card may change threat " + currentScenarioCard.ThreatToChange + ". Let's see the character rolls");
        if (currentScenarioSucess <= sucessValue)
        { // critical success : variable get -1
            threats[currentScenarioCard.ThreatToChange] = Math.Max(0, threats[currentScenarioCard.ThreatToChange] - 1);
            //Debug.Log("critical Scenario success of " + currentScenarioSucess + " made threat " + currentScenarioCard.ThreatToChange + " diminishe by one");
            currentScenarioCard.GetComponent<ScenarioCard>()._message += "\n" + ("Succès critique du scenario à " + (int)(100f - 100f * currentScenarioSucess) + "% !! " + Texts.ThreatMinus[currentScenarioCard.ThreatToChange]);
        }
        else if (currentScenarioSucess <= neutralValue)
        { // normal sucess : no change
          //Debug.Log("Scenario success of " + currentScenarioSucess + " made threat " + currentScenarioCard.ThreatToChange + " not change");
            currentScenarioCard.GetComponent<ScenarioCard>()._message += "\n" + ("Succès du scenario à " + (int)(100f - 100f * currentScenarioSucess) + "% ! " + Texts.ThreatStay);
        }
        else if (currentScenarioSucess <= failureValue)
        { // normal failure : variable get +1
            threats[currentScenarioCard.ThreatToChange] += 1;
            //Debug.Log("Scenario failure of " + currentScenarioSucess + " made threat " + currentScenarioCard.ThreatToChange + " increase by one");
            currentScenarioCard.GetComponent<ScenarioCard>()._message += "\n" + ("Echec du scenario à " + (int)(100f - 100f * currentScenarioSucess) + "% !! " + Texts.ThreatPlus[currentScenarioCard.ThreatToChange]);
        }
        else
        { // critical failure : variable get +1
            threats[currentScenarioCard.ThreatToChange] += 1;
            //Debug.Log("critical Scenario failure of " + currentScenarioSucess + " made threat " + currentScenarioCard.ThreatToChange + " increase by one");
            currentScenarioCard.GetComponent<ScenarioCard>()._message += "\n" + ("Echec critique du scenario à " + (int)(100f - 100f * currentScenarioSucess) + "% !!! " + Texts.ThreatPlus[currentScenarioCard.ThreatToChange]);
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
                currentItemCard.GetComponent<ItemCard>()._message += "\n" + ("Bad guy item has been played ! Threat " + currentItemCard.ThreatToInfluence + " will increase by " + currentItemCard.InfluenceThreatBy);
                threats[currentItemCard.ThreatToInfluence] += currentItemCard.InfluenceThreatBy;
            }
        }

        // good guy Item card, will make threat decrease
        if (currentItemCard.InfluenceThreatBy < 0)
        {
            // always has an influence (only does nothing if the threat is allready at 0
            currentItemCard.GetComponent<ItemCard>()._message += "\n" + ("Good guy item has been played ! Threat " + currentItemCard.ThreatToInfluence + " will decrease by " + currentItemCard.InfluenceThreatBy);
            threats[currentItemCard.ThreatToInfluence] = Math.Max(0, threats[currentItemCard.ThreatToInfluence] + currentItemCard.InfluenceThreatBy);
        }
    }

    private void ResetDisplay() {
        menuGroup.SetActive(false);
        animationGroup.SetActive(false);

        textsGroup.SetActive(false);
        badGuyTurnGroup.SetActive(false);
        goodGuyTurnGroup.SetActive(false);

        scanGroup.SetActive(false);
        cardsScanCanvas.SetActive(false);
        ARThreatsCanvas.SetActive(false);

        endScreenGroup.SetActive(false);
    }

    private void ManageStatusAction() {

        ResetDisplay();

        switch (_gameStatus)
        {
            case State.Menu:
                menuGroup.SetActive(true);
                break;
            case State.Intro:
                animationGroup.SetActive(true);
                loadIntro();
                AnimationManager.Instance.showNext();
                break;
            case State.NumTour:
                AnimationManager.Instance.clear();
                textsGroup.SetActive(true);
                bigTextToChange.GetComponent<Text>().text = "Tour "+nbTurn;
                smallTextToChange.GetComponent<Text>().text = "Plus que " + (nbTurnsMax - nbTurn) + " tours et les aventuriers s'échappent !";
                break;
            case State.Draw:
                textsGroup.SetActive(true);
                bigTextToChange.GetComponent<Text>().text = "Pioche";
                smallTextToChange.GetComponent<Text>().text = "- Le Colonel pioche 2 cartes scénario et 2 cartes objet\n- Les aventuriers piochent 2 cartes par personnage";
                break;
            case State.BadGuyPlaying:
                badGuyTurnGroup.SetActive(true);
                break;
            case State.PlayersPlaying:
                goodGuyTurnGroup.SetActive(true);
                break;
            case State.Scan:
                scanGroup.SetActive(true);

                cardsScanCanvas.SetActive(true);
                scanOnlyThreats = false;
                break;
            case State.Animation:
                animationGroup.SetActive(true);
                ComputeFate();
                AnimationManager.Instance.clear();
                loadAnimation();
                AnimationManager.Instance.showNext();
                break;
            case State.ShowThreatsAR:
                AnimationManager.Instance.clear();
                ARThreatsCanvas.SetActive(true);
                scanOnlyThreats = true;
                scanGroup.SetActive(true);
                break;
            case State.End:
                
                if (!isFinish())
                {
                    nbTurn++;
                    _gameStatus = State.NumTour;
                    ManageStatusAction();
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

    private void loadIntro()
    {
        for (int i = 1; i < 10; i++)
        {
            AnimationManager.Instance.addIntroAnimationToList(i);
        }
    }

    private void loadAnimation()
    {
        //Senario 
        try
        {
            for (int i = 1; i < 5; i++)
            {
                AnimationManager.Instance.addScenarAnimationToList(currentScenarioCard.GetComponent<ScenarioCard>().ScenarioId, i);
            }
        }
        catch (EntryPointNotFoundException) { }
        
        //Item
        if (currentItemCard != null)
        {
            AnimationManager.Instance.addTrapAnimationToList(currentItemCard.GetComponent<ItemCard>().ItemId, currentItemCard.GetComponent<ItemCard>()._message);
        }

        //Action
        foreach (ActionCard c in currentActionCards){
            AnimationManager.Instance.addActionAnimationToList(c.GetComponent<ActionCard>().CharacterId, c.GetComponent<ActionCard>().ActionId , 0, c.GetComponent<ActionCard>()._message);
        }

        //Result of scenario
        if (currentScenarioSucess <= sucessValue)
        {
            AnimationManager.Instance.addScenarFinalAnimationToList(currentScenarioCard.GetComponent<ScenarioCard>().ScenarioId, "ST", currentScenarioCard.GetComponent<ScenarioCard>()._message);
        }
        else if (currentScenarioSucess <= neutralValue)
        { // normal sucess
            AnimationManager.Instance.addScenarFinalAnimationToList(currentScenarioCard.GetComponent<ScenarioCard>().ScenarioId, "S", currentScenarioCard.GetComponent<ScenarioCard>()._message);
        }
        else if (currentScenarioSucess <= failureValue)
        { // normal failure
            AnimationManager.Instance.addScenarFinalAnimationToList(currentScenarioCard.GetComponent<ScenarioCard>().ScenarioId, "E", currentScenarioCard.GetComponent<ScenarioCard>()._message);
        }
        else
        { // critical failure
            AnimationManager.Instance.addScenarFinalAnimationToList(currentScenarioCard.GetComponent<ScenarioCard>().ScenarioId, "ET", currentScenarioCard.GetComponent<ScenarioCard>()._message);
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
        if (nbTurn >= nbTurnsMax)
            return true;
        return false;
    }
    #endregion //PRIVATE_METHOD
}


