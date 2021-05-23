using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bed : Interactive {

	private PlayerController playerCon;
	private GameController gameCon;
	private SoundController soundCon;
	[SerializeField]
	private AudioClip sleepSound;
	[SerializeField]
	private Sprite sleepSprite;
	[SerializeField]
	private Sprite defaultSprite;

	private bool inUse;

	// Use this for initialization
	void Start () {
		playerCon = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
		gameCon = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		soundCon = GameObject.FindGameObjectWithTag ("SoundController").GetComponent<SoundController> ();
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void interact() {
		goToBed ();
	}

	private void goToBed() {
		string phase = gameCon.getPhase();
		if (phase == "downtime") {
			StartCoroutine("GoToBed");
		} else {
			finishUse();
		}
	}

	private void setSleepSprite() {
		GetComponent<SpriteRenderer> ().sprite = sleepSprite;
	}

	private void setDefaultSprite() {
		GetComponent<SpriteRenderer> ().sprite = defaultSprite;
	}

	public void finishUse() {
		playerCon.isBusy = false;
	}

	override public void updateHighlightColor() {
		string phase = gameCon.getPhase();
		if (phase == "downtime") {
			GetComponent<SpriteOutline> ().color = positiveColor;
		} else {
			GetComponent<SpriteOutline> ().color = negativeColor;
		}
	}

	IEnumerator GoToBed() {
		playerCon.hidePlayer();
		setSleepSprite();
		soundCon.playPriorityOneShot (sleepSound);
		gameCon.miscFadeOut(0.005f);
		yield return new WaitForSeconds (3.5f);
		playerCon.goToSleep();
		setDefaultSprite();
		playerCon.showPlayer();
		gameCon.miscFadeIn(0.005f);
		yield return new WaitForSeconds (2.75f);
		gameCon.startNewNight();
		finishUse();
	}
}
