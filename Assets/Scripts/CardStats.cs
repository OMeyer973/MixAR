using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
// json deserializer used : https://github.com/SaladLab/Json.Net.Unity3D
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



// class that will load the cards statistics from json and instantiate them
public class CardStats : MonoBehaviour {

    #region PROTECTED_MEMBERS

    protected string _ActionCardsDataFileName = "ActionCardsData.json";
    protected string _TrapCardsDataFileName = "TrapCardsData.json";
    protected string _ScenarioCardsDataFileName = "ScenarioCardsData.json";

    protected ActionCardStats[] _actionCardStats = null;
    protected ScenarioCardStats[] _scenarioCardStats = null;
    protected TrapCardStats[] _trapCardStats = null;
    
    #endregion // PROTECTED_MEMBERS


    #region PUBLIC_METHODS

    // Use this for initialization
    void Awake ()
    {
        LoadActionCardsData();
        // LoadTrapCardsData();
        // LoadScenarioCardsData();
    }

    // return the ActionStats corresponding to the given cardName
    public ActionCardStats ActionStat(string cardName)
    {
        if (_actionCardStats == null)
        {
            Debug.LogError("trying to get action card stat before it has been loaded correctly");
            return null;
        }

        foreach (ActionCardStats a in _actionCardStats)
        {
            if (a.cardName == cardName)
                return a;
        }

        Debug.LogError("can't find card name in action stats - check the json file");
        return null;
    }

    #endregion // PUBLIC_METHODS


    #region PROTECTED_METHODS

    protected void LoadActionCardsData()
    {
        string actionCardsFile = ReadFile(_ActionCardsDataFileName);
        Debug.Log(actionCardsFile);

        ActionCardStats[] _actionCardStats = JsonConvert.DeserializeObject<ActionCardStats[]>(actionCardsFile);
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

