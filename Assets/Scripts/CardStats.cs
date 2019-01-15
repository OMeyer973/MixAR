using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


// class that will load the cards from json and instantiate them
public class CardStats : MonoBehaviour {

    #region PROTECTED_MEMBERS

    protected string _ActionCardsDataFileName = "ActionCardsData.json";
    protected string _TrapCardsDataFileName = "TrapCardsData.json";
    protected string _ScenarioCardsDataFileName = "ScenarioCardsData.json";

    #endregion // PROTECTED8MEMBERS


    #region PUBLIC_METHODS

    // Use this for initialization
    void Start ()
    {
        LoadActionCardsData();
        // LoadTrapCardsData();
        // LoadScenarioCardsData();
    }

    #endregion // PUBLIC_METHODS


    #region PROTECTED_METHODS

    protected void LoadActionCardsData()
    {
        string actionCardsData = ReadFile(_ActionCardsDataFileName);
        Debug.Log(actionCardsData);
        ActionCardData loadedCardData = JsonUtility.FromJson<ActionCardData>(actionCardsData);

        //instantiatedCard.GetComponent<ImageTargetBehaviour>().Initialize(cardsScanner, loadedCardData);
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
