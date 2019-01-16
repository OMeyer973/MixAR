using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {
    public GameObject _canvas;

    public void setActive()
    {
        _canvas.SetActive(true);
    }

    public void setInactive()
    {
        _canvas.SetActive(false);
    }
}
