using UnityEngine;

// class that represents the data describing an ActionCard.
// this is a buffer class used to load the card data since JsonUtility cannot deserialise class inherited from MinoBehavior
public class ActionCardData
{
    // members need to be public to be deserialized by JsonUtility
    public int id;
}


public class ActionCard : Card {

    protected int _id;

    // initialize the card members according to the parsed json cardData and points to the cardScanner in the scene
    public void Initialize(CardsScanner cardsScanner/*, ActionCardData cardData*/)
    {
        _cardsScanner = cardsScanner;
        //_id = cardData.id;
    }


    public void Print()
    {
        Debug.Log("ActionCard : " + _id);
    }
}
