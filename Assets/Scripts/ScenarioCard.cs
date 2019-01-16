using UnityEngine;

// class that represents the data describing an ActionCard.
// this is a buffer class used to load the card data since JsonUtility cannot deserialise class inherited from MonoBehavior
public class ScenarioCardStats
{
    // members need to be public to be deserialized by JsonUtility
    public string cardName;
    public int scenarioId;
}

public class ScenarioCard : Card {

    protected int _cardName;
    protected int _scenarioId;

    // initialize the card members according to the parsed json cardData and points to the cardScanner in the scene
    public void Initialize(CardsScanner cardsScanner, ScenarioCardStats cardStats)
    {
        _cardsScanner = cardsScanner;
        _scenarioId = cardStats.scenarioId;
        Debug.Log("Initializing scenario card ");
        Print();
    }


    public void Print()
    {
        Debug.Log("ScenarioCard : " + _cardName + " scenario " + _scenarioId);
    }
}
