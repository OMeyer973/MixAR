using UnityEngine;

// represents a card. can be tracked by the AR camera and have handy methods to be used
public class Card : DefaultTrackableEventHandler
{
    // todo : have this parameter as protected
    public CardsScanner _cardsScanner;

    protected bool _isOnScreen = false;

    // is the card displayed on the phone screen ?
    public bool IsOnScreen()
    {
        return _isOnScreen;
    }

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
        _isOnScreen = true;
        _cardsScanner.AddCardToTrack(this);
    }

    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();
        _isOnScreen = false;
        _cardsScanner.UpdateCardsList(this);
    }

}
