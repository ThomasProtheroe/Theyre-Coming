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

		panelText.text = "";
		panelImage.sprite = dialog.sprites[0];
		float timer = 0.0f;
		int charCount = 0;
		while(charCount < dialog.text.Length) {
			timer += Time.deltaTime;
			if (timer > 0.02f) {
				timer = 0.0f;
				charCount++;
				panelText.text = dialog.text.Substring (0, charCount);

				if (charCount % 10 < 5) {
					panelImage.sprite = dialog.sprites [0];
				} else {
					panelImage.sprite = dialog.sprites [1];
				}
			}

			yield return null;
		}
		panelImage.sprite = dialog.sprites [0];
			
		yield return new WaitForSeconds (dialog.duration);

		panelText.text = "";
		gameObject.SetActive(false);
	}
}
