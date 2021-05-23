using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour {

	[SerializeField]
	private Image fill;
	[SerializeField]
	private Image icon;
	[SerializeField]
	private Sprite[] conditionSprites;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void updateStaminaBar(float fillAmount) {
		fill.fillAmount = fillAmount;

		if (fillAmount > 0.95f) {
			icon.sprite = conditionSprites[0];
		} else if (fillAmount >= 0.75f) {
			icon.sprite = conditionSprites[1];
		} else if (fillAmount >= 0.50f) {
			icon.sprite = conditionSprites[2];
		} else if (fillAmount >= 0.25f) {
			icon.sprite = conditionSprites[3];
		} else {
			icon.sprite = conditionSprites[4];
		}
	}
}
