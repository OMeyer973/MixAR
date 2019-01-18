using UnityEngine;

// class that represents the data describing an ActionCard.
// this is a buffer class used to load the card data since JsonUtility cannot deserialise class inherited from MonoBehavior
public class TrapCardStats
{
    // members need to be public to be deserialized by JsonUtility
    public string cardName;
    public int trapId;
}

public class TrapCard : Card {

    public int CardName { get; private set; }
    public int TrapId { get; private set; }

    // initialize the card members according to the parsed json cardData and points to the cardScanner in the scene
    public void Initialize(CardsScanner cardsScanner, TrapCardStats cardStats)
    {
        _cardsScanner = cardsScanner;
        TrapId = cardStats.trapId;
        Debug.Log("Initializing trap card ");
        Print();
    }


    public void Print()
    {
        Debug.Log("TrapCard : " + CardName + " trap id " + TrapId);
    }
}
