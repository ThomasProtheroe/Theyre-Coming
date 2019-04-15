using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionPanel : MonoBehaviour {

	public Text panelText;
	public Image tierImage;
	public Image bleedIcon;
	public Image burnIcon;
	public Image blindIcon;

	public Sprite[] tierSprites;

	public bool isActive;

	public void showDescription(string text) {
		gameObject.SetActive(true);
		StopCoroutine ("ShowDescription");
		StartCoroutine ("ShowDescription", text);
	}

	public void showTierImage(int tier) {
		tierImage.sprite = tierSprites [tier];
		tierImage.enabled = true;
	}

	public void updateStatusIcons(List<string> statusEffects) {
		hideStatusIcons ();

		foreach (string iconName in statusEffects) {
			if (iconName == "bleed") {
				bleedIcon.enabled = true;
			} else if (iconName == "burn") { 
				burnIcon.enabled = true;
			} else if (iconName == "blind") { 
				blindIcon.enabled = true;
			}
		}
	}

	public void hideStatusIcons() {
		bleedIcon.enabled = false;
		burnIcon.enabled = false;
		blindIcon.enabled = false;
	}

	public void hideTierImage() {
		tierImage.enabled = false;
	}

	public void hideDescription() {
		StopCoroutine ("ShowDescription");

		panelText.text = "";
		gameObject.SetActive(false);
	}

	IEnumerator ShowDescription(string text) {
		gameObject.SetActive(true);
		panelText.text = text;

		yield return new WaitForSeconds (5.0f);

		panelText.text = "";
		gameObject.SetActive(false);
	}
}
