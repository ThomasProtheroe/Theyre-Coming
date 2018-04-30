using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionPanel : MonoBehaviour {

	public Text panelText;

	public bool isActive;

	public void showDescription(string text) {
		gameObject.SetActive(true);
		StopCoroutine ("ShowDescription");
		StartCoroutine ("ShowDescription", text);
	}

	public void hideDescription() {
		StopCoroutine ("ShowDescription");

		panelText.text = "";
		gameObject.SetActive(false);
	}

	IEnumerator ShowDescription(string text) {
		gameObject.SetActive(true);
		panelText.text = text;

		yield return new WaitForSeconds (4.0f);

		panelText.text = "";
		gameObject.SetActive(false);
	}
}
