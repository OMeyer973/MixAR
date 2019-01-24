using System.Collections.Generic;
using UnityEngine;
using Vuforia;

// Cards scanner : keeps track of the cards on screen and check if they are all of the correct type to proceed to the animation step.
// note : the scanner keeps in memory the cards even if they left the screen but the cards list is emptied if all the cards are off-screen
public class CardsScanner : MonoBehaviour
{
    #region PUBLIC_MEMBER_VARIABLES

    public GameObject successCanvas;
    public GameObject errorCanvas;
    public bool onlyAR = false;
    #endregion // PUBLIC_MEMBER_VARIABLES

    #region PROTECTED_MEMBER_VARIABLES

    protected List<Card> _trackedCards = new List<Card>();
    protected List<Card> _cardsToSend = new List<Card>();

    #endregion // PROTECTED_MEMBER_VARIABLES

    #region PROTECTED_METHODS
    // check if the cards in the list are good to proceed (1 scenario, 1 trap & 3 actions)
    protected void CheckCards()
    {
        if (_trackedCards.Count >= 5) // 5 : number of cards that need to be scanned at once
        {
            // TODO : replace nbActionCardCharacter0-1-2 with a list of size GameManager.nbCharacters
            int nbTrapCard = 0, nbScenarioCard = 0;
            int[] nbCharacterCards = new int[GameManager.nbCharacters];

            // count the number of action, scenario & trap cards in the list
            foreach (Card c in _trackedCards)
            {
                if (c is TrapCard)
                    nbTrapCard++;
                else if (c is ScenarioCard)
                    nbScenarioCard++;
                if (c is ActionCard)
                {
                    ActionCard tmpCard = (ActionCard)c;
                    nbCharacterCards[tmpCard.CharacterId] += 1;
                }
            }

            // if the configuration is good, proceed
            bool cardsAreGood = (nbTrapCard == 1) && (nbScenarioCard == 1);

            for (int i = 0; i < GameManager.nbCharacters; i++)
            {
                cardsAreGood = cardsAreGood && (nbCharacterCards[i] == 1);
            }

            if (cardsAreGood)
            {
                Debug.Log("Cards are good !");
                ShowSuccessCanvas();
            }
            else
            {
                // todo : have a screen pop up to say "hey, scan good cards !"
                Debug.Log("please scan 3 action cards (1 per character), 1 scenario card and 1 Trap card");
                ShowErrorCanvas();
            }
        }
    }

    private void ShowErrorCanvas()
    {
        HideCanvas();
        errorCanvas.SetActive(true);
    }

    private void ShowSuccessCanvas()
    {
        HideCanvas();
        successCanvas.SetActive(true);
        _cardsToSend.Clear();
        _cardsToSend.AddRange(_trackedCards);
    }

    #endregion // PROTECTED_METHODS

    #region PUBLIC_METHODS

    public void HideCanvas()
    {
        errorCanvas.SetActive(false);
        successCanvas.SetActive(false);
    }

    // called by a press on the validate button of the success canvas
    public void ValidateCardsScan()
    {
        // Debug.Log("CardsScanner sending scanned cards to GameManager");
        GameManager.Instance.SetCardsForNextTurn(_cardsToSend);
        _cardsToSend.Clear();
        HideCanvas();
        GameManager.Instance.PlayTurn();
    }

    // add a new card to track. if the list has less than 5 cards, just add a new one. 
    // else if a card in the list is off screen, pop it to add the new one.
    public void AddCardToTrack(Card card)
    {
        if( onlyAR == false || (onlyAR == true && card.GetType() == typeof(MainTarget)))
        {
            // if the card is allready in the list : do nothing
            foreach (Card c in _trackedCards)
            {
                if (c == card)
                    return;
            }
        
            // if the list has less than 5 items, just add it
            if (_trackedCards.Count < 5)
            {
                _trackedCards.Add(card);
            }
            // else if one card in the list is off screen, pop it and add the new card
            else for (int i = _trackedCards.Count - 1; i >= 0; i--)
            {
                if (!_trackedCards[i].IsOnScreen())
                {
                    _trackedCards.RemoveAt(i);
                    _trackedCards.Add(card);
                }
            }
            // else do nothing
            CheckCards();

        }
    }

    // updates the card list and empties it if all the cards are off-screen
    public void UpdateCardsList(Card ca)
    {

        //_trackedCards.Remove(ca);
        int cardsOnScreen = 0;
        foreach (Card c in _trackedCards)
        {
            if(c.IsOnScreen())
                cardsOnScreen++;
        }

        // if no cards are on screen : clear the list 
        if (cardsOnScreen <= 0)
        {
            _trackedCards.Clear();
        }
    }

    #endregion // PUBLIC_METHODS
}
