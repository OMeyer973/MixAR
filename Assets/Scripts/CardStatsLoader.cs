using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
// json deserializer used : https://github.com/SaladLab/Json.Net.Unity3D
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



// class that will load the cards statistics from json and instantiate them
public class CardStatsLoader : MonoBehaviour {

    #region PROTECTED_MEMBERS

    protected string _actionCardsDataFileName = "ActionCardsData.json";
    protected string _trapCardsDataFileName = "TrapCardsData.json";
    protected string _scenarioCardsDataFileName = "ScenarioCardsData.json";

    protected ActionCardStats[] _actionCardStats = null;
    protected ScenarioCardStats[] _scenarioCardStats = null;
    protected TrapCardStats[] _trapCardStats = null;

    #endregion // PROTECTED_MEMBERS


    #region PUBLIC_METHODS



    // return the ActionStats corresponding to the given cardName
    public ActionCardStats GetActionStats(string cardName)
    {
        if (_actionCardStats == null)
        {
            Debug.Log("Loading action cards data from json");
            LoadActionCardsData();
        }
        if (_actionCardStats == null)
            Debug.LogError("couldn't load LoadActionCardsData() properly");

        foreach (ActionCardStats a in _actionCardStats)
        {
            if (a.cardName == cardName)
                return a;
        }

        Debug.LogError("can't find card " + cardName + " in action stats - check the json file");
        return null;
    }
    
    // return the ScenarioStats corresponding to the given cardName
    public ScenarioCardStats GetScenarioStats(string cardName)
    {
        if (_scenarioCardStats == null)
        {
            Debug.Log("Loading scenario cards data from json");
            LoadScenarioCardsData();
        }
        if (_scenarioCardStats == null)
            Debug.LogError("couldn't load LoadScenarioCardsData() properly");

        foreach (ScenarioCardStats a in _scenarioCardStats)
        {
            if (a.cardName == cardName)
                return a;
        }

        Debug.LogError("can't find card " + cardName + " in action stats - check the json file");
        return null;
    }

    // return the TrapStats corresponding to the given cardName
    public TrapCardStats GetTrapStats(string cardName)
    {
        if (_trapCardStats == null)
        {
            Debug.Log("Loading trap cards data from json");
            LoadTrapCardsData();
        }
        if (_trapCardStats == null)
            Debug.LogError("couldn't load LoadTrapCardsData() properly");

        foreach (TrapCardStats a in _trapCardStats)
        {
            if (a.cardName == cardName)
                return a;
        }

        Debug.LogError("can't find card " + cardName + " in action stats - check the json file");
        return null;
    }
    
    #endregion // PUBLIC_METHODS


    #region PROTECTED_METHODS

    protected void LoadActionCardsData()
    {
        string actionCardsFile = ReadFile(_actionCardsDataFileName);
        // Debug.Log(actionCardsFile);

        _actionCardStats = JsonConvert.DeserializeObject<ActionCardStats[]>(actionCardsFile);
    }


    protected void LoadScenarioCardsData()
    {
        string scenarioCardsFile = ReadFile(_scenarioCardsDataFileName);
        // Debug.Log(scenarioCardsFile);

        _scenarioCardStats = JsonConvert.DeserializeObject<ScenarioCardStats[]>(scenarioCardsFile);
    }


    protected void LoadTrapCardsData()
    {
        string trapCardsFile = ReadFile(_trapCardsDataFileName);
        // Debug.Log(trapCardsFile);

        _trapCardStats = JsonConvert.DeserializeObject<TrapCardStats[]>(trapCardsFile);
    }


    // reads a file in  the streamingAssets folder and returns it's content
    protected string ReadFile(string filepath)
    {
        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine(Application.streamingAssetsPath, filepath);
        string dataAsJson = "";
        if (File.Exists(filePath))
        {
            // Read the json from the file into a string
            dataAsJson = File.ReadAllText(filePath);
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
        return dataAsJson;
    }

    #endregion // PROTECTED_METHODS

}

