using System.Collections.Generic;
using UnityEngine;


public class CardsScanner : MonoBehaviour
{
    #region PROTECTED_MEMBER_VARIABLES

    public List<CardTracker> _trackedCards;

    #endregion // PROTECTED_MEMBER_VARIABLES
    
    protected void CheckCards()
    {

        Debug.Log(_trackedCards.Count + " cards on screen !");
        if (_trackedCards.Count >= 5) // 5 : numbeer of cards that need to be scanned at once
        {
            Debug.Log("5 cards on screen !");
        }
    }

    #region PROTECTED_METHODS

    #endregion // PROTECTED_METHODS


    #region PUBLIC_METHODS

    public void AddCardToTrack(CardTracker card)
    {
        _trackedCards.Add(card);
        CheckCards();
    }

    public void RemoveCardToTrack(CardTracker card)
    {
        _trackedCards.Remove(card);
    }

    #endregion // PUBLIC_METHODS
}
