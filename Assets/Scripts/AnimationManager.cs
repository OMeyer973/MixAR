using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationManager : Singleton<AnimationManager>
{

    private List<GameObject> _bdElemList = new List<GameObject>();
    private int nbShowedAnimation = 0;

    //Used to print text hover animation
    public GameObject hoverAnimationTextGameObject;
    public Text TextAnimationText;


    public void showLast()
    {
        if (_bdElemList[nbShowedAnimation - 1].GetComponent<AnimationBox>().hasBeenShowed == true)
        {
            _bdElemList[nbShowedAnimation - 1].GetComponent<AnimationBox>().hideText();
        }
        else
        {
            if (nbShowedAnimation > 1)
            {
            
                _bdElemList[nbShowedAnimation - 1].GetComponent<AnimationBox>().setInvisible();
                _bdElemList[nbShowedAnimation - 2].GetComponent<AnimationBox>().setVisible();
                _bdElemList[nbShowedAnimation - 2].GetComponent<AnimationBox>().showText();
                nbShowedAnimation--;
            }
        }
    }

    public void showNext()
    {
        if (nbShowedAnimation > 0 && _bdElemList[nbShowedAnimation - 1].GetComponent<AnimationBox>().text != "" && _bdElemList[nbShowedAnimation - 1].GetComponent<AnimationBox>().hasBeenShowed == false)
        {
            _bdElemList[nbShowedAnimation - 1].GetComponent<AnimationBox>().showText();
            hoverAnimationTextGameObject.SetActive(true);
        }
        else
        {
            if (nbShowedAnimation < _bdElemList.Count) {
                if (nbShowedAnimation != 0)
                {
                    _bdElemList[nbShowedAnimation - 1].GetComponent<AnimationBox>().setInvisible();
                    _bdElemList[nbShowedAnimation - 1].GetComponent<AnimationBox>().hasBeenShowed = false;
                }
                _bdElemList[nbShowedAnimation].GetComponent<AnimationBox>().setVisible();
            }
            else
            {
                GameManager.Instance.nextState();
            }
            hoverAnimationTextGameObject.SetActive(false);
            nbShowedAnimation++;
        }
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

    public void addActionAnimationToList(int charNumber, int actionId, int sucessId, string textAssociated = "")
    {
        Vector3 pos = new Vector3(0, 0, 0);
        GameObject go = new GameObject("ActionAnimation");
        AnimationBox box = go.AddComponent<AnimationBox>();
        box.initAction(charNumber, actionId, sucessId, pos, textAssociated);
        _bdElemList.Add(go);
    }

    public void addTrapAnimationToList(int trapId, string textAssociated = "")
    {
        Vector3 pos = new Vector3(0, 0, 0);
        GameObject go = new GameObject("TrapAnimation");
        AnimationBox box = go.AddComponent<AnimationBox>();
        box.initTrap(trapId, pos, textAssociated);
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
