using System.Collections.Generic;
using UnityEngine;


public class CardsScanner : MonoBehaviour
{
    #region PROTECTED_MEMBER_VARIABLES

    public List<Card> _trackedCards;

    #endregion // PROTECTED_MEMBER_VARIABLES
    
    protected void CheckCards()
    {
        if (_trackedCards.Count >= 5) // 5 : numbeer of cards that need to be scanned at once
        {
            Debug.Log("5 cards on screen !");

            int nbTrapCard = 0, nbScenarioCard = 0, nbActionCard = 0;

            foreach (Card c in _trackedCards)
            {
                if (c is TrapCard)
                    nbTrapCard ++;
                else if (c is ScenarioCard)
                    nbScenarioCard ++;
                if (c is ActionCard)
                    nbActionCard ++;
            }

            if (nbTrapCard == 1 && nbScenarioCard == 1 && nbActionCard == 3)
            {
                Debug.Log("Cards are good !");
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

    public void AddCardToTrack(Card card)
    {
        _trackedCards.Add(card);
        CheckCards();
    }

    public void RemoveCardToTrack(Card card)
    {
        _trackedCards.Remove(card);
    }

    #endregion // PUBLIC_METHODS
}
