using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Cursor.lockState = CursorLockMode.None;

		if (!PlayerPrefs.HasKey ("tutorialPlay")) {
			PlayerPrefs.SetInt ("tutorialPlay", 1);
		}
	}

//TODO - Remove from final build
	void Update () {
		if (Input.GetKeyDown ("t")) {
				PlayerPrefs.SetInt("tutorialPlay", 1);
			}
	}
}
