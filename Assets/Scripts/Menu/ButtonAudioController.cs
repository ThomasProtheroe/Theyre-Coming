using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAudioController : MonoBehaviour {

	public AudioSource source;

	public AudioClip click;
	public AudioClip hover;

	public void pointerEnter() {
		if (GetComponent<Button> ().interactable) {
			playhover ();
		}
	}

	public void playClick() {
		source.PlayOneShot (click);
	}

	public void playhover() {
		source.PlayOneShot (hover);
	}
}
