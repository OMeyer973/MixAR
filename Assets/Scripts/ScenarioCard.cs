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

    protected int CardName { get; private set; }
    protected int ScenarioId { get; private set; }

    // initialize the card members according to the parsed json cardData and points to the cardScanner in the scene
    public void Initialize(CardsScanner cardsScanner, ScenarioCardStats cardStats)
    {
        _cardsScanner = cardsScanner;
        ScenarioId = cardStats.scenarioId;
        Debug.Log("Initializing scenario card ");
        Print();
    }


    public void Print()
    {
        Debug.Log("ScenarioCard : " + CardName + " scenario " + ScenarioId);
    }
}
