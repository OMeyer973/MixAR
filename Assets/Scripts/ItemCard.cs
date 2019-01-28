using UnityEngine;

// class that represents the data describing an ActionCard.
// this is a buffer class used to load the card data since JsonUtility cannot deserialise class inherited from MonoBehavior
public class ItemCardStats
{
    // members need to be public to be deserialized by JsonUtility
    public string cardName;
    public int itemId;
    public string message;

	public bool influenceCharacter;
	public int characterToInfluence;
    public float influenceCharacterBy;

    public bool influenceThreat;
	public int threatToInfluence;
    public int influenceThreatBy;
}

public class ItemCard : Card {

    public string CardName { get; private set; }
    public int ItemId { get; private set; }
    public string Message { get; private set; }

    public bool InfluenceCharacter { get; private set; }
    public int CharacterToInfluence { get; private set; }
    public float InfluenceCharacterBy { get; private set; }

    public bool InfluenceThreat { get; private set; }
    public int ThreatToInfluence { get; private set; }
    public int InfluenceThreatBy { get; private set; }

    // initialize the card members according to the parsed json cardData and points to the cardScanner in the scene
    public void Initialize(CardsScanner cardsScanner, ItemCardStats cardStats)
    {
        Debug.Log("Initializing item card ");

        _cardsScanner = cardsScanner;
        CardName = cardStats.cardName;
        ItemId = cardStats.itemId;
        Message = cardStats.message;

        InfluenceCharacter = cardStats.influenceCharacter;
        CharacterToInfluence = cardStats.characterToInfluence;
        InfluenceCharacterBy = cardStats.influenceCharacterBy;

        InfluenceThreat = cardStats.influenceThreat;
        ThreatToInfluence = cardStats.threatToInfluence;
        InfluenceThreatBy = cardStats.influenceThreatBy;

        Print();
    }


    public void Print()
    {
        Debug.Log("ItemCard : " + CardName + " id " + ItemId + " - " + Message + "\n" +
            (InfluenceCharacter ? (" will influence character " + CharacterToInfluence) : ("")) +
            (InfluenceThreat ? (" will influence threat " + ThreatToInfluence) : (""))
            );
    }
}
