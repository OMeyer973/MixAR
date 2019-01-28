using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : Singleton<AnimationManager>
{

    private List<GameObject> _bdElemList = new List<GameObject>();
    private int nbShowedAnimation = 0;

    public void showLast()
    {
        if (nbShowedAnimation > 1)
        {
            _bdElemList[nbShowedAnimation-1].GetComponent<AnimationBox>().setInvisible();
            _bdElemList[nbShowedAnimation-2].GetComponent<AnimationBox>().setVisible();
            nbShowedAnimation--;
        }
    }

    public void showNext()
    {
        if (nbShowedAnimation < _bdElemList.Count) {
            if(nbShowedAnimation != 0)
                _bdElemList[nbShowedAnimation-1].GetComponent<AnimationBox>().setInvisible();
            _bdElemList[nbShowedAnimation].GetComponent<AnimationBox>().setVisible();
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
        GameObject go = new GameObject("AnimationBox");
        AnimationBox box = go.AddComponent<AnimationBox>();
        box.init(prefab, pos);
        _bdElemList.Add(go);
    }

    public void addAnimationToList(int charNumber, int actionId, int sucessId)
    {
        Vector3 pos = new Vector3(0, 0, 0);
        GameObject go = new GameObject("AnimationBox");
        AnimationBox box = go.AddComponent<AnimationBox>();
        box.init(charNumber, actionId, sucessId, pos);
        _bdElemList.Add(go);
    }

    public void clear()
    {
        foreach (GameObject box in _bdElemList)
        {
            box.GetComponent<AnimationBox>().clear();
            Destroy(box);
        }
            
        _bdElemList.Clear();
        nbShowedAnimation = 0;
    }

}
