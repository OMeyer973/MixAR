﻿using UnityEngine;

// represents the main target where the AR variables will be displayed
public class MainTarget : DefaultTrackableEventHandler
{
    protected override void OnTrackingFound()
    {
        //base.OnTrackingFound(); // does stuff we don't want so we override it entirely
        Transform child = transform.GetChild(0);
        child.gameObject.SetActive(true);
        
        for (int i=0; i< GameManager.nbGameVariables; i++)
        {
            for (int j=0; j< GameManager.nbGameVariablesStates; j++)
            {
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
