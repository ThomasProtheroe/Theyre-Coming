using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour {

	public Image image;
	public Image panelImage;
	public Color activeColor;
	public Color inactiveColor;

	// Use this for initialization
	void Start () {
		
	}
	
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
}
