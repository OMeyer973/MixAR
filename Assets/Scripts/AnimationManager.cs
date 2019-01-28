using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : Singleton<AnimationManager>
{

    private List<AnimationBox> _bdElemList = new List<AnimationBox>();
    private int nbShowedAnimation = 0;

    public void showLast()
    {
        if (nbShowedAnimation > 1)
        {
            Debug.Log(nbShowedAnimation);
            _bdElemList[nbShowedAnimation-1].setInvisible();
            _bdElemList[nbShowedAnimation-2].setVisible();
            nbShowedAnimation--;
        }
    }

    public void showNext()
    {
        if (nbShowedAnimation < _bdElemList.Count) {
            if(nbShowedAnimation != 0)
                _bdElemList[nbShowedAnimation-1].setInvisible();
            _bdElemList[nbShowedAnimation].setVisible();
        }
        else
        {
            GameManager.Instance.nextState();
        }
        nbShowedAnimation++;
    }

    public void click()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.position.x < Screen.width / 2)
            {
                showLast();
            }
            else if (touch.position.x > Screen.width / 2)
            {
                showNext();
            }
        }
        else
        {
            var mousePosition= Input.mousePosition;
            if (mousePosition.x < Screen.width / 2)
            {
                showLast();
            }
            else if (mousePosition.x > Screen.width / 2)
            {
                showNext();
            }
        }

        
        
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
