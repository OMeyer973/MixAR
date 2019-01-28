using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : Singleton<AnimationManager>
{

    private List<AnimationBox> _bdElemList = new List<AnimationBox>();
    private int nbShowedAnimation = 0;

    public void showNext()
    {
        Debug.Log("TEST");
        if (nbShowedAnimation < _bdElemList.Count) { 
            _bdElemList[nbShowedAnimation].setVisible();
        }
        else
        {
            GameManager.Instance.nextState();
        }
        nbShowedAnimation++;
    }

    public void addAnimationToList(GameObject prefab)
    {
        Vector3 pos = new Vector3(0, 0, 0);
        _bdElemList.Add(new AnimationBox(prefab, pos));
    }

    public void addAnimationToList(int charNumber, int actionId, int sucessId)
    {
        Vector3 pos = new Vector3(0, 0, 0);
        _bdElemList.Add(new AnimationBox(charNumber, actionId, sucessId, pos));
    }

    public void clear()
    {
        foreach (AnimationBox box in _bdElemList)
            box.clear();
        _bdElemList.Clear();
        nbShowedAnimation = 0;
    }

}
