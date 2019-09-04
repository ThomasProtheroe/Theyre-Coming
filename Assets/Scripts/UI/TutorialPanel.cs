using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (PlayerPrefs.GetInt ("tutorialPlay") == 0) {
			destroyTutorialPanel();
		}
	}
	
	public void showTutorialPanel () {
		gameObject.SetActive(true);
	}

	public void hideTutorialPanel() {
		gameObject.SetActive(false);
	}

	public void destroyTutorialPanel() {
		Destroy(gameObject);
	}
}
