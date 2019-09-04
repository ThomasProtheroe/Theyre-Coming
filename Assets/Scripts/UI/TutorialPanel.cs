using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : MonoBehaviour {

	private bool viewed;

	// Use this for initialization
	void Start () {
		viewed = false;
		if (PlayerPrefs.GetInt ("tutorialPlay") == 0) {
			destroyTutorialPanel();
		}
	}
	
	public void showTutorialPanel () {
		//Destroy previously viewed tutorial panels
		foreach(GameObject tutorialobject in GameObject.FindGameObjectsWithTag("Tutorial")) {
			tutorialPanel = tutorialobject.getComponent<TutorialPanel> ();
			if (tutorialPanel.viewed) {
				Destroy(tutorialobject);
			}
		}

		viewed = true;
		gameObject.SetActive(true);
	}

	public void hideTutorialPanel() {
		gameObject.SetActive(false);
	}

	public void destroyTutorialPanel() {
		Destroy(gameObject);
	}
}
