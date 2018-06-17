using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstAidStation : Interactive {

	private PlayerController playerCon;
	[SerializeField]
	private Image outerBar;
	[SerializeField]
	private Image innerBar;
	[SerializeField]
	private GameObject cancelText;

	private float healTimer;
	public float healTime;
	[SerializeField]
	private int healAmount;
	[SerializeField]
	private int capacity;
	[HideInInspector]
	public int usesRemaining;

	private bool inUse;

	// Use this for initialization
	void Start () {
		usesRemaining = capacity;
		playerCon = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
	}

	// Update is called once per frame
	void Update () {
		if (healTimer > 0.0f) {
			healTimer -= Time.deltaTime;
			innerBar.fillAmount = (healTime - healTimer) / healTime;
			if (healTimer <= 0.0f) {
				healTimer = 0.0f;
				finishUse ();
			}
		}
	}

	public void interact() {
		startHealTimer ();
	}

	private void startHealTimer() {
		healTimer = healTime;
		outerBar.enabled = true;
		innerBar.enabled = true;
		cancelText.SetActive (true);
	}

	public void finishUse() {
		cancelUse ();

		usesRemaining -= 1;
		playerCon.heal (healAmount);
		playerCon.isBusy = false;
		playerCon.isHealing = false;
		cancelText.SetActive (false);
	}

	public void cancelUse() {
		healTimer = 0.0f;
		outerBar.enabled = false;
		innerBar.enabled = false;
		cancelText.SetActive (false);
	}

	override public void updateHighlightColor() {
		if (usesRemaining > 0 && playerCon.health < playerCon.maxHealth) {
			GetComponent<SpriteOutline> ().color = positiveColor;
		} else {
			GetComponent<SpriteOutline> ().color = negativeColor;
		}
	}

}
