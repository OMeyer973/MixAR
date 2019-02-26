using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {
    public GameManager GM; 
    private float beginTime = 0.0f;
    private float timeToWait;
    private bool isFinished = false;

    #region STATIC_METHODS

    // Use this for initialization
    public void launch(float timeToWait)
    {
        beginTime = Time.time;
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
        Debug.Log(timeLeft());
        if (beginTime != 0.0f && !isFinished && timeLeft() <= 0)
        {
            stop();
        }
    }

    public void stop()
    {
        isFinished = true;
        beginTime = 0;
        GM.nextState();
    }
    #endregion
}

public class TimerFinished : System.Exception
{

}