using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAudioController : MonoBehaviour {

	public OptionsAudioController controller;

	public void pointerEnter() {
		if (GetComponent<Button> ().interactable) {
			playhover ();
		}
	}

	public void playClick() {
		controller.playClick ();
	}

	public void playhover() {
		controller.playHover ();
	}
}
