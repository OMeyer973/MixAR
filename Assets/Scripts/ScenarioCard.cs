﻿using System.Collections.Generic;
using UnityEngine;

// class that represents the data describing an ActionCard.
// this is a buffer class used to load the card data since JsonUtility cannot deserialise class inherited from MonoBehavior
public class ScenarioCardStats
{
    // members need to be public to be deserialized by JsonUtility
    public string cardName;
    public int scenarioId;
    public List<List<int>> fateMatrix;
}

public class ScenarioCard : Card {

    protected int CardName { get; private set; }
    protected int ScenarioId { get; private set; }
    protected List<List<int>> FateMatrix { get; private set; } = new List<List<int>>();

    // initialize the card members according to the parsed json cardData and points to the cardScanner in the scene
    public void Initialize(CardsScanner cardsScanner, ScenarioCardStats cardStats)
    {
        Debug.Log("Initializing scenario card ");
        Debug.Log("cardstats " + cardStats);

        _cardsScanner = cardsScanner;
        ScenarioId = cardStats.scenarioId;
        //Debug.Log(cardStats.fateMatrix);
        FateMatrix.Clear();

        // transposition of the ScenarioCardStatFateMatrix because they are stored by character column in JSON
        // and we want to access them like this : FateMatrix[characterId][actionId]
        for (int i = 0; i < GameManager.nbCharacters; i++)
        {
            List<int> tmpCharActions = new List<int>();
            for (int j = 0; j < GameManager.nbGameVariables; j++)
            {
                tmpCharActions.Add(cardStats.fateMatrix[j][i]);
            }
            FateMatrix.Add(tmpCharActions);
        }
        
        Print();
    }

    public void Print()
    {
        Debug.Log
        (
            "ScenarioCard : " + CardName + " scenario " + ScenarioId + "\n" +
            "FateMatrix : char 0 : " + FateMatrix[0][0] + " " + FateMatrix[0][1] + " " + FateMatrix[0][2] + " " + FateMatrix[0][3] + "\n" +
            "             char 1 : " + FateMatrix[1][0] + " " + FateMatrix[1][1] + " " + FateMatrix[1][2] + " " + FateMatrix[1][3] + "\n" +
            "             char 2 : " + FateMatrix[2][0] + " " + FateMatrix[2][1] + " " + FateMatrix[2][2] + " " + FateMatrix[2][2] + "\n"
        );
    }
}
