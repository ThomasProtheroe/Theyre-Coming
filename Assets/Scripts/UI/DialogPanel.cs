using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogPanel : MonoBehaviour {

	public Image panelImage;
	public Text panelText;

	public void showDialog(Dialog dialog) {
		gameObject.SetActive(true);
		StopCoroutine ("ShowDialog");
		StartCoroutine ("ShowDialog", dialog);
	}

	public void hideDialog() {
		StopCoroutine ("ShowDialog");

		panelText.text = "";
		gameObject.SetActive(false);
	}

	IEnumerator ShowDialog(Dialog dialog) {
		gameObject.SetActive(true);
		panelText.text = dialog.text;
		panelImage.sprite = dialog.sprite;

		yield return new WaitForSeconds (dialog.duration);

		panelText.text = "";
		gameObject.SetActive(false);
	}
}
