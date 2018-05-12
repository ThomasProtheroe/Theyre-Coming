using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour {

	public Image image;
	public Image panelImage;
	public Color activeColor;
	public Color inactiveColor;

	private bool flashing;
	
	public void setImage(Sprite sprite) {
		image.enabled = true;
		image.sprite = sprite;
	}

	public void setEmpty() {
		image.enabled = false;
		image.sprite = null;
	}

	public void setActive(bool isActive) {
		if (isActive) {
			panelImage.color = activeColor;
		} else {
			panelImage.color = inactiveColor;
		}
	}

	public void setDurabilityIndicator(float durabilityNormalized) {
		if (image.sprite == null) {
			return;
		}

		Color c = image.color;
		c.g = durabilityNormalized / 2 + 0.5f;
		c.b = durabilityNormalized / 2 + 0.5f;
		image.color = c;

		if (durabilityNormalized <= 0.25 && !flashing) {
			startFlashing ();
		} else if (durabilityNormalized > 0.25 && flashing) {
			stopFlashing ();
		}
	}

	public void resetDurabilityIndicator() {
		Color c = new Color ();
		c.r = 1.0f;
		c.g = 1.0f;
		c.b = 1.0f;
		c.a = 1.0f;
		image.color = c;

		if (flashing) {
			stopFlashing ();
		}
	}

	private void startFlashing() {
		flashing = true;
		StartCoroutine ("Flashing");
	}

	private void stopFlashing() {
		flashing = false;
		StopCoroutine ("Flashing");
	}

	private IEnumerator Flashing() {
		//Turn red over time
		float durability = image.color.g;
		for (float f = durability; f >= 0; f -= 0.1f) {
			Color c = image.color;
			c.g = f;
			c.b = f;
			image.color = c;

			yield return null;
		}

		//Turn back to original color
		for (float f = 0f; f <= durability; f += 0.1f) {
			Color c = image.color;
			c.g = f;
			c.b = f;
			image.color = c;

			yield return null;
		}

		yield return new WaitForSeconds (1.0f);
		StartCoroutine ("Flashing");
	}
}
