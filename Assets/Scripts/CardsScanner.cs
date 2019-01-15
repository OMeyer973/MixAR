using System.Collections.Generic;
using UnityEngine;

// Cards scanner : keeps track of the cards on screen and check if they are all of the correct type to proceed to the animation step.
// note : the scanner keeps in memory the cards even if they left the screen but the cards list is emptied if all the cards are off-screen
public class CardsScanner : MonoBehaviour
{
    public GameObject validationObject;

    #region PROTECTED_MEMBER_VARIABLES

    public List<Card> _trackedCards;
    
    #endregion // PROTECTED_MEMBER_VARIABLES
    
    // check if the cards in the list are good to proceed (1 scenario, 1 trap & 3 actions)
    protected void CheckCards()
    {
        if (_trackedCards.Count >= 5) // 5 : numbeer of cards that need to be scanned at once
        {
            int nbTrapCard = 0, nbScenarioCard = 0, nbActionCard = 0;

            // count the number of action, scenario & trap cards in the list
            foreach (Card c in _trackedCards)
            {
                if (c is TrapCard)
                    nbTrapCard ++;
                else if (c is ScenarioCard)
                    nbScenarioCard ++;
                if (c is ActionCard)
                    nbActionCard ++;
            }
            
            // if the configuration is good, proceed
            if (nbTrapCard == 1 && nbScenarioCard == 1 && nbActionCard == 3)
            {
                Debug.Log("Cards are good !");
                validationObject.SetActive(true);
            }
            else
            {
                Debug.Log("please scan 3 action cards, 1 scenario card and 1 Trap card");
            }
        }
    }

    #region PROTECTED_METHODS

    #endregion // PROTECTED_METHODS


    #region PUBLIC_METHODS

    // add a new card to track. if the list has less than 5 cards, just add a new one. 
    // else if a card in the list is off screen, pop it to add the new one.
    public void AddCardToTrack(Card card)
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
        // else if one card in the lis is off screen, pop it and add the new card
        else foreach (Card c in _trackedCards)
        {
            if (!c.IsOnScreen())
            {
                _trackedCards.Remove(c);
                _trackedCards.Add(card);
            }
        }
        // else do nothing
        CheckCards();
    }

    // updates the card list and empties it if all the cards are off-screen
    public void UpdateCardsList()
    {
        int cardsOnScreen = 0;
        foreach (Card c in _trackedCards)
        {
            if(c.IsOnScreen())
                cardsOnScreen++;
        }
        if (cardsOnScreen <= 0)
            _trackedCards.Clear();
    }

    #endregion // PUBLIC_METHODS
}
