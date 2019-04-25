using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {
    public GameManager GM;
    public Text timerText;
    private float beginTime = 0.0f;
    private float timeToWait;
    private bool isFinished = false;

    #region STATIC_METHODS

    // Use this for initialization
    public void launch(float timeToWait)
    {
        beginTime = Time.time;
        isFinished = false;
        this.timeToWait = timeToWait;
    }

    #endregion
    
    #region PUBLIC_METHODS

    public float timeLeft()
    {
        return timeToWait - timeElapsed();
    }

    public float timeElapsed()
    {
        return Time.time - beginTime;
    }

    // Update is called once per frame
    void Update () {
        timerText.text = ((int) timeLeft()) + "s";
        if (timeLeft() <= 6.0f)
        {
            timerText.color = Color.red;
            timerText.fontSize = 18;
            timerText.fontStyle = FontStyle.Bold;
        }
        if (beginTime != 0.0f && !isFinished && timeLeft() <= 0.0f)
        {
            stop();
            GM.nextState();
        }
    }

    public void stop()
    {
        isFinished = true;
        beginTime = 0;
    }
    #endregion
}