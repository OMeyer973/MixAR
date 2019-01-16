using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class PlayerSettings : MonoBehaviour
{
    private const string SAVING_PATH = "/";
    private const string SAVING_FILE = "setting.dat";
    private string COMPLETE_PATH = SAVING_PATH + SAVING_FILE;
    private PlayerSettingsValues _settings = new PlayerSettingsValues();

    public Dropdown nbPlayerButton;
    public Toggle botButton;
    public Toggle predictButton;
    public Toggle timerButton;

    void Start()
    {
        loadSettings();
        setSettingsToCanvas();
    }
    //Return saving file
    private void setSettingsToCanvas()
    {
        nbPlayerButton.value = _settings._nbPlayer - 1;
        botButton.isOn = _settings._badGuyIsIA;
        predictButton.isOn = _settings._predictIsOn;
        timerButton.isOn = _settings._timerIsOn;
    }

    private FileStream getFile(bool writeModeOn)
    {
        string path = Application.persistentDataPath + COMPLETE_PATH;
        FileStream file;
        if (!File.Exists(path))
            throw new System.Exception("Unreachable file");

        if (writeModeOn)
            file = File.OpenWrite(path);
        else
            file = File.OpenRead(path);
        
        return file;
    }

    #region PUBLIC_METHODS
    public virtual string toString()
    {
        return "_nbPlayer : " + _settings._nbPlayer +
            "_badGuyIsIA : " + _settings._badGuyIsIA +
            "_timerIsOn : " + _settings._timerIsOn +
            "_predictIsOn : " + _settings._predictIsOn;
    }

    public void save()
    {
        string path = Application.persistentDataPath + COMPLETE_PATH;
        BinaryFormatter bf = new BinaryFormatter();
        if (!File.Exists(path))
            File.Create(path);
        FileStream file = getFile(true);
        bf.Serialize(file, this._settings);
        file.Close();
    }


    public void loadSettings()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = getFile(false);
        PlayerSettingsValues savedSettings = (PlayerSettingsValues)bf.Deserialize(file);
        this._settings = savedSettings;
        file.Close();
        Debug.Log(this.toString());
    }
    #endregion
 
    #region SETTERS
    public void nbPlayer(int nb) { _settings._nbPlayer = nb+1; }
    public void badGuyIA(bool IA) { _settings._badGuyIsIA = IA; }
    public void timer(bool timer) { _settings._timerIsOn = timer; }
    public void predict(bool predict) { _settings._predictIsOn = predict; }
    #endregion

}

[System.Serializable]
class PlayerSettingsValues
{
    public int _nbPlayer = 1;
    public bool _badGuyIsIA;
    public bool _timerIsOn;
    public bool _predictIsOn;

    

}
