﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneOnClick : MonoBehaviour {

	public Text title;

	public void startNewGame() {
		StartCoroutine ("fadeToBlack", "story");
	}

	public void startSandboxGame() {
		StartCoroutine ("fadeToBlack", "dev");
	}

	public void startEndlessGame() {
		StartCoroutine ("fadeToBlack", "endless");
	}

	IEnumerator fadeToBlack(string mode)
	{
		Button[] buttons = Button.FindObjectsOfType<Button> ();
		for (float f = 1.0f; f > 0.0f; f -= 0.02f) {
			foreach (Button button in buttons) {
				Image buttonImage = button.GetComponent<Image> ();
				Text buttonText = button.GetComponentInChildren<Text> ();

				Color imageColor = buttonImage.color;
				Color textColor = buttonText.color;

				imageColor.a = f;
				textColor.a = f;

				buttonImage.color = imageColor;
				buttonText.color = textColor;
			}

			Color c = title.color;
			c.a = f;
			title.color = c;

			yield return null;
		}

		Scenes.Load ("Main", "gameMode", mode);
	}
}
