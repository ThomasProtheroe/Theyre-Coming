using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneOnClick : MonoBehaviour {

	private AsyncOperation asyncLoad;

	public void startNewGame() {
		asyncLoad = Scenes.Load ("Main", "dev", "false");
		StartCoroutine ("fadeToBlack");
	}

	public void startSandboxGame() {
		asyncLoad = Scenes.Load ("Main", "dev", "true");
		StartCoroutine ("fadeToBlack");
	}

	public void runScene() {
		asyncLoad.allowSceneActivation = true;
	}


	IEnumerator fadeToBlack()
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

			yield return null;
		}

		yield return new WaitForSeconds (1.5f);

		asyncLoad.allowSceneActivation = true;
	}
}
