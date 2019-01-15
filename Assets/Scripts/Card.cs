using UnityEngine;

// represents a card. can be tracked by the AR camera and have handy methods to be used
public class Card : DefaultTrackableEventHandler
{
    // todo : have this parameter as protected
    public CardsScanner _cardsScanner;

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
        _cardsScanner.AddCardToTrack(this);
    }


    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();
        _cardsScanner.RemoveCardToTrack(this);
    }

}
