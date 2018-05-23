using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (PlayerPrefs.GetInt ("tutorialPlay") == 0) {
			Destroy (gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
