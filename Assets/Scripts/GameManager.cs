using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public PlayerSettings settings;

    public GameObject menu;

    public GameObject animation;
    public GameObject parallax;

    // Use this for initialization
    public void begin() {
		menu.SetActive(false);
        animation.SetActive(true);

        parallax.GetComponent<Parallax>().addSprite(0, 0, 0);
        parallax.GetComponent<Parallax>().addSprite(0, 0, 0);
        parallax.GetComponent<Parallax>().addSprite(0, 0, 0);
    }
	
	// Update is called once per frame
	void loop() {
		
	}
    
}
