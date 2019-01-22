using UnityEngine;

// represents the main target where the AR variables will be displayed
public class MainTarget : DefaultTrackableEventHandler
{
    protected override void OnTrackingFound()
    {
        //base.OnTrackingFound(); // does stuff we don't want so we override it entirely
        foreach (Transform child in transform)
        {
            Debug.Log(child.gameObject.name);
            child.gameObject.SetActive(true);
        }
    }

    protected override void OnTrackingLost()
    {
        //base.OnTrackingLost(); // does stuff we don't want so we override it entirely
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

}
