using UnityEngine;

// class that represents the data describing an ActionCard.
// this is a buffer class used to load the card data since JsonUtility cannot deserialise class inherited from MonoBehavior
public class ActionCardStats
{
    // members need to be public to be deserialized by JsonUtility
    public string cardName;
    public int playerId;
    public int actionId;
}


public class ActionCard : Card {

    public int _cardName;
    public int _actionId;
    public int _playerId;

    // initialize the card members according to the parsed json cardData and points to the cardScanner in the scene
    public void Initialize(CardsScanner cardsScanner, ActionCardStats cardStats)
    {
        _cardsScanner = cardsScanner;
        _actionId = cardStats.actionId;
        _playerId = cardStats.playerId;
        Debug.Log("Initializing action card ");
        Print();
    }


    public void Print()
    {
        Debug.Log("ActionCard : " + _cardName + " - action : " + _actionId + " by player " + _playerId);
    }
}
