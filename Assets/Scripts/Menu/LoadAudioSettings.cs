using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAudioSettings : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//TODO load from persistent file and set sliders
		Scenes.setParam ("masterVolume", "1.0");
		Scenes.setParam ("musicVolume", "1.0");
		Scenes.setParam ("effectsVolume", "1.0");
	}
}
