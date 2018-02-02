using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAudioController : MonoBehaviour {

	public AudioSource source;

	public AudioClip click;
	public AudioClip hover;

	public void playClick() {
		source.PlayOneShot (click);
	}

	public void playhover() {
		source.PlayOneShot (hover);
	}
}
