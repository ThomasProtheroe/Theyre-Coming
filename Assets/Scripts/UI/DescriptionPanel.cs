using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionPanel : MonoBehaviour {

	public Text itemName;
	public Text descriptionText;
	public Image tierImage;
	public Image bleedIcon;
	public Image burnIcon;
	public Image blindIcon;
	public Image shoddyIcon;
	public Color defaultNameColor;

	public Sprite[] tierSprites;
	public Color[] tierColors;

	public bool isActive;

	public void showDescription(string[] text) {
		gameObject.SetActive(true);
		StopCoroutine ("ShowDescription");
		StartCoroutine ("ShowDescription", text);
	}

	public void showTierImage(int tier) {
		tierImage.sprite = tierSprites [tier];
		tierImage.enabled = true;

		itemName.color = tierColors[tier];
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
			}  else if (iconName == "shoddy") { 
				shoddyIcon.enabled = true;
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
		itemName.color = defaultNameColor;
	}

	public void hideDescription() {
		StopCoroutine ("ShowDescription");

		descriptionText.text = "";
		gameObject.SetActive(false);
	}

	IEnumerator ShowDescription(string[] text) {
		gameObject.SetActive(true);
		itemName.text = text[0];
		descriptionText.text = text[1];

		yield return new WaitForSeconds (5.0f);

		itemName.text = "";
		descriptionText.text = "";
		gameObject.SetActive(false);
	}
}
