using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhancedAudioClip : MonoBehaviour {
	public AudioClip clip;
	public float volume;
	//reverb zone mix
	//Stereo Pan

	public void setVolume(float inVolume) {
		volume = inVolume;
	}
}
