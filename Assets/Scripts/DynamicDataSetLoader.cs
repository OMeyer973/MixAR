using UnityEngine;
using System.Collections;

using Vuforia;
using System.Collections.Generic;


public class DynamicDataSetLoader : MonoBehaviour
{

    #region PRIVATE_MEMBERS

    float _defaultCardLength = 0.085f;

    #endregion // PRIVATE_MEMBERS


    #region PUBLIC_MEMBERS

   // card scanner that each card will keep a reference on
    public CardsScanner cardsScanner;

    // card stats loader to retrieve the corrects stats for each card from the json
    public CardStatsLoader cardStatsLoader;

    public string dataSetName = "";  //  Assets/StreamingAssets/QCAR/DataSetName

    // specify these in Unity Inspector    // base prefabs for the different cards types
    public GameObject actionCardPrefab;
    public GameObject trapCardPrefab;
    public GameObject scenarioCardPrefab;
    // prefab for the visualization of the threats in AR
    public GameObject threatsPrefab;

    #endregion // PUBLIC_MEMBERS

    // Use this for initialization
    void Start()
    {
        // Vuforia 6.2+
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(LoadDataSet);
    }

    // full processus of loading a card : 
    // class CardStats loads all the cards stats from json as arrays of  ActionCardData, ScenarioCardData and TrapCardData
    // LoadDataset() creates the vuforia targets object from the database
    // InitializeTarget() Initializes the card data
    // - attach a ActionCard, ScenarioCard or TrapCard component in function of the card name in the db
    // - these [type]Card components are then initialized with their respective [type]CardData as parameter
    void InitializeTarget(GameObject target)
    {
        GameObject augmentation = new GameObject();
        // if the card name starts with action (if it is an action card)
        if (System.Text.RegularExpressions.Regex.Match(target.name.ToLower(), "^maintarget*").Success)
        {
            InitializeMainTarget(target);
            augmentation = (GameObject)GameObject.Instantiate(threatsPrefab);
        }
        else if (System.Text.RegularExpressions.Regex.Match(target.name.ToLower(), "^action*").Success)
        {
            InitializeActionCard(target);
            augmentation = (GameObject)GameObject.Instantiate(actionCardPrefab);
        }
        else if (System.Text.RegularExpressions.Regex.Match(target.name.ToLower(), "^scenario*").Success)
        {
            InitializeScenarioCard(target);
            augmentation = (GameObject)GameObject.Instantiate(scenarioCardPrefab);
        }
        else if (System.Text.RegularExpressions.Regex.Match(target.name.ToLower(), "^trap*").Success)
        {
            InitializeTrapCard(target);
            augmentation = (GameObject)GameObject.Instantiate(trapCardPrefab);
        }

        augmentation.transform.localPosition = new Vector3(0f, 0f, 0f);
        augmentation.transform.localRotation = Quaternion.identity;
        augmentation.transform.localScale = new Vector3(_defaultCardLength, _defaultCardLength, _defaultCardLength);
        augmentation.gameObject.SetActive(true);
        augmentation.transform.parent = target.transform;

        // required vuforia component
        target.AddComponent<TurnOffBehaviour>();
    }
    void InitializeMainTarget(GameObject target)
    {
        target.AddComponent<MainTarget>();
    }

    void InitializeActionCard(GameObject actionCard)
    {
        actionCard.AddComponent<ActionCard>();
        actionCard.GetComponent<ActionCard>().Initialize(cardsScanner, cardStatsLoader.GetActionStats(actionCard.name));
    }

    void InitializeScenarioCard(GameObject scenarioCard)
    {
        scenarioCard.AddComponent<ScenarioCard>();     
        scenarioCard.GetComponent<ScenarioCard>().Initialize(cardsScanner, cardStatsLoader.GetScenarioStats(scenarioCard.name));

    }

    void InitializeTrapCard(GameObject trapCard)
    {
        trapCard.AddComponent<TrapCard>();
        trapCard.GetComponent<TrapCard>().Initialize(cardsScanner, cardStatsLoader.GetTrapStats(trapCard.name));
    }

    void LoadDataSet()
    {
        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        DataSet dataSet = objectTracker.CreateDataSet();

        if (dataSet.Load(dataSetName))
        {

            objectTracker.Stop();  // stop tracker so that we can add new dataset

            if (!objectTracker.ActivateDataSet(dataSet))
            {
                // Note: ImageTracker cannot have more than 100 total targets activated
                Debug.Log("<color=yellow>Failed to Activate DataSet: " + dataSetName + "</color>");
            }

            if (!objectTracker.Start())
            {
                Debug.Log("<color=yellow>Tracker Failed to Start.</color>");
            }

            IEnumerable<TrackableBehaviour> tbs = TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();
            foreach (TrackableBehaviour tb in tbs)
            {
                if (tb.name == "New Game Object")
                {
                    // change generic name to include trackable name
                    tb.gameObject.name = tb.TrackableName;
                    InitializeTarget(tb.gameObject);
                }
            }
        }
        else
        {
            Debug.LogError("<color=yellow>Failed to load dataset: '" + dataSetName + "'</color>");
        }
    }
}