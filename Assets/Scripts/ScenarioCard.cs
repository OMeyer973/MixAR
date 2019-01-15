using UnityEngine;

public class ScenarioCard : Card {

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
