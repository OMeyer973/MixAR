using UnityEngine;

// class that represents the data describing an ActionCard.
// this is a buffer class used to load the card data since JsonUtility cannot deserialise class inherited from MonoBehavior
public class ActionCardStats
{
    // members need to be public to be deserialized by JsonUtility
    public string cardName;
    public int CharacterId;
    public int actionId;
    public bool changeThreat;
    public int threatToChange;
}


public class ActionCard : Card {

    public string CardName { get; private set; }
    public int ActionId { get; private set; }
    public int CharacterId { get; private set; }
    public bool ChangeThreat { get; private set; }
    public int ThreatToChange { get; private set; }

    // initialize the card members according to the parsed json cardData and points to the cardScanner in the scene
    public void Initialize(CardsScanner cardsScanner, ActionCardStats cardStats)
    {
        //Debug.Log("Initializing action card ");

        _cardsScanner = cardsScanner;
        CardName = cardStats.cardName;
        ActionId = cardStats.actionId;
        CharacterId = cardStats.CharacterId;
        ChangeThreat = cardStats.changeThreat;
        ThreatToChange = cardStats.threatToChange;

        //Print();
    }


    public void Print()
    {
        Debug.Log("ActionCard : " + CardName + " - action : " + ActionId + " by player " + CharacterId);
    }
}
