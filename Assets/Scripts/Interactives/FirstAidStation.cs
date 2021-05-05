using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstAidStation : Interactive {

	private PlayerController playerCon;
	private GameController gameCon;
	private SoundController soundCon;
	[SerializeField]
	private AudioClip healSound;
	[SerializeField]
	private Image outerBar;
	[SerializeField]
	private Image innerBar;
	[SerializeField]
	private GameObject cancelText;
	private Dialog emptyDialog;

	private float healTimer;
	public float healTime;
	[SerializeField]
	private int healAmount;
	[SerializeField]
	private int capacity;
	[HideInInspector]
	public int healthRemaining;

	private bool inUse;

	// Use this for initialization
	void Start () {
		healthRemaining = capacity / 2;  //Rounded down implicitly
		playerCon = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
		gameCon = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		soundCon = GameObject.FindGameObjectWithTag ("SoundController").GetComponent<SoundController> ();
		emptyDialog = new Dialog ("That's the last of the med supplies. Not good.");
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
		soundCon.playPriorityOneShot (healSound);

		int playerHealthMissing = playerCon.getHealthMissing();
		int healAmount = 0;
		if (playerHealthMissing <= healthRemaining) {
			healAmount = playerHealthMissing;
		} else {
			healAmount = healthRemaining;
		}
		
		playerCon.heal(healAmount);
		playerCon.decreaseStamina(getStaminaCost());
		playerCon.isBusy = false;
		playerCon.isHealing = false;
		cancelText.SetActive (false);

		healthRemaining -= healAmount;
		if (healthRemaining == 0) {
			gameCon.showDialog (emptyDialog);
		}
	}

	public void cancelUse() {
		healTimer = 0.0f;
		outerBar.enabled = false;
		innerBar.enabled = false;
		cancelText.SetActive (false);
	}

	public bool canUse() {
		string phase = gameCon.getPhase ();

		if (healthRemaining > 0 && (int)playerCon.stamina >= getStaminaCost() && phase == "downtime" && playerCon.health < playerCon.maxHealth) {
			return true;
		} else {
			return false;
		}
	}

	public int getStaminaCost() {
		return Constants.STAMINA_COST_FIRSTAID;
	}

	override public void updateHighlightColor() {
		if (canUse()) {
			GetComponent<SpriteOutline> ().color = positiveColor;
		} else {
			GetComponent<SpriteOutline> ().color = negativeColor;
		}
	}

}
