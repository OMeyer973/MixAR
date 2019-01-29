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
        //Debug.Log(GameManager.Instance.onlyThreats);
        //Debug.Log(GameManager.Instance.onlyThreats == false || (GameManager.Instance.onlyThreats == true && this.GetType() == typeof(MainTarget)));
        if (GameManager.Instance.onlyThreats == false || (GameManager.Instance.onlyThreats == true && this.GetType() == typeof(MainTarget)))
        {
            //base.OnTrackingFound(); // does stuff we don't want so we override it entirely
            foreach (Transform child in transform)
            {
                //Debug.Log(child.gameObject.name);
                child.gameObject.SetActive(true);
            }
            _isOnScreen = true;
            _cardsScanner.AddCardToTrack(this);
        }
    }

    protected override void OnTrackingLost()
    {
        //base.OnTrackingLost(); // does stuff we don't want so we override it entirely
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        _isOnScreen = false;
        _cardsScanner.UpdateCardsList(this);
    }

}
