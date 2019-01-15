using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneOnClick : MonoBehaviour {

	// Use this for initialization
	public void loadSceneByIndex(int sceneIndex){
        SceneManager.LoadScene(sceneIndex);
    }
}
