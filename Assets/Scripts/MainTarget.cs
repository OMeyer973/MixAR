using UnityEngine;

// represents the main target where the AR variables will be displayed
public class MainTarget : DefaultTrackableEventHandler
{
    // public GameManager _gameManager; // decomment in case we can't make GameManager a Singleton

    /* // in case we can't make GameManager a Siogleton
    public void Initialize(GameManager gameManager)
    {
        _gameManager = gameManager;
        Debug.Log("Initializing main target ");
    }
    */

    protected override void OnTrackingFound()
    {
        //base.OnTrackingFound(); // does stuff we don't want so we override it entirely
        Transform child = transform.GetChild(0);
        Debug.Log(child.gameObject.name);
        child.gameObject.SetActive(true);
        Debug.Log("coucou " + GameManager.Instance.gameVariables[0] + GameManager.Instance.gameVariables[1] + GameManager.Instance.gameVariables[2]);

        for (int i=0; i< GameManager.nbGameVariables; i++)
        {
            for (int j=0; j< GameManager.nbGameVariablesStates; j++)
            {
                Debug.Log(child.gameObject.name + "/var" + i + "_state" + j);
                Transform varState = transform.Find(child.gameObject.name + "/var" + i + "_state" + j);

                if (GameManager.Instance.gameVariables[i] == j)
                {
                    varState.gameObject.SetActive(true);
                }
                else
                {
                    varState.gameObject.SetActive(false);
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
