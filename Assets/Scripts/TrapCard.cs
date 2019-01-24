using UnityEngine;

// class that represents the data describing an ActionCard.
// this is a buffer class used to load the card data since JsonUtility cannot deserialise class inherited from MonoBehavior
public class TrapCardStats
{
    // members need to be public to be deserialized by JsonUtility
    public string cardName;
    public int trapId;
    public string message;
	public bool influenceCharacter;
	public int characterToInfluence;
	public bool influenceThreat;
	public int threatToInfluence;
}

public class TrapCard : Card {

    public string CardName { get; private set; }
    public int TrapId { get; private set; }
    public string Message { get; private set; }
    public bool InfluenceCharacter { get; private set; }
    public int CharacterToInfluence { get; private set; }
    public bool InfluenceThreat { get; private set; }
    public int ThreatToInfluence { get; private set; }

    // initialize the card members according to the parsed json cardData and points to the cardScanner in the scene
    public void Initialize(CardsScanner cardsScanner, TrapCardStats cardStats)
    {
        Debug.Log("Initializing trap card ");

        _cardsScanner = cardsScanner;
        CardName = cardStats.cardName;
        TrapId = cardStats.trapId;
        Message = cardStats.message;
        InfluenceCharacter = cardStats.influenceCharacter;
        CharacterToInfluence = cardStats.characterToInfluence;
        InfluenceThreat = cardStats.influenceThreat;
        ThreatToInfluence = cardStats.threatToInfluence;

        Print();
    }


    public void Print()
    {
        Debug.Log("TrapCard : " + CardName + " id " + TrapId + " - " + Message + "\n" +
            (InfluenceCharacter ? (" will influence character " + CharacterToInfluence) : ("")) +
            (InfluenceThreat ? (" will influence threat " + ThreatToInfluence) : (""))
            );
    }
}
