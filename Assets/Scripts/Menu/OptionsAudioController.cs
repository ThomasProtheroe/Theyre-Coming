using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsAudioController : MonoBehaviour {

	public void setMasterVolume(float volume) {
		Scenes.setParam ("masterVolume", volume.ToString());
	}

	public void setMusicVolume(float volume) {
		Scenes.setParam ("musicVolume", volume.ToString());
	}

	public void setEffectsVolume(float volume) {
		Scenes.setParam ("effectsVolume", volume.ToString());
	}
}
