using UnityEngine;

public class CardTracker : DefaultTrackableEventHandler
{
    public CardsScanner masterScanner;

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
        masterScanner.AddCardToTrack(this);
    }


    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();
        masterScanner.RemoveCardToTrack(this);
    }

}
