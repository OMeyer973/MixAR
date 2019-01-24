using UnityEngine;

// represents the main target where the AR variables will be displayed
public class MainTarget : DefaultTrackableEventHandler
{
    protected override void OnTrackingFound()
    {
        //base.OnTrackingFound(); // does stuff we don't want so we override it entirely
        Transform child = transform.GetChild(0);
        child.gameObject.SetActive(true);
        
        for (int i=0; i< GameManager.nbThreats; i++)
        {
            for (int j=0; j< GameManager.nbThreatsStates; j++)
            {
                Transform threatState = transform.Find(child.gameObject.name + "/threat" + i + "_state" + j);

                if (GameManager.Instance.threats[i] == j)
                {
                    threatState.gameObject.SetActive(true);
                }
                else
                {
                    threatState.gameObject.SetActive(false);
                }
            }
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
