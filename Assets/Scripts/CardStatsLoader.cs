using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
// json deserializer used : https://github.com/SaladLab/Json.Net.Unity3D
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;


// class that will load the cards statistics from json and instantiate them
public class CardStatsLoader : MonoBehaviour
{

    #region PROTECTED_MEMBERS

    protected string _actionCardsDataFileName = "ActionCardsData";
    protected string _itemCardsDataFileName = "ItemCardsData";
    protected string _scenarioCardsDataFileName = "ScenarioCardsData";

    protected ActionCardStats[] _actionCardStats = null;
    protected ScenarioCardStats[] _scenarioCardStats = null;
    protected ItemCardStats[] _itemCardStats = null;

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

        foreach (ScenarioCardStats a in _scenarioCardStats)
        {
            if (a.cardName == cardName)
                return a;
        }

        Debug.LogError("can't find card " + cardName + " in action stats - check the json file");
        return null;
    }

    // return the ItemStats corresponding to the given cardName
    public ItemCardStats GetItemStats(string cardName)
    {
        if (_itemCardStats == null)
        {
            Debug.Log("Loading item cards data from json");
            LoadItemCardsData();
        }

        foreach (ItemCardStats a in _itemCardStats)
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
        Debug.Log(actionCardsFile);
        _actionCardStats = JsonConvert.DeserializeObject<ActionCardStats[]>(actionCardsFile);

        // Debug.Log(_actionCardStats);
        // Debug.Log(_actionCardStats[0]);

        if (_actionCardStats == null)
            Debug.LogError("couldn't load LoadActionCardsData() properly");
    }


    protected void LoadScenarioCardsData()
    {
        string scenarioCardsFile = ReadFile(_scenarioCardsDataFileName);
        Debug.Log(scenarioCardsFile);
        _scenarioCardStats = JsonConvert.DeserializeObject<ScenarioCardStats[]>(scenarioCardsFile);

        if (_scenarioCardStats == null)
            Debug.LogError("couldn't load LoadScenarioCardsData() properly");
    }


    protected void LoadItemCardsData()
    {
        string itemCardsFile = ReadFile(_itemCardsDataFileName);
        // Debug.Log(itemCardsFile);
        _itemCardStats = JsonConvert.DeserializeObject<ItemCardStats[]>(itemCardsFile);

        if (_itemCardStats == null)
            Debug.LogError("couldn't load LoadItemCardsData() properly");
    }


    // reads a file in  the streamingAssets folder and returns it's content
    protected string ReadFile(string filepath)
    {
        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string dataAsJson = "";
        var targetFile = Resources.Load<TextAsset>(filepath);
        if (targetFile != null)
        {
            dataAsJson = targetFile.text;
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
        return dataAsJson;
    }

    #endregion // PROTECTED_METHODS

}

